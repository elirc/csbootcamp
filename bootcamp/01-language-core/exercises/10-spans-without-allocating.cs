// ─────────────────────────────────────────────────────────────────────────
//  10 · slices that do not allocate                          ★★★ stretch
//  concepts: ReadOnlySpan<char> · slicing · parsing without Split
//  run: dotnet run 10-spans-without-allocating.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A log parser runs over millions of lines, and every Split() and
//  Substring() it does allocates a new string that the collector then
//  has to clean up. A Span is a WINDOW onto memory you already have:
//  slicing it copies nothing.
//
//      SumCsv("1,2,3")        → 6
//      SumCsv("")             → 0
//      TrimQuotes("\"hi\"")   → hi        (only if both quotes are there)
//      IsPalindrome("racecar")→ true
//
//  Write all three against ReadOnlySpan<char>, using no Split, no
//  Substring, and no ToString except where a test needs one.
//
//  hint: a string converts to a ReadOnlySpan<char> implicitly.
//  span.IndexOf(',') finds the next separator, span[..i] is everything
//  before it, span[(i + 1)..] everything after. int.Parse accepts a
//  span directly.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static int SumCsv(ReadOnlySpan<char> line) => throw new NotImplementedException("TODO");

static ReadOnlySpan<char> TrimQuotes(ReadOnlySpan<char> text) => throw new NotImplementedException("TODO");

static bool IsPalindrome(ReadOnlySpan<char> text) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("sums a list of numbers", () => Eq(SumCsv("1,2,3"), 6));

Test("sums a single number", () => Eq(SumCsv("42"), 42));

Test("an empty line sums to zero", () => Eq(SumCsv(""), 0));

Test("copes with spaces around the numbers", () => Eq(SumCsv(" 1 , 2 "), 3));

Test("strips a matching pair of quotes", () =>
    Eq(TrimQuotes("\"hi\"").ToString(), "hi"));

Test("leaves an unmatched quote alone", () =>
{
    Eq(TrimQuotes("\"hi").ToString(), "\"hi");
    Eq(TrimQuotes("hi\"").ToString(), "hi\"");
});

Test("leaves an unquoted value alone", () => Eq(TrimQuotes("hi").ToString(), "hi"));

Test("recognises a palindrome", () =>
{
    Ok(IsPalindrome("racecar"));
    Ok(IsPalindrome(""));
    Ok(IsPalindrome("a"));
});

Test("rejects a non-palindrome", () => Ok(!IsPalindrome("racecars")));

return Report();
