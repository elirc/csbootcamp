// ─────────────────────────────────────────────────────────────────────────
//  03 · filter, project, flatten — SOLUTION                 ★☆☆ warm-up
//  concepts: Where · Select · SelectMany · empty sequences
//  run: dotnet run 03-select-where-flatten.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Where filters and keeps order. Select maps one element to one
//  element. SelectMany maps one element to a SEQUENCE and concatenates
//  the results, which is why it handles the empty group for free — an
//  empty inner sequence contributes nothing rather than a null hole.
//  These are the three shapes almost every data transformation reduces
//  to, and reading a pipeline as filter-project-flatten is how you make
//  sense of somebody else's forty-line query.
//  AverageAge is the real lesson. Average, Max, Min, First, Last, Single
//  and Aggregate-without-a-seed all THROW on an empty sequence, and the
//  message ("Sequence contains no elements") never says which query. The
//  fix is to decide what empty MEANS: here it is 0, elsewhere it might
//  be null or a sentinel, but it is a product decision either way. The
//  alternatives worth knowing: DefaultIfEmpty(0), or Average over a
//  nullable projection which returns null instead of throwing.
//  Note the tests call ToList inside the helpers rather than returning a
//  lazy query. Returning List<T> from a method whose name sounds like a
//  noun ("ActiveNames") is honest: the caller gets a value, not a
//  promise that will re-run when they enumerate it twice.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> ActiveNames(IEnumerable<User> users) =>
    users.Where(u => u.IsActive).Select(u => u.Name).ToList();

static List<string> Flatten(IEnumerable<string[]> groups) =>
    groups.SelectMany(group => group).ToList();

static double AverageAge(IEnumerable<User> users)
{
    var list = users as IList<User> ?? users.ToList();
    return list.Count == 0 ? 0 : list.Average(u => u.Age);
}

// ─── tests ───────────────────────────────────────────────────────────────

var people = new[]
{
    new User("ada", true, 36),
    new User("linus", false, 54),
    new User("grace", true, 45),
};

Test("ActiveNames keeps only the active users", () =>
    Eq(ActiveNames(people), new List<string> { "ada", "grace" }));

Test("ActiveNames keeps the original order", () =>
    Eq(ActiveNames(people.Reverse()), new List<string> { "grace", "ada" }));

Test("ActiveNames of nobody is empty", () => Eq(ActiveNames(Array.Empty<User>()).Count, 0));

Test("Flatten concatenates the groups", () =>
    Eq(Flatten(new[] { new[] { "a" }, new[] { "b", "c" } }), new List<string> { "a", "b", "c" }));

Test("Flatten skips empty groups", () =>
    Eq(Flatten(new[] { Array.Empty<string>(), new[] { "a" } }), new List<string> { "a" }));

Test("Flatten of nothing is empty", () =>
    Eq(Flatten(Array.Empty<string[]>()).Count, 0));

Test("AverageAge averages", () => Approx(AverageAge(people), 45.0, 0.001));

Test("AverageAge of an empty list is zero, not an exception", () =>
    Approx(AverageAge(Array.Empty<User>()), 0.0));

return Report();

// ─── the types ───────────────────────────────────────────────────────────

record User(string Name, bool IsActive, int Age);
