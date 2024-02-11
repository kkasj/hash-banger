namespace peer2peer; 

public class TaskResult {
    public TaskStatus Status { get;}
    public string Result { get;}
    public Range Range { get; set; }
    
    public TaskResult(TaskStatus status, string result, Range range) {
        this.Status = status;
        this.Result = result;
        this.Range = range;
    }
}