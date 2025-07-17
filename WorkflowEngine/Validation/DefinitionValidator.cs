using WorkflowEngine.Domain;

namespace WorkflowEngine.Validation;

public static class DefinitionValidator
{
    public static void Validate(WorkflowDefinition def)
    {
        // exactly one initial state
        if (def.States.Count(s => s.IsInitial) != 1)
            throw new InvalidOperationException("Workflow must have exactly one initial state.");

        // duplicate state IDs
        var ids = def.States.Select(s => s.Id).ToList();
        if (ids.Count != ids.Distinct().Count())
            throw new InvalidOperationException("Duplicate state IDs found.");

        var idSet = ids.ToHashSet();

        foreach (var act in def.Actions)
        {
            if (!idSet.Contains(act.ToState))
                throw new InvalidOperationException($"Action '{act.Name}' points to unknown target state.");

            foreach (var fs in act.FromStates)
                if (!idSet.Contains(fs))
                    throw new InvalidOperationException($"Action '{act.Name}' references unknown source state.");
        }
    }
}
