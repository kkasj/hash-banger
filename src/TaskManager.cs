using System;
using System.Threading;
using design_patterns.Utils;
using System.Collections.Concurrent;

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
        if(_problem.Args is not null && _problem.Args.IsDone) {
            CancelAllTasks();
            return;
        }
        var rangeUpdates = _problem.IteratorProxy.Updates;

        // take updates whose source is Sync
        var syncUpdates = rangeUpdates.Where(update => update.UpdateSource == UpdateSource.Sync);
        // delete them from the original list
        if (syncUpdates.Count() > 0){
            _problem.IteratorProxy.Updates = new ConcurrentBag<RangeUpdate>(rangeUpdates.Except(syncUpdates));
        }

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
        ReservedRange pickedRange = _problem.IteratorProxy.GetNext();
        CancellationTokenSource cts = new CancellationTokenSource();
        EncryptionTask task = new EncryptionTask(this, pickedRange, EncrypterFactory.CreateEncrypter(_problem.Args.EncryptionType), _problem.Args, cts.Token);
        Thread InstanceCaller = new Thread(
            new ThreadStart(task.Start));
        _runningTasks.Add(task, InstanceCaller);

        _cancellationTokens.Add(task, cts);

        _runningTasks[task].Start();
        // Console.WriteLine("Task started: " + pickedRange.Start + " - " + pickedRange.End);
        _problem.ReserveTask(this, pickedRange);
    }
    
    /// <summary>
    /// Called when a task has finished.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="result"></param>
    public void TaskFinished(EncryptionTask task, TaskResult result) {
        lock(_lock){
            // check for cancellation in case it was requested during lock acquisition wait  
            if (task.Token.IsCancellationRequested) {
                return;
            }

            // make sure the problem hasn't changed since the task was started 
            if (task.ProblemArgs == _problem.Args) {
                _problem.TaskDone(this, result.Range);
                if (result.Status == TaskStatus.Found) {
                    CancelAllTasks();
                    _problem.ProblemSolved(result.Result);
                    return;
                }
            }

            // replace the task with a new one
            ReplaceTask(task);

            // Console.WriteLine("Task finished: " + result.Range.Start + " - " + result.Range.End);
        }
    }
    
    /// <summary>
    /// Replaces a task with a new one.
    /// </summary>
    /// <param name="task"></param>
    private void ReplaceTask(EncryptionTask task) {
        _cancellationTokens[task].Cancel();

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
        }
    }
}
