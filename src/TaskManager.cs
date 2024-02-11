namespace peer2peer; 

public class TaskManager : RangeUpdater, ISubscriber {
    private Dictionary<EncryptionTask, Thread> _runningTasks = new Dictionary<EncryptionTask, Thread>();
    private Problem _problem;
    
    public TaskManager(Problem problem) {
        _problem = problem;
        _problem.Subscribe(this);
        Source = UpdateSource.Local;
    }
    public void Poke() {
        
    }

    private void ScheduleTask() {
        Range pickedRange = _problem._iterator.
        EncryptionTask task = new EncryptionTask(this, new Range(0, 100), new Encrypter());
    }
    
    private void TaskFinished(EncryptionTask task, TaskResult result) {
        
    }
    
    private void TerminateTask(EncryptionTask task) {
        
    }
    
}
