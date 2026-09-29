// ─────────────────────────────────────────────────────────────────────────
//  06 · sorting that means what you said                        ★★☆ core
//  concepts: OrderBy · ThenBy · stability · comparing strings as numbers
//  run: dotnet run 06-ordering.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A Ticket record is defined at the BOTTOM of this file.
//
//      ByLengthThenAlpha(["bb","a","cc"])  → ["a", "bb", "cc"]
//      TopPriority(tickets, 2)             → the two most urgent,
//                                            ties broken by title A-Z
//      SortNumeric(["10","9","2"])         → ["2", "9", "10"]
//
//  SortNumeric receives strings that hold numbers. Sorting them as text
//  puts "10" before "9", which is correct for text and wrong for the
//  answer anybody wanted.
//
//  hint: OrderBy then ThenBy expresses "primary, then tie-breaker" —
//  calling OrderBy twice throws the first sort away. Descending has its
//  own method. LINQ's OrderBy is STABLE (equal items keep their input
//  order); List.Sort is not.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> ByLengthThenAlpha(IEnumerable<string> words) =>
    throw new NotImplementedException("TODO");

static List<string> TopPriority(IEnumerable<Ticket> tickets, int count) =>
    throw new NotImplementedException("TODO");

static List<string> SortNumeric(IEnumerable<string> numbers) =>
    throw new NotImplementedException("TODO");

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
