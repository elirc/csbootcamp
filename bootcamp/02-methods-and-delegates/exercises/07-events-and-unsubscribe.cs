// ─────────────────────────────────────────────────────────────────────────
//  07 · raising an event without crashing                      ★★☆ core
//  concepts: events · multicast delegates · unsubscribing
//  run: dotnet run 07-events-and-unsubscribe.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  An event is a multicast delegate with the += and -= doors left open
//  and everything else locked: only the declaring class may raise it.
//
//  Finish the Publisher class at the BOTTOM of this file:
//
//      Publish(message)    calls every subscriber, in order; does
//                          nothing (and does NOT throw) with none
//      SubscriberCount     how many handlers are attached right now
//      Reset()             detach every handler
//
//  Publishing to nobody is the normal state of an event, not an error.
//
//  hint: an event with no subscribers is null. `Published?.Invoke(...)`
//  is the safe raise. `Delegate.GetInvocationList()` gives you the
//  attached handlers as an array — on a null delegate you cannot call
//  it at all, which is a case worth handling.

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

    public void Publish(string message) => throw new NotImplementedException("TODO");

    public int SubscriberCount => throw new NotImplementedException("TODO");

    public void Reset() => throw new NotImplementedException("TODO");
}
