// ─────────────────────────────────────────────────────────────────────────
//  07 · two answers from one call                            ★☆☆ warm-up
//  concepts: value tuples · named elements · deconstruction
//  run: dotnet run 07-tuples-and-deconstruction.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Sometimes a method has two answers and inventing a class for them is
//  ceremony. A value tuple with NAMED elements says it in one line:
//
//      MinMax([3, 1, 4])       → (Min: 1, Max: 4)
//      ParseLine("widget x3")  → (Name: "widget", Quantity: 3)
//      Format(("widget", 3))   → "3 x widget"
//
//  MinMax on an empty sequence returns (0, 0) — there is no answer, and
//  throwing would make the caller's life harder than it needs to be.
//  ParseLine splits on the last " x"; a line with no quantity means 1.
//
//  hint: return (a, b) and the tuple builds itself. On the way back out
//  you can deconstruct: var (min, max) = MinMax(...).

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static (int Min, int Max) MinMax(IEnumerable<int> numbers) => throw new NotImplementedException("TODO");

static (string Name, int Quantity) ParseLine(string line) => throw new NotImplementedException("TODO");

static string Format((string Name, int Quantity) item) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("MinMax finds both ends", () => Eq(MinMax(new[] { 3, 1, 4 }), (1, 4)));

Test("MinMax of one number is that number twice", () => Eq(MinMax(new[] { 7 }), (7, 7)));

Test("MinMax of nothing is (0, 0)", () => Eq(MinMax(Array.Empty<int>()), (0, 0)));

Test("the elements are named", () =>
{
    var result = MinMax(new[] { 5, 2, 9 });
    Eq(result.Min, 2);
    Eq(result.Max, 9);
});

Test("the result deconstructs", () =>
{
    var (min, max) = MinMax(new[] { 5, 2, 9 });
    Eq(min, 2);
    Eq(max, 9);
});

Test("ParseLine splits name and quantity", () =>
    Eq(ParseLine("widget x3"), ("widget", 3)));

Test("ParseLine defaults a missing quantity to one", () =>
    Eq(ParseLine("widget"), ("widget", 1)));

Test("ParseLine keeps an x inside the name", () =>
    Eq(ParseLine("box x large x2"), ("box x large", 2)));

Test("Format prints quantity first", () => Eq(Format(("widget", 3)), "3 x widget"));

return Report();
