// ─────────────────────────────────────────────────────────────────────────
//  05 · joining two sequences — SOLUTION                        ★★☆ core
//  concepts: Join · GroupJoin · left join · DefaultIfEmpty
//  run: dotnet run 05-joins.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Walkthrough:
//  Join is an INNER join: it emits one result per matching PAIR, so a
//  customer with two orders appears twice. That is why NamesWithOrders
//  needs Distinct — and why a naive Join used for a total double-counts.
//  The row multiplication is called fan-out, and it is the reason a
//  report can be exactly right on customers with one order and wrong on
//  everybody else.
//  GroupJoin is the left join: for each left item you get the item plus
//  a group of its matches, and the group is EMPTY rather than absent
//  when nothing matched. Sum over an empty sequence is 0, so the "no
//  orders" case needs no special handling at all — the shape of the
//  operator gives you the right answer.
//  That is the general lesson: pick the operator whose SHAPE matches the
//  question. If the answer must have one row per left item, you want
//  GroupJoin. If it must have one row per pair, you want Join. Deciding
//  that first prevents both the dropped-rows bug and the double-counting
//  bug, and it maps one-to-one onto SQL's INNER and LEFT JOIN.
//  DefaultIfEmpty is the other spelling: GroupJoin followed by
//  SelectMany with DefaultIfEmpty flattens back to pairs with nulls,
//  which is exactly what SQL's LEFT JOIN emits.
//  Performance: Join and GroupJoin build a hash lookup over the inner
//  sequence, so they are O(n + m), not the O(n * m) you get from a
//  nested Where. That is a real reason to use them over a loop.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> NamesWithOrders(IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
    customers.Join(orders, c => c.Id, o => o.CustomerId, (c, _) => c.Name)
             .Distinct()
             .Order()
             .ToList();

static Dictionary<string, decimal> TotalPerCustomer(
    IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
    customers.GroupJoin(
                 orders,
                 c => c.Id,
                 o => o.CustomerId,
                 (customer, theirs) => (customer.Name, Total: theirs.Sum(o => o.Total)))
             .ToDictionary(row => row.Name, row => row.Total);

static List<string> CustomersWithoutOrders(
    IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
    customers.GroupJoin(orders, c => c.Id, o => o.CustomerId, (customer, theirs) => (customer, theirs))
             .Where(row => !row.theirs.Any())
             .Select(row => row.customer.Name)
             .Order()
             .ToList();

// ─── tests ───────────────────────────────────────────────────────────────

var customers = new[]
{
    new Customer(1, "ada"),
    new Customer(2, "grace"),
    new Customer(3, "linus"),
};

var orders = new[]
{
    new Order(1, 100m),
    new Order(1, 50m),
    new Order(2, 25m),
};

Test("NamesWithOrders lists the buyers", () =>
    Eq(NamesWithOrders(customers, orders), new List<string> { "ada", "grace" }));

Test("NamesWithOrders names a repeat buyer once", () =>
    Eq(NamesWithOrders(customers, orders).Count(n => n == "ada"), 1));

Test("NamesWithOrders with no orders is empty", () =>
    Eq(NamesWithOrders(customers, Array.Empty<Order>()).Count, 0));

Test("TotalPerCustomer sums each customer's orders", () =>
{
    var totals = TotalPerCustomer(customers, orders);
    Eq(totals["ada"], 150m);
    Eq(totals["grace"], 25m);
});

Test("TotalPerCustomer keeps the customer who bought nothing", () =>
{
    var totals = TotalPerCustomer(customers, orders);
    Eq(totals.Count, 3, "a left join keeps every row on the left");
    Eq(totals["linus"], 0m);
});

Test("TotalPerCustomer with no orders still lists everyone", () =>
    Eq(TotalPerCustomer(customers, Array.Empty<Order>()).Count, 3));

Test("CustomersWithoutOrders finds the quiet ones", () =>
    Eq(CustomersWithoutOrders(customers, orders), new List<string> { "linus" }));

Test("CustomersWithoutOrders is empty when everyone bought something", () =>
    Eq(CustomersWithoutOrders(customers.Take(2), orders).Count, 0));

return Report();

// ─── the types ───────────────────────────────────────────────────────────

record Customer(int Id, string Name);

record Order(int CustomerId, decimal Total);
