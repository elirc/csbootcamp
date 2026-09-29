// ─────────────────────────────────────────────────────────────────────────
//  03 · what a lambda remembers — SOLUTION                     ★★☆ core
//  concepts: closures · captured variables · shared state
//  run: dotnet run 03-closures-and-capture.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  When a lambda uses a variable from the enclosing scope, the compiler
//  moves that variable into a hidden class and both the lambda and the
//  surrounding code point at the same field. That is the whole
//  mechanism, and it explains both halves of this file.
//  IndexReturners: a `for` loop declares ONE `i` for the entire loop, so
//  three lambdas capturing it all see the same field — after the loop
//  ends they all return 3. Copying into a per-iteration local
//  (`var index = i;`) gives each lambda its own field. C# fixed this for
//  `foreach` in C# 5 — the iteration variable is now per-iteration — but
//  `for` still shares, deliberately, because the loop needs to mutate it.
//  This is the stale-closure bug, and it is the same bug in JavaScript's
//  `var` loops and in React's effect dependencies.
//  MakeCounter: two lambdas capturing the SAME local is not a bug, it is
//  the feature. `count` outlives the method that declared it, and the
//  pair of delegates is a tiny object with private state — a closure is
//  an object with one method, and an object is a closure with several.
//  Once: capture a flag and a slot. The first call fills them; later
//  calls short-circuit. Note the factory reference is not cleared here —
//  in a long-lived cache you would null it out so the captured factory
//  (and everything IT captured) can be collected. That is how closures
//  turn into memory leaks.
//  Thread-safety note: none of this is thread-safe. For real lazy
//  initialisation the BCL already has Lazy<T>, which handles the race.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<Func<int>> IndexReturners(int count)
{
    var fns = new List<Func<int>>();
    for (var i = 0; i < count; i++)
    {
        var index = i; // a fresh variable per iteration — this is the fix
        fns.Add(() => index);
    }

    return fns;
}

static (Action Increment, Func<int> Read) MakeCounter()
{
    var count = 0;
    return (() => count++, () => count);
}

static Func<T> Once<T>(Func<T> factory)
{
    var done = false;
    T? value = default;
    return () =>
    {
        if (!done)
        {
            value = factory();
            done = true;
        }

        return value!;
    };
}

// ─── tests ───────────────────────────────────────────────────────────────

Test("each function returns its own index", () =>
{
    var fns = IndexReturners(3);
    Eq(fns.Select(f => f()).ToList(), new List<int> { 0, 1, 2 });
});

Test("asking for none gives none", () => Eq(IndexReturners(0).Count, 0));

Test("the counter starts at zero", () =>
{
    var (_, read) = MakeCounter();
    Eq(read(), 0);
});

Test("increment and read share one number", () =>
{
    var (increment, read) = MakeCounter();
    increment();
    increment();
    Eq(read(), 2);
});

Test("two counters are independent", () =>
{
    var a = MakeCounter();
    var b = MakeCounter();
    a.Increment();
    Eq((a.Read(), b.Read()), (1, 0));
});

Test("Once runs the factory the first time", () =>
{
    var calls = 0;
    var get = Once(() => ++calls);
    Eq(get(), 1);
    Eq(calls, 1);
});

Test("Once never runs the factory again", () =>
{
    var calls = 0;
    var get = Once(() => ++calls);
    get();
    get();
    get();
    Eq(calls, 1, "the factory ran more than once");
});

Test("Once returns the same answer every time", () =>
{
    var next = 0;
    var get = Once(() => next++);
    Eq((get(), get()), (0, 0));
});

return Report();
