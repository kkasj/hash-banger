public class IteratorProxy
{
    public TaskRangeIterator Iterator { get; set; }
    public List<RangeUpdate> Updates { get; set; }

    public IteratorProxy(TaskRangeIterator iterator)
    {
        Iterator = iterator;
        Updates = new List<RangeUpdate>();
    }

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

    public Range GetNext()
    {
        return Iterator.GetNext();
    }   
}