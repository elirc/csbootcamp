// ─────────────────────────────────────────────────────────────────────────
//  04 · grouping and the report it produces — SOLUTION          ★★☆ core
//  concepts: GroupBy · ToLookup · aggregating a group
//  run: dotnet run 04-grouping.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  GroupBy turns a flat sequence into a sequence of groups, where each
//  group is itself enumerable and carries a Key. Almost every report you
//  will ever write is group-then-aggregate, and the only variation is
//  what the aggregate is: Sum, Count, MaxBy, or another GroupBy.
//  BestSellerPerRegion is the two-level version and the one people get
//  wrong: you must aggregate WITHIN each region before comparing, or you
//  pick the single biggest line item rather than the biggest product.
//  The "sums a product before comparing" test is there precisely to
//  catch the naive MaxBy(s => s.Amount).
//  MaxBy (and MinBy) return the ELEMENT, not the value — the LINQ method
//  people reimplement by hand for years before noticing it exists. On an
//  empty sequence they return null rather than throwing, unlike Max.
//  GroupBy is deferred but it BUFFERS: it must read the whole source
//  before it can yield the first group, because it cannot know a group
//  is complete until the input ends. If you only need lookups and not a
//  report, ToLookup does the same work eagerly and, unlike a Dictionary,
//  returns an empty sequence for a missing key instead of throwing.
//  Ordering: GroupBy preserves the order in which keys first appear, and
//  the order of items inside each group. That is guaranteed for LINQ to
//  Objects and NOT for a database provider — never lean on it in EF.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Dictionary<string, decimal> TotalByRegion(IEnumerable<Sale> sales) =>
    sales.GroupBy(s => s.Region)
         .ToDictionary(g => g.Key, g => g.Sum(s => s.Amount));

static Dictionary<string, string> BestSellerPerRegion(IEnumerable<Sale> sales) =>
    sales.GroupBy(s => s.Region)
         .ToDictionary(
             region => region.Key,
             region => region.GroupBy(s => s.Product)
                             .MaxBy(product => product.Sum(s => s.Amount))!
                             .Key);

static List<string> RegionsWithAtLeast(IEnumerable<Sale> sales, int count) =>
    sales.GroupBy(s => s.Region)
         .Where(g => g.Count() >= count)
         .Select(g => g.Key)
         .Order()
         .ToList();

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
