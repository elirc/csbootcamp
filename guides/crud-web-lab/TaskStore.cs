namespace CrudWebLab;

public sealed record TaskInput(string? Title, bool IsDone);
public sealed record TaskItem(int Id, string Title, bool IsDone, long Version);
public enum WriteOutcome { Saved, Missing, Conflict }

// One-process teaching store. The lock covers BOTH comparison and mutation.
public sealed class TaskStore
{
    private readonly object gate = new();
    private readonly Dictionary<int, TaskItem> items = [];
    private int nextId;

    public TaskItem Create(TaskInput input)
    {
        lock (gate)
        {
            var item = new TaskItem(++nextId, input.Title!.Trim(), input.IsDone, 1);
            items.Add(item.Id, item);
            return item;
        }
    }

    public TaskItem? Get(int id) { lock (gate) return items.GetValueOrDefault(id); }
    public TaskItem[] List() { lock (gate) return items.Values.OrderBy(x => x.Id).ToArray(); }

    public (WriteOutcome Outcome, TaskItem? Item) Replace(int id, long expected, TaskInput input)
    {
        lock (gate)
        {
            if (!items.TryGetValue(id, out var current)) return (WriteOutcome.Missing, null);
            if (current.Version != expected) return (WriteOutcome.Conflict, null);
            var changed = current with { Title = input.Title!.Trim(), IsDone = input.IsDone, Version = checked(current.Version + 1) };
            items[id] = changed;
            return (WriteOutcome.Saved, changed);
        }
    }

    public WriteOutcome Delete(int id, long expected)
    {
        lock (gate)
        {
            if (!items.TryGetValue(id, out var current)) return WriteOutcome.Missing;
            if (current.Version != expected) return WriteOutcome.Conflict;
            items.Remove(id);
            return WriteOutcome.Saved;
        }
    }
}
