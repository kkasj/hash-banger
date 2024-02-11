using System;
using System.Threading;

public class EncryptionTask {
    public Range Range;
    public ProblemArgs ProblemArgs;
    private TaskManager _manager;
    private IEncrypter _encrypter;

    public EncryptionTask(TaskManager manager, Range range, IEncrypter encrypter, ProblemArgs problemArgs) {
        Range = range;
        ProblemArgs = problemArgs;
        _manager = manager;
        _encrypter = encrypter;
    }
    
    public void Start() {
        CancellationToken token = new CancellationToken();        

        for(int i = Range.Start; i < Range.End; i++) {
            // check for thread cancellation
            if (token.IsCancellationRequested) {
                return;
            }
            
            string data = DataIndexer.FromIndex(i);
            if (_encrypter.Encrypt(data) == ProblemArgs.ProblemHash) {
                _manager.TaskFinished(this, new TaskResult(TaskStatus.Found, data, Range));
                return;
            }
        }
        
        _manager.TaskFinished(this, new TaskResult(TaskStatus.NotFound, null, Range));
    }
}