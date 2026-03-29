#!/usr/bin/env dotnet
//MISE description="Apply SQL init + seed scripts to the running PostgreSQL"
//MISE alias="ks"
//MISE depends=["kind:up"]
#:property PublishAot=false
#:package CliWrap@*

using CliWrap;
using CliWrap.Buffered;

var repoRoot    = Environment.CurrentDirectory;
var sqlDir      = Path.Combine(repoRoot, "scripts");
var clusterName = Env("CLUSTER_NAME", "wcm-test");
var kubeCtx     = $"kind-{clusterName}";

Info("Seeding database…");

// Find PostgreSQL pod
var podResult = await Cli.Wrap("kubectl")
    .WithArguments(["get", "pods",
        "-l", "app.kubernetes.io/component=postgresql",
        "-o", "jsonpath={.items[0].metadata.name}",
        "--context", kubeCtx])
    .WithValidation(CommandResultValidation.None)
    .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
    .ExecuteBufferedAsync();
if (podResult.ExitCode != 0)
{
    Err($"kubectl failed (exit code {podResult.ExitCode})");
    return podResult.ExitCode;
}
var pgPod = podResult.StandardOutput.Trim();

// Apply SQL files in order
foreach (var sqlFile in Directory.GetFiles(sqlDir, "0*.sql").Order())
{
    var fname = Path.GetFileName(sqlFile);

    var cpResult = await Cli.Wrap("kubectl")
        .WithArguments(["cp", sqlFile, $"default/{pgPod}:/tmp/{fname}", "--context", kubeCtx])
        .WithValidation(CommandResultValidation.None)
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .ExecuteAsync();
    if (cpResult.ExitCode != 0)
    {
        Err($"kubectl cp failed for {fname} (exit code {cpResult.ExitCode})");
        return cpResult.ExitCode;
    }

    var execResult = await Cli.Wrap("kubectl")
        .WithArguments(["exec", pgPod, "--context", kubeCtx, "--",
            "psql", "-U", "postgres", "-d", "wcmdb", "-f", $"/tmp/{fname}"])
        .WithValidation(CommandResultValidation.None)
        .ExecuteBufferedAsync();

    // Show last line of output (matches bash tail -1 behaviour)
    var lastLine = execResult.StandardOutput.TrimEnd().Split('\n').LastOrDefault() ?? "";
    if (!string.IsNullOrEmpty(lastLine)) Console.WriteLine(lastLine);
    Ok(fname);
}
return 0;

// ── Helpers ──────────────────────────────────────────────
static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) ?? fallback;

static void Info(string msg) => Console.WriteLine($"\x1b[1;34m==> {msg}\x1b[0m");
static void Ok(string msg)   => Console.WriteLine($"\x1b[1;32m  ✓ {msg}\x1b[0m");
static void Err(string msg)  => Console.WriteLine($"\x1b[1;31m  ✗ {msg}\x1b[0m");
