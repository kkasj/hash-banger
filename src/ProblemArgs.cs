public class ProblemArgs {
    public string ProblemHash { get; set; }
    public EncryptionType EncryptionType { get; set; }

    public ProblemArgs(string problemHash, EncryptionType encryptionType) {
        ProblemHash = problemHash;
        EncryptionType = encryptionType;
    }
}