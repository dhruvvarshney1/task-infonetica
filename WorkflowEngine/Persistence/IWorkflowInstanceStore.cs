using WorkflowEngine.Domain;

namespace WorkflowEngine.Persistence;

public interface IWorkflowInstanceStore
{
    WorkflowInstance? GetInstance(Guid id);
    void SaveInstance(WorkflowInstance instance);
}
