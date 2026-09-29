# Quiz 01 · Language Core

Say the answer **out loud** before you expand it. Speaking it is the test;
recognising it is not.

Pairs with [`bootcamp/01-language-core`](../bootcamp/01-language-core/).

---

**1.** `double avg = 7 / 2;` — what is `avg`, and why?

<details><summary>answer</summary>

`3.0`. Both operands are `int`, so the division is integer division and the
fraction is gone before the `double` ever sees it. The assignment target
does not change how the division is performed. Fix: `(double)7 / 2`.
</details>

**2.** Why is `decimal` right for money and `double` wrong?

<details><summary>answer</summary>

`double` is base 2, so a base-10 fraction like `0.1` has no exact
representation and errors accumulate: `0.1 + 0.2 != 0.3`. `decimal` is
base 10 with 28–29 significant digits, so it represents what an invoice
represents exactly. `decimal` also remembers its scale — `0.10m` formats as
`"0.10"`.
</details>

**3.** What does `Math.Round(0.5)` return, and what does that tell you?

<details><summary>answer</summary>

`0`. .NET defaults to banker's rounding (`MidpointRounding.ToEven`), which
is correct for statistics and wrong for an invoice. Say
`MidpointRounding.AwayFromZero` when you mean money.
</details>

**4.** `object a = 1; object b = 1;` — is `a == b` true?

<details><summary>answer</summary>

No. Both are boxed: each is a separate object on the heap, and `==` on two
`object` variables is reference comparison. Use `Equals(a, b)` — the static
two-argument form, which is null-safe on both sides and dispatches to
`Int32.Equals`.
</details>

**5.** Is `Equals(1, 1L)` true?

<details><summary>answer</summary>

No. A boxed value keeps its exact type, and no numeric conversion happens
inside a box. This is why config bags that "worked in the test" break on a
value that arrived from JSON as a `long`.
</details>

**6.** What is the difference between `string` and `string?` at runtime?

<details><summary>answer</summary>

Nothing. Nullable reference types are compile-time analysis only — the `?`
is a promise the compiler checks and that a caller from an older or
non-annotated project can break. Still guard at boundaries.
</details>

**7.** `name != null` passed, and the UI shows a blank. What did you forget?

<details><summary>answer</summary>

There are three states, not two: null, blank, and present. `"   "` is not
null. `string.IsNullOrWhiteSpace` is the check you meant.
</details>

**8.** Why is `a.ToLower() == b.ToLower()` a bug?

<details><summary>answer</summary>

`ToLower()` uses the current culture. In Turkish, lowercasing `I` does not
produce `i`, so the two strings differ on a Turkish machine and match on
yours. For keys and ids use
`string.Equals(a, b, StringComparison.OrdinalIgnoreCase)`.
</details>

**9.** In a `switch` expression, why does the order of the arms matter?

<details><summary>answer</summary>

Arms are tried top to bottom and the first match wins. That is what lets
each arm assume the earlier ones failed — `< 1000` after `< 0` and `0` needs
no lower bound. It is also why a `_` arm placed early swallows everything
after it.
</details>

**10.** Name one thing a `switch` expression gives you that an if/else chain
does not.

<details><summary>answer</summary>

It is an *expression*: every path must produce a value, so you cannot forget
to return or fall out of the bottom with nothing assigned. The compiler also
warns when the arms are not exhaustive.
</details>

**11.** You pass a `struct` to a method and the method changes a field. What
does the caller see?

<details><summary>answer</summary>

Nothing changed. The method got a copy. That is also why mutable structs are
a trap: you mutate a copy and the change disappears. Make them `readonly`
and return modified copies with `with`.
</details>

**12.** A method returns `box` from a method called `CopyOf`. What is the bug
class, and what is the symptom?

<details><summary>answer</summary>

Shared reference. Both names point at one object, so editing the "copy"
changes the original. It passes any test that only checks the copy's
contents and fails the first test that mutates it.
</details>

**13.** What must a method with an `out` parameter do before every `return`,
and why does it matter for a `Try` method?

<details><summary>answer</summary>

Assign it — the compiler enforces it on every path, including the failure
path. It matters because a `Try` method that fails and leaves a stale value
behind hands it to callers who ignored the `bool`.
</details>

**14.** `out`, `ref`, `in` — one sentence each.

<details><summary>answer</summary>

`out`: I will give this to you (caller need not initialise; method must
assign). `ref`: we share this variable (assignments inside are visible
outside). `in`: pass by reference but read-only — for large structs you do
not want copied.
</details>

**15.** Why does `object o = 42;` matter in a hot loop?

<details><summary>answer</summary>

It boxes: a heap allocation per value, and work for the garbage collector.
It is why the BCL has generic overloads of nearly everything, and why
`object` should be a last resort rather than a convenience.
</details>

**16.** What can a `Span<T>` not do, and why?

<details><summary>answer</summary>

It cannot be a field of a class, be captured by a lambda, or live across an
`await`. It is a `ref struct` — a pointer and a length into memory owned by
someone else — and all three would let it outlive what it points at. The
compiler refuses rather than letting you dangle.
</details>
