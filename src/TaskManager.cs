using System;
using System.Threading;
using design_patterns.Utils;

/// <summary>
/// TaskManager is responsible for scheduling and managing tasks.
/// </summary>
public class TaskManager : RangeUpdater, ISubscriber {
    private readonly object _lock = new object();
    private Dictionary<EncryptionTask, Thread> _runningTasks = new Dictionary<EncryptionTask, Thread>();
    private Dictionary<EncryptionTask, CancellationTokenSource> _cancellationTokens = new Dictionary<EncryptionTask, CancellationTokenSource>();
    private Problem _problem;
    
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
                    ReplaceTask(task);
                }
            }
        }

        // schedule new tasks
        while (_runningTasks.Count < ProblemParameters.MAX_TASKS) {
            ScheduleTask();
        }
    }

    /// <summary>
    /// Schedules a new task to be executed.
    /// </summary>
    private void ScheduleTask() {
        Range pickedRange = _problem.IteratorProxy.GetNext();
        CancellationTokenSource cts = new CancellationTokenSource();
        EncryptionTask task = new EncryptionTask(this, pickedRange, EncrypterFactory.CreateEncrypter(_problem.Args.EncryptionType), _problem.Args, cts.Token);
        Thread InstanceCaller = new Thread(
            new ThreadStart(task.Start));
        _runningTasks.Add(task, InstanceCaller);

        _cancellationTokens.Add(task, cts);

        _runningTasks[task].Start();
        // Console.WriteLine("Task started: " + pickedRange.Start + " - " + pickedRange.End);
    }
    
    /// <summary>
    /// Called when a task has finished.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="result"></param>
    public void TaskFinished(EncryptionTask task, TaskResult result) {
        lock(_lock){
            // make sure the problem hasn't changed since the task was started 
            if (task.ProblemArgs == _problem.Args) {
                _problem.TaskDone(this, result.Range);
                if (result.Status == TaskStatus.Found) {
                    Console.WriteLine("Found: " + result.Result);
                    CancelAllTasks();
                    // quit the program
                    Environment.Exit(0);
                }
            }

            // replace the task with a new one
            // ReplaceTask(task);
            _runningTasks.Remove(task);
            _cancellationTokens.Remove(task);
            ScheduleTask();


            // Console.WriteLine("Task finished: " + result.Range.Start + " - " + result.Range.End);
        }
    }
    
    /// <summary>
    /// Replaces a task with a new one.
    /// </summary>
    /// <param name="task"></param>
    private void ReplaceTask(EncryptionTask task) {
        // Console.WriteLine("Task replaced: " + task.Range.Start + " - " + task.Range.End);
        _cancellationTokens[task].Cancel();

        // wait for the task to finish
        _runningTasks[task].Join();

        _runningTasks.Remove(task);
        _cancellationTokens.Remove(task);

        // schedule a new task to replace the terminated one
        ScheduleTask();
    }
    
    /// <summary>
    /// Cancels all running tasks.
    /// </summary>
    private void CancelAllTasks() {
        foreach (var task in _runningTasks.Keys) {
            _cancellationTokens[task].Cancel();
            _runningTasks[task].Join();
        }
    }
}
