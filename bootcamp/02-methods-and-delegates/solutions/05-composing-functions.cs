// ─────────────────────────────────────────────────────────────────────────
//  05 · building a pipeline out of functions — SOLUTION        ★★☆ core
//  concepts: function composition · multicast delegates
//  run: dotnet run 05-composing-functions.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Compose is the mathematical order — the one on the right runs first —
//  and Pipe is the reading order. Both exist because both are natural
//  somewhere: Compose reads like the formula, Pipe reads like the data
//  flow. Pick one per codebase and stay with it; mixing them is how a
//  transform ends up applied backwards.
//  Pipe is a fold over functions. Starting the Aggregate with the
//  identity `x => x` is what makes the empty case work for free, and
//  that is the general shape: the identity element of composition is
//  the do-nothing function, exactly as 0 is for addition. Note the
//  pipeline is built ONCE — Aggregate runs at construction, and the
//  returned delegate is a chain of closures, not a loop over an array.
//  Tap is the debugging combinator: same value out, side effect on the
//  way through. It is how you inspect a pipeline without dismantling
//  it, and it generalises to logging, metrics, and caching.
//  `seen.Add` as an Action<int> is a method group again — no lambda
//  needed, and it reads as the intent.
//  The related .NET feature worth knowing: delegates are MULTICAST. `a
//  += b` on an Action makes one delegate that calls both, in order.
//  For a Func the combined delegate returns only the LAST result, which
//  quietly discards the others — a real trap, and the reason multicast
//  is for events rather than for computing values.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Func<T, TResult> Compose<T, TMiddle, TResult>(
    Func<TMiddle, TResult> outer, Func<T, TMiddle> inner) => x => outer(inner(x));

static Func<T, T> Pipe<T>(params Func<T, T>[] steps) =>
    steps.Aggregate<Func<T, T>, Func<T, T>>(x => x, (chain, step) => x => step(chain(x)));

static Func<T, TResult> Tap<T, TResult>(Func<T, TResult> f, Action<T> observe) =>
    x =>
    {
        observe(x);
        return f(x);
    };

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
