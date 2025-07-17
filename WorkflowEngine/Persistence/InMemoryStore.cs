using System.Collections.Concurrent;
using WorkflowEngine.Domain;

namespace WorkflowEngine.Persistence;

public sealed class InMemoryStore : IWorkflowDefinitionStore, IWorkflowInstanceStore
{
    private readonly ConcurrentDictionary<Guid, WorkflowDefinition> _defs = new();
    private readonly ConcurrentDictionary<Guid, WorkflowInstance>   _inst = new();

    // ---- Definitions -------------------------------------------------------
    public WorkflowDefinition? GetDefinition(Guid id) =>
        _defs.TryGetValue(id, out var d) ? d : null;

    public IEnumerable<WorkflowDefinition> AllDefinitions() => _defs.Values;

    public void SaveDefinition(WorkflowDefinition d) => _defs[d.Id] = d;

    // ---- Instances ---------------------------------------------------------
    public WorkflowInstance? GetInstance(Guid id) =>
        _inst.TryGetValue(id, out var i) ? i : null;

    public void SaveInstance(WorkflowInstance i) => _inst[i.Id] = i;
}
