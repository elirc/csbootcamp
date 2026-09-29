// ─────────────────────────────────────────────────────────────────────────
//  02 · what a copy copies                                  ★☆☆ warm-up
//  concepts: value types · reference types · struct vs class
//  run: dotnet run 02-value-vs-reference.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Two types are defined at the BOTTOM of this file: Point is a struct
//  (a value), Box is a class (a reference). Passing one to a method
//  copies the value; passing the other copies the arrow, not the thing.
//
//  Write three methods and let the tests show you the difference:
//
//      MovedRight(p, 3)  → a NEW point 3 to the right; p is untouched
//      Rename(box, "b")  → renames the CALLER'S box, returns nothing
//      CopyOf(box)       → a NEW box with the same name, independent
//
//  hint: `p with { X = p.X + dx }` builds a modified copy of a record
//  struct. For CopyOf you want `new Box(...)`, not `box`.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static Point MovedRight(Point p, int dx) => throw new NotImplementedException("TODO");

static void Rename(Box box, string name) => throw new NotImplementedException("TODO");

static Box CopyOf(Box box) => throw new NotImplementedException("TODO");

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
