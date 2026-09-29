// ─────────────────────────────────────────────────────────────────────────
//  03 · patterns that match what you meant                      ★★☆ core
//  concepts: Regex · anchors · character classes · Replace
//  run: dotnet run 03-regex-basics.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Three jobs a regex is genuinely the right tool for:
//
//      IsReference("AB-1234")     → true
//      IsReference("xAB-1234")    → false   (anchored: the WHOLE string)
//      Hashtags("hi #a and #b!")  → ["#a", "#b"]
//      MaskDigits("card 4111111111111111 ok")
//                                 → "card **** ok"
//
//  A reference is exactly two uppercase letters, a hyphen, and four
//  digits. MaskDigits replaces any run of 12 or more digits with four
//  asterisks and leaves shorter numbers alone.
//
//  hint: without anchors a regex matches ANYWHERE in the string, which
//  is the single most common regex bug in validation code. Use a
//  compiled static readonly Regex for anything used more than once, and
//  remember that + is greedy — it takes as much as it can.

#:project ../../_lib/Check/Check.csproj

using System.Text.RegularExpressions;
using static Bootcamp.Check;

static bool IsReference(string value) => throw new NotImplementedException("TODO");

static List<string> Hashtags(string text) => throw new NotImplementedException("TODO");

static string MaskDigits(string text) => throw new NotImplementedException("TODO");

// ─── tests ───────────────────────────────────────────────────────────────

Test("a good reference matches", () => Ok(IsReference("AB-1234")));

Test("the whole string must match", () =>
{
    Ok(!IsReference("xAB-1234"));
    Ok(!IsReference("AB-1234x"));
});

Test("the shape is exact", () =>
{
    Ok(!IsReference("ab-1234"), "letters must be uppercase");
    Ok(!IsReference("AB-123"), "four digits, not three");
    Ok(!IsReference("ABC-1234"), "two letters, not three");
});

Test("Hashtags finds them all", () =>
    Eq(Hashtags("hi #a and #b!"), new List<string> { "#a", "#b" }));

Test("Hashtags stops at punctuation and spaces", () =>
    Eq(Hashtags("#one, #two"), new List<string> { "#one", "#two" }));

Test("Hashtags of text with none is empty", () => Eq(Hashtags("nothing here").Count, 0));

Test("MaskDigits hides a long run", () =>
    Eq(MaskDigits("card 4111111111111111 ok"), "card **** ok"));

Test("MaskDigits leaves short numbers alone", () =>
    Eq(MaskDigits("order 42 of 7"), "order 42 of 7"));

Test("MaskDigits handles two runs", () =>
    Eq(MaskDigits("111111111111 and 222222222222"), "**** and ****"));

return Report();
