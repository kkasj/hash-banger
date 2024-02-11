/// <summary>
/// This class is a proxy for the TaskRangeIterator class. It is used to keep track of the updates made to the iterator and to apply them to the iterator when needed.
/// </summary>
public class IteratorProxy
{
    public TaskRangeIterator Iterator { get; set; }
    public List<RangeUpdate> Updates { get; set; }

    public IteratorProxy(TaskRangeIterator iterator)
    {
        Iterator = iterator;
        Updates = new List<RangeUpdate>();
    }

    /// <summary>
    /// Updates the iterator with the given range update.
    /// </summary>
    /// <param name="rangeUpdate">
    /// The range update to apply to the iterator.
    /// </param>
    public void UpdateRange(RangeUpdate rangeUpdate)
    {
        Updates.Append(rangeUpdate);

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
    public Range GetNext()
    {
        return Iterator.GetNext();
    }   

    /// <summary>
    /// Resets the iterator state.
    /// </summary>
    public void Reset()
    {
        Iterator.Reset();
    }
}