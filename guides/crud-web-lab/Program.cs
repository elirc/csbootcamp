using System.Globalization;
using CrudWebLab;

var selfTest = args.Contains("--self-test");
var builder = WebApplication.CreateBuilder(args.Where(x => x != "--self-test").ToArray());
builder.Services.AddSingleton<TaskStore>();
if (selfTest) builder.Logging.ClearProviders();
var app = builder.Build();

app.MapGet("/tasks", (TaskStore store) => Results.Ok(store.List()));
app.MapGet("/tasks/{id:int}", (int id, TaskStore store, HttpResponse response) =>
{
    var item = store.Get(id);
    if (item is null) return Results.NotFound();
    response.Headers.ETag = Tag(item.Version);
    return Results.Ok(item);
});
app.MapPost("/tasks", (TaskInput input, TaskStore store, HttpResponse response) =>
{
    if (!Valid(input)) return Results.BadRequest(new { error = "Title must contain 1 to 120 characters after trimming." });
    var item = store.Create(input);
    response.Headers.ETag = Tag(item.Version);
    return Results.Created($"/tasks/{item.Id}", item);
});
app.MapPut("/tasks/{id:int}", (int id, TaskInput input, TaskStore store, HttpContext context) =>
{
    if (!Valid(input)) return Results.BadRequest();
    var headerError = ExpectedVersion(context.Request, out var expected);
    if (headerError is not null) return headerError;
    var (outcome, item) = store.Replace(id, expected, input);
    if (outcome == WriteOutcome.Missing) return Results.NotFound();
    if (outcome == WriteOutcome.Conflict) return Results.StatusCode(412);
    context.Response.Headers.ETag = Tag(item!.Version);
    return Results.Ok(item);
});
app.MapDelete("/tasks/{id:int}", (int id, TaskStore store, HttpRequest request) =>
{
    var headerError = ExpectedVersion(request, out var expected);
    if (headerError is not null) return headerError;
    return store.Delete(id, expected) switch
    {
        WriteOutcome.Missing => Results.NotFound(),
        WriteOutcome.Conflict => Results.StatusCode(412),
        _ => Results.NoContent()
    };
});

if (selfTest)
{
    app.Urls.Clear();
    app.Urls.Add("http://127.0.0.1:0");
    await app.StartAsync();
    try { return await HttpChecks.Run(app.Urls.Single()); }
    finally { await app.StopAsync(); await app.DisposeAsync(); }
}
await app.RunAsync("http://127.0.0.1:5088");
return 0;

static bool Valid(TaskInput input) => input.Title?.Trim().Length is >= 1 and <= 120;
static string Tag(long version) => $"\"{version.ToString(CultureInfo.InvariantCulture)}\"";

// This lab accepts one strong numeric tag, not the complete HTTP ETag grammar.
static IResult? ExpectedVersion(HttpRequest request, out long version)
{
    version = 0;
    if (!request.Headers.TryGetValue("If-Match", out var values)) return Results.StatusCode(428);
    var text = values.ToString();
    if (values.Count != 1 || text.Length < 3 || text[0] != '"' || text[^1] != '"' ||
        !long.TryParse(text.AsSpan(1, text.Length - 2), NumberStyles.None, CultureInfo.InvariantCulture, out version) || version < 1)
        return Results.BadRequest(new { error = "Use the single quoted numeric ETag returned by GET." });
    return null;
}
