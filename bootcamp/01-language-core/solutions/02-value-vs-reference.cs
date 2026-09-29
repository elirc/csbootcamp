// ─────────────────────────────────────────────────────────────────────────
//  02 · what a copy copies — SOLUTION                       ★☆☆ warm-up
//  concepts: value types · reference types · struct vs class
//  run: dotnet run 02-value-vs-reference.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  One rule explains all six tests: a variable of a VALUE type holds the
//  data; a variable of a REFERENCE type holds an arrow to data that
//  lives elsewhere. Assigning or passing copies whatever the variable
//  holds — the data in the first case, the arrow in the second.
//  So MovedRight cannot mutate the caller's Point even if it tries: it
//  was handed a copy. `with` makes that explicit by producing a new
//  value rather than editing one. Rename CAN mutate the caller's Box,
//  because both names point at the same object.
//  CopyOf is the interesting one. Returning `box` would compile, pass
//  the "same name" test, and fail the next one — a "copy" that is the
//  original is the shared-reference bug, and it is the single most
//  common data bug in any language with references.
//  Equality follows the same split. A record struct gets value equality
//  for free, so two Points with the same numbers are Equal. A plain
//  class gets reference equality, so two Boxes with the same name are
//  two different objects — which is why `==` on your own classes is
//  almost never what a business rule means.
//  In .NET this shows up as: structs for small immutable values
//  (Point, DateOnly, Money), classes for things with identity (a
//  Customer, an Order). Mutable structs are a trap for exactly this
//  reason — you mutate a copy and the change disappears.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Point MovedRight(Point p, int dx) => p with { X = p.X + dx };

static void Rename(Box box, string name) => box.Name = name;

static Box CopyOf(Box box) => new Box(box.Name);

// ─── tests ───────────────────────────────────────────────────────────────

Test("MovedRight returns the shifted point", () =>
    Eq(MovedRight(new Point(1, 2), 3), new Point(4, 2)));

Test("MovedRight leaves the caller's point alone", () =>
{
    var start = new Point(1, 2);
    MovedRight(start, 3);
    Eq(start, new Point(1, 2), "a struct is copied when you pass it");
});

Test("Rename changes the caller's box", () =>
{
    var box = new Box("before");
    Rename(box, "after");
    Eq(box.Name, "after", "a class is passed by reference");
});

Test("CopyOf produces a box with the same name", () =>
    Eq(CopyOf(new Box("widgets")).Name, "widgets"));

Test("changing the copy does not touch the original", () =>
{
    var original = new Box("widgets");
    var copy = CopyOf(original);
    copy.Name = "gadgets";
    Eq(original.Name, "widgets", "CopyOf handed back the same object");
});

Test("equal values are Equal, equal objects are not", () =>
{
    Ok(MovedRight(new Point(1, 2), 0).Equals(new Point(1, 2)),
        "two structs holding the same numbers should be equal");
    var box = new Box("a");
    Ok(!ReferenceEquals(CopyOf(box), box), "a copy must be a different object");
});

return Report();

// ─── the types ───────────────────────────────────────────────────────────

readonly record struct Point(int X, int Y);

class Box(string name)
{
    public string Name { get; set; } = name;
}
