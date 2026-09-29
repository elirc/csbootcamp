// ─────────────────────────────────────────────────────────────────────────
//  03 · filter, project, flatten                            ★☆☆ warm-up
//  concepts: Where · Select · SelectMany · empty sequences
//  run: dotnet run 03-select-where-flatten.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  The three verbs that do most of the work in any codebase. A User
//  record is defined at the BOTTOM of this file.
//
//      ActiveNames(users)        → the names of the active ones, in order
//      Flatten([["a"],["b","c"]])→ ["a", "b", "c"]
//      AverageAge(users)         → 0 for an empty list, NOT an exception
//
//  That last one is the whole point of the exercise: LINQ's Average
//  THROWS on an empty sequence, and nearly every "sequence contains no
//  elements" crash in production is this line.
//
//  hint: Select maps one to one, SelectMany maps one to many and
//  concatenates. For the average, ask whether there is anything to
//  average before averaging — Any() is the cheap question.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> ActiveNames(IEnumerable<User> users) => throw new NotImplementedException("TODO");

static List<string> Flatten(IEnumerable<string[]> groups) => throw new NotImplementedException("TODO");

static double AverageAge(IEnumerable<User> users) => throw new NotImplementedException("TODO");

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
