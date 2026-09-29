// progress.cs — your scoreboard. Runs every exercise and marks it:
//
//     dotnet run bootcamp/progress.cs        the whole track
//     dotnet run bootcamp/progress.cs 03     one module
//
//   ☐ todo      you haven't written it yet
//   ◐ started   some tests pass, some are still stubs
//   ✘ failing   you wrote something and it's wrong
//   ✔ done      all green
//   🐛 bug not fixed yet  (hunt modules: red is the starting state)
//
// Nothing is stored: the scoreboard is recomputed from the files every time,
// which is the only source of truth that cannot drift.

#:project _lib/Check/Check.csproj

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

var root = FindBootcampRoot() ?? Directory.GetCurrentDirectory();
var filter = args.Length > 0 ? args[0] : null;

var modules = Directory.GetDirectories(root)
    .Select(d => (Dir: d, Name: Path.GetFileName(d)))
    .Where(m => !m.Name.StartsWith('_') && !m.Name.StartsWith('.'))
    .Where(m => filter is null || m.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
    .OrderBy(m => m.Name)
    .ToList();

var files = new List<(string Module, string File, string Path)>();
foreach (var (dir, name) in modules)
{
    var exercises = Path.Combine(dir, "exercises");
    if (!Directory.Exists(exercises)) continue;
    foreach (var file in Directory.GetFiles(exercises, "*.cs").OrderBy(f => f))
    {
        if (Path.GetFileName(file).StartsWith('_')) continue;
        files.Add((name, Path.GetFileName(file), file));
    }
}

if (files.Count == 0)
{
    Console.Error.WriteLine("no exercises found" + (filter is null ? "" : $" for '{filter}'"));
    return 1;
}

Console.WriteLine($"grading {files.Count} exercises…");
Console.WriteLine();

var marks = new ConcurrentDictionary<string, Mark>();
await Parallel.ForEachAsync(
    files,
    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
    async (f, token) => marks[f.Path] = await GradeAsync(f.Path, token));

var doneTotal = 0;
string? lastModule = null;
foreach (var (module, file, path) in files)
{
    if (module != lastModule)
    {
        var inModule = files.Where(f => f.Module == module).ToList();
        var doneHere = inModule.Count(f => marks[f.Path].Symbol == "✔");
        Console.WriteLine();
        Console.WriteLine($"{module}  {doneHere}/{inModule.Count}" +
            (IsHunt(module) ? "  (hunt module — red is the starting state)" : ""));
        lastModule = module;
    }

    var mark = marks[path];
    Console.WriteLine($"  {mark.Symbol} {file}{(mark.Note is null ? "" : "  " + mark.Note)}");
    if (mark.Symbol == "✔") doneTotal++;
}

var width = 25;
var filled = files.Count == 0 ? 0 : (int)Math.Round((double)doneTotal / files.Count * width);
Console.WriteLine();
Console.WriteLine("  " + new string('█', filled) + new string('░', width - filled) +
    $"  {doneTotal}/{files.Count} exercises ({(files.Count == 0 ? 0 : doneTotal * 100 / files.Count)}%)");

return 0;

// ── helpers ───────────────────────────────────────────────────────────────

static bool IsHunt(string module) =>
    module.Contains("-debug", StringComparison.OrdinalIgnoreCase) ||
    module.EndsWith("-hunts", StringComparison.OrdinalIgnoreCase);

async Task<Mark> GradeAsync(string path, CancellationToken token)
{
    var module = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(path))!);
    var psi = new ProcessStartInfo("dotnet")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WorkingDirectory = Path.GetDirectoryName(path)!,
        Environment = { ["NO_COLOR"] = "1" },
    };
    psi.ArgumentList.Add("run");
    psi.ArgumentList.Add(path);

    using var process = Process.Start(psi)!;
    var stdoutTask = process.StandardOutput.ReadToEndAsync(token);
    _ = process.StandardError.ReadToEndAsync(token);
    await process.WaitForExitAsync(token);
    var stdout = await stdoutTask;

    var match = Regex.Match(stdout, @"#done passed=(\d+) failed=(\d+) todo=(\d+)");
    if (!match.Success) return new Mark("✘", "does not compile or crashed");

    var passed = int.Parse(match.Groups[1].Value);
    var failed = int.Parse(match.Groups[2].Value);
    var todo = int.Parse(match.Groups[3].Value);

    if (failed == 0 && todo == 0) return new Mark("✔", null);
    if (IsHunt(module)) return new Mark("🐛", "(bug not fixed yet)");
    if (todo > 0 && passed == 0 && failed == 0) return new Mark("☐", null);
    if (failed > 0) return new Mark("✘", $"({failed} failing)");
    return new Mark("◐", $"({passed} of {passed + todo} written)");
}

static string? FindBootcampRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null)
    {
        if (Directory.Exists(Path.Combine(dir.FullName, "bootcamp", "_lib", "Check")))
            return Path.Combine(dir.FullName, "bootcamp");
        if (Directory.Exists(Path.Combine(dir.FullName, "_lib", "Check")))
            return dir.FullName;
        dir = dir.Parent;
    }

    return null;
}

record Mark(string Symbol, string? Note);
