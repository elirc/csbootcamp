// ─────────────────────────────────────────────────────────────────────────
//  01 · pick the right container                            ★☆☆ warm-up
//  concepts: List · Dictionary · HashSet · lookup cost
//  run: dotnet run 01-pick-the-collection.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Three jobs. Each one is easy with the right container and quadratic
//  with the wrong one.
//
//      Unique(["a","b","a"])       → ["a", "b"]   (first-seen order kept)
//      Counts(["a","b","a"])       → { a: 2, b: 1 }
//      AnyDuplicates([1, 2, 1])    → true
//
//  Unique must keep the order the items first appeared in — the naive
//  "sort and dedupe" answer is wrong here.
//
//  hint: HashSet<T>.Add returns FALSE when the item was already there,
//  which answers "have I seen this?" and records it in one step and one
//  hash lookup. Dictionary has CollectionsMarshal-free helpers too:
//  TryGetValue, GetValueOrDefault, and an indexer that creates on write.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> Unique(IEnumerable<string> tags) => throw new NotImplementedException("TODO");

static Dictionary<string, int> Counts(IEnumerable<string> words) => throw new NotImplementedException("TODO");

static bool AnyDuplicates(IEnumerable<int> ids) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("Unique drops repeats", () =>
    Eq(Unique(new[] { "a", "b", "a" }), new List<string> { "a", "b" }));

Test("Unique keeps first-seen order", () =>
    Eq(Unique(new[] { "c", "a", "c", "b" }), new List<string> { "c", "a", "b" }));

Test("Unique of nothing is nothing", () => Eq(Unique(Array.Empty<string>()).Count, 0));

Test("Counts tallies each word", () =>
{
    var counts = Counts(new[] { "a", "b", "a" });
    Eq(counts["a"], 2);
    Eq(counts["b"], 1);
});

Test("Counts has one entry per distinct word", () =>
    Eq(Counts(new[] { "a", "a", "a" }).Count, 1));

Test("Counts of nothing is empty", () => Eq(Counts(Array.Empty<string>()).Count, 0));

Test("AnyDuplicates finds a repeat", () => Ok(AnyDuplicates(new[] { 1, 2, 1 })));

Test("AnyDuplicates is false for distinct ids", () => Ok(!AnyDuplicates(new[] { 1, 2, 3 })));

Test("AnyDuplicates is false for nothing", () => Ok(!AnyDuplicates(Array.Empty<int>())));

return Report();
