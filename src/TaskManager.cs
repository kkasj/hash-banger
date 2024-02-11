using System;
using System.Threading;

public class TaskManager : RangeUpdater, ISubscriber {
    private Dictionary<EncryptionTask, Thread> _runningTasks = new Dictionary<EncryptionTask, Thread>();
    private Dictionary<EncryptionTask, CancellationTokenSource> _cancellationTokens = new Dictionary<EncryptionTask, CancellationTokenSource>();
    private Problem _problem;
    const int MAX_TASKS = 10;
    
    public TaskManager(Problem problem) {
        _problem = problem;
        _problem.Subscribe(this);
        Source = UpdateSource.Local;
    }
    public void Poke() {
        var updates = _problem.IteratorProxy.Updates;
        // take updates whose source is Sync
        var syncUpdates = updates.Where(update => update.UpdateSource == UpdateSource.Sync);
        // delete them from the original list
        if (syncUpdates.Count() > 0)
            _problem.IteratorProxy.Updates = updates.Except(syncUpdates).ToList();

        // search for conflicts with running tasks 
        // and resolve them by terminating the task
        foreach (var update in syncUpdates) {
            foreach (var task in _runningTasks.Keys) {
                if (task.Range == update.Range) {
                    TerminateTask(task);
                }
            }
        }
    }

    private void ScheduleTask() {
        Range pickedRange = _problem.IteratorProxy.GetNext();
        EncryptionTask task = new EncryptionTask(this, pickedRange, EncrypterFactory.CreateEncrypter(_problem.Args.EncryptionType), _problem.Args);
        Thread InstanceCaller = new Thread(
            new ThreadStart(task.Start));
        _runningTasks.Add(task, InstanceCaller);

        CancellationTokenSource cts = new CancellationTokenSource();
        _cancellationTokens.Add(task, cts);

        _runningTasks[task].Start(cts.Token);
    }
    
    public void TaskFinished(EncryptionTask task, TaskResult result) {
        // make sure the problem hasn't changed since the task was started 
        if (task.ProblemArgs != _problem.Args) {
            return;
        }
        _problem.TaskDone(this, result.Range);
    }
    
    private void TerminateTask(EncryptionTask task) {
        _cancellationTokens[task].Cancel();

        // wait for the task to finish
        _runningTasks[task].Join();

        _runningTasks.Remove(task);
        _cancellationTokens.Remove(task);
    }
    
}
