using WorkflowEngine.Domain;

namespace WorkflowEngine.Validation;

public static class TransitionValidator
{
    public static void Validate(
        WorkflowDefinition def,
        WorkflowInstance  inst,
        ActionTransition  act)
    {
        if (!act.Enabled)
            throw new InvalidOperationException("Action is disabled.");

        if (!act.FromStates.Contains(inst.CurrentState))
            throw new InvalidOperationException("Action cannot be fired from the current state.");

        var currentState = def.States.Single(s => s.Id == inst.CurrentState);
        if (currentState.IsFinal)
            throw new InvalidOperationException("Instance is already in a final state.");

        var targetState = def.States.Single(s => s.Id == act.ToState);
        if (!targetState.Enabled)
            throw new InvalidOperationException("Target state is disabled.");
    }
}
