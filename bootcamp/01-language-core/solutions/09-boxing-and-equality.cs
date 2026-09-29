// ─────────────────────────────────────────────────────────────────────────
//  09 · boxing, and what equals means — SOLUTION             ★★★ stretch
//  concepts: boxing · object.Equals · type checks
//  run: dotnet run 09-boxing-and-equality.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Boxing is what happens when a value type is stored in a reference
//  slot: the runtime allocates an object on the heap and copies the
//  value into it. `object x = 1;` is a heap allocation, and every one
//  of those boxes is a separate object.
//  That is why `==` is the wrong operator here. On two `object`
//  variables, `==` is REFERENCE comparison — it asks "the same box?",
//  not "the same value?" — so two boxes each holding 1 compare unequal
//  and the bug only shows up once the values stop being cached or
//  interned. `object.Equals(a, b)` (the static two-argument form) is
//  null-safe on both sides and dispatches to the real Equals, which for
//  int compares numbers and for string compares characters.
//  int versus long is the second lesson: boxed values keep their exact
//  type, and Int32.Equals(Int64) is false. No numeric conversion
//  happens once a value is inside a box, which is exactly why config
//  bags that "worked in the test" break on a value that arrived as a
//  long from JSON.
//  `value is int n` is the modern unbox: it checks the type and gives
//  you the value in one step, with no cast that can throw and no
//  double type check. Prefer it to `if (v is int) { (int)v }`.
//  Distinct() uses EqualityComparer<object>.Default, which lands on the
//  same Equals — so 1 and "1" stay two values, and null counts once.
//  The performance note worth carrying: boxing allocates. A hot loop
//  over object is a garbage-collection problem, and it is why the BCL
//  has generic overloads everywhere. Reach for object only when the
//  type genuinely is not known until runtime.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool AreEqual(object? a, object? b) => Equals(a, b);

static int SumInts(IEnumerable<object?> values) =>
    values.Sum(v => v is int n ? n : 0);

static int CountDistinct(IEnumerable<object?> values) =>
    values.Distinct().Count();

// ─── tests ───────────────────────────────────────────────────────────────

Test("two boxed ints holding the same number are equal", () => Ok(AreEqual(1, 1)));

Test("an int and a long are not equal even at the same value", () => Ok(!AreEqual(1, 1L)));

Test("strings compare by value", () =>
{
    Ok(AreEqual("a", "a"));
    Ok(!AreEqual("a", "b"));
});

Test("null equals null, and nothing else", () =>
{
    Ok(AreEqual(null, null));
    Ok(!AreEqual(null, 1));
    Ok(!AreEqual(1, null));
});

Test("SumInts adds only the ints", () =>
    Eq(SumInts(new object?[] { 1, "x", 2, null, true }), 3));

Test("SumInts of nothing is zero", () => Eq(SumInts(Array.Empty<object?>()), 0));

Test("CountDistinct counts values, not boxes", () =>
    Eq(CountDistinct(new object?[] { 1, 1, "1", 2 }), 3));

Test("CountDistinct treats null as one value", () =>
    Eq(CountDistinct(new object?[] { null, null, 1 }), 2));

return Report();
