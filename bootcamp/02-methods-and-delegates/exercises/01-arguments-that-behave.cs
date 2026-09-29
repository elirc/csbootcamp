// ─────────────────────────────────────────────────────────────────────────
//  01 · arguments that behave                               ★☆☆ warm-up
//  concepts: optional arguments · named arguments · params
//  run: dotnet run 01-arguments-that-behave.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Three signatures, each showing a different way C# lets a caller say
//  less:
//
//      Money(12.5m)                    → "EUR 12.50"
//      Money(12.5m, "USD")             → "USD 12.50"
//      Money(12.5m, symbol: "GBP")     → "GBP 12.50"
//
//      Longest("a", "bbb", "cc")       → "bbb"     (params: any count)
//      Longest()                       → ""
//
//      Slug("Hello World")             → "hello-world"
//      Slug("Hello World", separator: '_') → "hello_world"
//
//  hint: an optional parameter needs a compile-time constant default.
//  `params string[] words` lets the caller pass any number of loose
//  arguments — including none, which is an EMPTY array, not null.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string Money(decimal amount, string symbol = "EUR") => throw new NotImplementedException("TODO");

static string Longest(params string[] words) => throw new NotImplementedException("TODO");

static string Slug(string text, char separator = '-') => throw new NotImplementedException("TODO");

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
