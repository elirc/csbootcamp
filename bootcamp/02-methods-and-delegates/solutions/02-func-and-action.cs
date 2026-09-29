// ─────────────────────────────────────────────────────────────────────────
//  02 · behaviour as an argument — SOLUTION                 ★☆☆ warm-up
//  concepts: Func · Action · delegates as parameters
//  run: dotnet run 02-func-and-action.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Func<in T, out TResult> and Action<in T> are just delegate types the
//  BCL declares for you, so you rarely need the `delegate` keyword any
//  more. The last type argument of a Func is always the RETURN type:
//  Func<int, string> takes an int and gives a string.
//  Adder is the first closure in this track. The returned lambda
//  captures `by` — the variable, not a snapshot of the value — and each
//  call to Adder creates a fresh capture, which is why two adders never
//  interfere. Module 02's exercise 03 is where that distinction starts
//  to bite.
//  WhereNot is generic and lazy: it returns an IEnumerable built by
//  Where, so nothing runs until somebody enumerates. The tests call
//  ToList to force it, which is exactly the habit you want — a lazy
//  sequence handed across a boundary is a bug waiting for a closed
//  database connection.
//  Method groups matter for readability: `WhereNot(items,
//  string.IsNullOrEmpty)` says the rule; `x => string.IsNullOrEmpty(x)`
//  says the rule wrapped in noise.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static int ApplyTwice(Func<int, int> f, int start) => f(f(start));

static void Repeat(int times, Action<int> body)
{
    for (var i = 0; i < times; i++) body(i);
}

static Func<int, int> Adder(int by) => value => value + by;

static IEnumerable<T> WhereNot<T>(IEnumerable<T> items, Func<T, bool> predicate) =>
    items.Where(item => !predicate(item));

// ─── tests ───────────────────────────────────────────────────────────────

Test("ApplyTwice runs the function twice", () => Eq(ApplyTwice(x => x + 3, 10), 16));

Test("ApplyTwice works with any function", () => Eq(ApplyTwice(x => x * x, 3), 81));

Test("Repeat calls the body once per turn, counting from zero", () =>
{
    var seen = new List<int>();
    Repeat(3, i => seen.Add(i));
    Eq(seen, new List<int> { 0, 1, 2 });
});

Test("Repeat of zero calls nothing", () =>
{
    var calls = 0;
    Repeat(0, _ => calls++);
    Eq(calls, 0);
});

Test("Adder builds a function that remembers its number", () =>
{
    var addFive = Adder(5);
    Eq(addFive(10), 15);
    Eq(addFive(1), 6);
});

Test("two Adders do not share state", () =>
{
    var addOne = Adder(1);
    var addTen = Adder(10);
    Eq((addOne(0), addTen(0)), (1, 10));
});

Test("WhereNot keeps the items the predicate rejects", () =>
    Eq(WhereNot(new[] { 1, 2, 3, 4 }, n => n % 2 == 0).ToList(), new List<int> { 1, 3 }));

Test("WhereNot works on any type", () =>
    Eq(WhereNot(new[] { "a", "", "b" }, string.IsNullOrEmpty).ToList(),
        new List<string> { "a", "b" }));

return Report();
