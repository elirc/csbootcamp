// ─────────────────────────────────────────────────────────────────────────
//  04 · dates that do not drift                                 ★★☆ core
//  concepts: DateOnly · date arithmetic · the age bug
//  run: dotnet run 04-dates-without-tears.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  A calendar date is not a moment in time, and using DateTime for one
//  is how bookings move by a day when the server changes timezone.
//  DateOnly exists for exactly this. Three calculations:
//
//      NextBusinessDay(Fri 2026-03-06) → Mon 2026-03-09
//      NightsBetween(03-01, 03-04)     → 3
//      AgeOn(born 2000-06-15, on 2026-06-14) → 25   (birthday not yet)
//      AgeOn(born 2000-06-15, on 2026-06-15) → 26   (birthday today)
//
//  NextBusinessDay always moves forward at least one day and never
//  lands on a Saturday or Sunday.
//
//  hint: DateOnly has AddDays and DayOfWeek. The gap between two dates
//  is (a.DayNumber - b.DayNumber). For the age, compare the month and
//  day AFTER subtracting the years — that comparison is the exercise.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static DateOnly NextBusinessDay(DateOnly day) => throw new NotImplementedException("TODO");

static int NightsBetween(DateOnly checkIn, DateOnly checkOut) => throw new NotImplementedException("TODO");

static int AgeOn(DateOnly birthday, DateOnly on) => throw new NotImplementedException("TODO");

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
