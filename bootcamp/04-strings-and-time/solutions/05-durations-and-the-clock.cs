// ─────────────────────────────────────────────────────────────────────────
//  05 · durations, and a clock you can control — SOLUTION        ★★☆ core
//  concepts: TimeSpan · injected time · testable expiry
//  run: dotnet run 05-durations-and-the-clock.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  The design lesson comes first: time is a DEPENDENCY. A method that
//  reads DateTimeOffset.UtcNow has a hidden input you cannot set, so its
//  boundary cases — one second before expiry, exactly at expiry — can
//  only be tested by waiting, which nobody does, which is why those are
//  the cases that break. One interface with one property removes the
//  problem entirely, and the tests here run in microseconds because of
//  it. (.NET 8 ships TimeProvider for exactly this; a hand-rolled IClock
//  is the same idea and works everywhere.)
//  IsExpired is then one comparison, and the >= is the whole
//  specification: "exactly at the deadline counts as expired" is a
//  decision, and writing it as >= rather than > is where it gets
//  recorded. Off-by-one in time is the same bug class as off-by-one in
//  an index.
//  Humanize is a switch on magnitude, largest unit first. TimeSpan
//  exposes both Hours (the hour COMPONENT, 0-23) and TotalHours (the
//  whole span as a fraction) and mixing them up is the standard bug —
//  a 26-hour span has Hours == 2. Here the components are what we want
//  because the largest unit is chosen explicitly.
//  Backoff caps by taking the MINIMUM of the doubled value and the
//  ceiling, and it does the doubling in a way that cannot overflow: cap
//  the exponent first. `1 << 1000` is undefined-ish nonsense and
//  Math.Pow(2, 1000) is Infinity, which converts to a TimeSpan that
//  throws. Guarding the input rather than the output is the habit.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static bool IsExpired(DateTimeOffset issuedAt, TimeSpan ttl, IClock clock) =>
    clock.Now >= issuedAt + ttl;

static string Humanize(TimeSpan span) => span switch
{
    { TotalHours: >= 1 } => $"{(int)span.TotalHours}h {span.Minutes}m",
    { TotalMinutes: >= 1 } => $"{(int)span.TotalMinutes}m {span.Seconds}s",
    _ => $"{(int)span.TotalSeconds}s",
};

static TimeSpan Backoff(int attempt)
{
    var capped = Math.Min(attempt, 5); // 2^5 = 32 seconds, already over the cap
    var seconds = Math.Min(Math.Pow(2, capped), 30);
    return TimeSpan.FromSeconds(seconds);
}

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
