// ─────────────────────────────────────────────────────────────────────────
//  04 · nullable without fear — SOLUTION                       ★★☆ core
//  concepts: nullable reference types · ?? · ?. · blank strings
//  run: dotnet run 04-nullable-without-fear.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  The lesson is that C# has THREE states where you were thinking of
//  two: null, blank, and present. `first != null` is not the check the
//  product wants — a name of "  " passes it and prints as nothing. Ask
//  `string.IsNullOrWhiteSpace` and the three states collapse into the
//  two you actually branch on.
//  DisplayName then becomes: keep the parts that are present, join with
//  a space, and fall back when nothing survived. Building a list and
//  joining beats nested if/else because the rule ("skip the missing
//  ones") is stated once instead of once per combination.
//  TotalLength shows `?.` and `??` doing their real job: `v?.Length`
//  is null when v is null rather than throwing, and `?? 0` turns the
//  absence into the identity element for addition. That pairing — null
//  -conditional to skip, null-coalescing to default — is most of what
//  nullable code looks like.
//  Nullable reference types are compile-time only. `string?` and
//  `string` are the same type at runtime; the ? is a promise the
//  compiler checks and a caller in an older project can break. Guard
//  at the boundary anyway.
//  Classic wrong turns: `first + " " + last` (double spaces and
//  trailing spaces when a part is missing) and the null-forgiving `!`
//  operator used to silence a warning you have not actually handled.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string DisplayName(string? first, string? last)
{
    var parts = new[] { first, last }.Where(p => !string.IsNullOrWhiteSpace(p));
    var name = string.Join(" ", parts).Trim();
    return name.Length == 0 ? "(unknown)" : name;
}

static int TotalLength(IEnumerable<string?> values) =>
    values.Sum(v => v?.Length ?? 0);

static string FirstNonBlank(IEnumerable<string?> candidates) =>
    candidates.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c)) ?? "";

// ─── tests ───────────────────────────────────────────────────────────────

Test("both names present", () => Eq(DisplayName("Ada", "Lovelace"), "Ada Lovelace"));

Test("one name missing", () =>
{
    Eq(DisplayName("Ada", null), "Ada");
    Eq(DisplayName(null, "Lovelace"), "Lovelace");
});

Test("blank counts as missing", () => Eq(DisplayName("  ", ""), "(unknown)"));

Test("nothing at all", () => Eq(DisplayName(null, null), "(unknown)"));

Test("TotalLength treats null as zero length", () =>
    Eq(TotalLength(new string?[] { "ab", null, "c" }), 3));

Test("TotalLength of an empty sequence is zero", () =>
    Eq(TotalLength(Array.Empty<string?>()), 0));

Test("FirstNonBlank skips nulls and whitespace", () =>
    Eq(FirstNonBlank(new string?[] { null, "   ", "x", "y" }), "x"));

Test("FirstNonBlank falls back to an empty string", () =>
    Eq(FirstNonBlank(new string?[] { null, "" }), ""));

return Report();
