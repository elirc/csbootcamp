// ─────────────────────────────────────────────────────────────────────────
//  05 · durations, and a clock you can control                  ★★☆ core
//  concepts: TimeSpan · injected time · testable expiry
//  run: dotnet run 05-durations-and-the-clock.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Code that calls DateTimeOffset.UtcNow directly cannot be tested at
//  its boundaries — you would have to wait. Take the clock as a
//  parameter instead. IClock and a FixedClock for tests are at the
//  BOTTOM of this file.
//
//      IsExpired(issued, ttl: 5m, clock at issued + 4m59s) → false
//      IsExpired(issued, ttl: 5m, clock at issued + 5m)    → true
//      Humanize(2h 5m)      → "2h 5m"
//      Humanize(45s)        → "45s"
//      Backoff(0)           → 1s      (then 2s, 4s, 8s…, capped at 30s)
//
//  Expiry is inclusive: exactly at the deadline counts as expired.
//
//  hint: TimeSpan.FromSeconds and the arithmetic operators do the work.
//  For the backoff, doubling is a left shift or Math.Pow — mind the cap
//  BEFORE the number gets huge, because attempt 40 overflows.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool IsExpired(DateTimeOffset issuedAt, TimeSpan ttl, IClock clock) =>
    throw new NotImplementedException("TODO");

static string Humanize(TimeSpan span) => throw new NotImplementedException("TODO");

static TimeSpan Backoff(int attempt) => throw new NotImplementedException("TODO");

// ─── scaffolding ─────────────────────────────────────────────────────────

var issued = new DateTimeOffset(2026, 3, 1, 12, 0, 0, TimeSpan.Zero);
var ttl = TimeSpan.FromMinutes(5);

// ─── tests ───────────────────────────────────────────────────────────────

Test("not expired before the deadline", () =>
    Ok(!IsExpired(issued, ttl, new FixedClock(issued.AddMinutes(4).AddSeconds(59)))));

Test("expired exactly at the deadline", () =>
    Ok(IsExpired(issued, ttl, new FixedClock(issued.Add(ttl)))));

Test("expired well after", () =>
    Ok(IsExpired(issued, ttl, new FixedClock(issued.AddHours(1)))));

Test("not expired at the moment it was issued", () =>
    Ok(!IsExpired(issued, ttl, new FixedClock(issued))));

Test("Humanize shows hours and minutes", () =>
    Eq(Humanize(new TimeSpan(2, 5, 0)), "2h 5m"));

Test("Humanize shows minutes and seconds", () =>
    Eq(Humanize(new TimeSpan(0, 5, 3)), "5m 3s"));

Test("Humanize shows seconds alone", () =>
{
    Eq(Humanize(TimeSpan.FromSeconds(45)), "45s");
    Eq(Humanize(TimeSpan.Zero), "0s");
});

Test("Backoff doubles", () =>
    Eq(new[] { Backoff(0), Backoff(1), Backoff(2), Backoff(3) },
       new[]
       {
           TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2),
           TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8),
       }));

Test("Backoff is capped and never overflows", () =>
{
    Eq(Backoff(10), TimeSpan.FromSeconds(30));
    Eq(Backoff(1000), TimeSpan.FromSeconds(30));
});

return Report();

// ─── the types ───────────────────────────────────────────────────────────

interface IClock
{
    DateTimeOffset Now { get; }
}

sealed class FixedClock(DateTimeOffset now) : IClock
{
    public DateTimeOffset Now { get; } = now;
}
