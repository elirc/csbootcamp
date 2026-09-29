// ─────────────────────────────────────────────────────────────────────────
//  03 · numbers that lie — SOLUTION                         ★☆☆ warm-up
//  concepts: integer division · double vs decimal · rounding
//  run: dotnet run 03-numbers-that-lie.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Average: `total / count` where both are int is INTEGER division —
//  7 / 2 is 3, and the fraction is gone before the double ever sees it.
//  The assignment target does not change how the division is done. Cast
//  one operand first: `(double)total / count`.
//  WithTax: money is decimal, never double. `decimal` is base 10, so
//  0.1m + 0.2m is exactly 0.3m, while 0.1 + 0.2 as doubles is
//  0.30000000000000004. The other half is the rounding MODE: .NET's
//  default is banker's rounding (MidpointRounding.ToEven), so
//  Math.Round(0.075m, 2) gives 0.08 but Math.Round(0.045m, 2) gives
//  0.04 — correct for statistics, wrong for an invoice. Say what you
//  mean: MidpointRounding.AwayFromZero.
//  SafeDivide: integer division by zero throws DivideByZeroException —
//  unlike double division, which quietly gives Infinity. Guard the
//  denominator.
//  The classic wrong turn is writing `double Average(int a, int b) =>
//  a / b;` and testing it with 10 and 5, which passes. Every
//  integer-division bug in production was tested with numbers that
//  happened to divide evenly.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static double Average(int total, int count) => (double)total / count;

static decimal WithTax(decimal price, decimal ratePercent) =>
    Math.Round(price * (1 + ratePercent / 100m), 2, MidpointRounding.AwayFromZero);

static int SafeDivide(int a, int b) => b == 0 ? 0 : a / b;

// ─── tests ───────────────────────────────────────────────────────────────

Test("Average keeps the fraction", () => Approx(Average(7, 2), 3.5));

Test("Average of an exact division is still right", () => Approx(Average(10, 5), 2.0));

Test("WithTax adds the percentage", () => Eq(WithTax(100m, 21m), 121.00m));

Test("WithTax rounds to cents", () => Eq(WithTax(19.99m, 21m), 24.19m));

Test("WithTax rounds a half cent away from zero", () => Eq(WithTax(0.05m, 50m), 0.08m));

Test("SafeDivide divides normally", () => Eq(SafeDivide(10, 2), 5));

Test("SafeDivide returns 0 instead of throwing", () => Eq(SafeDivide(10, 0), 0));

return Report();
