#!/usr/bin/env dotnet
//MISE description="Bump version in all hardcoded locations"
//MISE alias="rbv"
#:property PublishAot=false

using System.Text.RegularExpressions;

var repoRoot = Environment.CurrentDirectory;

if (args.Length == 0)
{
    Err("Usage: mise run release:bump-version <version>");
    return 1;
}
var newVersion = args[0];

Info($"Bumping version to {newVersion}...");

// 1. Helm Chart (version + appVersion)
var chartPath = Path.Combine(repoRoot, "deploy", "helm", "wcm-api", "Chart.yaml");
if (File.Exists(chartPath))
{
    var content = File.ReadAllText(chartPath);
    content = Regex.Replace(content, @"^version: .+$", $"version: {newVersion}", RegexOptions.Multiline);
    content = Regex.Replace(content, @"^appVersion: .+$", $"appVersion: \"{newVersion}\"", RegexOptions.Multiline);
    File.WriteAllText(chartPath, content);
    Ok($"Chart.yaml: version={newVersion}, appVersion={newVersion}");
}

// 2. OpenChoreo values (image.tag)
var ocValuesPath = Path.Combine(repoRoot, "deploy", "helm", "values-openchoreo.yaml");
if (File.Exists(ocValuesPath))
{
    var content = File.ReadAllText(ocValuesPath);
    content = Regex.Replace(content, @"^(\s*tag:\s*).+$", $"${{1}}\"{newVersion}\"", RegexOptions.Multiline);
    File.WriteAllText(ocValuesPath, content);
    Ok($"values-openchoreo.yaml: tag={newVersion}");
}

// 3. Kind helpers (API_IMAGE tag)
var kindHelpersPath = Path.Combine(repoRoot, ".mise", "tasks", "kind", "_helpers.sh");
if (File.Exists(kindHelpersPath))
{
    var content = File.ReadAllText(kindHelpersPath);
    content = Regex.Replace(content, @"(API_IMAGE=.*:)[^\s""]+", $"${{1}}{newVersion}");
    File.WriteAllText(kindHelpersPath, content);
    Ok($"_helpers.sh: API_IMAGE tag={newVersion}");
}

Ok($"Version bumped to {newVersion} in all locations");
return 0;

// ── Helpers ──────────────────────────────────────────────
static void Info(string msg) => Console.WriteLine($"\x1b[1;34m==> {msg}\x1b[0m");
static void Ok(string msg)   => Console.WriteLine($"\x1b[1;32m  + {msg}\x1b[0m");
static void Err(string msg)  => Console.WriteLine($"\x1b[1;31m  x {msg}\x1b[0m");
