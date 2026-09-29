// verify.cs — the maintainer check. Runs every file and enforces the contract:
//
//     dotnet run bootcamp/verify.cs        every module
//     dotnet run bootcamp/verify.cs 03     one module (prefix match)
//
//   solutions/*.cs  →  0 failed AND 0 todo   (a solution must be complete)
//   exercises/*.cs  →  0 failed AND >0 todo  (a starter must be untouched)
//
// Hunt modules invert the exercise rule: their directory name contains
// "-debug" or ends in "-hunts", the file ships finished and broken, and the
// contract becomes ">0 failed AND 0 todo" — an exercise that already passes
// has no bug to find and is reported broken.
//
// Exit code 1 if anything is broken. Runs files in parallel; the first pass
// is slow because every file is compiled, later passes hit the build cache.

#:project _lib/Check/Check.csproj

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

var here = AppContext.BaseDirectory;
var root = FindBootcampRoot() ?? Directory.GetCurrentDirectory();
var filter = args.Length > 0 ? args[0] : null;
var started = Stopwatch.StartNew();

var jobs = new List<Job>();
foreach (var moduleDir in Directory.GetDirectories(root).OrderBy(d => d))
{
    var module = Path.GetFileName(moduleDir);
    if (module.StartsWith('_') || module.StartsWith('.')) continue;
    if (filter is not null && !module.StartsWith(filter, StringComparison.OrdinalIgnoreCase)) continue;

    foreach (var kind in new[] { "exercises", "solutions" })
    {
        var dir = Path.Combine(moduleDir, kind);
        if (!Directory.Exists(dir)) continue;
        foreach (var file in Directory.GetFiles(dir, "*.cs").OrderBy(f => f))
        {
            if (Path.GetFileName(file).StartsWith('_')) continue;
            jobs.Add(new Job(module, kind, Path.GetFileName(file), file));
        }
    }
}

if (jobs.Count == 0)
{
    Console.Error.WriteLine($"no exercise files found under {root}" +
        (filter is null ? "" : $" matching '{filter}'"));
    return 1;
}

Console.WriteLine($"running {jobs.Count} files (parallel {Environment.ProcessorCount})…");

var results = new ConcurrentDictionary<string, Outcome>();
await Parallel.ForEachAsync(
    jobs,
    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
    async (job, token) => results[job.Path] = await RunAsync(job, token));

var ordered = jobs.Select(j => (Job: j, Outcome: results[j.Path])).ToList();

string? lastModule = null;
foreach (var (job, outcome) in ordered)
{
    if (job.Module != lastModule)
    {
        Console.WriteLine();
        Console.WriteLine(job.Module + (IsHunt(job.Module) ? "  (hunt module — exercises ship red)" : ""));
        lastModule = job.Module;
    }

    var tag = outcome.Broken is null ? "ok" : $"BROKEN — {outcome.Broken}";
    Console.WriteLine($"  {(outcome.Broken is null ? "✔" : "✘")} {job.Kind}/{job.File}  {tag}");
    if (outcome.Broken is not null && outcome.Detail.Length > 0)
    {
        foreach (var line in outcome.Detail.Split('\n').TakeLast(8))
        {
            if (line.Trim().Length > 0) Console.WriteLine("      │ " + line.TrimEnd());
        }
    }
}

var broken = ordered.Count(r => r.Outcome.Broken is not null);
var tests = ordered.Sum(r => r.Outcome.Passed + r.Outcome.Failed + r.Outcome.Todo);

Console.WriteLine();
Console.WriteLine($"{ordered.Count} files · {tests} tests · {broken} broken · " +
    $"{started.Elapsed.TotalSeconds:F1}s");

return broken > 0 ? 1 : 0;

// ── helpers ───────────────────────────────────────────────────────────────

static bool IsHunt(string module) =>
    module.Contains("-debug", StringComparison.OrdinalIgnoreCase) ||
    module.EndsWith("-hunts", StringComparison.OrdinalIgnoreCase);

async Task<Outcome> RunAsync(Job job, CancellationToken token)
{
    var psi = new ProcessStartInfo("dotnet")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WorkingDirectory = Path.GetDirectoryName(job.Path)!,
        Environment = { ["NO_COLOR"] = "1" },
    };
    psi.ArgumentList.Add("run");
    psi.ArgumentList.Add(job.Path);

    using var process = Process.Start(psi)!;
    var stdoutTask = process.StandardOutput.ReadToEndAsync(token);
    var stderrTask = process.StandardError.ReadToEndAsync(token);
    await process.WaitForExitAsync(token);
    var stdout = await stdoutTask;
    var stderr = await stderrTask;

    var match = Regex.Match(stdout, @"#done passed=(\d+) failed=(\d+) todo=(\d+)");
    if (!match.Success)
    {
        var detail = (stderr + stdout).Trim();
        return new Outcome(0, 0, 0, "crashed (no #done line)",
            detail.Length > 900 ? detail[^900..] : detail);
    }

    var passed = int.Parse(match.Groups[1].Value);
    var failed = int.Parse(match.Groups[2].Value);
    var todo = int.Parse(match.Groups[3].Value);
    var total = passed + failed + todo;
    var huntExercise = job.Kind == "exercises" && IsHunt(job.Module);

    string? broken = null;
    if (total == 0) broken = "no tests ran";
    else if (job.Kind == "solutions" && failed > 0) broken = $"{failed} failing test(s)";
    else if (job.Kind == "solutions" && todo > 0) broken = $"{todo} TODO left in a solution";
    else if (huntExercise && failed == 0) broken = "hunt exercise already passes (no bug to find)";
    else if (huntExercise && todo > 0) broken = $"{todo} TODO in a hunt exercise (it must ship complete)";
    else if (!huntExercise && job.Kind == "exercises" && failed > 0)
        broken = $"{failed} failing test(s) in a starter";
    else if (!huntExercise && job.Kind == "exercises" && todo == 0)
        broken = "nothing left to do — is the stub missing?";

    return new Outcome(passed, failed, todo, broken, broken is null ? "" : stdout);
}

static string? FindBootcampRoot()
{
    // The tool may be launched from anywhere; walk up looking for _lib/Check.
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null)
    {
        var candidate = Path.Combine(dir.FullName, "bootcamp", "_lib", "Check");
        if (Directory.Exists(candidate)) return Path.Combine(dir.FullName, "bootcamp");
        if (Directory.Exists(Path.Combine(dir.FullName, "_lib", "Check"))) return dir.FullName;
        dir = dir.Parent;
    }

    return null;
}

record Job(string Module, string Kind, string File, string Path);

record Outcome(int Passed, int Failed, int Todo, string? Broken, string Detail);
