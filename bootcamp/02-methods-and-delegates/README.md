# 02 · Methods and Delegates

The moment behaviour becomes a value you can pass around, most of the design
patterns you were going to write as classes collapse into one parameter.
LINQ is built on it. Dependency injection is built on it. So is every
`Where`, every event handler, every retry policy, and every callback you
will ever debug.

This module is about that one idea and the machinery C# gives you for it:
delegates, lambdas, closures, extension methods, and events.

## The mental model

### 1 · A delegate is a typed reference to a method

```csharp
Func<int, int> square = x => x * x;        // takes int, returns int
Action<string> log = Console.WriteLine;    // takes string, returns nothing
Predicate<int> isEven = n => n % 2 == 0;   // takes int, returns bool
```

`Func`'s **last** type argument is the return type: `Func<string, int, bool>`
takes a `string` and an `int` and returns a `bool`. `Action` never returns
anything. You almost never need the `delegate` keyword any more.

A **method group** can be used wherever a delegate is expected — no lambda
required, and it reads better:

```csharp
names.Where(string.IsNullOrEmpty)      // not: x => string.IsNullOrEmpty(x)
```

### 2 · A lambda captures variables, not values

```csharp
var by = 5;
Func<int, int> add = x => x + by;
by = 100;
add(1);        // 101 — it captured the VARIABLE
```

The compiler moves `by` into a hidden object that both the lambda and the
enclosing code share. That is the whole mechanism, and both the useful
behaviour (a counter with private state) and the classic bug (every lambda
in a `for` loop seeing the final value) follow from it.

### 3 · Extension methods are static methods in disguise

```csharp
"hello".Truncate(3);          // compiles to Extensions.Truncate("hello", 3)
```

No subclassing, no runtime cost, no access to private state, and it loses to
a real instance method of the same name. Because the call is static, an
extension declared on `string?` can be called on a null variable without
throwing at the call site.

### 4 · An event is a multicast delegate with the doors locked

```csharp
public event EventHandler<string>? Published;
Published?.Invoke(this, message);        // the only safe way to raise it
```

Outside the declaring class you may only `+=` and `-=`. Inside, you raise it.
An event with no subscribers is `null` — not an empty list — which is why the
null-conditional invoke is the idiom rather than a style choice.

## The details that bite

1. **A `for` loop shares one variable across all iterations.** Capture it in
   a lambda and every lambda sees the last value. Copy it into a local
   inside the body. (`foreach` was fixed in C# 5 and captures per
   iteration.)
2. **`-=` matches on delegate identity.** Two lambdas with identical bodies
   are different objects, so unsubscribing an inline lambda removes
   nothing. Keep the handler in a variable.
3. **`+=` does not deduplicate.** Subscribe the same handler twice and it
   fires twice. This is the "why is this running three times" bug.
4. **A publisher holds a reference to every subscriber.** A long-lived
   event keeps short-lived objects alive forever. Unsubscribe in `Dispose`.
5. **A multicast `Func` returns only the last result.** Combining
   value-returning delegates silently throws away every other answer —
   multicast is for events, not for computing values.
6. **Optional argument defaults are baked into the call site.** Change a
   default in a library and callers keep the old value until they
   recompile.
7. **LINQ is lazy.** `Where` returns a query, not a result. Handing an
   un-enumerated sequence across a boundary is how you get "the connection
   is closed" from a line that looks like a list.
8. **An unbounded memoize is a memory leak.** Caching by user input with no
   size limit is a slow crash, not an optimisation.

## Exercises

| # | file | ★ | what you build |
| --- | --- | --- | --- |
| 01 | `01-arguments-that-behave.cs` | ★☆☆ | optional, named and `params` arguments |
| 02 | `02-func-and-action.cs` | ★☆☆ | behaviour as a parameter, and a function that returns one |
| 03 | `03-closures-and-capture.cs` | ★★☆ | the loop-variable trap, shared state, and a run-once wrapper |
| 04 | `04-extension-methods.cs` | ★★☆ | `Truncate`, `IsBlank`, `WhereNotNull` — the three you will actually reuse |
| 05 | `05-composing-functions.cs` | ★★☆ | `Compose`, `Pipe`, `Tap` — pipelines out of small parts |
| 06 | `06-memoize.cs` | ★★☆ | a cache in a closure, and why the KEY is the hard part |
| 07 | `07-events-and-unsubscribe.cs` | ★★☆ | raising an event safely, counting handlers, detaching them |
| 08 | `08-currying-and-partial.cs` | ★★★ | one argument at a time — dependency injection without a container |

Do 01–07 in order. 08 is the stretch: it teaches nothing new about the
runtime but changes how you see constructor injection.

```
dotnet run exercises/03-closures-and-capture.cs
dotnet run solutions/03-closures-and-capture.cs
dotnet run ../verify.cs 02
dotnet run ../progress.cs 02
```

---

**Stuck?** `cheatsheets/csharp-language.md` (delegates, lambdas, events) · **Self-check:** `quizzes/02-methods-and-delegates.md` · **Next:** `bootcamp/03-collections-and-linq`
