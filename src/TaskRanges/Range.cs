namespace design_patterns.TaskRanges;

/// <summary>
/// Range is a class that is used to store a range of numbers
/// </summary>
public class TaskRange : IComparable<TaskRange>
{
    public int Start { get; set; }
    public int End { get; set; }

    public TaskRange(int start, int end)
    {
        Start = start;
        End = end;
    }

    public int CompareTo(TaskRange other)
    {
        return Start.CompareTo(other.Start);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        TaskRange other = (TaskRange)obj;
        return Start == other.Start && End == other.End;
    }
}

/// <summary>
/// ReservedRange is a class that is used to store a range that has been reserved
/// and the time at which it was reserved
/// </summary>
public class ReservedRange : TaskRange
{
    public DateTime ReservedAt { get; set; }

    public ReservedRange(int start, int end, DateTime reservedAt) : base(start, end)
    {
        ReservedAt = reservedAt;
    }


    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        ReservedRange other = (ReservedRange)obj;
        return Start == other.Start && End == other.End && ReservedAt == other.ReservedAt;
    }
} 

