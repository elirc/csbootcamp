# LINQ cheat sheet

Every operator you will actually use, what it costs, and what it throws.
The column that matters most is the last one.

Verified against **.NET 10** (LINQ to Objects). A database provider may
behave differently — noted where it does.

## The laziness rule

```csharp
var q = items.Where(Expensive);   // nothing has run
q.ToList();                       // runs
q.ToList();                       // runs AGAIN
```

**Deferred** operators return a query. **Immediate** operators run right
away and give you a value.

| deferred (streaming) | deferred (buffers first) | immediate |
| --- | --- | --- |
| `Where` `Select` `SelectMany` `Take` `Skip` `TakeWhile` `SkipWhile` `Distinct` `Concat` `Zip` `Cast` `OfType` `DefaultIfEmpty` `Chunk` | `OrderBy` `ThenBy` `GroupBy` `Join` `GroupJoin` `Reverse` `ToLookup`* | `ToList` `ToArray` `ToDictionary` `ToHashSet` `Count` `Sum` `Min` `Max` `Average` `Aggregate` `First` `Single` `Last` `Any` `All` `Contains` `ElementAt` `SequenceEqual` |

\* `ToLookup` is immediate; `GroupBy` is deferred but must read the whole
source before it can yield the first group.

**Two rules that follow:**

1. Materialise (`ToList`) before returning a sequence from a method, or the
   caller re-runs your query — possibly after the source changed or the
   database connection closed.
2. Never enumerate an `IEnumerable<T>` parameter twice. `src.Max() -
   src.Min()` walks it twice. Materialise once, or take `IReadOnlyList<T>`.

## Filtering and projecting

```csharp
xs.Where(x => x.IsActive)                 // keeps order
xs.Select(x => x.Name)                    // one to one
xs.Select((x, i) => (i, x))               // with an index
xs.SelectMany(x => x.Tags)                // one to many, flattened
xs.OfType<Manager>()                      // filter by type
xs.Cast<Manager>()                        // THROWS on the first non-match
xs.Chunk(100)                             // batches of 100
```

## Sorting

```csharp
xs.OrderBy(x => x.Length).ThenBy(x => x, StringComparer.Ordinal)
xs.OrderByDescending(x => x.Score).ThenBy(x => x.Name)
xs.Order()                                // natural order (.NET 7+)
xs.Reverse()
```

- `OrderBy` twice is **not** a tie-breaker — the second re-sorts from
  scratch. Use `ThenBy`.
- LINQ's `OrderBy` is **stable**; `List.Sort` / `Array.Sort` are not.
- Sorting numeric strings sorts them as text: `"10"` before `"9"`. Project
  to a number in the key selector.

## Grouping and joining

```csharp
sales.GroupBy(s => s.Region)                        // IGrouping: has .Key, is a sequence
     .ToDictionary(g => g.Key, g => g.Sum(s => s.Amount))

sales.ToLookup(s => s.Region)                       // eager; missing key → EMPTY, not a throw

customers.Join(orders, c => c.Id, o => o.CustomerId, (c, o) => ...)      // INNER: one row per PAIR
customers.GroupJoin(orders, c => c.Id, o => o.CustomerId, (c, os) => ...) // LEFT: one row per CUSTOMER
```

| you want | operator |
| --- | --- |
| one row per left item, even with no matches | `GroupJoin` |
| one row per matching pair | `Join` |
| pairs, with nulls for no match (SQL LEFT JOIN shape) | `GroupJoin` + `SelectMany(…DefaultIfEmpty())` |

`Join` on a one-to-many relationship **multiplies rows** (fan-out). Summing
after an inner join is the classic double-counting bug.

Both `Join` and `GroupJoin` build a hash lookup: O(n + m), not the O(n·m)
of a nested `Where`.

## Aggregating

```csharp
xs.Count()  xs.Count(p)  xs.Sum(s)  xs.Min()  xs.Max()  xs.Average(s)
xs.MaxBy(x => x.Score)          // the ELEMENT, not the value; null if empty
xs.Aggregate(seed, (acc, x) => next)
xs.Aggregate((a, b) => …)       // NO seed: throws on empty
```

## What throws on an empty sequence

| throws | safe alternative |
| --- | --- |
| `First()` `Last()` `Single()` `ElementAt()` | `FirstOrDefault()` `LastOrDefault()` `SingleOrDefault()` `ElementAtOrDefault()` |
| `Min()` `Max()` `Average()` (non-nullable) | guard with `Any()`, or `DefaultIfEmpty(0)` |
| `Aggregate` without a seed | pass a seed |

`Single()` also throws when there is **more than one** — that is the point
of it. `First()` does not.

## Sets and membership

```csharp
xs.Distinct()          xs.DistinctBy(x => x.Id)     // .NET 6+
xs.Union(ys)           xs.Intersect(ys)   xs.Except(ys)
xs.Any()   xs.Any(p)   xs.All(p)   xs.Contains(x)
xs.SequenceEqual(ys)
```

- `Any()` beats `Count() > 0`: it stops at the first item.
- `Distinct`, `GroupBy`, `Union` and friends use the **default equality
  comparer**. For a `class` that is reference equality, so duplicates
  survive. Use a `record`, a `…By` overload, or an `IEqualityComparer<T>`.
- `Except` and `Distinct` also de-duplicate the result.

## Materialising

```csharp
xs.ToList()  xs.ToArray()  xs.ToHashSet()
xs.ToDictionary(x => x.Id)              // THROWS on a duplicate key
xs.ToLookup(x => x.CategoryId)          // duplicates are the point
```

`ToDictionary` throwing on duplicates is a feature when duplicates are
impossible and an incident when they are merely unlikely. Decide out loud.

## Generating

```csharp
Enumerable.Range(0, 10)
Enumerable.Repeat("x", 3)
Enumerable.Empty<int>()
Array.Empty<int>()
```

## Cost quick-reference

| operation | cost |
| --- | --- |
| `List` index | O(1) |
| `List.Contains` | O(n) |
| `Dictionary` / `HashSet` lookup | O(1) average |
| `OrderBy` | O(n log n), buffers everything |
| `GroupBy` / `ToLookup` | O(n), buffers everything |
| `Join` / `GroupJoin` | O(n + m), buffers the inner side |
| `Where` / `Select` | O(1) per item, streams |

## Smells worth a second look in review

- An `IEnumerable<T>` parameter used twice in one method body.
- A method returning `IEnumerable<T>` built from a `Where` over a field.
- `Count() > 0` instead of `Any()`.
- `OrderBy(...).OrderBy(...)`.
- `Join` followed by `Sum`.
- `Distinct()` on a sequence of a mutable `class`.
- `.Result` or `.Wait()` anywhere near a query (see the async module).
