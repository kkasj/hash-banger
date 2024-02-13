using System.Collections.Concurrent;

/// <summary>
/// This class is a proxy for the TaskRangeIterator class. It is used to keep track of the updates made to the iterator and to apply them to the iterator when needed.
/// </summary>
public class IteratorProxy
{
    public TaskRangeIterator Iterator { get; set; }
    private object _lock = new object();
    public ConcurrentBag<RangeUpdate> Updates;

    public IteratorProxy(TaskRangeIterator iterator)
    {
        Iterator = iterator;
        Updates = new ConcurrentBag<RangeUpdate>();
    }

    /// <summary>
    /// Updates the iterator with the given range update.
    /// </summary>
    /// <param name="rangeUpdate">
    /// The range update to apply to the iterator.
    /// </param>
    public void UpdateRange(RangeUpdate rangeUpdate)
    {
        Updates.Add(rangeUpdate);

        switch (rangeUpdate.UpdateType)
        {
            case UpdateType.RangeReservation:
                Iterator.ReserveRange(rangeUpdate.Range);
                break;
            case UpdateType.RangeDone:
                Iterator.RemoveReservedRange(rangeUpdate.Range);
                break;
        }
    }

    /// <summary>
    /// Gets the next range from the iterator.
    /// </summary>
    /// <returns></returns>
    public ReservedRange GetNext()
    {
        return Iterator.GetNext();
    }   

    /// <summary>
    /// Resets the iterator state.
    /// </summary>
    public void Reset(TaskRangeIterator? iterator = null)
    {
        Updates = new ConcurrentBag<RangeUpdate>();
        if (iterator != null)
        {
            Iterator = iterator;
        }
        else
        {
            Iterator.Reset();
        }
    }
}