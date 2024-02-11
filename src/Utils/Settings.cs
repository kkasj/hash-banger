namespace design_patterns.Utils;
using System.Text.Json;

public static class Settings {
    public const int DefaultPort = 50000;
    public static int Port = DefaultPort;
}

public static class ProblemParameters {
    public static int DATA_LENGTH = 6;
    public static int CHUNK_LENGTH = 100000;
    // expiration datetime delta
    public static TimeSpan EXPIRATION_DELTA = new TimeSpan(0, 30, 0);
    public static Range DEFAULT_RANGE = new Range(1, 100000000);
    public static int MAX_TASKS = 20;
    public static string PASSWORD = "hashha";
}