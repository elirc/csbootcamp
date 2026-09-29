// ─────────────────────────────────────────────────────────────────────────
//  06 · remember the answer — SOLUTION                         ★★☆ core
//  concepts: closures over a dictionary · caching · cache keys
//  run: dotnet run 06-memoize.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  The cache is a local variable captured by the returned lambda, which
//  is what makes each wrapper's cache private. Declaring the dictionary
//  outside Memoize — a static field, say — would make all wrappers share
//  one, and that is the cache-key collision bug: two callers, one bucket,
//  and answers that belong to somebody else.
//  TryGetValue is the right lookup because it answers "is it there?" and
//  "what is it?" in ONE hash probe, and because it distinguishes a
//  stored null from a missing entry. `if (cache[key] != null)` gets both
//  wrong: it throws on a miss and recomputes stored nulls forever.
//  MemoizeTwo shows why tuples are such good keys: ValueTuple has
//  structural equality and a combined hash code already, so (1, 2) and
//  (2, 1) are different keys with no work from you. Concatenating the
//  arguments into a string — "1:2" — is the classic wrong turn, and it
//  collides the moment an argument contains the separator.
//  What this deliberately does NOT do: bound the cache, expire entries,
//  or handle concurrency. An unbounded memoize on user input is a memory
//  leak with extra steps. Real caches need a size limit (see the LRU in
//  module 09) and, for shared use, ConcurrentDictionary.GetOrAdd —
//  which, note, may still run the factory more than once under a race.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Func<TArg, TResult> Memoize<TArg, TResult>(Func<TArg, TResult> f) where TArg : notnull
{
    var cache = new Dictionary<TArg, TResult>();
    return arg =>
    {
        if (cache.TryGetValue(arg, out var hit)) return hit;

        var value = f(arg);
        cache[arg] = value;
        return value;
    };
}

static Func<T1, T2, TResult> MemoizeTwo<T1, T2, TResult>(Func<T1, T2, TResult> f)
    where T1 : notnull where T2 : notnull
{
    var cache = new Dictionary<(T1, T2), TResult>();
    return (a, b) =>
    {
        if (cache.TryGetValue((a, b), out var hit)) return hit;

        var value = f(a, b);
        cache[(a, b)] = value;
        return value;
    };
}

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
