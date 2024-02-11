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
        Range pickedRange = _problem.Iterator.GetNext();
        EncryptionTask task = new EncryptionTask(this, pickedRange, EncrypterFactory.CreateEncrypter(_problem.Args.EncryptionType), _problem.Args.ProblemHash);
        Thread InstanceCaller = new Thread(
            new ThreadStart(task.Start));
        _runningTasks.Add(task, InstanceCaller);
        _runningTasks[task].Start();
    }
    
    public void TaskFinished(EncryptionTask task, TaskResult result) {
        _problem.TaskDone(this, result.Range);
    }
    
    private void TerminateTask(EncryptionTask task) {
        _runningTasks[task].Interrupt(); // TODO: is it works?
    }
    
}
