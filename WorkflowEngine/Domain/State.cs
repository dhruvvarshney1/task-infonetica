namespace WorkflowEngine.Domain;

public sealed record State(
    Guid Id,
    string Name,
    bool IsInitial = false,
    bool IsFinal   = false,
    bool Enabled   = true);
