// ─────────────────────────────────────────────────────────────────────────
//  07 · raising an event without crashing — SOLUTION           ★★☆ core
//  concepts: events · multicast delegates · unsubscribing
//  run: dotnet run 07-events-and-unsubscribe.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  A delegate field holding no handlers is NULL, not an empty list, so
//  `Published(this, m)` throws a NullReferenceException the moment
//  nobody is listening — which in production is the first minute after
//  a subscriber is refactored away. `Published?.Invoke(this, m)` is the
//  idiom, and it also takes a snapshot of the delegate, which is why it
//  survives another thread unsubscribing mid-raise.
//  Handlers fire in subscription order and the SAME handler attached
//  twice fires twice: += appends, it does not deduplicate. That is the
//  double-fire bug — an event subscribed in a method that runs on every
//  navigation, never unsubscribed, firing twice, then three times.
//  -= removes ONE attachment, matching on delegate equality. Two lambdas
//  with identical bodies are different objects, so `x += (_, _) => f();`
//  followed by `x -= (_, _) => f();` removes nothing at all. Keep a
//  reference to the handler you attached — that is why the tests declare
//  `EventHandler<string> handler` as a variable.
//  Setting the event to null inside the class detaches everyone at once.
//  From OUTSIDE the class you cannot do that, cannot invoke it, and
//  cannot even read it — that is the entire difference between an event
//  and a public delegate field, and it is why events are safe to expose.
//  The leak worth remembering: the publisher holds a reference to every
//  subscriber. A long-lived publisher keeps short-lived subscribers
//  alive forever. Unsubscribe in Dispose, always.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

// ─── tests ───────────────────────────────────────────────────────────────

Test("publishing with no subscribers does nothing", () =>
{
    var publisher = new Publisher();
    publisher.Publish("hello");
    Eq(publisher.SubscriberCount, 0);
});

Test("a subscriber receives the message", () =>
{
    var seen = new List<string>();
    var publisher = new Publisher();
    publisher.Published += (_, message) => seen.Add(message);
    publisher.Publish("hello");
    Eq(seen, new List<string> { "hello" });
});

Test("every subscriber receives it, in order", () =>
{
    var seen = new List<string>();
    var publisher = new Publisher();
    publisher.Published += (_, m) => seen.Add("a:" + m);
    publisher.Published += (_, m) => seen.Add("b:" + m);
    publisher.Publish("x");
    Eq(seen, new List<string> { "a:x", "b:x" });
});

Test("SubscriberCount tracks attachments", () =>
{
    var publisher = new Publisher();
    EventHandler<string> handler = (_, _) => { };
    Eq(publisher.SubscriberCount, 0);
    publisher.Published += handler;
    Eq(publisher.SubscriberCount, 1);
    publisher.Published += handler;
    Eq(publisher.SubscriberCount, 2, "the same handler attached twice fires twice");
});

Test("detaching removes one attachment", () =>
{
    var calls = 0;
    var publisher = new Publisher();
    EventHandler<string> handler = (_, _) => calls++;
    publisher.Published += handler;
    publisher.Published -= handler;
    publisher.Publish("x");
    Eq((publisher.SubscriberCount, calls), (0, 0));
});

Test("Reset detaches everyone", () =>
{
    var calls = 0;
    var publisher = new Publisher();
    publisher.Published += (_, _) => calls++;
    publisher.Published += (_, _) => calls++;
    publisher.Reset();
    publisher.Publish("x");
    Eq((publisher.SubscriberCount, calls), (0, 0));
});

Test("the sender is the publisher", () =>
{
    object? sender = null;
    var publisher = new Publisher();
    publisher.Published += (s, _) => sender = s;
    publisher.Publish("x");
    Ok(ReferenceEquals(sender, publisher));
});

return Report();

// ─── the code you edit ───────────────────────────────────────────────────

class Publisher
{
    public event EventHandler<string>? Published;

    public void Publish(string message) => Published?.Invoke(this, message);

    public int SubscriberCount => Published?.GetInvocationList().Length ?? 0;

    public void Reset() => Published = null;
}
