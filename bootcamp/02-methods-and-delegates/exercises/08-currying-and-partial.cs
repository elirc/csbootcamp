// ─────────────────────────────────────────────────────────────────────────
//  08 · one argument at a time                               ★★★ stretch
//  concepts: currying · partial application · configuration as closure
//  run: dotnet run 08-currying-and-partial.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Currying turns a two-argument function into a one-argument function
//  that returns another one. Partial application fixes an argument now
//  and leaves the rest for later. Both are the same idea as dependency
//  injection, minus the container.
//
//      Curry(Add)(1)(2)         → 3
//      Uncurry(Curry(Add))(1,2) → 3
//      PartialLeft(Add, 10)(5)  → 15
//
//  Then the practical version of the same trick: Configure takes a
//  logger prefix and returns a ready-to-use logging function.
//
//      Configure("api")("boom") → "[api] boom"
//
//  hint: every one of these is a lambda returning a lambda. Write the
//  types out longhand once — Func<T1, Func<T2, TResult>> — and the
//  bodies become obvious.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> f) =>
    throw new NotImplementedException("TODO");

static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(Func<T1, Func<T2, TResult>> f) =>
    throw new NotImplementedException("TODO");

static Func<T2, TResult> PartialLeft<T1, T2, TResult>(Func<T1, T2, TResult> f, T1 first) =>
    throw new NotImplementedException("TODO");

static Func<string, string> Configure(string prefix) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("Curry takes the arguments one at a time", () =>
{
    var add = Curry<int, int, int>((a, b) => a + b);
    Eq(add(1)(2), 3);
});

Test("the half-applied function can be reused", () =>
{
    var add = Curry<int, int, int>((a, b) => a + b);
    var addTen = add(10);
    Eq((addTen(1), addTen(2)), (11, 12));
});

Test("Uncurry puts it back", () =>
{
    var add = Curry<int, int, int>((a, b) => a + b);
    Eq(Uncurry(add)(1, 2), 3);
});

Test("Curry and Uncurry round-trip any function", () =>
{
    Func<string, int, string> repeat = (s, n) => string.Concat(Enumerable.Repeat(s, n));
    Eq(Uncurry(Curry(repeat))("ab", 2), "abab");
});

Test("PartialLeft fixes the first argument", () =>
    Eq(PartialLeft<int, int, int>((a, b) => a + b, 10)(5), 15));

Test("PartialLeft keeps the argument order", () =>
    Eq(PartialLeft<string, string, string>((a, b) => a + b, "left")("right"), "leftright"));

Test("Configure builds a logger with its prefix baked in", () =>
{
    var log = Configure("api");
    Eq(log("boom"), "[api] boom");
});

Test("two configured loggers keep their own prefix", () =>
{
    var api = Configure("api");
    var db = Configure("db");
    Eq((api("x"), db("x")), ("[api] x", "[db] x"));
});

return Report();
