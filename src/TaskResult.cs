namespace peer2peer; 

public class TaskResult {
    public TaskStatus Status { get;}
    public string Result { get;}
    
    public TaskResult(TaskStatus status, string result) {
        this.Status = status;
        this.Result = result;
    }
}