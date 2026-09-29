// ─────────────────────────────────────────────────────────────────────────
//  10 · slices that do not allocate — SOLUTION               ★★★ stretch
//  concepts: ReadOnlySpan<char> · slicing · parsing without Split
//  run: dotnet run 10-spans-without-allocating.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  A Span is a pointer and a length: it describes a region of memory
//  that belongs to somebody else. Slicing produces another pointer and
//  length, so `span[..i]` costs nothing while `text.Substring(0, i)`
//  copies characters into a fresh string. Over a million log lines
//  that difference is the whole garbage-collection bill.
//  SumCsv walks the line one separator at a time: find the comma, parse
//  the head, keep the tail, repeat, and parse whatever is left at the
//  end. int.Parse has span overloads precisely so this loop never has
//  to build a string. Trimming is a slice too.
//  TrimQuotes shows why "span in, span out" is the right signature: the
//  result is a WINDOW onto the caller's characters, not a copy, so
//  nothing is allocated until somebody actually needs a string (which
//  is what the tests do with ToString). Check both ends before
//  trimming, or an unmatched quote silently eats a real character.
//  IsPalindrome is the two-pointer walk with no reversal and no copy —
//  compare the ends, step inward, stop when they cross.
//  The rules that come with the power: a ref struct like Span cannot be
//  a field of a class, cannot be captured by a lambda, cannot be used
//  across an await, and must not outlive the memory it points at. If
//  you need any of those, you need a string or an array — and the
//  compiler will tell you so rather than letting you dangle.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static int SumCsv(ReadOnlySpan<char> line)
{
    var total = 0;
    var rest = line.Trim();
    if (rest.IsEmpty) return 0;

    while (true)
    {
        var comma = rest.IndexOf(',');
        if (comma < 0) return total + int.Parse(rest.Trim());

        total += int.Parse(rest[..comma].Trim());
        rest = rest[(comma + 1)..];
    }
}

static ReadOnlySpan<char> TrimQuotes(ReadOnlySpan<char> text) =>
    text.Length >= 2 && text[0] == '"' && text[^1] == '"'
        ? text[1..^1]
        : text;

static bool IsPalindrome(ReadOnlySpan<char> text)
{
    for (int left = 0, right = text.Length - 1; left < right; left++, right--)
    {
        if (text[left] != text[right]) return false;
    }

    return true;
}

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
