// ─────────────────────────────────────────────────────────────────────────
//  09 · boxing, and what equals means                       ★★★ stretch
//  concepts: boxing · object.Equals · type checks
//  run: dotnet run 09-boxing-and-equality.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A settings bag stores whatever the config file had: object values
//  that might be an int, a string, a bool. Three helpers over that bag,
//  and every one of them meets boxing:
//
//      AreEqual(1, 1)          → true    (two boxes, one value)
//      AreEqual(1, 1L)         → false   (int and long are not the same)
//      AreEqual("a", "a")      → true
//      AreEqual(null, null)    → true
//      SumInts([1, "x", 2])    → 3       (skip what is not an int)
//      CountDistinct([1,1,"1",2]) → 3    ("1" is not 1)
//
//  All three must work without throwing on any input, including null.
//
//  hint: object.Equals(a, b) is the static one — it handles both nulls
//  and calls the right Equals underneath. A type test with a pattern
//  (`value is int n`) both checks and unboxes in one step.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool AreEqual(object? a, object? b) => throw new NotImplementedException("TODO");

static int SumInts(IEnumerable<object?> values) => throw new NotImplementedException("TODO");

static int CountDistinct(IEnumerable<object?> values) => throw new NotImplementedException("TODO");

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
