// ─────────────────────────────────────────────────────────────────────────
//  05 · strings that compare — SOLUTION                        ★★☆ core
//  concepts: StringComparison · ordinal vs culture · normalising keys
//  run: dotnet run 05-strings-that-compare.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Every string comparison in .NET picks one of two behaviours, and if
//  you do not choose, someone else's culture chooses for you.
//  ORDINAL compares the underlying UTF-16 code units: fast, exact, and
//  identical on every machine. CULTURE-AWARE comparison asks the
//  current locale how letters sort and fold, which is right for text a
//  human reads and wrong for anything a program keys on.
//  Tags, ids, file names, protocol tokens and dictionary keys are all
//  the second kind of string. Use StringComparison.OrdinalIgnoreCase to
//  fold case without inviting the locale in — and remember the famous
//  counterexample: in Turkish, lowercasing 'I' does NOT give 'i', so
//  comparing a.ToLower() with b.ToLower() compares two different
//  strings on a Turkish machine and passes every test on yours.
//  NormalizeKey uses ToLowerInvariant for the same reason: the key you
//  write today may be read by a process in another locale tomorrow.
//  JoinTags shows the composition habit: filter, then join. string.Join
//  puts the separator BETWEEN items only, so filtering first is what
//  keeps ", " from appearing where a blank used to be.
//  Rule of thumb: reach for Ordinal / OrdinalIgnoreCase by default, and
//  only use culture comparison for sorting text you are about to show
//  to a person.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool SameTag(string a, string b) =>
    string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

static string NormalizeKey(string raw) => raw.Trim().ToLowerInvariant();

static bool HasIdPrefix(string value, string prefix) =>
    value.StartsWith(prefix, StringComparison.Ordinal);

static string JoinTags(IEnumerable<string> tags) =>
    string.Join(", ", tags.Where(t => !string.IsNullOrWhiteSpace(t)));

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
