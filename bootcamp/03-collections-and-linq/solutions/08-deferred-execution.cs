// ─────────────────────────────────────────────────────────────────────────
//  08 · when does a query actually run? — SOLUTION           ★★★ stretch
//  concepts: deferred execution · multiple enumeration · materialising
//  run: dotnet run 08-deferred-execution.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Where, Select, OrderBy and friends return a query OBJECT. Nothing has
//  happened yet. The work starts when something enumerates it — foreach,
//  ToList, Count, First — and it starts AGAIN on the next enumeration.
//  EvensSnapshot: returning `source.Where(...)` compiles and passes the
//  first test, then fails the second, because the query still points at
//  the caller's list and re-reads it when the test enumerates. ToList
//  turns the recipe into a result. The rule: never hand a lazy query
//  across a boundary the caller does not control — this is the same bug
//  as returning a query after the database context is disposed.
//  FirstOver must stay lazy. FirstOrDefault stops at the first match,
//  so it terminates on an infinite sequence; `source.Where(...).ToList()
//  .FirstOrDefault()` would hang forever. Laziness is not just an
//  optimisation here, it is the difference between working and not.
//  Distance is the multiple-enumeration trap. `source.Max() -
//  source.Min()` walks the sequence TWICE, which is invisible on a List
//  and expensive on a database query, a file read, or a generator — and
//  simply WRONG on a sequence that cannot be replayed. Materialise once,
//  then ask both questions.
//  How to spot it in review: a parameter typed IEnumerable<T> used more
//  than once in a method body. Either materialise at the top or accept
//  IReadOnlyList<T> and make the requirement part of the signature.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<int> EvensSnapshot(List<int> source) =>
    source.Where(n => n % 2 == 0).ToList();

static int FirstOver(IEnumerable<int> source, int limit)
{
    foreach (var item in source)
    {
        if (item > limit) return item;
    }

    return -1;
}

static int Distance(IEnumerable<int> source)
{
    var items = source as IReadOnlyList<int> ?? source.ToList();
    return items.Max() - items.Min();
}

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
