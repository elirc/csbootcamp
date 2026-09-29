// ─────────────────────────────────────────────────────────────────────────
//  04 · grouping and the report it produces                     ★★☆ core
//  concepts: GroupBy · ToLookup · aggregating a group
//  run: dotnet run 04-grouping.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Sale records are defined at the BOTTOM of this file. Three reports:
//
//      TotalByRegion(sales)   → { "north": 300m, "south": 150m }
//      BestSellerPerRegion(s) → { "north": "widget", ... }  (highest total)
//      RegionsWithAtLeast(s, 2)
//                             → regions with 2 or more sales, sorted A-Z
//
//  Every region that appears in the data appears in the first two
//  reports, and no region that does not.
//
//  hint: GroupBy(keySelector) gives you a sequence of groups; each
//  group IS an IEnumerable of its items and also carries a .Key. You
//  can go straight to a dictionary with ToDictionary(g => g.Key, g =>
//  ...). MaxBy on a group answers "which item is biggest".

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Dictionary<string, decimal> TotalByRegion(IEnumerable<Sale> sales) =>
    throw new NotImplementedException("TODO");

static Dictionary<string, string> BestSellerPerRegion(IEnumerable<Sale> sales) =>
    throw new NotImplementedException("TODO");

static List<string> RegionsWithAtLeast(IEnumerable<Sale> sales, int count) =>
    throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

var sales = new[]
{
    new Sale("north", "widget", 100m),
    new Sale("south", "gadget", 150m),
    new Sale("north", "widget", 150m),
    new Sale("north", "gadget", 50m),
};

Test("TotalByRegion sums each region", () =>
{
    var totals = TotalByRegion(sales);
    Eq(totals["north"], 300m);
    Eq(totals["south"], 150m);
});

Test("TotalByRegion has one entry per region", () => Eq(TotalByRegion(sales).Count, 2));

Test("TotalByRegion of nothing is empty", () =>
    Eq(TotalByRegion(Array.Empty<Sale>()).Count, 0));

Test("BestSellerPerRegion picks the highest total per region", () =>
{
    var best = BestSellerPerRegion(sales);
    Eq(best["north"], "widget");
    Eq(best["south"], "gadget");
});

Test("BestSellerPerRegion sums a product before comparing", () =>
{
    var twoSmall = new[]
    {
        new Sale("east", "small", 60m),
        new Sale("east", "small", 60m),
        new Sale("east", "big", 100m),
    };
    Eq(BestSellerPerRegion(twoSmall)["east"], "small");
});

Test("RegionsWithAtLeast filters by group size", () =>
    Eq(RegionsWithAtLeast(sales, 2), new List<string> { "north" }));

Test("RegionsWithAtLeast returns them sorted", () =>
    Eq(RegionsWithAtLeast(sales, 1), new List<string> { "north", "south" }));

Test("RegionsWithAtLeast can return nothing", () =>
    Eq(RegionsWithAtLeast(sales, 99).Count, 0));

return Report();

// ─── the types ───────────────────────────────────────────────────────────

record Sale(string Region, string Product, decimal Amount);
