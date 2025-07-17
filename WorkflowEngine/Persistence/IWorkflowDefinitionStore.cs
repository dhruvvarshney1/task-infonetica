using WorkflowEngine.Domain;

namespace WorkflowEngine.Persistence;

public interface IWorkflowDefinitionStore
{
    WorkflowDefinition? GetDefinition(Guid id);
    IEnumerable<WorkflowDefinition> AllDefinitions();
    void SaveDefinition(WorkflowDefinition definition);
}
