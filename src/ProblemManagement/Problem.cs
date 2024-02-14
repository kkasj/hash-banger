using System.Collections.Concurrent;
using System.Linq.Expressions;
using design_patterns.Utils;
using design_patterns.Messaging;
using design_patterns.Messaging.MessageArgs;
using design_patterns.TaskRanges;
using design_patterns.Tasking;

namespace design_patterns.ProblemManagement;

public class Problem : Observable
{
    public ConcurrentDictionary<ProblemUpdate, byte> Updates;
    public ProblemArgs? Args;
    public TaskRangeIterator Iterator;
    
    public Problem(TaskRangeIterator iterator){
        Iterator = iterator;
        Updates = new ConcurrentDictionary<ProblemUpdate, byte>();
    }
    private void Update(ProblemUpdate problemUpdate){
        Updates.TryAdd(problemUpdate, 0);
        PokeAllSubscribers();
        
        // if the number of acknowledgements is equal to the number of subscribers
        // delete the update
        if (problemUpdate.Acknowledgements.Count == Subscribers.Count){
            byte value;
            Updates.TryRemove(problemUpdate, out value);
        }
    }
    
    public TaskRange GetNewRange(object? source){
        TaskRange newRange = Iterator.GetNext();
        ReserveRange(source, newRange);
        return newRange;
    }
    public void GotNewProblem(object? source, NewProblemArgs newProblemArgs){
        NewRangeCollectionUpdate collectionUpdate = new NewRangeCollectionUpdate(source, newProblemArgs.RangeCollection);
        Update(collectionUpdate);

        if (newProblemArgs.RangeCollection is not null){
            Iterator = new RandomTaskRangeIterator(newProblemArgs.RangeCollection);
        }

        Args = new ProblemArgs(newProblemArgs.ProblemHash, newProblemArgs.EncryptionType);

        NewProblemUpdate problemUpdate = new NewProblemUpdate(source, newProblemArgs);
        Update(problemUpdate);
    }
    public void ReserveRange(object? source, TaskRange range){
        RangeReservationUpdate problemUpdate = new RangeReservationUpdate(source, range);
        Update(problemUpdate);
    }
    public void ProblemSolved(object? source, string result){
        Args.IsDone = true;
        Console.WriteLine("Found: " + result);
        ProblemSolvedUpdate problemUpdate = new ProblemSolvedUpdate(source, result);
        Update(problemUpdate);
    }
    public void RangeDone(object? source, TaskRange range){
        RangeDoneUpdate problemUpdate = new RangeDoneUpdate(source, range);
        Update(problemUpdate);
    }
}