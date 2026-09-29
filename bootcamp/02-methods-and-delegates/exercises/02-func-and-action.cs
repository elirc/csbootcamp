// ─────────────────────────────────────────────────────────────────────────
//  02 · behaviour as an argument                            ★☆☆ warm-up
//  concepts: Func · Action · delegates as parameters
//  run: dotnet run 02-func-and-action.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A delegate is a value that happens to be a method. Func<A, B> takes
//  an A and returns a B; Action<A> takes an A and returns nothing.
//  Once behaviour is a value you can pass it, store it, and build it.
//
//      ApplyTwice(x => x + 3, 10)      → 16
//      Repeat(3, i => log.Add(i))      → calls with 0, 1, 2
//      Adder(5)(10)                    → 15
//      WhereNot([1,2,3,4], IsEven)     → [1, 3]
//
//  Adder RETURNS a delegate: calling Adder(5) gives you back a function
//  that adds five to whatever you hand it later.
//
//  hint: a method group can be used where a delegate is expected —
//  `WhereNot(nums, IsEven)` needs no lambda. Repeat's counter starts at
//  zero, like every index in .NET.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static int ApplyTwice(Func<int, int> f, int start) => throw new NotImplementedException("TODO");

static void Repeat(int times, Action<int> body) => throw new NotImplementedException("TODO");

static Func<int, int> Adder(int by) => throw new NotImplementedException("TODO");

static IEnumerable<T> WhereNot<T>(IEnumerable<T> items, Func<T, bool> predicate) =>
    throw new NotImplementedException("TODO");

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
