namespace peer2peer; 

public class TaskManager : ISubscriber {
    private Dictionary<EncryptionTask, Thread> _runningTasks = new Dictionary<EncryptionTask, Thread>();
    private Observable _problem;
    
    public TaskManager(Observable problem) {
        _problem = problem;
        _problem.Subscribe(this);
    }
    public void Poke() {
        
    }

    private void ScheduleTask() {
        
    }
    
    private void TaskFinished(EncryptionTask task, TaskResult result) {
        
    }
    
    private void TerminateTask(EncryptionTask task) {
        
    }
    
}
