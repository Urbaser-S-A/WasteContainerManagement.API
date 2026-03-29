#!/usr/bin/env dotnet
//MISE description="Create kind cluster, build & load images, deploy Helm chart"
//MISE alias="ku"
#:property PublishAot=false
#:package CliWrap@*

using CliWrap;
using CliWrap.Buffered;

var repoRoot = Environment.CurrentDirectory;
var chartPath = Path.Combine(repoRoot, "deploy", "helm", "wcm-api");
var apiImage = "wcm-api:0.1.2";

var clusterName = Env("CLUSTER_NAME", "wcm-test");
var kubeCtx = $"kind-{clusterName}";
var releaseName = Env("RELEASE_NAME", "wcm");

// Detect container runtime (podman or docker)
var runtime = await DetectRuntime();

// 1. Create cluster (idempotent)
var clustersOutput = await Buffered("kind", ["get", "clusters"]);
var clusterExists = clustersOutput
    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Any(c => c.Trim() == clusterName);

if (clusterExists)
{
    Warn($"Cluster '{clusterName}' already exists — reusing");
}
else
{
    Info($"Creating kind cluster '{clusterName}'…");
    await Run("kind", ["create", "cluster", "--name", clusterName, "--wait", "60s"]);
    Ok("Cluster created");
}

// 2. Build API image
Info($"Building API image ({apiImage})…");
await Run(runtime, [
    "build",
    "-f", Path.Combine(repoRoot, "WCM.API.ApiService", "Containerfile.release"),
    "-t", apiImage,
    repoRoot
], quiet: true);
Ok("Image built");

// 3. Resolve PostgreSQL image from env or defaults
var pgRepo = Env("POSTGRES_REPO", "docker.io/library/postgres");
var pgTag = Env("POSTGRES_TAG", "18.3");
var pgImage = $"{pgRepo}:{pgTag}";

// 4. Load images into kind
Info("Loading images into kind…");
await Run("kind", ["load", "docker-image", apiImage, "--name", clusterName], quiet: true);
await Run("kind", ["load", "docker-image", pgImage, "--name", clusterName], quiet: true);
Ok("Images loaded");

// 5. Install / upgrade chart
Info("Installing Helm chart…");
string[] helmValues = [
    "--set", "image.registry=localhost",
    "--set", "image.pullPolicy=Never",
    "--set", "app.environment=LocalDevelopment",
    "--set", "postgresql.persistence.enabled=false",
    "--set", "postgresql.auth.password=kindtest",
    "--set", $"postgresql.image.repository={pgRepo}",
    "--set", $"postgresql.image.tag={pgTag}",
    "--set", "postgresql.image.digest=",
    "--set", "postgresql.image.pullPolicy=Never",
    "--set", "extraEnv.PGGSSENCMODE=disable",
    "--kube-context", kubeCtx,
    "--wait", "--timeout", "180s"
];

var helmStatus = await Cli.Wrap("helm")
    .WithArguments(["status", releaseName, "--kube-context", kubeCtx])
    .WithValidation(CommandResultValidation.None)
    .ExecuteBufferedAsync();
    

var helmCmd = helmStatus.ExitCode == 0 ? "upgrade" : "install";
await Run("helm", [helmCmd, releaseName, chartPath, .. helmValues]);
Ok("Chart deployed");

// ── Helpers ──────────────────────────────────────────────
static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) ?? fallback;

static void Info(string msg) => Console.WriteLine($"\x1b[1;34m==> {msg}\x1b[0m");
static void Ok(string msg) => Console.WriteLine($"\x1b[1;32m  ✓ {msg}\x1b[0m");
static void Warn(string msg) => Console.WriteLine($"\x1b[1;33m  ⚠ {msg}\x1b[0m");
static void Err(string msg) => Console.WriteLine($"\x1b[1;31m  ✗ {msg}\x1b[0m");

static async Task Run(string cmd, string[] args, bool quiet = false)
{
    var outPipe = quiet ? PipeTarget.ToStream(Stream.Null) : PipeTarget.ToStream(Console.OpenStandardOutput());
    var result = await Cli.Wrap(cmd)
        .WithArguments(args)
        .WithValidation(CommandResultValidation.None)
        .WithStandardOutputPipe(outPipe)
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .ExecuteAsync();
    if (result.ExitCode != 0)
    {
        Err($"{cmd} failed (exit code {result.ExitCode})");
        Environment.Exit(result.ExitCode);
    }
}

static async Task<string> Buffered(string cmd, string[] args)
{
    var result = await Cli.Wrap(cmd)
        .WithArguments(args)
        .WithValidation(CommandResultValidation.None)
        .ExecuteBufferedAsync();
    return result.StandardOutput;
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
