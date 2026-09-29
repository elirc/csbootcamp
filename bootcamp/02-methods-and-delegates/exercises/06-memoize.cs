// ─────────────────────────────────────────────────────────────────────────
//  06 · remember the answer                                    ★★☆ core
//  concepts: closures over a dictionary · caching · cache keys
//  run: dotnet run 06-memoize.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Memoize wraps an expensive function so each distinct argument is
//  computed once and remembered:
//
//      var fast = Memoize(Slow);
//      fast(2); fast(2); fast(3);   → Slow ran twice, not three times
//
//  Two rules that matter more than the caching itself:
//    · a null RESULT still counts as computed — do not recompute it
//    · the cache is per wrapper, so two Memoize calls share nothing
//
//  Then MemoizeTwo, for a function of two arguments, because the
//  interesting part of a cache is always the KEY.
//
//      MemoizeTwo(Add)(1, 2)   → cached under the pair (1, 2)
//
//  hint: Dictionary.TryGetValue tells you "present" and hands you the
//  value in one lookup. For two arguments, a tuple is already a
//  perfectly good dictionary key — value equality comes for free.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Func<TArg, TResult> Memoize<TArg, TResult>(Func<TArg, TResult> f) where TArg : notnull =>
    throw new NotImplementedException("TODO");

static Func<T1, T2, TResult> MemoizeTwo<T1, T2, TResult>(Func<T1, T2, TResult> f)
    where T1 : notnull where T2 : notnull => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("the wrapped function still returns the right answer", () =>
{
    var square = Memoize<int, int>(x => x * x);
    Eq(square(4), 16);
});

Test("the same argument is computed once", () =>
{
    var calls = 0;
    var square = Memoize<int, int>(x => { calls++; return x * x; });
    square(4);
    square(4);
    square(4);
    Eq(calls, 1);
});

Test("different arguments are computed separately", () =>
{
    var calls = 0;
    var square = Memoize<int, int>(x => { calls++; return x * x; });
    Eq((square(2), square(3), square(2)), (4, 9, 4));
    Eq(calls, 2);
});

Test("a null result is still remembered", () =>
{
    var calls = 0;
    var lookup = Memoize<string, string?>(_ => { calls++; return null; });
    lookup("missing");
    lookup("missing");
    Eq(calls, 1, "null is an answer, not a cache miss");
});

Test("two wrappers do not share a cache", () =>
{
    var calls = 0;
    Func<int, int> f = x => { calls++; return x; };
    var a = Memoize(f);
    var b = Memoize(f);
    a(1);
    b(1);
    Eq(calls, 2);
});

Test("MemoizeTwo keys on both arguments", () =>
{
    var calls = 0;
    var add = MemoizeTwo<int, int, int>((x, y) => { calls++; return x + y; });
    Eq((add(1, 2), add(1, 2), add(2, 1)), (3, 3, 3));
    Eq(calls, 2, "(1,2) and (2,1) are different keys");
});

Test("MemoizeTwo works with mixed types", () =>
{
    var repeat = MemoizeTwo<string, int, string>((s, n) => string.Concat(Enumerable.Repeat(s, n)));
    Eq(repeat("ab", 2), "abab");
});

return Report();
