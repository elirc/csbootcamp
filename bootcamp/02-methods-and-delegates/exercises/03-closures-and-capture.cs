// ─────────────────────────────────────────────────────────────────────────
//  03 · what a lambda remembers                                ★★☆ core
//  concepts: closures · captured variables · shared state
//  run: dotnet run 03-closures-and-capture.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A lambda captures VARIABLES, not values. Everything below follows
//  from that one sentence, including the bug it hides.
//
//      IndexReturners(3)   → three functions returning 0, 1 and 2
//      MakeCounter()       → (Increment, Read) sharing one number
//      Once(() => Work())  → a function that runs Work at most once
//                            and returns the same answer after that
//
//  IndexReturners is the classic: build a list of functions inside a
//  loop, and each one must remember ITS OWN index — not whatever the
//  loop variable ended up as.
//
//  hint: a `for` loop has ONE variable that every iteration shares. A
//  `foreach` variable is a fresh one per iteration. If you use `for`,
//  copy the counter into a local inside the body before capturing it.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<Func<int>> IndexReturners(int count) => throw new NotImplementedException("TODO");

static (Action Increment, Func<int> Read) MakeCounter() => throw new NotImplementedException("TODO");

static Func<T> Once<T>(Func<T> factory) => throw new NotImplementedException("TODO");

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
