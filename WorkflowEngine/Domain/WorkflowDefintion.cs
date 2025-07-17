namespace WorkflowEngine.Domain;

public sealed record WorkflowDefinition(
    Guid Id,
    string Name,
    IReadOnlyList<State> States,
    IReadOnlyList<ActionTransition> Actions);
