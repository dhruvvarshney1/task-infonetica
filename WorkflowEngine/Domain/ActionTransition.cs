namespace WorkflowEngine.Domain;

public sealed record ActionTransition(
    Guid Id,
    string Name,
    IEnumerable<Guid> FromStates,
    Guid ToState,
    bool Enabled = true);
