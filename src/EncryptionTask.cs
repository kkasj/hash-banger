namespace peer2peer; 

public class EncryptionTask {
    private TaskManager _manager;
    private Range _range;
    private IEncrypter _encrypter;

    EncryptionTask(TaskManager manager, Range range, IEncrypter encrypter) {
        _manager = manager;
        _range = range;
        _encrypter = encrypter;
    }
    
    public void Start() {
        //TODO:
    }
}