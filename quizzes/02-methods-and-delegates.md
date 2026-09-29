# Quiz 02 · Methods and Delegates

Say the answer out loud before you expand it.

Pairs with
[`bootcamp/02-methods-and-delegates`](../bootcamp/02-methods-and-delegates/).

---

**1.** In `Func<string, int, bool>`, which type is the return type?

<details><summary>answer</summary>

`bool` — the **last** type argument is always the return type. It takes a
`string` and an `int`. `Action` has no return type argument at all.
</details>

**2.** A lambda captures a variable. Does it capture the value or the
variable?

<details><summary>answer</summary>

The variable. The compiler moves it into a hidden object that the lambda and
the enclosing code share, so a later assignment is visible inside the
lambda. Every closure behaviour — useful and buggy — follows from that.
</details>

**3.** Three lambdas built in a `for` loop all return `3`. Why, and what is
the fix?

<details><summary>answer</summary>

A `for` loop declares **one** loop variable for the whole loop, so all three
captured the same variable and see its final value. Copy it into a local
inside the loop body (`var j = i;`) and capture that. `foreach` was changed
in C# 5 to give a fresh variable per iteration, so it does not have this
problem.
</details>

**4.** You wrote `x += (s, e) => Handle();` and later `x -= (s, e) =>
Handle();`. What was removed?

<details><summary>answer</summary>

Nothing. `-=` matches on delegate identity, and two lambdas with identical
bodies are different objects. Keep the handler in a variable and unsubscribe
that.
</details>

**5.** Your event handler runs three times per click. What happened?

<details><summary>answer</summary>

`+=` was called three times and never `-=`. Subscribing does not
deduplicate, and a handler attached in a method that runs on every
navigation accumulates.
</details>

**6.** An event with no subscribers — what is its value, and what does that
mean for raising it?

<details><summary>answer</summary>

`null`, not an empty list. `Published(this, x)` throws a
`NullReferenceException`; `Published?.Invoke(this, x)` is the idiom. The
null-conditional form also snapshots the delegate, so it survives another
thread unsubscribing mid-raise.
</details>

**7.** Why is a `public delegate` field not the same as an `event`?

<details><summary>answer</summary>

Outside the declaring class an `event` allows only `+=` and `-=`. A public
delegate field lets any caller overwrite the whole invocation list, read it,
or raise it. The event keyword is the encapsulation.
</details>

**8.** Two `Func<int,int>` delegates combined with `+`. What does calling the
result return?

<details><summary>answer</summary>

Only the **last** one's result; the others are computed and discarded.
Multicast is designed for events (void), not for computing values.
</details>

**9.** What does an extension method have access to, and what does it lose
to?

<details><summary>answer</summary>

Only the public surface of the type — it is a static method, so no private
state, no overriding. It loses to a real instance method of the same name
every time, so adding one to the type later silently changes behaviour.
</details>

**10.** Why does calling an extension method on a `null` reference not throw
at the call site?

<details><summary>answer</summary>

Because it is a static call: the null is just an argument. That is why
guard-style helpers like `value.IsBlank()` can be declared on `string?`.
</details>

**11.** In `Memoize`, why must the cache be a local captured by the returned
lambda rather than a static field?

<details><summary>answer</summary>

So each wrapper gets its own cache. A shared static cache is the cache-key
collision bug: two callers, one bucket, and answers handed to the wrong one.
</details>

**12.** Why `TryGetValue` instead of `ContainsKey` followed by the indexer?

<details><summary>answer</summary>

One hash probe instead of two, no race between the check and the read, and
it distinguishes a stored `null` from a missing entry — which is what makes
a memoized `null` result stay cached instead of being recomputed forever.
</details>

**13.** What is wrong with an unbounded `Memoize` keyed on user input?

<details><summary>answer</summary>

It is a memory leak. Every distinct input is retained forever, along with
everything the cached value references. Real caches need a size bound and an
eviction policy.
</details>

**14.** `Compose(f, g)` and `Pipe(f, g)` — which runs `f` first?

<details><summary>answer</summary>

`Pipe`. `Compose` is the mathematical order, so `Compose(f, g)(x)` is
`f(g(x))` — `g` first. Pick one convention per codebase; mixing them is how
a transform gets applied backwards.
</details>

**15.** Why does `Pipe()` with no steps return the input unchanged, without a
special case?

<details><summary>answer</summary>

Because the fold is seeded with the identity function `x => x`, which is the
identity element of composition — the same reason a sum seeded with 0 gets
the empty case right for free.
</details>

**16.** What is partial application, in terms of something you already do
every day?

<details><summary>answer</summary>

Constructor injection. `Configure("api")` returns a function with its prefix
baked in; a constructor that stores a dependency in a field is the same
thing with more ceremony.
</details>
