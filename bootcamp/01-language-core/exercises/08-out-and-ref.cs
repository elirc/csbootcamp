// ─────────────────────────────────────────────────────────────────────────
//  08 · out, ref, and the Try pattern                           ★★☆ core
//  concepts: out parameters · ref parameters · TryParse
//  run: dotnet run 08-out-and-ref.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  .NET's answer to "this might not work and I do not want an
//  exception" is the Try pattern: return bool, hand the value back
//  through an out parameter, and set it to default on failure.
//
//      TryParsePort("8080", out var p)  → true,  p = 8080
//      TryParsePort("70000", out var p) → false, p = 0   (out of range)
//      TryParsePort("abc", out var p)   → false, p = 0
//      TryParsePort(null, out var p)    → false, p = 0
//
//  A valid port is 1 to 65535 inclusive. Then two more:
//
//      Swap(ref a, ref b)               swaps the caller's variables
//      CountAndSum([1,2,3], out count)  → 6, count = 3
//
//  hint: int.TryParse already does half of TryParsePort. An out
//  parameter MUST be assigned on every path before you return — the
//  compiler enforces it, including the failure path.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool TryParsePort(string? text, out int port) => throw new NotImplementedException("TODO");

static void Swap(ref int a, ref int b) => throw new NotImplementedException("TODO");

static int CountAndSum(IEnumerable<int> numbers, out int count) => throw new NotImplementedException("TODO");

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
