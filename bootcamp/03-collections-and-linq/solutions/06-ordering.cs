// ─────────────────────────────────────────────────────────────────────────
//  06 · sorting that means what you said — SOLUTION             ★★☆ core
//  concepts: OrderBy · ThenBy · stability · comparing strings as numbers
//  run: dotnet run 06-ordering.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  OrderBy starts a sort; ThenBy adds a tie-breaker to the SAME sort.
//  Writing OrderBy twice does not layer them — the second one re-sorts
//  from scratch and the first is wasted work with a misleading name.
//  That is the single most common LINQ ordering bug, and it is invisible
//  until two items tie.
//  TopPriority combines descending primary with ascending secondary,
//  which reads exactly as the requirement does: most urgent first, ties
//  alphabetical. Take after ordering is fine — LINQ to Objects will
//  still sort the whole sequence, but the intent is clear and the cost
//  only matters at scale (where you would reach for a heap; see the
//  data-structures module).
//  SortNumeric is the "10 before 9" trap. Sorting text compares
//  character by character, so "10" < "9" because '1' < '9'. Projecting
//  to a number in the key selector fixes it while KEEPING the original
//  strings in the output — that is why the key selector exists rather
//  than transforming the sequence first. The "007" test proves you did
//  not accidentally return the parsed numbers.
//  Stability: LINQ's OrderBy is documented stable, so equal keys stay in
//  input order — worth relying on. List.Sort and Array.Sort use an
//  unstable introsort, so equal elements can be reordered. If ties must
//  keep their order, either use LINQ or add an explicit tie-breaker.
//  For a custom rule you cannot express as a key, OrderBy has an
//  overload taking an IComparer<T>. Reach for the key selector first —
//  a comparer that is not a total order (inconsistent ties) will throw
//  "IComparer.Compare() method returns inconsistent results".

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> ByLengthThenAlpha(IEnumerable<string> words) =>
    words.OrderBy(w => w.Length)
         .ThenBy(w => w, StringComparer.Ordinal)
         .ToList();

static List<string> TopPriority(IEnumerable<Ticket> tickets, int count) =>
    tickets.OrderByDescending(t => t.Priority)
           .ThenBy(t => t.Title, StringComparer.Ordinal)
           .Take(count)
           .Select(t => t.Title)
           .ToList();

static List<string> SortNumeric(IEnumerable<string> numbers) =>
    numbers.OrderBy(int.Parse).ToList();

// ─── tests ───────────────────────────────────────────────────────────────

var tickets = new[]
{
    new Ticket("restore backups", 1),
    new Ticket("site is down", 3),
    new Ticket("add a tooltip", 1),
    new Ticket("payments failing", 3),
};

Test("shorter words come first", () =>
    Eq(ByLengthThenAlpha(new[] { "bb", "a", "cc" }), new List<string> { "a", "bb", "cc" }));

Test("same length sorts alphabetically", () =>
    Eq(ByLengthThenAlpha(new[] { "cc", "aa", "bb" }), new List<string> { "aa", "bb", "cc" }));

Test("ordering an empty list is fine", () =>
    Eq(ByLengthThenAlpha(Array.Empty<string>()).Count, 0));

Test("TopPriority takes the most urgent first", () =>
    Eq(TopPriority(tickets, 2), new List<string> { "payments failing", "site is down" }));

Test("TopPriority breaks ties by title", () =>
    Eq(TopPriority(tickets, 4),
        new List<string> { "payments failing", "site is down", "add a tooltip", "restore backups" }));

Test("TopPriority asking for more than there is returns everything", () =>
    Eq(TopPriority(tickets, 99).Count, 4));

Test("SortNumeric orders by value, not by text", () =>
    Eq(SortNumeric(new[] { "10", "9", "2" }), new List<string> { "2", "9", "10" }));

Test("SortNumeric keeps the original strings", () =>
    Eq(SortNumeric(new[] { "007", "8" }), new List<string> { "007", "8" }));

return Report();

// ─── the types ───────────────────────────────────────────────────────────

record Ticket(string Title, int Priority);
