#!/usr/bin/env dotnet
//MISE description="Full kind test cycle: cluster → build → deploy → seed → smoke test → teardown"
//MISE alias="ka"
//MISE depends=["kind:test"]
#:property PublishAot=false
#:package CliWrap@*

using CliWrap;

var clusterName = Env("CLUSTER_NAME", "wcm-test");

if (Env("KEEP_CLUSTER", "") != "1")
{
    var result = await Cli.Wrap("mise")
        .WithArguments(["run", "kind:down"])
        .WithValidation(CommandResultValidation.None)
        .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .ExecuteAsync();
    if (result.ExitCode != 0)
    {
        Err($"kind:down failed (exit code {result.ExitCode})");
        return result.ExitCode;
    }
}
else
{
    Warn($"Cluster '{clusterName}' kept alive (KEEP_CLUSTER=1)");
}
return 0;

// ── Helpers ──────────────────────────────────────────────
static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) ?? fallback;

static void Warn(string msg) => Console.WriteLine($"\x1b[1;33m  ⚠ {msg}\x1b[0m");
static void Err(string msg) => Console.WriteLine($"\x1b[1;31m  ✗ {msg}\x1b[0m");
