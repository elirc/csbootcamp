// ─────────────────────────────────────────────────────────────────────────
//  01 · building strings without wasting them — SOLUTION     ★☆☆ warm-up
//  concepts: immutability · StringBuilder · string.Join
//  run: dotnet run 01-building-strings.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  A string never changes. `s += x` allocates a new string and copies
//  both halves, so building n pieces in a loop copies roughly n squared
//  characters and leaves n dead strings for the collector. For two or
//  three pieces that is irrelevant; inside a loop it is the classic
//  performance bug, and StringBuilder (a growable buffer, one final
//  allocation) is the answer.
//  But before StringBuilder, try string.Join. It solves the problem
//  everyone gets wrong by hand — the separator goes BETWEEN items, so
//  there is no trailing comma to trim and no "is this the first
//  iteration?" flag. All three methods here fall out of Join plus a
//  projection, which is why none of them needs a builder.
//  Csv shows the projection carrying a rule: quote only the fields that
//  need it. Real CSV also escapes embedded quotes and handles newlines —
//  this is a teaching-sized version, and the honest advice is to use a
//  real CSV library the moment the data is not yours.
//  Indent splits, maps, and rejoins. Splitting on '\n' and rejoining
//  with '\n' round-trips exactly, including blank lines, which is why
//  the blank-line test passes without a special case.
//  One more tool worth knowing: raw string literals (three quotes) for
//  multi-line text with no escaping, and interpolation with alignment
//  and format specifiers for tables.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string Csv(IEnumerable<string> fields) =>
    string.Join(",", fields.Select(f => f.Contains(',') ? $"\"{f}\"" : f));

static string Repeat(string text, int times, string separator) =>
    string.Join(separator, Enumerable.Repeat(text, times));

static string Indent(string block, int spaces)
{
    var pad = new string(' ', spaces);
    return string.Join("\n", block.Split('\n').Select(line => pad + line));
}

// ─── tests ───────────────────────────────────────────────────────────────

Test("Csv joins plain fields with commas", () =>
    Eq(Csv(new[] { "a", "b" }), "a,b"));

Test("Csv quotes a field containing a comma", () =>
    Eq(Csv(new[] { "a", "b,c" }), "a,\"b,c\""));

Test("Csv of one field has no separator", () => Eq(Csv(new[] { "a" }), "a"));

Test("Csv of nothing is empty", () => Eq(Csv(Array.Empty<string>()), ""));

Test("Repeat repeats with separators between", () =>
    Eq(Repeat("ab", 3, "-"), "ab-ab-ab"));

Test("Repeat once has no separator", () => Eq(Repeat("ab", 1, "-"), "ab"));

Test("Repeat zero times is empty", () => Eq(Repeat("ab", 0, "-"), ""));

Test("Indent indents every line", () => Eq(Indent("x\ny", 2), "  x\n  y"));

Test("Indent indents a blank line too", () => Eq(Indent("x\n\ny", 1), " x\n \n y"));

return Report();
