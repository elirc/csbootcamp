# C# language cheat sheet

Your offline MDN substitute for the language itself. Lookup tables and
three-line examples — the *why* lives in the module READMEs.

Verified against **.NET 10 / C# 14** on Windows.

## Types at a glance

| kind | examples | copied by | equality by default |
| --- | --- | --- | --- |
| value (`struct`) | `int`, `double`, `decimal`, `bool`, `char`, `DateOnly`, `Guid`, `TimeSpan`, tuples | value | field-by-field |
| reference (`class`) | `string`, arrays, `List<T>`, your classes | reference | reference |
| `record` (class) | `record User(string Name)` | reference | **value** |
| `record struct` | `record struct Point(int X, int Y)` | value | value |

```csharp
var b = a;            // struct: independent copy   |  class: same object
p with { X = 3 }      // a modified COPY of a record / record struct
```

## Numbers

| type | base | for | not for |
| --- | --- | --- | --- |
| `int` / `long` | 2 | counts, ids, indexes | anything divided |
| `double` | 2 | measurement, ratios, science | money |
| `decimal` | 10 | money, tax, audited figures | tight loops |

```csharp
7 / 2                 // 3     — integer division, always
(double)7 / 2         // 3.5
0.1 + 0.2 == 0.3      // false (double)
0.1m + 0.2m == 0.3m   // true  (decimal)
Math.Round(0.5)                                   // 0  — banker's rounding
Math.Round(0.5, MidpointRounding.AwayFromZero)    // 1  — invoice rounding
checked(int.MaxValue + 1)   // throws OverflowException (unchecked wraps)
```

## Null

```csharp
string? maybe = Find();
maybe?.Length              // null instead of a throw
maybe ?? "fallback"        // default when null
maybe ??= "fallback";      // assign only when null
maybe!.Length              // "trust me" — usually how NREs are born
string.IsNullOrWhiteSpace(maybe)   // null, "" and "   " all count as missing
```

`string?` vs `string` is **compile-time only**. Same type at runtime; guard
at boundaries anyway.

## Strings

```csharp
$"{name} is {age}"                     // interpolation
$"{amount,10:F2}"                      // width 10, 2 decimals
"""                                    // raw literal: no escaping needed
  {"json": true}
  """
string.Join(", ", items)               // separator BETWEEN items
new StringBuilder().Append(x).ToString()
text.Split(',', StringSplitOptions.RemoveEmptyEntries)
text.AsSpan()[1..^1]                   // slice, no allocation
```

**Comparison — always say which:**

```csharp
string.Equals(a, b, StringComparison.OrdinalIgnoreCase)  // keys, ids, tokens
a.StartsWith(p, StringComparison.Ordinal)
a.ToLowerInvariant()                                     // for a stored key
string.Compare(a, b, StringComparison.CurrentCulture)    // only for display order
```

`a.ToLower() == b.ToLower()` is a bug in Turkish locales.

## Patterns

```csharp
value switch
{
    < 0 => "negative",                       // relational
    0 => "zero",                             // constant
    < 1000 and not 42 => "small",            // combinators
    _ => "large",                            // discard = everything else
}

obj switch
{
    int n when n > 5 => n,                   // type pattern + guard
    string { Length: > 0 } s => s.Length,    // property pattern
    Order { Total: var t, IsPaid: true } => t,
    null => 0,
    _ => -1,
}

if (value is int n) { /* checked AND unboxed */ }
var (min, max) = MinMax(xs);                 // deconstruction
```

## Methods

```csharp
static int F(int a, int b = 2) => a + b;     // optional (baked into the CALL SITE)
F(1, b: 5);                                  // named
static int Sum(params int[] xs) => ...;      // params: empty array when none
static bool TryGet(string k, out int v)      // out: MUST be assigned on every path
static void Swap(ref int a, ref int b)       // ref: shares the caller's variable
static int Size(in BigStruct s)              // in: pass by reference, read-only
TryGet("k", out _);                          // discard
```

## Delegates, lambdas, events

```csharp
Func<int, string> f;        // last type argument is the RETURN type
Action<string> a;           // returns nothing
Predicate<int> p;           // returns bool

items.Where(string.IsNullOrEmpty);           // method group, no lambda

var by = 5;
Func<int,int> add = x => x + by;             // captures the VARIABLE, not the value

for (var i = 0; i < 3; i++) fns.Add(() => i);        // BUG: all see 3
for (var i = 0; i < 3; i++) { var j = i; fns.Add(() => j); }  // fixed
foreach (var x in xs) fns.Add(() => x);              // fine since C# 5

public event EventHandler<string>? Changed;
Changed?.Invoke(this, "x");                  // an event with no subscribers is NULL
Changed -= handler;                          // matches on identity: keep a reference
```

## Exceptions

```csharp
try { ... }
catch (HttpRequestException ex) when (ex.StatusCode == 404) { }   // filter
catch (Exception ex) { throw; }              // rethrow: KEEPS the stack trace
                                             // `throw ex;` RESETS it — never do that
finally { }                                  // runs on every path
using var stream = File.OpenRead(path);      // Dispose at end of scope

ArgumentNullException.ThrowIfNull(arg);
ArgumentOutOfRangeException.ThrowIfNegative(count);
```

## Types you write

```csharp
record User(string Name, int Age);           // value equality, ToString, with, deconstruct
sealed class Service(IClock clock)           // primary constructor
{
    public required string Name { get; init; }   // must be set in the initialiser
    private readonly IClock _clock = clock;
}
interface IClock { DateTimeOffset Now { get; } }
```

`sealed` by default is a good habit: it documents intent and lets the JIT
devirtualise calls.

## Time

```csharp
DateOnly.FromDateTime(DateTime.Today)   // a calendar date
new DateOnly(2026, 3, 1).AddDays(1)
b.DayNumber - a.DayNumber               // days between, no hours involved
DateTimeOffset.UtcNow                   // a moment (store UTC)
TimeSpan.FromMinutes(5)
span.Hours        // the COMPONENT (0-23)
span.TotalHours   // the whole span as a fraction
born.AddYears(age) > today ? age - 1 : age    // the age calculation
```

Take an `IClock` / `TimeProvider` parameter instead of reading `UtcNow`
inside a method you want to test.

## Regex

```csharp
Regex.IsMatch(v, @"^[A-Z]{2}-\d{4}$")   // ANCHOR validation patterns
Regex.Matches(t, @"#\w+").Select(m => m.Value)
Regex.Replace(t, @"\d{12,}", "****")
new Regex(pattern, RegexOptions.Compiled)     // hoist to a static readonly field
[GeneratedRegex(@"^\d+$")] private static partial Regex Digits();
```

Avoid nested quantifiers over ambiguous groups (`(\s*\w+)+`) — catastrophic
backtracking is a denial-of-service primitive.

## Spans

```csharp
ReadOnlySpan<char> s = "hello";
s[1..3]           // a window, no allocation
s.IndexOf(',')
int.Parse(s.Trim())
```

A `ref struct` cannot be a field, be captured by a lambda, or live across an
`await`. The compiler enforces it.

## Quick "which do I use"

| question | answer |
| --- | --- |
| compare two `object`s by value | `Equals(a, b)` (static, null-safe) |
| dedupe | `HashSet<T>`, or `Distinct()` |
| lookup by key | `Dictionary<K,V>.TryGetValue` |
| does absence mean a bug? | indexer. Otherwise `TryGetValue` / `GetValueOrDefault` |
| might fail, and that is normal | `TryParse` / return `T?` |
| might fail, and that is exceptional | throw |
| a value with no identity | `record` or `record struct` |
| a thing with identity | `class` |
