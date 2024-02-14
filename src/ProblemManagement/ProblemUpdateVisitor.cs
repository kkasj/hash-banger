using design_patterns.Messaging;
using design_patterns.TaskRanges;
using design_patterns.Tasking;

namespace design_patterns.ProblemManagement;

public interface ProblemUpdateVisitor
{
    public void Visit(RangeReservationUpdate update);
    public void Visit(RangeDoneUpdate update);
    public void Visit(NewRangeCollectionUpdate update);
    public void Visit(NewProblemUpdate update);
    public void Visit(ProblemSolvedUpdate update);
}
    


public class MessageHandlerProblemUpdateVisitor : ProblemUpdateVisitor
{
    private MessageHandler _messageHandler;
    
    public MessageHandlerProblemUpdateVisitor(MessageHandler messageHandler){
        _messageHandler = messageHandler;
    }
    public void Visit(RangeReservationUpdate update){
        _messageHandler.BroadcastRangeReservation(update.Range);
    }
    public void Visit(RangeDoneUpdate update){
        _messageHandler.BroadcastRangeDone(update.Range);
    }
    public void Visit(NewRangeCollectionUpdate update){
        return;
    }
    public void Visit(NewProblemUpdate update){
        _messageHandler.BroadcastNewProblem();
    }
    public void Visit(ProblemSolvedUpdate update){
        _messageHandler.BroadcastResult(update.Result);
    }
}
    

public class TaskManagerProblemUpdateVisitor : ProblemUpdateVisitor
{
    private TaskManager _taskManager;
    
    public TaskManagerProblemUpdateVisitor(TaskManager taskManager){
        _taskManager = taskManager;
    }
    public void Visit(RangeReservationUpdate update){
        _taskManager.ReplaceConflictingTasks(update.Range);
    }
    public void Visit(RangeDoneUpdate update){
        _taskManager.ReplaceConflictingTasks(update.Range);
    }
    public void Visit(NewRangeCollectionUpdate update){
        return;
    }
    public void Visit(NewProblemUpdate update){
        _taskManager.CancelAllTasks();
        _taskManager.StartTasks();
    }
    public void Visit(ProblemSolvedUpdate update){
        _taskManager.CancelAllTasks();
        return;
    }
}
    

public class CollectionManagerProblemUpdateVisitor : ProblemUpdateVisitor
{
    private TaskRangeCollectionManager _collectionManager;
    
    public CollectionManagerProblemUpdateVisitor(TaskRangeCollectionManager collectionManager){
        _collectionManager = collectionManager;
    }
    public void Visit(RangeReservationUpdate update){
        _collectionManager.ReserveRange(update.Range);
    }
    public void Visit(RangeDoneUpdate update){
        _collectionManager.RemoveReservedRange(update.Range);
    }
    public void Visit(NewRangeCollectionUpdate update){
        if (update.RangeCollection is not null){
            _collectionManager.RangeCollection = update.RangeCollection;
        }
        else{
            _collectionManager.Reset();
        }
    }
    public void Visit(NewProblemUpdate update){
        return;
    }
    public void Visit(ProblemSolvedUpdate update){
        _collectionManager.Reset();
    }
}
    