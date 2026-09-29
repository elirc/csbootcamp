// ─────────────────────────────────────────────────────────────────────────
//  08 · when does a query actually run?                      ★★★ stretch
//  concepts: deferred execution · multiple enumeration · materialising
//  run: dotnet run 08-deferred-execution.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A LINQ query is a RECIPE, not a result. It runs when somebody
//  enumerates it — every time somebody enumerates it. Three methods
//  that each depend on getting that right:
//
//      EvensSnapshot(list)   → the evens AS THEY ARE NOW; later edits
//                              to the caller's list must not show up
//      FirstOver(source, n)  → the first item above n, and it must stop
//                              there (the test passes an INFINITE
//                              sequence, so eager code hangs forever)
//      Distance(source)      → Max - Min, reading the source ONCE
//                              (the test counts how often it is walked)
//
//  Two of these need you to materialise, one needs you not to, and one
//  needs you to walk the sequence exactly once. Read the tests.
//
//  hint: ToList runs the query and gives you a value. Anything that
//  returns IEnumerable stays lazy. Two LINQ calls over the same source
//  walk it twice.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<int> EvensSnapshot(List<int> source) => throw new NotImplementedException("TODO");

static int FirstOver(IEnumerable<int> source, int limit) => throw new NotImplementedException("TODO");

static int Distance(IEnumerable<int> source) => throw new NotImplementedException("TODO");

// ─── scaffolding: sequences that tell you how they were used ─────────────

static IEnumerable<int> Naturals()
{
    var n = 0;
    while (true) yield return n++;
}

static IEnumerable<int> Watched(int[] items, Action onWalk)
{
    onWalk();
    foreach (var item in items) yield return item;
}

// ─── tests ───────────────────────────────────────────────────────────────

Test("EvensSnapshot filters", () =>
    Eq(EvensSnapshot(new List<int> { 1, 2, 3, 4 }), new List<int> { 2, 4 }));

Test("EvensSnapshot does not see later edits", () =>
{
    var source = new List<int> { 2 };
    var snapshot = EvensSnapshot(source);
    source.Add(4);
    Eq(snapshot, new List<int> { 2 }, "the query was still lazy when it was returned");
});

Test("EvensSnapshot of nothing is empty", () => Eq(EvensSnapshot(new List<int>()).Count, 0));

Test("FirstOver finds the first item above the limit", () =>
    Eq(FirstOver(new[] { 1, 5, 9 }, 4), 5));

Test("FirstOver stops as soon as it finds one", () =>
    Eq(FirstOver(Naturals(), 10), 11));

Test("FirstOver returns -1 when nothing qualifies", () =>
    Eq(FirstOver(new[] { 1, 2 }, 10), -1));

Test("Distance measures the spread", () => Eq(Distance(new[] { 3, 9, 5 }), 6));

Test("Distance walks the source exactly once", () =>
{
    var walks = 0;
    var source = Watched(new[] { 3, 9, 5 }, () => walks++);
    Eq(Distance(source), 6);
    Eq(walks, 1, "Max and Min over the same query enumerate it twice");
});

Test("Distance of a single item is zero", () => Eq(Distance(new[] { 7 }), 0));

return Report();
