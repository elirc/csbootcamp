# 04 · Strings and Time

Two topics, one theme: **both look simple and both have a hidden parameter
you did not set.** For strings it is the culture. For dates it is the clock
and the timezone. Nearly every bug in this module is invisible on your
machine and appears on a server configured differently — which is the worst
kind, because the test suite is green.

## The mental model

### 1 · A string never changes

```csharp
var s = "a";
s += "b";      // a NEW string; the old one is garbage
```

For two or three pieces that is irrelevant. In a loop it copies roughly
n² characters. Reach for `string.Join` first (it solves the
separator-between-items problem, so there is no trailing comma to trim) and
`StringBuilder` when `Join` cannot express the shape.

### 2 · Formatting has two audiences

| audience | wants | use |
| --- | --- | --- |
| a machine (CSV, JSON, a log, another service) | stable, culture-free, round-trippable | `CultureInfo.InvariantCulture` |
| a human | their own separators and symbols | a specific `CultureInfo` |

The bug is using the *ambient* culture for the machine case. Your export
writes `1.234,50` on a German server and the importer that wrote it cannot
read it back.

```csharp
amount.ToString(CultureInfo.InvariantCulture)   // "1234.5" everywhere
decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var v)
```

Note `decimal` keeps its **scale**: `0.10m` formats as `"0.10"`, because the
value remembers how precise it was. `double` cannot do that.

### 3 · A regex matches anywhere unless you anchor it

```csharp
Regex.IsMatch("xAB-1234junk", @"[A-Z]{2}-\d{4}")     // true  — probably a bug
Regex.IsMatch("xAB-1234junk", @"^[A-Z]{2}-\d{4}$")   // false — validation
```

Every validation pattern needs `^` and `$`. In .NET `$` also matches before
a trailing newline; `\z` is the strict end.

### 4 · Pick the type that matches the concept

| concept | type | why |
| --- | --- | --- |
| a birthday, a check-in date, an invoice date | `DateOnly` | no time to be shifted across midnight |
| a time of day with no date | `TimeOnly` | opening hours, alarms |
| a moment that happened | `DateTimeOffset` (UTC) | unambiguous, comparable |
| a duration | `TimeSpan` | not a date at all |
| `DateTime` | — | carries a `Kind` flag that is silently wrong more often than right |

### 5 · Time is a dependency

```csharp
bool IsExpired(DateTimeOffset issued, TimeSpan ttl, IClock clock) =>
    clock.Now >= issued + ttl;
```

A method that reads `DateTimeOffset.UtcNow` internally has an input you
cannot set, so its boundary cases can only be tested by waiting — which is
why those are the cases that break. Pass the clock. (.NET 8 ships
`TimeProvider` for this; a one-property `IClock` is the same idea.)

## The details that bite

1. **`ToLower()` and `ToUpper()` use the current culture.** For keys, ids
   and protocol tokens use `ToLowerInvariant` or, better,
   `StringComparison.OrdinalIgnoreCase` on the comparison itself.
2. **An unanchored validation regex accepts anything with your pattern
   somewhere inside it.**
3. **Constructing a `Regex` parses it.** Doing that per request repeats the
   work forever. Hoist it into a `static readonly` field, or use the static
   `Regex.IsMatch` helpers (they keep a small cache), or `[GeneratedRegex]`.
4. **Nested quantifiers backtrack catastrophically.** `(\s*\w+)+` on a
   30-character near-match can pin a core. Keep validation patterns boring
   and cap the input length.
5. **`TimeSpan.Hours` is the component (0–23); `TotalHours` is the whole
   span.** A 26-hour span has `Hours == 2`.
6. **Date subtraction through `DateTime` drags in hours**, which can be 23
   or 25 long when a clock changes. `DateOnly.DayNumber` differences cannot.
7. **`AddYears` clamps 29 February to the 28th** in a common year rather
   than throwing — a decision the library makes for you.
8. **Ages are not `(now - born).Days / 365`.** Compute the anniversary and
   check whether it has passed.

## Exercises

| # | file | ★ | what you build |
| --- | --- | --- | --- |
| 01 | `01-building-strings.cs` | ★☆☆ | `Join` over `+=`, a CSV field that needs quoting, indenting a block |
| 02 | `02-formatting-and-parsing.cs` | ★★☆ | one format for machines, one for humans, and a parse that cannot throw |
| 03 | `03-regex-basics.cs` | ★★☆ | anchors, a match-all, and a masking replace |
| 04 | `04-dates-without-tears.cs` | ★★☆ | business days, nights between, and the age bug |
| 05 | `05-durations-and-the-clock.cs` | ★★☆ | expiry you can test, humanised spans, capped backoff |

```
dotnet run exercises/04-dates-without-tears.cs
dotnet run solutions/04-dates-without-tears.cs
dotnet run ../verify.cs 04
dotnet run ../progress.cs 04
```

---

**Stuck?** `cheatsheets/csharp-language.md` (strings, formatting, regex, time) · **Self-check:** `quizzes/04-strings-and-time.md` · **Next:** `bootcamp/05-types-and-oop`
