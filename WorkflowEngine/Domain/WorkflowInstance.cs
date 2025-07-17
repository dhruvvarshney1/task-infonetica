namespace WorkflowEngine.Domain;

public sealed record HistoryEntry(Guid ActionId, DateTimeOffset AtUtc);

public sealed class WorkflowInstance
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid DefinitionId { get; init; }
    public Guid CurrentState { get; private set; }
    public List<HistoryEntry> History { get; } = new();

    public WorkflowInstance(WorkflowDefinition def)
    {
        DefinitionId = def.Id;
        CurrentState = def.States.Single(s => s.IsInitial).Id;
    }

    internal void Apply(Guid actionId, Guid newStateId)
    {
        CurrentState = newStateId;
        History.Add(new HistoryEntry(actionId, DateTimeOffset.UtcNow));
    }
}
