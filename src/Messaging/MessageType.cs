namespace design_patterns.Messaging; 

public enum MessageType {
    // Server Messages
    Hello,
    NewNodeConnected,
    NodeDisconnected,
    NodesReceived,
    // Client Messages
    NewProblem,
    CancelProblem,
    ProblemSolved,
    RangeStarted,
    RangeCompleted,
}