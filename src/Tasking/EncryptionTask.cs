using System;
using System.Threading;
using design_patterns.ProblemManagement;
using design_patterns.Encryption;
using design_patterns.TaskRanges;

namespace design_patterns.Tasking;

/// <summary>
/// Represents a task that is responsible for encrypting data and comparing it to a given hash.
/// </summary>
public class EncryptionTask {
    public TaskRange Range;
    public ProblemArgs ProblemArgs;
    public CancellationToken Token;
    private TaskManager _manager;
    private IEncrypter _encrypter;

    public EncryptionTask(TaskManager manager, TaskRange range, IEncrypter encrypter, ProblemArgs problemArgs, CancellationToken token) {
        Range = range;
        ProblemArgs = problemArgs;
        Token = token;
        _manager = manager;
        _encrypter = encrypter;
    }
    
    /// <summary>
    /// Starts the task.
    /// </summary>
    public void Start() {
        for(int i = Range.Start; i < Range.End; i++) {
            // check for thread cancellation
            if (Token.IsCancellationRequested) {
                return;
            }
            
            string data = DataIndexer.FromIndex(i);
            if (_encrypter.Encrypt(data) == ProblemArgs.ProblemHash) {
                _manager.TaskDone(this, new TaskResult(TaskStatus.Found, data, Range));
                return;
            }
        }
        
        _manager.TaskDone(this, new TaskResult(TaskStatus.NotFound, null, Range));
        return;
    }
}