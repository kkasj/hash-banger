namespace peer2peer; 

public class EncryptionTask {
    private TaskManager _manager;
    private Range _range;
    private IEncrypter _encrypter;
    private string _problemHash;

    public EncryptionTask(TaskManager manager, Range range, IEncrypter encrypter, string problemHash) {
        _manager = manager;
        _range = range;
        _encrypter = encrypter;
        _problemHash = problemHash;
    }
    
    public void Start() {
        //TODO:
        
        _manager.TaskFinished(this, new TaskResult());
    }
}