// ─────────────────────────────────────────────────────────────────────────
//  08 · out, ref, and the Try pattern — SOLUTION                ★★☆ core
//  concepts: out parameters · ref parameters · TryParse
//  run: dotnet run 08-out-and-ref.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  The three keywords answer three different questions.
//  OUT means "I will give this to you" — the caller need not initialise
//  it, and the method MUST assign it on every path. That last rule is
//  why the failure path here writes 0 rather than leaving the value
//  alone: a Try method that fails and leaves a stale number behind is
//  the source of a whole family of bugs, because callers read the value
//  after ignoring the bool.
//  REF means "we share this variable" — assignments inside are visible
//  outside, which is what makes Swap possible at all. Passing a class
//  by value already lets you mutate the OBJECT; ref is what lets you
//  replace the caller's variable itself.
//  IN (not used here) means "I promise not to change it" and passes a
//  large struct without copying.
//  Note the layering: TryParsePort delegates the hard part to
//  int.TryParse and adds one domain rule (the range). Validation that
//  wraps a parser, rather than re-implementing it, is how you keep
//  edge cases someone else already got right.
//  When NOT to use out: when there is no bool. CountAndSum works, but
//  returning (Sum, Count) as a named tuple reads better and composes;
//  out shines specifically in the Try pattern, where the bool is the
//  point and the value is the payload.
//  Discards keep the intent visible: `out _` says "there is a value and
//  I genuinely do not want it".

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool TryParsePort(string? text, out int port)
{
    port = 0;
    if (!int.TryParse(text, out var parsed)) return false;
    if (parsed is < 1 or > 65535) return false;

    port = parsed;
    return true;
}

static void Swap(ref int a, ref int b) => (a, b) = (b, a);

static int CountAndSum(IEnumerable<int> numbers, out int count)
{
    count = 0;
    var sum = 0;
    foreach (var n in numbers)
    {
        count++;
        sum += n;
    }

    return sum;
}

// ─── tests ───────────────────────────────────────────────────────────────

Test("a good port parses", () =>
{
    Ok(TryParsePort("8080", out var port));
    Eq(port, 8080);
});

Test("the boundaries are inside", () =>
{
    Ok(TryParsePort("1", out var low));
    Eq(low, 1);
    Ok(TryParsePort("65535", out var high));
    Eq(high, 65535);
});

Test("zero and too-large are rejected", () =>
{
    Ok(!TryParsePort("0", out _));
    Ok(!TryParsePort("65536", out _));
});

Test("junk is rejected and the out value is zeroed", () =>
{
    Ok(!TryParsePort("abc", out var port));
    Eq(port, 0, "a failed Try must not leave a stale value behind");
});

Test("null is rejected", () => Ok(!TryParsePort(null, out _)));

Test("Swap swaps the caller's variables", () =>
{
    var a = 1;
    var b = 2;
    Swap(ref a, ref b);
    Eq((a, b), (2, 1));
});

Test("CountAndSum returns the sum and reports the count", () =>
{
    var sum = CountAndSum(new[] { 1, 2, 3 }, out var count);
    Eq(sum, 6);
    Eq(count, 3);
});

Test("CountAndSum copes with an empty sequence", () =>
{
    var sum = CountAndSum(Array.Empty<int>(), out var count);
    Eq((sum, count), (0, 0));
});

return Report();
