// ─────────────────────────────────────────────────────────────────────────
//  02 · the four ways to read a dictionary                  ★☆☆ warm-up
//  concepts: TryGetValue · GetValueOrDefault · building an index
//  run: dotnet run 02-dictionary-patterns.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A Dictionary read has four spellings and they behave differently on
//  a missing key. This exercise makes you use the right one three
//  times.
//
//      Lookup(map, "missing")           → ""      (never throws)
//      IndexByFirstLetter(["ant","bee","ape"])
//                                       → { a: ["ant","ape"], b: ["bee"] }
//      Invert({ a: "1", b: "2" })       → { 1: "a", 2: "b" }
//
//  Invert on duplicate values keeps the LAST one seen — say so in the
//  code rather than letting an exception decide.
//
//  hint: the indexer THROWS on a missing read and CREATES on a write.
//  TryGetValue answers both questions in one probe.
//  CollectionsMarshal aside, `if (!map.TryGetValue(k, out var list)) {
//  list = new(); map[k] = list; }` is the standard grouping shape.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string Lookup(Dictionary<string, string> map, string key) => throw new NotImplementedException("TODO");

static Dictionary<char, List<string>> IndexByFirstLetter(IEnumerable<string> words) =>
    throw new NotImplementedException("TODO");

static Dictionary<string, string> Invert(Dictionary<string, string> map) =>
    throw new NotImplementedException("TODO");

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
