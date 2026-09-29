# Quiz 03 · Collections and LINQ

Say the answer out loud before you expand it.

Pairs with
[`bootcamp/03-collections-and-linq`](../bootcamp/03-collections-and-linq/).

---

**1.** `var q = items.Where(Expensive);` — how many times has `Expensive`
run?

<details><summary>answer</summary>

Zero. `Where` returns a query object; nothing runs until something
enumerates it — and it runs again on the next enumeration.
</details>

**2.** A method returns `source.Where(p)` directly. Name two ways that bites
the caller.

<details><summary>answer</summary>

(1) The query still points at the caller's collection, so later edits to the
source show up in the "result". (2) Enumerating it twice does the work
twice — and if the source was a database query or a stream, it may throw or
be impossible to replay. Materialise with `ToList` before crossing a
boundary.
</details>

**3.** `src.Max() - src.Min()` — what is wrong with it?

<details><summary>answer</summary>

It enumerates `src` twice. On a `List` that is waste; on a generator, file
or query it is a bug. The review smell is an `IEnumerable<T>` parameter used
more than once in one method.
</details>

**4.** Which LINQ operators throw on an empty sequence?

<details><summary>answer</summary>

`First`, `Last`, `Single`, `ElementAt`, `Min`, `Max`, non-nullable `Average`,
and `Aggregate` without a seed. The `…OrDefault` variants and a seeded
`Aggregate` do not. "Sequence contains no elements" never tells you which
query, so decide what empty means yourself.
</details>

**5.** When does `Single()` throw that `First()` does not?

<details><summary>answer</summary>

When there is more than one match. That is the point of it: `Single`
asserts uniqueness.
</details>

**6.** `Any()` versus `Count() > 0` — which and why?

<details><summary>answer</summary>

`Any()`. It stops at the first item; `Count()` walks the whole sequence.
</details>

**7.** You need one row per customer, including customers with no orders.
`Join` or `GroupJoin`?

<details><summary>answer</summary>

`GroupJoin` — the left join. Each left item comes back with a possibly
**empty** group, and `Sum` over empty is 0, so the "no orders" case needs no
special handling. `Join` is an inner join and drops those customers.
</details>

**8.** You summed totals after a `Join` and every number is too big. What
happened?

<details><summary>answer</summary>

Fan-out. `Join` emits one row per matching **pair**, so a customer with
three orders appears three times and anything joined on the one side is
counted three times. Same bug as in SQL.
</details>

**9.** `xs.OrderBy(a).OrderBy(b)` — what did you get?

<details><summary>answer</summary>

A sequence sorted by `b` only. The second `OrderBy` re-sorts from scratch;
the first was wasted work. `ThenBy` is the tie-breaker.
</details>

**10.** Sorting `["10", "9", "2"]` gives `["10", "2", "9"]`. Why, and how do
you fix it while keeping the original strings?

<details><summary>answer</summary>

Text comparison is character by character, and `'1' < '9'`. Project to a
number in the **key selector** — `OrderBy(int.Parse)` — which sorts by value
and still returns the original strings.
</details>

**11.** `Distinct()` on a `List<Customer>` (a class) removed nothing. Why?

<details><summary>answer</summary>

The default equality comparer for a class is reference equality, so two
"equal" customers are two objects. Use a `record`, `DistinctBy(x => x.Id)`,
or an `IEqualityComparer<T>`.
</details>

**12.** Why does a `Dictionary` or `HashSet` need a good `GetHashCode`, and
which types give you one free?

<details><summary>answer</summary>

Because lookup hashes the key to find the bucket; without value-based
hashing, two equal keys land in different buckets. Records, value tuples,
strings, and primitives give you one. A plain class hashes by reference.
</details>

**13.** The `Dictionary` indexer: what does it do on a missing key when
reading, and when writing?

<details><summary>answer</summary>

Reading throws `KeyNotFoundException`; writing creates the entry. That
asymmetry surprises people exactly once.
</details>

**14.** What is the one return value that makes `HashSet<T>.Add` so useful?

<details><summary>answer</summary>

`false` when the item was already present. It answers "have I seen this?"
and records it in a single probe — which also removes the check-then-act
race.
</details>

**15.** `ToDictionary` threw `ArgumentException` in production but never in
test. What is the lesson?

<details><summary>answer</summary>

Duplicate key. `ToDictionary` throws on duplicates — a feature when they are
impossible, an incident when they are merely unlikely. Decide out loud:
`ToLookup`, a `GroupBy`, or "last one wins" written explicitly.
</details>

**16.** Is `GroupBy` deferred? Is it streaming?

<details><summary>answer</summary>

Deferred, but not streaming: it must read the entire source before it can
yield the first group, because it cannot know a group is complete until the
input ends. `ToLookup` does the same work immediately.
</details>

**17.** Every fold is three decisions. Name them.

<details><summary>answer</summary>

What the accumulator holds, what the seed is, and how one item updates it.
Choosing the seed well is what makes the empty case fall out with no `if`.
</details>

**18.** LINQ's `OrderBy` versus `List.Sort` — one behavioural difference
that can change your output.

<details><summary>answer</summary>

`OrderBy` is stable (equal keys keep their input order); `List.Sort` and
`Array.Sort` use an unstable introsort. If ties must keep their order, use
LINQ or add an explicit tie-breaker.
</details>
