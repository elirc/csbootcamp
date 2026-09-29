// ─────────────────────────────────────────────────────────────────────────
//  07 · two answers from one call — SOLUTION                 ★☆☆ warm-up
//  concepts: value tuples · named elements · deconstruction
//  run: dotnet run 07-tuples-and-deconstruction.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  A ValueTuple is a struct the compiler writes for you. Naming the
//  elements in the return type costs nothing at runtime — the names are
//  metadata — and it turns result.Item1 into result.Min, which is the
//  whole point. Nobody remembers what Item2 meant.
//  MinMax enumerates ONCE into a list before asking two questions. The
//  obvious version, numbers.Min() and numbers.Max(), walks the sequence
//  twice, and on an IEnumerable that is a database query, a file read,
//  or a generator, walking twice is a correctness bug rather than a
//  performance note.
//  ParseLine uses LastIndexOf so a name containing " x" survives, and
//  int.TryParse so a malformed quantity falls back rather than throws.
//  This is the parse-do-not-validate habit in miniature: turn a loose
//  string into a shaped value once, at the edge.
//  When does a tuple stop being right? When the pair escapes the method
//  that made it — a tuple in a public API, a field, or a dictionary
//  value is a record waiting to be born. Tuples are for local
//  plumbing; records are for things that have a name in the domain.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static (int Min, int Max) MinMax(IEnumerable<int> numbers)
{
    var list = numbers as IList<int> ?? numbers.ToList();
    if (list.Count == 0) return (0, 0);

    var min = list[0];
    var max = list[0];
    foreach (var n in list)
    {
        if (n < min) min = n;
        if (n > max) max = n;
    }

    return (min, max);
}

static (string Name, int Quantity) ParseLine(string line)
{
    var at = line.LastIndexOf(" x", StringComparison.Ordinal);
    if (at < 0) return (line.Trim(), 1);

    var tail = line[(at + 2)..];
    return int.TryParse(tail, out var quantity)
        ? (line[..at].Trim(), quantity)
        : (line.Trim(), 1);
}

static string Format((string Name, int Quantity) item) =>
    $"{item.Quantity} x {item.Name}";

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
