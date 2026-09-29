// ─────────────────────────────────────────────────────────────────────────
//  04 · dates that do not drift — SOLUTION                       ★★☆ core
//  concepts: DateOnly · date arithmetic · the age bug
//  run: dotnet run 04-dates-without-tears.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Use the type that matches the concept. A birthday, a check-in date
//  and an invoice date are CALENDAR dates: no time, no zone, no
//  midnight to be pushed across by a UTC conversion. DateOnly says that
//  in the type system. A moment in time — when a row was written, when a
//  token expires — is a DateTimeOffset, stored in UTC. Plain DateTime
//  sits awkwardly between the two and carries a Kind flag that is
//  silently wrong more often than it is right.
//  NextBusinessDay steps once and then keeps stepping while it is on a
//  weekend. Writing it as "add 1, then skip" rather than a table of
//  special cases means Friday, Saturday and Sunday all fall out of the
//  same two lines.
//  NightsBetween uses DayNumber, the count of days since year 1. It is
//  the honest way to subtract dates: no TimeSpan, no hours, nothing that
//  can be 23 or 25 long because a clock changed.
//  AgeOn is the classic. Subtracting the years gives the answer only if
//  the birthday has already happened this year — so compute the
//  anniversary in the target year and take one off when the date has not
//  reached it. The leap-day test is why AddYears is the right tool
//  rather than constructing a new date by hand: AddYears CLAMPS 29
//  February to 28 February in a common year instead of throwing, which
//  decides — for you — that a leap-day birthday falls on the 28th. That
//  is a defensible convention and it is also a product decision; if your
//  domain says 1 March, you have to write that yourself. The point is
//  that the library made a choice and you should know which one.
//  The general rule: any date calculation that mentions "today" should
//  take the date as a PARAMETER. A method that reads DateTime.Now
//  internally cannot be tested at a boundary, which is exactly where its
//  bugs live. The next exercise makes that injection explicit.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static DateOnly NextBusinessDay(DateOnly day)
{
    var next = day.AddDays(1);
    while (next.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
    {
        next = next.AddDays(1);
    }

    return next;
}

static int NightsBetween(DateOnly checkIn, DateOnly checkOut) =>
    checkOut.DayNumber - checkIn.DayNumber;

static int AgeOn(DateOnly birthday, DateOnly on)
{
    var age = on.Year - birthday.Year;
    return birthday.AddYears(age) > on ? age - 1 : age;
}

// ─── tests ───────────────────────────────────────────────────────────────

Test("a weekday moves to the next day", () =>
    Eq(NextBusinessDay(new DateOnly(2026, 3, 4)), new DateOnly(2026, 3, 5)));

Test("Friday jumps the weekend", () =>
    Eq(NextBusinessDay(new DateOnly(2026, 3, 6)), new DateOnly(2026, 3, 9)));

Test("Saturday and Sunday both land on Monday", () =>
{
    Eq(NextBusinessDay(new DateOnly(2026, 3, 7)), new DateOnly(2026, 3, 9));
    Eq(NextBusinessDay(new DateOnly(2026, 3, 8)), new DateOnly(2026, 3, 9));
});

Test("NightsBetween counts the nights", () =>
    Eq(NightsBetween(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 4)), 3));

Test("same day is no nights", () =>
    Eq(NightsBetween(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 1)), 0));

Test("NightsBetween spans a month end", () =>
    Eq(NightsBetween(new DateOnly(2026, 1, 30), new DateOnly(2026, 2, 2)), 3));

Test("age on the day before the birthday", () =>
    Eq(AgeOn(new DateOnly(2000, 6, 15), new DateOnly(2026, 6, 14)), 25));

Test("age on the birthday itself", () =>
    Eq(AgeOn(new DateOnly(2000, 6, 15), new DateOnly(2026, 6, 15)), 26));

Test("a leap-day birthday counts on 28 February in a common year", () =>
    Eq(AgeOn(new DateOnly(2000, 2, 29), new DateOnly(2026, 2, 28)), 26));

return Report();
