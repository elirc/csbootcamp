// ─────────────────────────────────────────────────────────────────────────
//  02 · text in, text out, and the culture in between           ★★☆ core
//  concepts: format strings · InvariantCulture · TryParse
//  run: dotnet run 02-formatting-and-parsing.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  The same number is "1234.5" to a machine, "1,234.50" to a British
//  reader and "1.234,50" to a German one. Which one you produce is a
//  choice, and not making it is how a CSV export becomes unreadable by
//  the importer that wrote it.
//
//      ForMachines(1234.5m)        → "1234.5"     (invariant, round-trip)
//      ForHumans(1234.5m)          → "1,234.50"   (en-GB, two decimals)
//      ParseAmount("1234.5")       → 1234.5m
//      ParseAmount("nonsense")     → null
//      Pad("ab", 5)                → "ab   "      (left-aligned, width 5)
//
//  ParseAmount must accept machine format regardless of the machine's
//  own culture, and must never throw.
//
//  hint: every ToString and Parse has an IFormatProvider overload.
//  CultureInfo.InvariantCulture is the one that never changes.
//  decimal.TryParse takes NumberStyles and a culture.

#:project ../../_lib/Check/Check.csproj

using System.Globalization;
using static Bootcamp.Check;

static string ForMachines(decimal amount) => throw new NotImplementedException("TODO");

static string ForHumans(decimal amount) => throw new NotImplementedException("TODO");

static decimal? ParseAmount(string? text) => throw new NotImplementedException("TODO");

static string Pad(string text, int width) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("ForMachines uses a dot and no grouping", () => Eq(ForMachines(1234.5m), "1234.5"));

Test("ForMachines keeps the exact value", () => Eq(ForMachines(0.10m), "0.10"));

Test("ForHumans groups and pads to two decimals", () => Eq(ForHumans(1234.5m), "1,234.50"));

Test("ForHumans handles a small number", () => Eq(ForHumans(0.5m), "0.50"));

Test("ParseAmount reads machine format", () => Eq(ParseAmount("1234.5"), 1234.5m));

Test("ParseAmount rejects junk without throwing", () => Eq(ParseAmount("nonsense"), null));

Test("ParseAmount rejects null and blank", () =>
{
    Eq(ParseAmount(null), null);
    Eq(ParseAmount("   "), null);
});

Test("ParseAmount round-trips ForMachines", () => Eq(ParseAmount(ForMachines(12.34m)), 12.34m));

Test("Pad left-aligns to the width", () => Eq(Pad("ab", 5), "ab   "));

Test("Pad leaves a long value alone", () => Eq(Pad("abcdef", 3), "abcdef"));

return Report();
