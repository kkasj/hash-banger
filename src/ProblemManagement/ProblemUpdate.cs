using System.Collections.Concurrent;
using System.Reflection.Metadata;
using design_patterns.Messaging;
using design_patterns.Messaging.MessageArgs;
using design_patterns.TaskRanges;
using design_patterns.Tasking;

namespace design_patterns.ProblemManagement;

public abstract class ProblemUpdate
{
    public object? Source { get; set; }
    public ConcurrentDictionary<object, byte> Acknowledgements { get; set; }
    
    public ProblemUpdate(){
        Acknowledgements = new ConcurrentDictionary<object, byte>();
    }
    public abstract void Accept(ProblemUpdateVisitor visitor);
    public bool Acknowledge(object obj){
        if (Acknowledgements.ContainsKey(obj)){
            return true;
        }
        Acknowledgements.TryAdd(obj, 0);
        return false;
    }
    public override string ToString(){
        return "ProblemUpdate";
    }
}


public class RangeReservationUpdate : ProblemUpdate
{
    public TaskRange Range;

    public RangeReservationUpdate(object? source, TaskRange range){
        Source = source;
        Range = range;
        Acknowledgements = new ConcurrentDictionary<object, byte>();
    }
    public override void Accept(ProblemUpdateVisitor visitor){
        visitor.Visit(this);
    }
    public override string ToString(){
        return "RangeReservationUpdate";
    }
}


public class RangeDoneUpdate : ProblemUpdate
{
    public TaskRange Range;

    public RangeDoneUpdate(object? source, TaskRange range){
        Source = source;
        Range = range;
        Acknowledgements = new ConcurrentDictionary<object, byte>();
    }
    public override void Accept(ProblemUpdateVisitor visitor){
        visitor.Visit(this);
    }
    public override string ToString(){
        return "RangeDoneUpdate";
    }
}

public class NewRangeCollectionUpdate : ProblemUpdate
{
    public TaskRangeCollection? RangeCollection;

    public NewRangeCollectionUpdate(object? source, TaskRangeCollection? rangeCollection){
        Source = source;
        RangeCollection = rangeCollection;
        Acknowledgements = new ConcurrentDictionary<object, byte>();
    }
    public override void Accept(ProblemUpdateVisitor visitor){
        visitor.Visit(this);
    }
    public override string ToString(){
        return "NewRangeCollectionUpdate";
    }
}

public class NewProblemUpdate : ProblemUpdate
{
    public NewProblemArgs NewProblemArgs;

    public NewProblemUpdate(object? source, NewProblemArgs newProblemArgs){
        Source = source;
        NewProblemArgs = newProblemArgs;
        Acknowledgements = new ConcurrentDictionary<object, byte>();
    }
    public override void Accept(ProblemUpdateVisitor visitor){
        visitor.Visit(this);
    }
    public override string ToString(){
        return "NewProblemUpdate";
    }
}
    

public class ProblemSolvedUpdate : ProblemUpdate
{
    public string Result;

    public ProblemSolvedUpdate(object? source, string result){
        Source = source;
        Result = result;
        Acknowledgements = new ConcurrentDictionary<object, byte>();
    }
    public override void Accept(ProblemUpdateVisitor visitor){
        visitor.Visit(this);
    }
    public override string ToString(){
        return "ProblemSolvedUpdate";
    }
}
    