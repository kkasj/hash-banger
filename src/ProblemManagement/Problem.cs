using System.Collections.Concurrent;
using System.Linq.Expressions;
using design_patterns.Utils;
using design_patterns.Messaging;
using design_patterns.Messaging.MessageArgs;
using design_patterns.TaskRanges;
using design_patterns.Tasking;

namespace design_patterns.ProblemManagement;

/// <summary>
/// Represents a problem.
/// 
/// Manages the current state of the problem and notifies subscribers of changes.
/// </summary>
public class Problem : Observable
{
    public ConcurrentDictionary<ProblemUpdate, byte> Updates;
    public ProblemArgs? Args;
    public TaskRangeIterator Iterator;
    
    public Problem(TaskRangeIterator iterator){
        Iterator = iterator;
        Updates = new ConcurrentDictionary<ProblemUpdate, byte>();
    }

    /// <summary>
    /// Adds the update to the queue and notifies subscribers.
    /// </summary>
    /// <param name="problemUpdate"></param>
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
    
    /// <summary>
    /// Gets a new range from the iterator and reserves it.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public TaskRange GetNewRange(object? source){
        TaskRange newRange = Iterator.GetNext();
        ReserveRange(source, newRange);
        return newRange;
    }

    /// <summary>
    /// Notifies subscribers of a new problem.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newProblemArgs"></param>
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

    /// <summary>
    /// Notifies subscribers that a range has been reserved.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="range"></param>
    public void ReserveRange(object? source, TaskRange range){
        RangeReservationUpdate problemUpdate = new RangeReservationUpdate(source, range);
        Update(problemUpdate);
    }

    /// <summary>
    /// Notifies subscribers that the problem has been solved.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="result"></param>
    public void ProblemSolved(object? source, string result){
        Args.IsDone = true;
        Console.WriteLine("Found: " + result);
        ProblemSolvedUpdate problemUpdate = new ProblemSolvedUpdate(source, result);
        Update(problemUpdate);
    }

    /// <summary>
    /// Notifies subscribers that a range has been completed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="range"></param>
    public void RangeDone(object? source, TaskRange range){
        RangeDoneUpdate problemUpdate = new RangeDoneUpdate(source, range);
        Update(problemUpdate);
    }
}