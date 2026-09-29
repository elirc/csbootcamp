// ─────────────────────────────────────────────────────────────────────────
//  04 · methods on types you do not own — SOLUTION             ★★☆ core
//  concepts: extension methods · this-parameters · discoverability
//  run: dotnet run 04-extension-methods.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  An extension method is pure compiler sugar: `"x".Truncate(3)` is
//  rewritten to `Extensions.Truncate("x", 3)`. Nothing is added to
//  string, no subclassing happens, and there is no runtime cost. That
//  also explains the limits — an extension cannot see private state,
//  cannot be overridden, and loses to a real instance method of the same
//  name every time.
//  Because the call is static, calling one on `null` does NOT throw at
//  the call site: the method simply receives null. That is why
//  `IsBlank()` can be declared on `string?` and used on a null variable,
//  and it is the trick behind guard-style helpers.
//  Truncate: the ellipsis is one of the `max` characters, so the slice
//  is `max - 1` long. Getting that wrong by one is the entire bug class
//  this method exists to teach — and the "exact length" test is the one
//  that catches it.
//  WhereNotNull is generic with a `class` constraint so the compiler can
//  hand back IEnumerable<T> (non-nullable) from IEnumerable<T?>. It is
//  the single most useful extension in a nullable-aware codebase,
//  because `Where(x => x != null)` still gives you a sequence the
//  compiler thinks is nullable.
//  Use them for behaviour that genuinely belongs to a type you cannot
//  edit. Do not use them to bolt domain logic onto primitives — that is
//  how a codebase ends up with fifty methods on string and no model.

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
        value.Length <= max ? value : value[..(max - 1)] + "…";

    public static bool IsBlank(this string? value) => string.IsNullOrWhiteSpace(value);

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items) where T : class =>
        items.Where(item => item is not null)!;
}
