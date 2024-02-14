using design_patterns.TaskRanges;

namespace design_patterns.Tasking;

/// <summary>
/// Represents the task result.
/// </summary>
public class TaskResult {
    public TaskStatus Status { get;}
    public string? Result { get;}
    public TaskRange Range { get; set; }
    
    public TaskResult(TaskStatus status, string? result, TaskRange range) {
        Status = status;
        Result = result;
        Range = range;
    }
}