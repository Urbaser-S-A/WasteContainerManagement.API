#!/usr/bin/env dotnet
//MISE description="Run API smoke tests against the kind cluster"
//MISE alias="kt"
//MISE depends=["kind:seed"]
#:property PublishAot=false
#:package CliWrap@*

using System.Diagnostics;
using CliWrap;
using CliWrap.Buffered;

var clusterName = Env("CLUSTER_NAME", "wcm-test");
var kubeCtx = $"kind-{clusterName}";

Info("Running smoke tests…");

// Find API pod
var podResult = await Cli.Wrap("kubectl")
    .WithArguments(["get", "pods",
        "-l", "app.kubernetes.io/name=wcm-api,!app.kubernetes.io/component",
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
var apiPod = podResult.StandardOutput.Trim();

// Port-forward using Process directly (CliWrap's pipe setup doesn't suit long-running bg processes)
var pfStderr = new List<string>();
var pf = new Process();
pf.StartInfo.FileName = "kubectl";
pf.StartInfo.ArgumentList.Add("port-forward");
pf.StartInfo.ArgumentList.Add(apiPod);
pf.StartInfo.ArgumentList.Add("18080:8080");
pf.StartInfo.ArgumentList.Add("--context");
pf.StartInfo.ArgumentList.Add(kubeCtx);
pf.StartInfo.RedirectStandardOutput = true;
pf.StartInfo.RedirectStandardError = true;
pf.StartInfo.UseShellExecute = false;
pf.ErrorDataReceived += (_, e) => { if (e.Data is not null) pfStderr.Add(e.Data); };
pf.Start();
pf.BeginOutputReadLine();
pf.BeginErrorReadLine();

// Wait for port-forward to actually accept connections
using var client = new HttpClient { BaseAddress = new Uri("http://localhost:18080") };
var ready = false;
for (var i = 0; i < 15 && !ready; i++)
{
    if (pf.HasExited)
    {
        Err($"Port-forward exited (code {pf.ExitCode}): {string.Join('\n', pfStderr)}");
        pf.Dispose();
        return 1;
    }
    await Task.Delay(1000);
    try { (await client.GetAsync("/health")).Dispose(); ready = true; }
    catch { }
}
if (!ready)
{
    Err("Port-forward never became ready (15s timeout)");
    pf.Kill();
    pf.Dispose();
    return 1;
}

var failed = false;

async Task AssertHttp(string path, int expectCode = 200, string? label = null)
{
    label ??= path;
    try
    {
        var response = await client.GetAsync(path);
        var code = (int)response.StatusCode;
        if (code == expectCode)
            Ok($"{label} → {code}");
        else
        {
            Err($"{label} → {code} (expected {expectCode})");
            failed = true;
        }
    }
    catch (Exception ex)
    {
        Err($"{label} → {ex.Message}");
        failed = true;
    }
}

await AssertHttp("/health", label: "Health");
await AssertHttp("/alive", label: "Liveness");
await AssertHttp("/api/v1/zones", label: "Zones");
await AssertHttp("/api/v1/waste-types", label: "WasteTypes");
await AssertHttp("/api/v1/containers", label: "Containers");

// Cleanup port-forward
pf.Kill();
pf.WaitForExit();
pf.Dispose();

if (failed)
{
    Err("Some tests failed");
    return 1;
}
Ok("All smoke tests passed");
return 0;

// ── Helpers ──────────────────────────────────────────────
static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) ?? fallback;

static void Info(string msg) => Console.WriteLine($"\x1b[1;34m==> {msg}\x1b[0m");
static void Ok(string msg) => Console.WriteLine($"\x1b[1;32m  ✓ {msg}\x1b[0m");
static void Err(string msg) => Console.WriteLine($"\x1b[1;31m  ✗ {msg}\x1b[0m");
