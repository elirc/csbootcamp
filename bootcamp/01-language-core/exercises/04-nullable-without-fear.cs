// ─────────────────────────────────────────────────────────────────────────
//  04 · nullable without fear                                  ★★☆ core
//  concepts: nullable reference types · ?? · ?. · blank strings
//  run: dotnet run 04-nullable-without-fear.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Three helpers that all deal with "the value might not be there", the
//  single most common shape in line-of-business code:
//
//      DisplayName("Ada", "Lovelace")  → "Ada Lovelace"
//      DisplayName("Ada", null)        → "Ada"
//      DisplayName(null, null)         → "(unknown)"
//      DisplayName("  ", "")           → "(unknown)"
//
//      TotalLength(["ab", null, "c"])  → 3
//      FirstNonBlank([null, " ", "x"]) → "x"   (none found → "")
//
//  A value that is present but blank counts as missing everywhere here.
//  That is a product decision, and it is the one users expect.
//
//  hint: string.IsNullOrWhiteSpace answers "missing?" in one call.
//  `??` supplies a fallback; `?.` skips a member access on null.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string DisplayName(string? first, string? last) => throw new NotImplementedException("TODO");

static int TotalLength(IEnumerable<string?> values) => throw new NotImplementedException("TODO");

static string FirstNonBlank(IEnumerable<string?> candidates) => throw new NotImplementedException("TODO");

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
