#!/usr/bin/env dotnet
//MISE description="Build and push multi-arch container images to GHCR"
//MISE alias="rcb"
#:property PublishAot=false
#:package CliWrap@*

using CliWrap;
using CliWrap.Buffered;

var repoRoot       = Environment.CurrentDirectory;
var containerfile  = Path.Combine(repoRoot, "WCM.API.ApiService", "Containerfile.release");
var ghcrRegistry   = "ghcr.io";
var ghcrImage      = $"{ghcrRegistry}/urbaser-s-a/wcm-api";
var version        = ResolveVersion(args);
var runtime        = await DetectRuntime();

Info($"Building multi-arch container image: {ghcrImage}:{version}");

// 1. QEMU binfmt registration (cross-arch emulation)
Info("Registering QEMU binfmt handlers...");
var qemuResult = await Cli.Wrap(runtime)
    .WithArguments(["run", "--rm", "--privileged", "tonistiigi/binfmt", "--install", "all"])
    .WithValidation(CommandResultValidation.None)
    .ExecuteAsync();
if (qemuResult.ExitCode != 0)
    Warn("QEMU binfmt registration failed — multi-arch build may not work for foreign architectures");

// 2. Ensure buildx builder exists
var inspectResult = await Cli.Wrap(runtime)
    .WithArguments(["buildx", "inspect", "wcm-builder"])
    .WithValidation(CommandResultValidation.None)
    .ExecuteBufferedAsync();

if (inspectResult.ExitCode != 0)
{
    Info("Creating buildx builder 'wcm-builder'...");
    await Run(runtime, ["buildx", "create", "--name", "wcm-builder", "--driver", "docker-container", "--bootstrap", "--use"]);
}

// 3. Registry login (if token available)
var registryToken = Environment.GetEnvironmentVariable("REGISTRY_TOKEN") ?? "";
if (!string.IsNullOrEmpty(registryToken))
{
    Info($"Logging in to {ghcrRegistry}...");
    var user = Env("REGISTRY_USER", Env("GITHUB_ACTOR", ""));
    var loginResult = await Cli.Wrap(runtime)
        .WithArguments(["login", ghcrRegistry, "-u", user, "--password-stdin"])
        .WithValidation(CommandResultValidation.None)
        .WithStandardInputPipe(PipeSource.FromString(registryToken))
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .ExecuteAsync();
    if (loginResult.ExitCode != 0)
    {
        Err($"Registry login failed (exit code {loginResult.ExitCode})");
        return loginResult.ExitCode;
    }
}

// 4. Build + push multi-arch image
Info("Building linux/amd64 + linux/arm64...");
await Run(runtime, [
    "buildx", "build",
    "--builder", "wcm-builder",
    "--platform", "linux/amd64,linux/arm64",
    "-f", containerfile,
    "-t", $"{ghcrImage}:{version}",
    "-t", $"{ghcrImage}:latest",
    "--push",
    repoRoot
]);

Ok($"Multi-arch image pushed: {ghcrImage}:{version}");
Ok($"Multi-arch image pushed: {ghcrImage}:latest");
return 0;

// ── Helpers ──────────────────────────────────────────────
static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) ?? fallback;

static string ResolveVersion(string[] args)
{
    var version = Environment.GetEnvironmentVariable("RELEASE_VERSION")
        ?? (args.Length > 0 ? args[0] : null);
    if (string.IsNullOrEmpty(version))
    {
        Err("No version specified. Set RELEASE_VERSION or pass as argument.");
        Environment.Exit(1);
    }
    return version!;
}

static async Task Run(string cmd, string[] args)
{
    var result = await Cli.Wrap(cmd)
        .WithArguments(args)
        .WithValidation(CommandResultValidation.None)
        .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .ExecuteAsync();
    if (result.ExitCode != 0)
    {
        Err($"{cmd} failed (exit code {result.ExitCode})");
        Environment.Exit(result.ExitCode);
    }
}

static async Task<string> DetectRuntime()
{
    try
    {
        var result = await Cli.Wrap("podman")
            .WithArguments(["info"])
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();
        return result.ExitCode == 0 ? "podman" : "docker";
    }
    catch { return "docker"; }
}

static void Info(string msg) => Console.WriteLine($"\x1b[1;34m==> {msg}\x1b[0m");
static void Ok(string msg)   => Console.WriteLine($"\x1b[1;32m  + {msg}\x1b[0m");
static void Warn(string msg) => Console.WriteLine($"\x1b[1;33m  ! {msg}\x1b[0m");
static void Err(string msg)  => Console.WriteLine($"\x1b[1;31m  x {msg}\x1b[0m");
