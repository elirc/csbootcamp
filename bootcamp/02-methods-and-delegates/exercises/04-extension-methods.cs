// ─────────────────────────────────────────────────────────────────────────
//  04 · methods on types you do not own                        ★★☆ core
//  concepts: extension methods · this-parameters · discoverability
//  run: dotnet run 04-extension-methods.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  An extension method is a static method that LOOKS like an instance
//  method. It is how LINQ adds Where to every sequence ever written,
//  including sequences that shipped before LINQ existed.
//
//  The code you edit is at the BOTTOM of this file, in a static class.
//  Fill in three of them:
//
//      "hello world".Truncate(8)  → "hello w…"   (8 chars, last is …)
//      "hello".Truncate(8)        → "hello"      (short enough, untouched)
//      "  ".IsBlank()             → true
//      new[]{ "a", null }.WhereNotNull()  → ["a"]
//
//  Truncate never returns more than `max` characters, and the ellipsis
//  counts as one of them.
//
//  hint: the first parameter carries `this` and is the value the caller
//  wrote before the dot. An extension on a NULLABLE type can be called
//  on null — the method just receives null, with no crash at the call.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

// ─── tests ───────────────────────────────────────────────────────────────

Test("Truncate leaves a short string alone", () => Eq("hello".Truncate(8), "hello"));

Test("Truncate cuts and marks a long string", () => Eq("hello world".Truncate(8), "hello w…"));

Test("Truncate never exceeds the limit", () => Eq("hello world".Truncate(3).Length, 3));

Test("Truncate handles the exact length", () => Eq("12345678".Truncate(8), "12345678"));

Test("IsBlank is true for null, empty and whitespace", () =>
{
    string? nothing = null;
    Ok(nothing.IsBlank());
    Ok("".IsBlank());
    Ok("   ".IsBlank());
});

Test("IsBlank is false for real text", () => Ok(!"a".IsBlank()));

Test("WhereNotNull drops the nulls", () =>
    Eq(new string?[] { "a", null, "b" }.WhereNotNull().ToList(), new List<string> { "a", "b" }));

Test("WhereNotNull keeps everything when there are no nulls", () =>
    Eq(new string?[] { "a" }.WhereNotNull().Count(), 1));

return Report();

// ─── the code you edit ───────────────────────────────────────────────────

static class Extensions
{
    public static string Truncate(this string value, int max) =>
        throw new NotImplementedException("TODO");

    public static bool IsBlank(this string? value) =>
        throw new NotImplementedException("TODO");

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items) where T : class =>
        throw new NotImplementedException("TODO");
}
