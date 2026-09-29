// ─────────────────────────────────────────────────────────────────────────
//  05 · building a pipeline out of functions                   ★★☆ core
//  concepts: function composition · multicast delegates
//  run: dotnet run 05-composing-functions.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Small functions are only useful if they combine. Three combinators:
//
//      Compose(f, g)(x)        → f(g(x))        (maths order: g first)
//      Pipe([f, g, h])(x)      → h(g(f(x)))     (reading order: f first)
//      Tap(f, log)(x)          → f(x), and log saw x on the way through
//
//  Pipe over an empty list is the identity function: it returns what it
//  was given. That is not a special case to code around; it is what
//  falls out of a correct fold.
//
//  hint: each of these RETURNS a function. Aggregate over the steps
//  builds the pipeline once, at construction time, rather than looping
//  on every call.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Func<T, TResult> Compose<T, TMiddle, TResult>(
    Func<TMiddle, TResult> outer, Func<T, TMiddle> inner) => throw new NotImplementedException("TODO");

static Func<T, T> Pipe<T>(params Func<T, T>[] steps) => throw new NotImplementedException("TODO");

static Func<T, TResult> Tap<T, TResult>(Func<T, TResult> f, Action<T> observe) =>
    throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("Compose runs the inner function first", () =>
{
    var shout = Compose<string, string, string>(s => s + "!", s => s.ToUpperInvariant());
    Eq(shout("hi"), "HI!");
});

Test("Compose can change the type on the way through", () =>
{
    var lengthPlusOne = Compose<string, int, int>(n => n + 1, s => s.Length);
    Eq(lengthPlusOne("abc"), 4);
});

Test("Pipe applies the steps left to right", () =>
{
    var f = Pipe<int>(x => x + 1, x => x * 10);
    Eq(f(2), 30);
});

Test("Pipe of nothing is the identity", () => Eq(Pipe<int>()(7), 7));

Test("Pipe of one step is that step", () => Eq(Pipe<int>(x => x - 1)(7), 6));

Test("Tap returns the original answer", () => Eq(Tap<int, int>(x => x * 2, _ => { })(5), 10));

Test("Tap sees the input", () =>
{
    var seen = new List<int>();
    var doubled = Tap<int, int>(x => x * 2, seen.Add);
    doubled(3);
    doubled(4);
    Eq(seen, new List<int> { 3, 4 });
});

Test("Tap observes even when the pipeline is composed", () =>
{
    var seen = new List<int>();
    var f = Pipe<int>(Tap<int, int>(x => x + 1, seen.Add), x => x * 2);
    Eq(f(1), 4);
    Eq(seen, new List<int> { 1 });
});

return Report();
