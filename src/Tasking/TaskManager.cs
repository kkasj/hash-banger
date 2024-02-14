using System;
using System.Threading;
using System.Collections.Concurrent;
using design_patterns.Utils;
using design_patterns.Encryption;
using design_patterns.TaskRanges;
using design_patterns.ProblemManagement;

namespace design_patterns.Tasking;

/// <summary>
/// TaskManager is responsible for scheduling and managing tasks.
/// </summary>
public class TaskManager : Subscriber {
    private readonly object _lock = new object();
    protected Dictionary<EncryptionTask, Thread> runningTasks = new Dictionary<EncryptionTask, Thread>();
    protected Dictionary<EncryptionTask, CancellationTokenSource> cancellationTokens = new Dictionary<EncryptionTask, CancellationTokenSource>();
    
    public TaskManager(Problem problem) {
        this.problem = problem;
        this.problem.Subscribe(this);
        visitor = new TaskManagerProblemUpdateVisitor(this);
    }


    /// <summary>
    /// Schedules a new task to be executed.
    /// </summary>
    private void ScheduleTask() {
        TaskRange pickedRange = problem.GetNewRange(this);
        CancellationTokenSource cts = new CancellationTokenSource();
        EncryptionTask task = new EncryptionTask(this, pickedRange, EncrypterFactory.CreateEncrypter(problem.Args.EncryptionType), problem.Args, cts.Token);
        Thread InstanceCaller = new Thread(
            new ThreadStart(task.Start));
        runningTasks.Add(task, InstanceCaller);
        cancellationTokens.Add(task, cts);
        runningTasks[task].Start();
    }
    
    /// <summary>
    /// Called when a task has finished.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="result"></param>
    public void TaskDone(EncryptionTask task, TaskResult result) {
        lock(_lock){
            // check for cancellation in case it was requested during lock acquisition wait  
            if (task.Token.IsCancellationRequested) {
                return;
            }

            problem.RangeDone(this, result.Range);
            if (result.Status == TaskStatus.Found) {
                CancelAllTasks();
                problem.ProblemSolved(this, result.Result);
                return;
            }

            // replace the task with a new one
            ReplaceTask(task);
        }
    }
    
    /// <summary>
    /// Replaces a task with a new one.
    /// </summary>
    /// <param name="task"></param>
    private void ReplaceTask(EncryptionTask task) {
        cancellationTokens[task].Cancel();

        runningTasks.Remove(task);
        cancellationTokens.Remove(task);

        // schedule a new task to replace the terminated one
        ScheduleTask();
    }
    
    /// <summary>
    /// Cancels all running tasks.
    /// </summary>
    public void CancelAllTasks() {
        foreach (var task in runningTasks.Keys) {
            cancellationTokens[task].Cancel();
        }
    }

    public void ReplaceConflictingTasks(TaskRange range) {
        foreach (var task in runningTasks.Keys) {
            if (task.Range == range) {
                ReplaceTask(task);
            }
        }
    }

    public void StartTasks() {
        while (runningTasks.Count < ProblemParameters.MAX_TASKS) {
            ScheduleTask();
        }
    }
}
