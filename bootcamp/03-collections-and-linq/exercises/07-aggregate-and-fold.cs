// ─────────────────────────────────────────────────────────────────────────
//  07 · folding a sequence into an answer                       ★★☆ core
//  concepts: Aggregate · accumulators · seeds
//  run: dotnet run 07-aggregate-and-fold.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Sum and Count are folds with the boring accumulator. Here are three
//  with interesting ones:
//
//      RunningTotals([1,2,3])        → [1, 3, 6]
//      Tally(["w","l","w"])          → (Wins: 2, Losses: 1)
//      LongestStreak(["w","w","l","w"]) → 2
//
//  A streak is the longest run of consecutive "w". Every one of these
//  must work on an empty sequence without special-casing it at the top
//  — pick a seed that makes the empty case fall out.
//
//  hint: Aggregate(seed, (accumulator, item) => nextAccumulator). The
//  accumulator can be a tuple when you need to carry two numbers, which
//  is exactly what a streak needs: the best so far and the current run.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<int> RunningTotals(IEnumerable<int> numbers) => throw new NotImplementedException("TODO");

static (int Wins, int Losses) Tally(IEnumerable<string> results) => throw new NotImplementedException("TODO");

static int LongestStreak(IEnumerable<string> results) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("RunningTotals accumulates", () =>
    Eq(RunningTotals(new[] { 1, 2, 3 }), new List<int> { 1, 3, 6 }));

Test("RunningTotals handles negatives", () =>
    Eq(RunningTotals(new[] { 5, -2 }), new List<int> { 5, 3 }));

Test("RunningTotals of nothing is empty", () =>
    Eq(RunningTotals(Array.Empty<int>()).Count, 0));

Test("Tally counts wins and losses", () =>
    Eq(Tally(new[] { "w", "l", "w" }), (2, 1)));

Test("Tally ignores anything else", () =>
    Eq(Tally(new[] { "w", "draw", "l" }), (1, 1)));

Test("Tally of nothing is zero and zero", () =>
    Eq(Tally(Array.Empty<string>()), (0, 0)));

Test("LongestStreak finds the longest run", () =>
    Eq(LongestStreak(new[] { "w", "w", "l", "w" }), 2));

Test("LongestStreak counts a run that ends the sequence", () =>
    Eq(LongestStreak(new[] { "l", "w", "w", "w" }), 3));

Test("LongestStreak of no wins is zero", () =>
{
    Eq(LongestStreak(new[] { "l", "l" }), 0);
    Eq(LongestStreak(Array.Empty<string>()), 0);
});

return Report();
