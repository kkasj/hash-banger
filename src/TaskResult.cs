/// <summary>
/// Represents the task result.
/// </summary>
public class TaskResult {
    public TaskStatus Status { get;}
    public string? Result { get;}
    public Range Range { get; set; }
    
    public TaskResult(TaskStatus status, string? result, Range range) {
        Status = status;
        Result = result;
        Range = range;
    }
}