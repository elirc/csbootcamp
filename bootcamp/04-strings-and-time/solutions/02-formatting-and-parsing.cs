// ─────────────────────────────────────────────────────────────────────────
//  02 · text in, text out — SOLUTION                            ★★☆ core
//  concepts: format strings · InvariantCulture · TryParse
//  run: dotnet run 02-formatting-and-parsing.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  There are exactly two audiences for formatted text and they want
//  opposite things. A MACHINE wants a stable, culture-free shape it can
//  read back — CultureInfo.InvariantCulture, no grouping separators, and
//  ideally a round-trip format. A HUMAN wants their own conventions —
//  a specific CultureInfo, grouping, currency symbols, local date order.
//  The bug is using the ambient culture for the machine case: the export
//  runs on a German server, writes "1.234,50", and the importer reads
//  1.23450 or throws. It passes every test in London.
//  ForMachines just calls ToString with the invariant culture: a
//  decimal's default format already round-trips and PRESERVES its scale,
//  so 0.10m prints as "0.10" and 1234.5m as "1234.5". A custom format
//  string like "0.0#" would drop that trailing zero — decimal remembers
//  how precise the value was, and double never could.
//  ParseAmount pins the culture too, and uses TryParse so bad input is a
//  value (null) rather than an exception. Returning decimal? makes the
//  failure part of the type: the caller cannot forget to handle it,
//  which an exception lets them do by default.
//  NumberStyles.Number allows a leading sign, decimal point and
//  whitespace but not currency symbols or exponents — narrow it further
//  if the input is meant to be strict.
//  Pad shows the third spelling: PadRight, or an alignment in an
//  interpolated string. Both stop at the width you asked for and neither
//  truncates, which is why the long-value test passes untouched.

#:project ../../_lib/Check/Check.csproj

using System.Globalization;
using static Bootcamp.Check;

static string ForMachines(decimal amount) => amount.ToString(CultureInfo.InvariantCulture);

static string ForHumans(decimal amount) =>
    amount.ToString("N2", CultureInfo.GetCultureInfo("en-GB"));

static decimal? ParseAmount(string? text) =>
    decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
        ? value
        : null;

static string Pad(string text, int width) => text.PadRight(width);

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
