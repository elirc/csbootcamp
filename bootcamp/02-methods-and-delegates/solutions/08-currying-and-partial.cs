// ─────────────────────────────────────────────────────────────────────────
//  08 · one argument at a time — SOLUTION                    ★★★ stretch
//  concepts: currying · partial application · configuration as closure
//  run: dotnet run 08-currying-and-partial.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Read the type and the body writes itself. Curry must return
//  Func<T1, Func<T2, TResult>>, so it is `a => b => f(a, b)`: take the
//  first argument, hand back something that waits for the second.
//  Uncurry is the inverse, `(a, b) => f(a)(b)`, and the round-trip test
//  is what proves they really are inverses rather than two functions
//  that happen to work on ints.
//  PartialLeft is currying with the first argument already supplied —
//  the same closure, created eagerly. That is the whole idea behind
//  configuration: Configure("api") is partial application over a
//  logging function, and the returned delegate carries its prefix with
//  it. A constructor that stores a dependency in a field is doing
//  exactly this, with more ceremony.
//  Where it pays off in C#: building a validator once and reusing it,
//  adapting a two-argument API to something that expects
//  Func<T, TResult>, and pushing configuration to the edge so the inner
//  function has no idea where its settings came from. Where it does not:
//  C# has no partial-application syntax, so a chain of three or more
//  arrows becomes unreadable fast. Past two arguments, take a record.
//  Watch what is captured. `Configure(prefix)` captures the parameter,
//  so each call keeps its own — the same per-call capture that made two
//  Adders independent in exercise 02.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> f) =>
    a => b => f(a, b);

static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(Func<T1, Func<T2, TResult>> f) =>
    (a, b) => f(a)(b);

static Func<T2, TResult> PartialLeft<T1, T2, TResult>(Func<T1, T2, TResult> f, T1 first) =>
    second => f(first, second);

static Func<string, string> Configure(string prefix) => message => $"[{prefix}] {message}";

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
