// namespace design_patterns.Messaging.MessageArgs; 

public class NewProblemArgs {
    public string ProblemHash { get; private set; }
    public EncryptionType EncryptionType { get; private set; }
    public TaskRangeIterator? Iterator { get; private set;}
    
    public NewProblemArgs(string problemHash, EncryptionType encryptionType, TaskRangeIterator? iterator) {
        ProblemHash = problemHash;
        EncryptionType = encryptionType;
        Iterator = iterator;
    }
}