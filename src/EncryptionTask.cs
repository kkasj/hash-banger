using System;
using System.Threading;

/// <summary>
/// Represents a task that is responsible for encrypting data and comparing it to a given hash.
/// </summary>
public class EncryptionTask {
    public Range Range;
    public ProblemArgs ProblemArgs;
    private TaskManager _manager;
    private IEncrypter _encrypter;
    private CancellationToken _token;

    public EncryptionTask(TaskManager manager, Range range, IEncrypter encrypter, ProblemArgs problemArgs, CancellationToken token) {
        Range = range;
        ProblemArgs = problemArgs;
        _manager = manager;
        _encrypter = encrypter;
        _token = token;
    }
    
    /// <summary>
    /// Starts the task.
    /// </summary>
    public void Start() {
        for(int i = Range.Start; i < Range.End; i++) {
            // check for thread cancellation
            if (_token.IsCancellationRequested) {
                return;
            }
            
            string data = DataIndexer.FromIndex(i);
            if (_encrypter.Encrypt(data) == ProblemArgs.ProblemHash) {
                _manager.TaskFinished(this, new TaskResult(TaskStatus.Found, data, Range));
                return;
            }
        }
        
        _manager.TaskFinished(this, new TaskResult(TaskStatus.NotFound, null, Range));
        return;
    }
}