// ─────────────────────────────────────────────────────────────────────────
//  03 · numbers that lie                                    ★☆☆ warm-up
//  concepts: integer division · double vs decimal · rounding
//  run: dotnet run 03-numbers-that-lie.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Three small calculations, each with a trap C# will not warn you
//  about:
//
//      Average(7, 2)            → 3.5    (not 3)
//      WithTax(19.99m, 21m)     → 24.19m (money, rounded to cents)
//      SafeDivide(10, 0)        → 0      (no exception)
//
//  WithTax takes a price and a tax rate in PERCENT, adds the tax, and
//  rounds to two decimal places. A total ending .005 rounds UP — that
//  is what an invoice does, and it is not what .NET does by default.
//
//  hint: `int / int` is integer division no matter what you assign it
//  to. Math.Round has an overload that takes a MidpointRounding.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static double Average(int total, int count) => throw new NotImplementedException("TODO");

static decimal WithTax(decimal price, decimal ratePercent) => throw new NotImplementedException("TODO");

static int SafeDivide(int a, int b) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("Average keeps the fraction", () => Approx(Average(7, 2), 3.5));

Test("Average of an exact division is still right", () => Approx(Average(10, 5), 2.0));

Test("WithTax adds the percentage", () => Eq(WithTax(100m, 21m), 121.00m));

Test("WithTax rounds to cents", () => Eq(WithTax(19.99m, 21m), 24.19m));

Test("WithTax rounds a half cent away from zero", () => Eq(WithTax(0.05m, 50m), 0.08m));

Test("SafeDivide divides normally", () => Eq(SafeDivide(10, 2), 5));

Test("SafeDivide returns 0 instead of throwing", () => Eq(SafeDivide(10, 0), 0));

return Report();
