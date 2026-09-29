// ─────────────────────────────────────────────────────────────────────────
//  03 · patterns that match what you meant — SOLUTION           ★★☆ core
//  concepts: Regex · anchors · character classes · Replace
//  run: dotnet run 03-regex-basics.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Anchors are the whole lesson of IsReference. Regex.IsMatch asks "does
//  this pattern occur ANYWHERE in the input", so an unanchored
//  validation regex happily accepts "xAB-1234junk". Wrapping the pattern
//  in ^ and $ makes it "the entire string is this shape". Every
//  validator you write needs them, and forgetting them is the bug that
//  lets an attacker append anything they like to a value you checked.
//  (In .NET, $ also matches before a trailing newline — for input that
//  might carry one, backslash-z is the strict end-of-string anchor.)
//  Where the pattern LIVES matters. Constructing a Regex parses and
//  compiles it, so `new Regex(...)` inside a per-request method repeats
//  that work forever. The static Regex.IsMatch/Matches/Replace methods
//  used here keep a small internal cache, which is why they are fine for
//  a handful of patterns. In a real class you would hoist them into
//  `static readonly Regex` fields (top-level statements cannot declare
//  those — a file-based app has locals, not fields), and add
//  RegexOptions.Compiled for a genuinely hot one. .NET 7+ also has the
//  [GeneratedRegex] source generator, which builds the matcher at
//  compile time with no startup cost at all.
//  Hashtags uses Matches, which returns every non-overlapping match, and
//  a character class that says where a tag ENDS: word characters only,
//  so the comma and the space stop it. Being explicit about the ending
//  is usually easier than being clever about the beginning.
//  MaskDigits shows Replace with a quantifier range: {12,} is "twelve or
//  more", and + being greedy means a long run is replaced once rather
//  than in pieces. Test the boundary — 11 digits must survive — because
//  a quantifier off by one is invisible until real data arrives.
//  And the standing warning: a nested quantifier over an ambiguous group
//  can backtrack catastrophically. Keep validation patterns boring, and
//  cap input length before matching.

#:project ../../_lib/Check/Check.csproj

using System.Text.RegularExpressions;
using static Bootcamp.Check;

static bool IsReference(string value) => Regex.IsMatch(value, @"^[A-Z]{2}-\d{4}$");

static List<string> Hashtags(string text) =>
    Regex.Matches(text, @"#\w+").Select(m => m.Value).ToList();

static string MaskDigits(string text) => Regex.Replace(text, @"\d{12,}", "****");

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
