// ─────────────────────────────────────────────────────────────────────────
//  06 · say it with patterns — SOLUTION                        ★★☆ core
//  concepts: switch expressions · property patterns · relational patterns
//  run: dotnet run 06-pattern-matching.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  A switch expression is a chain of "shape produces value" arms tried
//  top to bottom, and the first match wins. That ordering is doing real
//  work: because the negative case is tested first, every later arm can
//  assume a non-negative number, and because "less than 1000" comes
//  before "less than 10000", no arm has to restate the lower bound. The
//  same rules written as ifs need a two-sided range in the middle, and
//  that is exactly where off-by-one bugs live.
//  Relational patterns and the combinators and / or / not let you say a
//  range once. The discard arm _ is "everything else"; without one the
//  compiler warns when your arms are not exhaustive, which is a feature
//  worth leaning on rather than silencing.
//  Describe uses PROPERTY patterns: { Weight: 0 } matches on a member
//  without a cast or a null check, and { IsExpress: true } reads as the
//  business rule it encodes. Ordering matters again — the empty order is
//  matched before the express/standard split, which is why an express
//  order of nothing still ships nothing.
//  Why prefer this to if/else: a switch expression is an EXPRESSION, so
//  every path must produce a value. You cannot forget to return and you
//  cannot fall out of the bottom with nothing assigned — two common
//  logic bugs, made unrepresentable.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string ShippingBand(int grams) => grams switch
{
    < 0 => "invalid",
    0 => "free",
    < 1000 => "small",
    < 10000 => "medium",
    _ => "pallet",
};

static string Describe(Order order) => order switch
{
    { Weight: 0 } => "nothing to ship",
    { IsExpress: true, Destination: var d } => $"express to {d}",
    { Destination: var d } => $"standard to {d}",
};

// ─── tests ───────────────────────────────────────────────────────────────

Test("nothing weighs nothing", () => Eq(ShippingBand(0), "free"));

Test("small parcels", () =>
{
    Eq(ShippingBand(1), "small");
    Eq(ShippingBand(999), "small");
});

Test("medium parcels", () =>
{
    Eq(ShippingBand(1000), "medium");
    Eq(ShippingBand(9999), "medium");
});

Test("pallets", () => Eq(ShippingBand(10000), "pallet"));

Test("a negative weight is invalid", () => Eq(ShippingBand(-1), "invalid"));

Test("Describe names the service and the destination", () =>
    Eq(Describe(new Order(500, "NL", false)), "standard to NL"));

Test("Describe knows express", () =>
    Eq(Describe(new Order(500, "NL", true)), "express to NL"));

Test("an empty order ships nothing, express or not", () =>
{
    Eq(Describe(new Order(0, "NL", true)), "nothing to ship");
    Eq(Describe(new Order(0, "BE", false)), "nothing to ship");
});

return Report();

// ─── the types ───────────────────────────────────────────────────────────

record Order(int Weight, string Destination, bool IsExpress);
