public class ProblemArgs {
    public string ProblemHash { get; set; }
    public EncryptionType EncryptionType { get; set; }
    public bool IsDone { get; set; }
    public string Result { get; set; }

    public ProblemArgs(string problemHash, EncryptionType encryptionType) {
        ProblemHash = problemHash;
        EncryptionType = encryptionType;
        IsDone = false;
        Result = "";
    }

    // equality
    public override bool Equals(object? obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }
        ProblemArgs other = (ProblemArgs)obj;
        return ProblemHash == other.ProblemHash && EncryptionType == other.EncryptionType;
    }
}