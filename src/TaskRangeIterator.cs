using System;
using System.Collections;

public class Range : IComparable<Range>
{
    public int Start { get; set; }
    public int End { get; set; }

    public Range(int start, int end)
    {
        Start = start;
        End = end;
    }

    public int CompareTo(Range other)
    {
        return Start.CompareTo(other.Start);
    }
}

// ReservedRange is a subclass of Range that also includes reservation timestamp
public class ReservedRange : Range
{
    public DateTime ReservedAt { get; set; }

    public ReservedRange(int start, int end, DateTime reservedAt) : base(start, end)
    {
        ReservedAt = reservedAt;
    }
} 


public class RangeSortedList : SortedList
{
    public RangeSortedList() : base()
    {
    }

    public void Add(Range range)
    {
        base.Add(range.Start, range);
    }

    public void Remove(Range range)
    {
        base.Remove(range.Start);
    }

    // implement indexing
    public Range this[int index]
    {
        get
        {
            Range range = (Range)base.GetByIndex(index);
            return range;
        }
    }
}



public class TaskRangeIterator
{
    private RangeSortedList availableRanges;
    private RangeSortedList reservedRanges;
    const int CHUNK_LENGTH = 10;
    // expiration datetime delta
    TimeSpan EXPIRATION_DELTA = new TimeSpan(0, 30, 0);
    Range DEFAULT_RANGE = new Range(1, 100);

    public TaskRangeIterator()
    {
        availableRanges = new RangeSortedList();
        reservedRanges = new RangeSortedList();
        Reset();
    }

    public ReservedRange GetNext()
    {
        // free all reserved ranges that have expired
        FreeExpiredReservedRanges();

        // if there are no available ranges, throw an exception
        // otherwise, get the first available range

        if (availableRanges.Count == 0)
        {
            throw new Exception("No available ranges");
        }

        Range availableRange = availableRanges[0];

        // availableRange always has length of at least CHUNK_LENGTH
        DateTime reservedAt = DateTime.Now;
        ReservedRange reservedRange = new ReservedRange(availableRange.Start, availableRange.Start + CHUNK_LENGTH - 1, reservedAt);
        ReserveRange(reservedRange);

        return reservedRange;
    }

    public void ReserveRange(Range range)
    {
        // find the available range that contains the start of this range
        // if it exists, split it into two ranges
        // and add the removed range to the reserved ranges
        // if it doesn't exist, throw an exception

        int start = range.Start;
        int end = range.End;

        for (int i = 0; i < availableRanges.Count; i++)
        {
            Range availableRange = availableRanges[i];
            if (start >= availableRange.Start && start <= availableRange.End)
            {
                if (start == availableRange.Start && end == availableRange.End)
                {
                    availableRanges.RemoveAt(i);
                }
                else if (start == availableRange.Start)
                {
                    availableRanges.RemoveAt(i);
                    availableRanges.Add(end + 1, availableRange.End);
                }
                else if (end == availableRange.End)
                {
                    availableRanges.RemoveAt(i);
                    availableRanges.Add(availableRange.Start, start - 1);
                }
                else
                {
                    availableRanges.RemoveAt(i);
                    availableRanges.Add(availableRange.Start, start - 1);
                    availableRanges.Add(end + 1, availableRange.End);
                }

                DateTime reservedAt = DateTime.Now;
                ReservedRange reservedRange = new ReservedRange(start, end, reservedAt);
                reservedRanges.Add(reservedRange);
                return;
            }
        }
    }

    public void FreeExpiredReservedRanges()
    {
        // free all reserved ranges that have expired
        // if there are no reserved ranges, do nothing

        if (reservedRanges.Count == 0)
        {
            return;
        }

        DateTime now = DateTime.Now;
        for (int i = 0; i < reservedRanges.Count; i++)
        {
            ReservedRange reservedRange = (ReservedRange)reservedRanges[i];
            if (now - reservedRange.ReservedAt > EXPIRATION_DELTA)
            {
                FreeReservedRange(reservedRange);
            }
        }
    }

    public void FreeReservedRange(Range range)
    {
        // find the reserved range that contains the start of this range
        // if it exists, split it into two ranges
        // and add the removed range to the available ranges
        // if it doesn't exist, throw an exception
        // do it like above

        int start = range.Start;
        int end = range.End;

        for (int i = 0; i < reservedRanges.Count; i++)
        {
            Range reservedRange = reservedRanges[i];
            if (start >= reservedRange.Start && start <= reservedRange.End)
            {
                if (start == reservedRange.Start && end == reservedRange.End)
                {
                    reservedRanges.RemoveAt(i);
                }
                else if (start == reservedRange.Start)
                {
                    reservedRanges.RemoveAt(i);
                    reservedRanges.Add(end + 1, reservedRange.End);
                }
                else if (end == reservedRange.End)
                {
                    reservedRanges.RemoveAt(i);
                    reservedRanges.Add(reservedRange.Start, start - 1);
                }
                else
                {
                    reservedRanges.RemoveAt(i);
                    reservedRanges.Add(reservedRange.Start, start - 1);
                    reservedRanges.Add(end + 1, reservedRange.End);
                }

                availableRanges.Add(range);
                return;
            }
        }
    }

    public void RemoveReservedRange(Range range)
    {
        // find the reserved range that contains the start of this range
        // if it exists, split it into two ranges
        // and add the removed range to the available ranges
        // if it doesn't exist, throw an exception

        int start = range.Start;
        int end = range.End;

        for (int i = 0; i < reservedRanges.Count; i++)
        {
            Range reservedRange = reservedRanges[i];
            if (start >= reservedRange.Start && start <= reservedRange.End)
            {
                if (start == reservedRange.Start && end == reservedRange.End)
                {
                    reservedRanges.RemoveAt(i);
                }
                else if (start == reservedRange.Start)
                {
                    reservedRanges.RemoveAt(i);
                    reservedRanges.Add(end + 1, reservedRange.End);
                }
                else if (end == reservedRange.End)
                {
                    reservedRanges.RemoveAt(i);
                    reservedRanges.Add(reservedRange.Start, start - 1);
                }
                else
                {
                    reservedRanges.RemoveAt(i);
                    reservedRanges.Add(reservedRange.Start, start - 1);
                    reservedRanges.Add(end + 1, reservedRange.End);
                }

                return;
            }
        }
    }

    public void Reset()
    {
        availableRanges.Clear();
        reservedRanges.Clear();
        availableRanges.Add(DEFAULT_RANGE);
    }

    public void PrintState()
    {
        Console.WriteLine("Available ranges:");
        for (int i = 0; i < availableRanges.Count; i++)
        {
            Range range = availableRanges[i];
            Console.WriteLine("Start: " + range.Start + " End: " + range.End);
        }

        Console.WriteLine("Reserved ranges:");
        for (int i = 0; i < reservedRanges.Count; i++)
        {
            ReservedRange range = (ReservedRange)reservedRanges[i];
            Console.WriteLine("Start: " + range.Start + " End: " + range.End + " ReservedAt: " + range.ReservedAt);
        }
    }
}
