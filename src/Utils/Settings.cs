namespace design_patterns.Utils;
using System.Text.Json;

public static class Settings {
    public const int DefaultPort = 50000;
    public static int Port = DefaultPort;
}

public static class ProblemParameters {
    public static TimeSpan EXPIRATION_DELTA = new TimeSpan(0, 30, 0);
    public static int MAX_TASKS = 5;
    public static string PASSWORD = "hashha";
    public static int DATA_LENGTH = PASSWORD.Length;
    public static Range DEFAULT_RANGE = new Range(DataIndexer.ToIndex(new string('a', DATA_LENGTH)), DataIndexer.ToIndex(new string('z', DATA_LENGTH)));
    public static int CHUNK_LENGTH = 456976; // = 26^4, must be a divisor of DEFAULT_RANGE (so a power of 26)
    // public static int CHUNK_LENGTH = 17576; // = 26^3, must be a divisor of DEFAULT_RANGE (so a power of 26)

    public static void UpdatePassword(string newPassword) {
        PASSWORD = newPassword;
        DATA_LENGTH = PASSWORD.Length;
        DEFAULT_RANGE = new Range(DataIndexer.ToIndex(new string('a', DATA_LENGTH)), DataIndexer.ToIndex(new string('z', DATA_LENGTH)));
    }   
}