using System.Net;
using System.Net.Http.Json;

namespace CrudWebLab;

public static class HttpChecks
{
    public static async Task<int> Run(string address)
    {
        using var client = new HttpClient { BaseAddress = new Uri(address), Timeout = TimeSpan.FromSeconds(10) };
        var passed = 0;
        var failed = 0;
        void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
        async Task Test(string name, Func<Task> action)
        {
            try { await action(); passed++; Console.WriteLine($"ok - {name}"); }
            catch (Exception error) { failed++; Console.WriteLine($"not ok - {name}: {error.Message}"); }
        }
        async Task<(TaskItem Item, string Tag)> Create(string title = "Learn CRUD")
        {
            using var response = await client.PostAsJsonAsync("/tasks", new TaskInput(title, false));
            Check(response.StatusCode == HttpStatusCode.Created, "create status");
            var item = (await response.Content.ReadFromJsonAsync<TaskItem>())!;
            Check(response.Headers.Location?.OriginalString == $"/tasks/{item.Id}", "location header");
            return (item, response.Headers.ETag!.ToString());
        }
        async Task<HttpResponseMessage> Write(HttpMethod method, int id, string? tag, TaskInput? input = null)
        {
            using var request = new HttpRequestMessage(method, $"/tasks/{id}");
            if (tag is not null) request.Headers.TryAddWithoutValidation("If-Match", tag);
            if (input is not null) request.Content = JsonContent.Create(input);
            return await client.SendAsync(request);
        }

        await Test("create trims title and exposes Location plus initial ETag", async () =>
        {
            var (item, tag) = await Create("  First  ");
            Check(item.Title == "First" && item.Version == 1 && tag == "\"1\"", "create representation");
        });
        await Test("blank title is rejected without adding a task", async () =>
        {
            var before = (await client.GetFromJsonAsync<TaskItem[]>("/tasks"))!.Length;
            using var response = await client.PostAsJsonAsync("/tasks", new TaskInput("   ", false));
            Check(response.StatusCode == HttpStatusCode.BadRequest, "blank status");
            Check((await client.GetFromJsonAsync<TaskItem[]>("/tasks"))!.Length == before, "invalid create persisted");
        });
        await Test("title length boundary accepts 120 and rejects 121", async () =>
        {
            await Create(new string('x', 120));
            using var response = await client.PostAsJsonAsync("/tasks", new TaskInput(new string('x', 121), false));
            Check(response.StatusCode == HttpStatusCode.BadRequest, "oversized title");
        });
        await Test("read returns representation and ETag", async () =>
        {
            var (item, tag) = await Create();
            using var response = await client.GetAsync($"/tasks/{item.Id}");
            Check(response.StatusCode == HttpStatusCode.OK && response.Headers.ETag!.ToString() == tag, "read tag");
            Check(await response.Content.ReadFromJsonAsync<TaskItem>() == item, "read body");
        });
        await Test("unknown identifier returns not found", async () =>
        {
            using var response = await client.GetAsync("/tasks/2147483647");
            Check(response.StatusCode == HttpStatusCode.NotFound, "missing status");
        });
        await Test("replace requires If-Match and leaves state unchanged", async () =>
        {
            var (item, _) = await Create();
            using var response = await Write(HttpMethod.Put, item.Id, null, new("Overwrite", true));
            Check((int)response.StatusCode == 428, "missing precondition");
            Check(await client.GetFromJsonAsync<TaskItem>($"/tasks/{item.Id}") == item, "missing tag mutated");
        });
        await Test("malformed If-Match is rejected without changing state", async () =>
        {
            var (item, _) = await Create();
            foreach (var bad in new[] { "1", "\"0\"", "W/\"1\"", "*", "\"1\", \"2\"" })
            {
                using var response = await Write(HttpMethod.Put, item.Id, bad, new("Bad", true));
                Check(response.StatusCode == HttpStatusCode.BadRequest, $"accepted {bad}");
            }
            Check(await client.GetFromJsonAsync<TaskItem>($"/tasks/{item.Id}") == item, "bad tag mutated");
        });
        await Test("valid replace increments version and preserves identifier", async () =>
        {
            var (item, tag) = await Create();
            using var response = await Write(HttpMethod.Put, item.Id, tag, new("  Revised  ", true));
            var changed = await response.Content.ReadFromJsonAsync<TaskItem>();
            Check(response.StatusCode == HttpStatusCode.OK && changed == item with { Title = "Revised", IsDone = true, Version = 2 }, "replacement body");
            Check(response.Headers.ETag!.ToString() == "\"2\"", "replacement tag");
        });
        await Test("stale replace returns 412 and preserves winner", async () =>
        {
            var (item, tag) = await Create();
            using var first = await Write(HttpMethod.Put, item.Id, tag, new("Winner", true));
            Check(first.StatusCode == HttpStatusCode.OK, "first save");
            using var stale = await Write(HttpMethod.Put, item.Id, tag, new("Loser", false));
            Check((int)stale.StatusCode == 412, "stale status");
            Check((await client.GetFromJsonAsync<TaskItem>($"/tasks/{item.Id}"))!.Title == "Winner", "lost update");
        });
        await Test("invalid replacement does not consume version", async () =>
        {
            var (item, tag) = await Create();
            using var bad = await Write(HttpMethod.Put, item.Id, tag, new(" ", true));
            Check(bad.StatusCode == HttpStatusCode.BadRequest, "invalid replacement");
            Check(await client.GetFromJsonAsync<TaskItem>($"/tasks/{item.Id}") == item, "invalid mutation");
        });
        await Test("concurrent editors with same tag have one winner", async () =>
        {
            var (item, tag) = await Create();
            var responses = await Task.WhenAll(Write(HttpMethod.Put, item.Id, tag, new("A", false)), Write(HttpMethod.Put, item.Id, tag, new("B", true)));
            try
            {
                Check(responses.Count(x => x.StatusCode == HttpStatusCode.OK) == 1 && responses.Count(x => (int)x.StatusCode == 412) == 1, "expected one winner and one conflict");
                var winner = (await client.GetFromJsonAsync<TaskItem>($"/tasks/{item.Id}"))!;
                Check(winner.Version == 2 && (winner.Title == "A" || winner.Title == "B"), "winner state");
            }
            finally { foreach (var response in responses) response.Dispose(); }
        });
        await Test("delete requires current version and removes only that task", async () =>
        {
            var (item, tag) = await Create();
            using var missing = await Write(HttpMethod.Delete, item.Id, null);
            using var stale = await Write(HttpMethod.Delete, item.Id, "\"99\"");
            Check((int)missing.StatusCode == 428 && (int)stale.StatusCode == 412, "delete guards");
            Check(await client.GetFromJsonAsync<TaskItem>($"/tasks/{item.Id}") == item, "rejected delete mutated");
            using var deleted = await Write(HttpMethod.Delete, item.Id, tag);
            Check(deleted.StatusCode == HttpStatusCode.NoContent, "delete success");
            using var gone = await client.GetAsync($"/tasks/{item.Id}");
            using var repeated = await Write(HttpMethod.Delete, item.Id, tag);
            Check(gone.StatusCode == HttpStatusCode.NotFound && repeated.StatusCode == HttpStatusCode.NotFound, "delete persisted");
        });
        await Test("replace missing task returns 404", async () =>
        {
            using var response = await Write(HttpMethod.Put, int.MaxValue, "\"1\"", new("Missing", false));
            Check(response.StatusCode == HttpStatusCode.NotFound, "missing replacement");
        });
        Console.WriteLine($"# tests {passed + failed}\n# pass {passed}\n# fail {failed}\n# skipped 0");
        return failed == 0 ? 0 : 1;
    }
}
