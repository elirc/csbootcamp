// ─────────────────────────────────────────────────────────────────────────
//  06 · say it with patterns                                   ★★☆ core
//  concepts: switch expressions · property patterns · relational patterns
//  run: dotnet run 06-pattern-matching.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  The shipping desk has rules, and rules are what switch expressions
//  are for. An Order (defined at the BOTTOM of this file) has a weight
//  in grams, a destination, and whether it is express.
//
//      ShippingBand(0)       → "free"      (nothing to ship)
//      ShippingBand(999)     → "small"     (1..999)
//      ShippingBand(5000)    → "medium"    (1000..9999)
//      ShippingBand(20000)   → "pallet"    (10000 and up)
//      ShippingBand(-1)      → "invalid"
//
//      Describe(new Order(500, "NL", false)) → "standard to NL"
//      Describe(new Order(500, "NL", true))  → "express to NL"
//      Describe(new Order(0, "NL", true))    → "nothing to ship"
//
//  Write both with switch expressions — no if/else chains. That is the
//  exercise: the shape, not the answer.
//
//  hint: relational patterns (less-than, greater-than) combine with
//  and / or / not. A property pattern looks like { Weight: 0 } or
//  { IsExpress: true, Destination: var d }.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static string ShippingBand(int grams) => throw new NotImplementedException("TODO");

static string Describe(Order order) => throw new NotImplementedException("TODO");

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
