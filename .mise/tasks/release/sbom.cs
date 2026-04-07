#!/usr/bin/env dotnet
//MISE description="Generate CycloneDX SBOM from .NET project"
//MISE alias="rsb"
#:property PublishAot=false
#:package CliWrap@*

using CliWrap;

var repoRoot = Environment.CurrentDirectory;
var sbomDir  = Path.Combine(repoRoot, "out", "sbom");
var project  = Path.Combine(repoRoot, "WCM.API.ApiService", "WCM.API.ApiService.csproj");
var version  = ResolveVersion(args);

Info($"Generating CycloneDX SBOM for WCM.API v{version}...");

if (Directory.Exists(sbomDir))
    Directory.Delete(sbomDir, true);
Directory.CreateDirectory(sbomDir);

var outputFile = Path.Combine(sbomDir, $"wcm-api-{version}-cyclonedx.json");

var result = await Cli.Wrap("cyclonedx-cli")
    .WithArguments([
        "analyze",
        "--input-file", project,
        "--output-file", outputFile,
        "--output-format", "json"
    ])
    .WithValidation(CommandResultValidation.None)
    .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
    .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
    .ExecuteAsync();

if (result.ExitCode != 0)
{
    Err($"cyclonedx-cli failed (exit code {result.ExitCode})");
    return result.ExitCode;
}

Ok($"CycloneDX SBOM generated: out/sbom/wcm-api-{version}-cyclonedx.json");
return 0;

// ── Helpers ──────────────────────────────────────────────
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

static void Info(string msg) => Console.WriteLine($"\x1b[1;34m==> {msg}\x1b[0m");
static void Ok(string msg)   => Console.WriteLine($"\x1b[1;32m  + {msg}\x1b[0m");
static void Err(string msg)  => Console.WriteLine($"\x1b[1;31m  x {msg}\x1b[0m");
