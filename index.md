---
_layout: landing
---

# HashBanger Documentation

## Hardware Architecture Diagram
System is a network of interconnected worker nodes, that exchange information about current status of a problem and work that has to be done in order to solve it.
To establish a connection between nodes, central server is used which itself is not a part of problem communication.
![Diagram fizyczny](./resources/diagram_fizyczny.jpg)
## Logic Architecture - Class Diagram

Main parts of the solutions are:
    - Problem
    - Thread management and encryption
    - Network communication

#### Problem
Problem contains actual state of finish, and it's last yet unhandled changes.

It is responsible for managing the problem - it keeps all done and reserved ranges of solution. Anyone can access problem to get it.

#### Network Communication


#### Thread Management and Encryption
This part is resposnible for actually solving the problem.
It takes possible unchecked combination and schedule threads task to try them.

At the end it notifies Problem part about the progress

```mermaid
classDiagram
    class DataIndexer{
        +static string FromIndex(int index)
        +static int ToIndex(string data)
    }
    class EncrypterFactory{
        +static IEncrypter CreateEncrypter(EncryptionType encryptionType)
    }
    class IEncrypter {
        +abstract string Encrypt(string data)
    }
    <<interface>> IEncrypter
    class SHA1Encrypter{
        +string Encrypt(string data)
    }
    class EncryptionTask{
        +Range Range
        +ProblemArgs ProblemArgs
        +CancellationToken Token
        -TaskManager _manager
        -IEncrypter _encrypter

        +EncryptionTask(TaskManager manager, Range range, IEncrypter encrypter, ProblemArgs, problemArgs, CancellationToken token)
        +void Start()
    }
    class ISubscriber{
        +abstract void Poke()
    }
    class IteratorProxy{
        +TaskRangeIterator Iterator
        +List<RangeUpdate> Updates

        +IteratorProxy(TaskRangeIterator iterator)
        +void UpdateRange(RangeUpdate rangeUpdate)
        +Range GetNext()
        +void Reset()
    }
    class Observable{
        -List<ISubscriber> subscribers
        +void Subscribe(ISubscriber subscriber)
        +void PokeAllSubscribers()
    }
    class Problem{
        +IteratorProxy IteratorProxy
        +ProblemArgs Args

        +Problem(IteratorProxy IteratorProxy, ProblemArgs args)
        +void GotNewProblem(NewProblemArgs newProblemArgs)
        +void TaskDone(RangeUpdater updater, Range range)
        +void ReserveTask(RangeUpdater updater, Range range)
        +void ProblemSolved(string result)
    }
    class ProblemArgs{
        +string ProblemHash
        +EncryptionType EncryptionType
        +bool IsDone
        +string Result

        +ProblemArgs(string problemHash, EncryptionType encryptionType)
    }
    class RangeUpdate{
        +UpdateType UpdateType
        +UpdateSource UpdateSource
        +Range range

        +RangeUpdate(UpdateType updateType, UpdateSource updateSource)
    }
    class RangeUpdater{
        +UpdateSource Source
    }
    <<abstract>> RangeUpdater
    class TaskManager{
        -readonly object _lock
        -Dictionary<EncryptionTask, Thread> _runningTasks
        -Dictionary<EncryptionTask, CancellationTokenSource> _cancellationTokens
        -Problem _problem

        +TaskManager(Problem problem)
        +void Poke()
        +void ScheduleTask()
        +void TaskFinished(EncryptionTask task, TaskResult result)
        -void ReplaceTask(EncryptionTask task)
        -void CancelAllTasks()
    }
    class Range{
        +int Start
        +int End

        +Range(int start, int end)
        +int CompareTo(Range other)
        +bool Equals(object obj)
    }
    class ReservedRange{
        +DateTime ReservedAt

        +ReservedRange(int start, int end, DateTime reservedAt)
        +bool Equals(object obj)
    }
    class RangeSortedList{
        +RangeSortedList()
        +void Add(Range range)
        +void Add(int start, int end)
        +void Remove(Range range)
        +Range this(int index)
    }
    class TaskRangeIterator{
        -RangeSortedList availableRanges
        -RangeSortedList reservedRanges

        +TaskRangeIterator()
        +ReservedRange GetNext()
        +void MergeRanges()
        +void ReserveRange(Range range)
        +void FreeExpiredReservedRanges()
        +void FreeReservedRange(Range range)
        +void RemoveReservedRange(Range range)
        +void Reset()
        +void PrintState()
    }
    class TaskResult{
        +TaskStatus Status
        +string? Result
        +Range Range

        +TaskResult(TaskStatus status, string? result, Range range)
    }
    class TaskStatus{
        Found
        NotFound
    }
    <<enum>> TaskStatus
    class UpdateType{
        RangeReservation
        RangeDone
    }
    <<enum>> UpdateType
    class UpdateSource{
        Local
        Sync
    }
    <<enum>> UpdateSource
    class EncryptionType{
        SHA1
    }
    <<enum>> EncryptionType
    class LocalPeer{
        -readonly NetPeer _peer
        +delegate void MessageReceivedEventHandler(string message, NetConnection sender)
        +event MessageReceivedEventHandler MessageReceived

        -void ProcessNet
        +LocalPeer()
        +Register()
        +BroadcastMessage(Message msg)
        +void SendMessage(NetConnection receiver, Message msg)
        +Message CreateMessage(MessageType type, string content)
        +void NodeConnected(NewNodeConnectedArgs args)
        +void NodeDisconnected(NodeDisconnectedArgs args)
        +void NodesReceived(NodesReceivedArgs args)
    }
    class Message{
        +MessageType Type
        +string Content

        +Message(MessageType type, string content)
    }
    class MessageType{
        Hello,
        NewNodeConnected,
        NodeDisconnected,
        NodesReceived,
        NewProblem,
        CancelProblem,
        ProblemSolved,
        RangeStarted,
        RangeCompleted,
    }
    <<enum>> MessageType
    class MessageHandler{
        -readonly LocalPeer _localPeer
        -Problem _problem
        +UpdateSource Source
        
        +MessageHandler(LocalPeer localPeer, Problem problem)
        +void Poke()
        -void HandleMessage(string newMessage)
        +static Message Deserialize(string message)
        +static string Serialize(Message message)
    }
    class CancelProblemArgs
    class NewNodeConnectedArgs{
        +string Socket

        +NewNodeConnectedArgs(string socket)
    }
    class NewProblemArgs{
        +string ProblemHash
        +EncryptionType EncryptionType
        +TaskRangeIterator? Iterator

        +NewProblemArgs(string problemHash, EncryptionType encryptionType, TaskRangeIterator? iterator)
    }
    class NodeDisconnectedArgs{
        +string Socket

        +NodeDisconnectedArgs(string socket)
    }
    class NodesReceivedArgs{
        +List<string> Sockets

        +NodesReceivedArgs(List<string> sockets)
    }
    class ProblemSolvedArgs
    class RangeCompletedArgs
    class RangeStartedArgs

    
    

    Range <|-- IComparable
    ReservedRange <|-- Range
    RangeSortedList <|-- SortedList
    SHA1Encrypter ..|> IEncrypter
    EncryptionTask <|-- TaskManager
    Problem <|-- IteratorProxy
    IteratorProxy <|-- TaskRangeIterator
    MessageHandler --> Problem: Subscribe
    TaskManager --> Problem: Subscribe
    Peer <|-- MessageHandler
    MessageHandler <|.. ISubscriber
    MessageHandler <|.. RangeUpdater
    TaskManager <|.. ISubscriber
    TaskManager <|.. RangeUpdater
    Problem <|.. Observable

    IEncrypter ..|> SHA1Encrypter
    EncrypterFactory <.. EncryptionType
    EncrypterFactory --> IEncrypter
    IEncrypter *-- EncryptionTask

    
```
## Used Design Patterns

### Observer Pattern

In this case observer pattern helps in decoupling the Problem from MessageHandler and TaskManager, so the problem keeps being separated from network connection and thread management system parts.

It assures scalability of the solution, because it is easy to access the problem.

Thanks to this solution we can easily add runtime statistics system or GUI.  

```mermaid
classDiagram

    class Problem{
        +IteratorProxy IteratorProxy
        +ProblemArgs Args

        +Problem(IteratorProxy IteratorProxy, ProblemArgs args)
        +void GotNewProblem(NewProblemArgs newProblemArgs)
        +void TaskDone(RangeUpdater updater, Range range)
        +void ReserveTask(RangeUpdater updater, Range range)
        +void ProblemSolved(string result)
    }

    class MessageHandler{
        -readonly LocalPeer _localPeer
        -Problem _problem
        +UpdateSource Source
        
        +MessageHandler(LocalPeer localPeer, Problem problem)
        +void Poke()
        -void HandleMessage(string newMessage)
        +static Message Deserialize(string message)
        +static string Serialize(Message message)
    }

    class Observable{
        -List<ISubscriber> subscribers
        +void Subscribe(ISubscriber subscriber)
        +void PokeAllSubscribers()
    }

    class ISubscriber{
        +abstract void Poke()
    }
    
    class TaskManager{
        -readonly object _lock
        -Dictionary<EncryptionTask, Thread> _runningTasks
        -Dictionary<EncryptionTask, CancellationTokenSource> _cancellationTokens
        -Problem _problem

        +TaskManager(Problem problem)
        +void Poke()
        +void ScheduleTask()
        +void TaskFinished(EncryptionTask task, TaskResult result)
        -void ReplaceTask(EncryptionTask task)
        -void CancelAllTasks()
    }

    MessageHandler <|.. ISubscriber
    MessageHandler <|.. RangeUpdater
    TaskManager <|.. RangeUpdater
    MessageHandler --> Problem: Subscribe
    TaskManager --> Problem: Subscribe
    Problem <|.. Observable
    TaskManager <|.. ISubscriber

```

### Strategy Pattern

In this case strategy patern assures capability for future use case extensions. It allows to easily implement new hash methods.

Architecture is able to easily swap encryption methods for problems.

```mermaid
classDiagram

    class IEncrypter {
        +abstract string Encrypt(string data)
    }

    <<interface>> IEncrypter
    class SHA1Encrypter{
        +string Encrypt(string data)
    }

    class EncryptionTask{
        +Range Range
        +ProblemArgs ProblemArgs
        +CancellationToken Token
        -TaskManager _manager
        -IEncrypter _encrypter

        +EncryptionTask(TaskManager manager, Range range, IEncrypter encrypter, ProblemArgs, problemArgs, CancellationToken token)
        +void Start()
    }

    IEncrypter *-- EncryptionTask
    IEncrypter ..|> SHA1Encrypter

```

### Factory Pattern
In this case factory patterns is strongly combined with strategy pattern. Architecture allows easy creation of specific Encryptors for the problem.

It makes future extensions even more easy to implement.

```mermaid
classDiagram
    class EncryptionType{
        SHA1
    }

    <<enum>> EncryptionType
    class IEncrypter {
        +abstract string Encrypt(string data)
    }

    <<interface>> IEncrypter
    class SHA1Encrypter{
        +string Encrypt(string data)
    }

    class EncrypterFactory{
        +static IEncrypter CreateEncrypter(EncryptionType encryptionType)
    }

    IEncrypter ..|> SHA1Encrypter
    EncrypterFactory <.. EncryptionType
    EncrypterFactory --> IEncrypter

```





