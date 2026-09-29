// ─────────────────────────────────────────────────────────────────────────
//  05 · joining two sequences                                   ★★☆ core
//  concepts: Join · GroupJoin · left join · DefaultIfEmpty
//  run: dotnet run 05-joins.cs
// ─────────────────────────────────────────────────────────────────────────
//
//  Customers and Orders (records at the BOTTOM of this file). Three
//  questions, and the difference between them is the whole lesson:
//
//      NamesWithOrders(c, o)      → names that have at least one order
//      TotalPerCustomer(c, o)     → EVERY customer, with 0 for no orders
//      CustomersWithoutOrders(c,o)→ the names with nothing
//
//  TotalPerCustomer is a LEFT join: a customer who never bought
//  anything still appears, with a total of zero. Getting that wrong —
//  silently dropping the rows with nothing to show — is one of the most
//  common reporting bugs there is, because the report still looks
//  plausible.
//
//  hint: Join gives you inner-join pairs. GroupJoin gives you each left
//  item together with its (possibly empty) group of matches, which is
//  exactly what a left join is. Sort the outputs A-Z.

#:project ../../_lib/Check/Check.csproj

using static Bootcamp.Check;

static List<string> NamesWithOrders(IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
    throw new NotImplementedException("TODO");

static Dictionary<string, decimal> TotalPerCustomer(
    IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
    throw new NotImplementedException("TODO");

static List<string> CustomersWithoutOrders(
    IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
    throw new NotImplementedException("TODO");

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
