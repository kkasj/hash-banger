using design_patterns.ProblemManagement;
using design_patterns.TaskRanges;
using design_patterns.Tasking;

namespace design_patterns.TaskRanges;

/// <summary>
/// Represents a task range collection manager.
/// Serves as an intermediary between the problem and the task range collection.
/// </summary>
public class TaskRangeCollectionManager : Subscriber
{
    public TaskRangeCollection RangeCollection { get; set; }

    public TaskRangeCollectionManager(Problem problem, TaskRangeCollection rangeCollection){
        this.problem = problem;
        problem.Subscribe(this);
        RangeCollection = rangeCollection;
        visitor = new CollectionManagerProblemUpdateVisitor(this);
    }

    /// <summary>
    /// Reserves a range in the collection.
    /// </summary>
    /// <param name="range"></param>
    public void ReserveRange(TaskRange range){
        // RangeCollection.FreeExpiredReservedRanges();
        RangeCollection.ReserveRange(range);
    }

    /// <summary>
    /// Removes a reserved range from the collection.
    /// </summary>
    /// <param name="range"></param>
    public void RemoveReservedRange(TaskRange range){
        RangeCollection.RemoveReservedRange(range);
    }

    /// <summary>
    /// Resets the collection.
    /// </summary>
    public void Reset(){
        RangeCollection.Reset();
    }
}
    