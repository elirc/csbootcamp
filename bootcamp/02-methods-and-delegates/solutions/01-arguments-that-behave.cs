// ─────────────────────────────────────────────────────────────────────────
//  01 · arguments that behave — SOLUTION                    ★☆☆ warm-up
//  concepts: optional arguments · named arguments · params
//  run: dotnet run 01-arguments-that-behave.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Optional arguments are compiled into the CALL SITE, not the method.
//  That is a real consequence, not trivia: if a library changes a
//  default, callers keep the old value until they are recompiled. Inside
//  one solution it does not matter; across a package boundary it is a
//  reason to prefer an overload.
//  Named arguments let the caller re-order and, more importantly,
//  document. `Slug(text, separator: '_')` at the call site beats
//  `Slug(text, '_')` in every review, because a bare char tells the
//  reader nothing.
//  `params` receives an EMPTY array when the caller passes nothing, so
//  Longest can use LINQ without a null check — but note that an empty
//  sequence has no maximum, which is why the aggregate starts from ""
//  rather than calling MaxBy on nothing.
//  Formatting: "F2" and the ToString overloads are culture-aware, so a
//  German machine would print 12,50. CultureInfo.InvariantCulture is
//  what you want for anything a machine will read back.

#:project ../../_lib/Check/Check.csproj

using System.Globalization;
using static Bootcamp.Check;

static string Money(decimal amount, string symbol = "EUR") =>
    $"{symbol} {amount.ToString("F2", CultureInfo.InvariantCulture)}";

static string Longest(params string[] words) =>
    words.Aggregate("", (best, word) => word.Length > best.Length ? word : best);

static string Slug(string text, char separator = '-') =>
    string.Join(separator, text.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));

// ─── tests ───────────────────────────────────────────────────────────────

Test("Money defaults to euros and two decimals", () => Eq(Money(12.5m), "EUR 12.50"));

Test("Money takes another symbol positionally", () => Eq(Money(12.5m, "USD"), "USD 12.50"));

Test("Money takes another symbol by name", () => Eq(Money(12.5m, symbol: "GBP"), "GBP 12.50"));

Test("Money pads a whole number", () => Eq(Money(3m), "EUR 3.00"));

Test("Longest picks the longest word", () => Eq(Longest("a", "bbb", "cc"), "bbb"));

Test("Longest with no arguments returns empty", () => Eq(Longest(), ""));

Test("Longest keeps the first of a tie", () => Eq(Longest("aa", "bb"), "aa"));

Test("Slug lowercases and joins with a dash", () => Eq(Slug("Hello World"), "hello-world"));

Test("Slug takes another separator by name", () => Eq(Slug("Hello World", separator: '_'), "hello_world"));

return Report();
