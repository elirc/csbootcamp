// ─────────────────────────────────────────────────────────────────────────
//  01 · building strings without wasting them                ★☆☆ warm-up
//  concepts: immutability · StringBuilder · string.Join
//  run: dotnet run 01-building-strings.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Strings are immutable, so every += builds a whole new one. Three
//  small builders, each with a rule that makes concatenation in a loop
//  the wrong tool:
//
//      Csv(["a", "b,c"])          → a,"b,c"     (quote a field with a comma)
//      Repeat("ab", 3, "-")       → ab-ab-ab
//      Indent("x\ny", 2)          → "  x\n  y"  (every line, blank ones too)
//
//  Indent must use "\n" as the line separator so the tests behave the
//  same on every machine.
//
//  hint: string.Join handles the separator-between-items problem for
//  you — no trailing separator to trim. StringBuilder is for the cases
//  Join cannot express, and Split(char) plus Join is often shorter than
//  either.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string Csv(IEnumerable<string> fields) => throw new NotImplementedException("TODO");

static string Repeat(string text, int times, string separator) => throw new NotImplementedException("TODO");

static string Indent(string block, int spaces) => throw new NotImplementedException("TODO");

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
