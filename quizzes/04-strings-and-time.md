# Quiz 04 · Strings and Time

Say the answer out loud before you expand it.

Pairs with [`bootcamp/04-strings-and-time`](../bootcamp/04-strings-and-time/).

---

**1.** Why is `s += x` in a loop a performance bug?

<details><summary>answer</summary>

Strings are immutable: each `+=` allocates a new string and copies both
halves, so n pieces copy roughly n² characters and leave n dead strings.
Use `string.Join` where it fits, `StringBuilder` where it does not.
</details>

**2.** What problem does `string.Join` solve that manual concatenation gets
wrong?

<details><summary>answer</summary>

The separator goes **between** items only — no trailing comma to trim and no
"is this the first iteration" flag. That flag is where the bugs are.
</details>

**3.** Your CSV export is unreadable by your own importer after deploying to
a German server. What did you do?

<details><summary>answer</summary>

Formatted with the ambient culture: `1234.5` became `"1.234,50"`. Machine
formats must pin `CultureInfo.InvariantCulture`, on the way out **and** on
the way back in.
</details>

**4.** `0.10m.ToString(CultureInfo.InvariantCulture)` — what do you get, and
why can `double` not do that?

<details><summary>answer</summary>

`"0.10"`. A `decimal` carries its scale, so it remembers the value was
written to two places. `double` has no scale — only a binary approximation
of the magnitude.
</details>

**5.** Why prefer `decimal.TryParse` returning `decimal?` over
`decimal.Parse` in a try/catch?

<details><summary>answer</summary>

It makes failure part of the type. The caller cannot forget to handle a
`decimal?`; they can very easily forget a `catch`. Exceptions are for the
exceptional, and "a user typed nonsense" is not exceptional.
</details>

**6.** `Regex.IsMatch("xAB-1234junk", @"[A-Z]{2}-\d{4}")` — true or false, and
why does it matter?

<details><summary>answer</summary>

True. A regex matches **anywhere** unless anchored. An unanchored validation
pattern accepts anything with your shape hidden inside it, which is how
values you "validated" carry a payload. Wrap it in `^` and `$`.
</details>

**7.** In .NET, what does `$` match that you might not expect?

<details><summary>answer</summary>

It also matches immediately before a trailing newline at the end of the
input. `\z` is the strict end-of-string anchor.
</details>

**8.** Why is `new Regex(pattern)` inside a per-request method a problem?

<details><summary>answer</summary>

Constructing a `Regex` parses the pattern; doing it per call repeats that
work forever. Hoist it into a `static readonly` field, use the static
`Regex.IsMatch` helpers (they keep a small cache), or `[GeneratedRegex]`.
</details>

**9.** What is catastrophic backtracking, and what pattern shape causes it?

<details><summary>answer</summary>

An input that *almost* matches forces the engine to try every way of
splitting the text before it can say no, and the number of ways doubles per
character. The shape is a quantifier over a group that itself ends in a
quantifier with an optional separator — `(a+)+`, `(\s*\w+)*`, `(\w+,?)+`.
One small request can pin a CPU core.
</details>

**10.** Booking dates shift by a day after a deploy. What type were they
stored in, and what should they be?

<details><summary>answer</summary>

A `DateTime` — a moment, subject to timezone conversion across midnight. A
check-in date is a **calendar date**: `DateOnly`, which has no time to
shift.
</details>

**11.** `DateOnly`, `TimeOnly`, `DateTimeOffset`, `TimeSpan` — one use each.

<details><summary>answer</summary>

`DateOnly`: a birthday or invoice date. `TimeOnly`: opening hours.
`DateTimeOffset` (UTC): when something happened. `TimeSpan`: a duration —
not a date at all.
</details>

**12.** A span is 26 hours long. What is `span.Hours`?

<details><summary>answer</summary>

`2` — it is the hour **component**. `TotalHours` is the whole span
(26.0). Mixing them up is the standard `TimeSpan` bug.
</details>

**13.** Why is `(today - born).Days / 365` the wrong way to compute an age?

<details><summary>answer</summary>

Leap years make it drift, and it answers a different question. Compute the
anniversary — `born.AddYears(age)` — and subtract one if it has not been
reached yet.
</details>

**14.** What does `AddYears` do to 29 February in a common year?

<details><summary>answer</summary>

Clamps it to the 28th rather than throwing. That silently decides when a
leap-day birthday falls; if your domain says 1 March, you have to write it.
</details>

**15.** Why should `IsExpired` take a clock parameter?

<details><summary>answer</summary>

Because otherwise its boundary cases — one second before expiry, exactly at
expiry — can only be tested by waiting, so nobody tests them, so that is
where the bugs are. One interface with one property (or .NET 8's
`TimeProvider`) makes those tests instant.
</details>

**16.** Why compute days as `a.DayNumber - b.DayNumber` rather than
subtracting two `DateTime`s?

<details><summary>answer</summary>

Subtracting `DateTime`s produces a `TimeSpan` measured in hours, and a day
with a clock change is 23 or 25 hours long. `DayNumber` counts days, so the
answer cannot drift.
</details>
