#!/usr/bin/env dotnet
//MISE description="Delete the kind test cluster"
//MISE alias="kd"
#:property PublishAot=false
#:package CliWrap@*

using CliWrap;

var clusterName = Env("CLUSTER_NAME", "wcm-test");

Info($"Deleting kind cluster '{clusterName}'…");
var result = await Cli.Wrap("kind")
    .WithArguments(["delete", "cluster", "--name", clusterName])
    .WithValidation(CommandResultValidation.None)
    .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
    .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
    .ExecuteAsync();
if (result.ExitCode != 0)
{
    Err($"kind failed (exit code {result.ExitCode})");
    return result.ExitCode;
}
Ok("Cluster deleted");
return 0;

// ── Helpers ──────────────────────────────────────────────
static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) ?? fallback;

static void Info(string msg) => Console.WriteLine($"\x1b[1;34m==> {msg}\x1b[0m");
static void Ok(string msg)   => Console.WriteLine($"\x1b[1;32m  ✓ {msg}\x1b[0m");
static void Err(string msg)  => Console.WriteLine($"\x1b[1;31m  ✗ {msg}\x1b[0m");
