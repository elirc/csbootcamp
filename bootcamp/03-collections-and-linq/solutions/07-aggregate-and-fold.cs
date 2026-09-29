// ─────────────────────────────────────────────────────────────────────────
//  07 · folding a sequence into an answer — SOLUTION            ★★☆ core
//  concepts: Aggregate · accumulators · seeds
//  run: dotnet run 07-aggregate-and-fold.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Every fold is the same three decisions: what the accumulator holds,
//  what the seed is, and how one item updates it. Get those right and
//  the code writes itself; get the SEED right and the empty case
//  disappears without an if.
//  Tally carries two numbers, so the accumulator is a tuple seeded with
//  (0, 0). LongestStreak carries two as well — the best run seen and the
//  run in progress — because you cannot know the best without tracking
//  the current, and you cannot track the current without resetting it.
//  Seeding with (0, 0) is what makes both the empty sequence and the
//  all-losses case answer 0 for free.
//  The streak fold ends with Math.Max over the pair, which is the part
//  people forget: the last run has no loss after it to trigger the
//  comparison, so a sequence ending in wins reports the previous best.
//  That is the bug the "run that ends the sequence" test exists to
//  catch, and it is the same off-by-one shape as forgetting to flush a
//  buffer at end of file.
//  RunningTotals is a SCAN rather than a fold — it emits an answer per
//  item instead of one at the end. LINQ has no Scan, so the explicit
//  loop is the honest version; Aggregate with a list accumulator works
//  but allocates and reads worse. Knowing when NOT to use Aggregate is
//  part of using it well: if a plain foreach is clearer, write the
//  foreach.
//  One warning: Aggregate with no seed uses the first element as the
//  seed and THROWS on an empty sequence. Always pass a seed unless you
//  have already proved the sequence is non-empty.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<int> RunningTotals(IEnumerable<int> numbers)
{
    var totals = new List<int>();
    var sum = 0;
    foreach (var n in numbers)
    {
        sum += n;
        totals.Add(sum);
    }

    return totals;
}

static (int Wins, int Losses) Tally(IEnumerable<string> results) =>
    results.Aggregate(
        (Wins: 0, Losses: 0),
        (tally, result) => result switch
        {
            "w" => (tally.Wins + 1, tally.Losses),
            "l" => (tally.Wins, tally.Losses + 1),
            _ => tally,
        });

static int LongestStreak(IEnumerable<string> results)
{
    var (best, current) = results.Aggregate(
        (Best: 0, Current: 0),
        (state, result) => result == "w"
            ? (Math.Max(state.Best, state.Current + 1), state.Current + 1)
            : (state.Best, 0));

    return Math.Max(best, current);
}

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
