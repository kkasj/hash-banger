using design_patterns.Encryption;
using design_patterns.TaskRanges;

namespace design_patterns.Messaging.MessageArgs;

public class NewProblemArgs {
    public string ProblemHash { get; private set; }
    public EncryptionType EncryptionType { get; private set; }
    public TaskRangeCollection? RangeCollection { get; private set;}
    
    public NewProblemArgs(string problemHash, EncryptionType encryptionType, TaskRangeCollection? rangeCollection) {
        ProblemHash = problemHash;
        EncryptionType = encryptionType;
        RangeCollection = rangeCollection;
    }
}