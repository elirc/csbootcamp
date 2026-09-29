// ─────────────────────────────────────────────────────────────────────────
//  01 · pick the right container — SOLUTION                 ★☆☆ warm-up
//  concepts: List · Dictionary · HashSet · lookup cost
//  run: dotnet run 01-pick-the-collection.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  All three answers are the same answer: a hash-based container turns
//  "have I seen this?" from a scan into a single probe. The version
//  everyone writes first — `if (!result.Contains(tag))` on a List — is
//  O(n) per item and O(n squared) overall, which is invisible on the
//  three-item test and painful on the fifty-thousand-row import.
//  HashSet.Add doing double duty (adding AND telling you whether it was
//  new) is the trick worth memorising: it collapses check-then-act into
//  one operation, which also removes the race in concurrent code.
//  Unique keeps insertion order because it iterates the source and uses
//  the set only as a filter. A HashSet itself has NO defined order —
//  never enumerate one and rely on what comes out. Distinct() from LINQ
//  does exactly this and is what you would ship; writing it once by hand
//  is how you know what it costs.
//  Counts uses GetValueOrDefault so the first sighting starts from zero
//  without a Contains check. The Dictionary indexer creates on write and
//  throws on read of a missing key — an asymmetry that surprises people
//  exactly once.
//  Cost table worth carrying: List indexing O(1), List.Contains O(n),
//  Dictionary and HashSet lookup O(1) average, and both of those need a
//  sane GetHashCode on the key type — which you get free from records
//  and value tuples, and must write by hand for a class.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> Unique(IEnumerable<string> tags)
{
    var seen = new HashSet<string>();
    var result = new List<string>();
    foreach (var tag in tags)
    {
        if (seen.Add(tag)) result.Add(tag);
    }

    return result;
}

static Dictionary<string, int> Counts(IEnumerable<string> words)
{
    var counts = new Dictionary<string, int>();
    foreach (var word in words)
    {
        counts[word] = counts.GetValueOrDefault(word) + 1;
    }

    return counts;
}

static bool AnyDuplicates(IEnumerable<int> ids)
{
    var seen = new HashSet<int>();
    return ids.Any(id => !seen.Add(id));
}

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
