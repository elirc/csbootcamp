// ─────────────────────────────────────────────────────────────────────────
//  02 · the four ways to read a dictionary — SOLUTION       ★☆☆ warm-up
//  concepts: TryGetValue · GetValueOrDefault · building an index
//  run: dotnet run 02-dictionary-patterns.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  The four reads, and when each is right:
//    map[key]                  throws if missing — use when absence is a bug
//    map.TryGetValue(k, out v) one probe, tells you and gives you the value
//    map.GetValueOrDefault(k)  one probe, absence becomes default(T)
//    map.ContainsKey(k)        a probe that answers only half the question
//  `if (map.ContainsKey(k)) use(map[k]);` hashes the key TWICE and, in
//  concurrent code, can be wrong between the two lines. TryGetValue is
//  the same code with one lookup and no race.
//  IndexByFirstLetter is the manual GroupBy, and the shape — try to get
//  the bucket, create it if it is not there, then add — is the same
//  shape you will write for caches, adjacency lists and multi-maps.
//  Note the assignment `map[key] = list` after creating: the indexer
//  CREATES on write, which is the asymmetry with the throwing read.
//  Invert says out loud that the last duplicate wins. The alternative,
//  ToDictionary(v => v.Value, v => v.Key), throws ArgumentException on a
//  duplicate — fine when duplicates are impossible, a production
//  incident when they are merely unlikely. Deciding explicitly is the
//  lesson; the exception is not a decision.
//  A dictionary has no order. It looks insertion-ordered in small tests
//  and stops being so after a removal — never rely on it.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string Lookup(Dictionary<string, string> map, string key) =>
    map.GetValueOrDefault(key, "");

static Dictionary<char, List<string>> IndexByFirstLetter(IEnumerable<string> words)
{
    var index = new Dictionary<char, List<string>>();
    foreach (var word in words)
    {
        if (word.Length == 0) continue;

        var letter = word[0];
        if (!index.TryGetValue(letter, out var bucket))
        {
            bucket = new List<string>();
            index[letter] = bucket;
        }

        bucket.Add(word);
    }

    return index;
}

static Dictionary<string, string> Invert(Dictionary<string, string> map)
{
    var inverted = new Dictionary<string, string>();
    foreach (var (key, value) in map)
    {
        inverted[value] = key; // last one wins, deliberately
    }

    return inverted;
}

// ─── tests ───────────────────────────────────────────────────────────────

Test("Lookup finds a present key", () =>
    Eq(Lookup(new Dictionary<string, string> { ["a"] = "1" }, "a"), "1"));

Test("Lookup returns empty for a missing key", () =>
    Eq(Lookup(new Dictionary<string, string>(), "nope"), ""));

Test("IndexByFirstLetter groups the words", () =>
{
    var index = IndexByFirstLetter(new[] { "ant", "bee", "ape" });
    Eq(index['a'], new List<string> { "ant", "ape" });
    Eq(index['b'], new List<string> { "bee" });
});

Test("IndexByFirstLetter has one key per distinct letter", () =>
    Eq(IndexByFirstLetter(new[] { "ant", "ape" }).Count, 1));

Test("IndexByFirstLetter keeps the input order inside a group", () =>
    Eq(IndexByFirstLetter(new[] { "ape", "ant" })['a'], new List<string> { "ape", "ant" }));

Test("IndexByFirstLetter of nothing is empty", () =>
    Eq(IndexByFirstLetter(Array.Empty<string>()).Count, 0));

Test("Invert swaps keys and values", () =>
{
    var inverted = Invert(new Dictionary<string, string> { ["a"] = "1", ["b"] = "2" });
    Eq(inverted["1"], "a");
    Eq(inverted["2"], "b");
});

Test("Invert keeps the last of a duplicated value", () =>
{
    var inverted = Invert(new Dictionary<string, string> { ["a"] = "1", ["b"] = "1" });
    Eq(inverted.Count, 1);
    Eq(inverted["1"], "b");
});

return Report();
