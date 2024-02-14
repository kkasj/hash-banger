namespace design_patterns.TaskRanges;

/// <summary>
/// RangeSortedList is a class that is used to store ranges and sort them by their start value
/// </summary>
public class RangeSortedList : SortedList<int, TaskRange>
{
    public RangeSortedList() : base()
    {
    }

    public void Add(TaskRange range)
    {
        base.Add(range.Start, range);
    }

    public void Add(int start, int end)
    {
        TaskRange range = new TaskRange(start, end);
        base.Add(start, range);
    }

    public void Remove(TaskRange range)
    {
        base.Remove(range.Start);
    }

    // implement indexing
    public new TaskRange this[int index]
    {
        get
        {
            var key = this.Keys[index];
            return base[key];
        }
    }
}