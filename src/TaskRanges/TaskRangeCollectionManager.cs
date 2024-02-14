using design_patterns.ProblemManagement;
using design_patterns.TaskRanges;
using design_patterns.Tasking;

namespace design_patterns.TaskRanges;

public class TaskRangeCollectionManager : Subscriber
{
    public TaskRangeCollection RangeCollection { get; set; }

    public TaskRangeCollectionManager(Problem problem, TaskRangeCollection rangeCollection){
        this.problem = problem;
        problem.Subscribe(this);
        RangeCollection = rangeCollection;
        visitor = new CollectionManagerProblemUpdateVisitor(this);
    }
    public void ReserveRange(TaskRange range){
        // RangeCollection.FreeExpiredReservedRanges();
        RangeCollection.ReserveRange(range);
    }
    public void RemoveReservedRange(TaskRange range){
        RangeCollection.RemoveReservedRange(range);
    }
    public void Reset(){
        RangeCollection.Reset();
    }
}
    