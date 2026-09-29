// ─────────────────────────────────────────────────────────────────────────
//  05 · strings that compare                                   ★★☆ core
//  concepts: StringComparison · ordinal vs culture · normalising keys
//  run: dotnet run 05-strings-that-compare.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A tag system. Tags are compared case-insensitively but are otherwise
//  exact — trailing spaces are a different tag, because a user typed
//  them and a dictionary key does not forgive:
//
//      SameTag("Sale", "sale")      → true
//      SameTag("sale", "sale ")     → false
//      NormalizeKey("  Ada  ")      → "ada"
//      HasIdPrefix("ORD-42", "ORD") → true
//      JoinTags(["a", "", "b"])     → "a, b"
//
//  Every comparison you write must say WHICH comparison it means.
//
//  hint: string.Equals and StartsWith both take a StringComparison.
//  For a key you will store and look up later, invariant beats current
//  culture — the machine that writes it may not be the one that reads.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool SameTag(string a, string b) => throw new NotImplementedException("TODO");

static string NormalizeKey(string raw) => throw new NotImplementedException("TODO");

static bool HasIdPrefix(string value, string prefix) => throw new NotImplementedException("TODO");

static string JoinTags(IEnumerable<string> tags) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("SameTag ignores case", () => Ok(SameTag("Sale", "sale")));

Test("SameTag does not ignore spaces", () => Ok(!SameTag("sale", "sale ")));

Test("SameTag matches identical tags", () => Ok(SameTag("black-friday", "black-friday")));

Test("NormalizeKey trims and lowercases", () => Eq(NormalizeKey("  Ada  "), "ada"));

Test("NormalizeKey leaves an already-clean key alone", () => Eq(NormalizeKey("ada"), "ada"));

Test("HasIdPrefix matches the start", () => Ok(HasIdPrefix("ORD-42", "ORD")));

Test("HasIdPrefix is case-sensitive", () => Ok(!HasIdPrefix("ord-42", "ORD")));

Test("JoinTags skips blanks", () => Eq(JoinTags(new[] { "a", "", "  ", "b" }), "a, b"));

return Report();
