using System;
using System.Collections;
using design_patterns.Utils;

namespace design_patterns.TaskRanges;

/// <summary>
/// TaskRangeIterator is a class that is used to iterate through a range of numbers
/// and reserve a chunk of numbers for a certain amount of time
/// </summary>
public class TaskRangeCollection
{
    public RangeSortedList AvailableRanges { get; set; }
    public RangeSortedList ReservedRanges { get; set; }

    public List<TaskRange> SerializableAvailableRanges
    {
        get => new List<TaskRange>(AvailableRanges.Values);
        set
        {
            AvailableRanges.Clear();
            foreach (var range in value)
            {
                AvailableRanges.Add(range);
            }
        }
    }

    public List<ReservedRange> SerializableReservedRanges
    {
        get => new List<ReservedRange>(ReservedRanges.Values.Cast<ReservedRange>());
        set
        {
            ReservedRanges.Clear();
            foreach (var range in value)
            {
                ReservedRanges.Add(range);
            }
        }
    }

    public TaskRangeCollection()
    {
        AvailableRanges = new RangeSortedList();
        ReservedRanges = new RangeSortedList();
        Reset();
    }

    /// <summary>
    /// GetNext returns a random range from the available ranges
    /// </summary>
    /// <returns>
    /// A ReservedRange object
    /// </returns>
    public ReservedRange GetNext()
    {
        // free all reserved ranges that have expired
        FreeExpiredReservedRanges();

        // if there are no available ranges, throw an exception
        // otherwise, get the first available range

        if (AvailableRanges.Count == 0)
        {
            throw new Exception("No available ranges");
        }

        // get random available range
        Random random = new Random();
        int index = random.Next(AvailableRanges.Count);
        TaskRange availableRange = AvailableRanges[index];

        // availableRange always has length of at least CHUNK_LENGTH
        DateTime reservedAt = DateTime.Now;

        // reserve random chunk from the available range
        random = new Random();
        int nr_chunks = (int)(availableRange.End - availableRange.Start)/ProblemParameters.CHUNK_LENGTH;
        int start_chunk = random.Next(0, nr_chunks);
        ReservedRange reservedRange = new ReservedRange(availableRange.Start + start_chunk*ProblemParameters.CHUNK_LENGTH, availableRange.Start + (start_chunk+1)*ProblemParameters.CHUNK_LENGTH - 1, reservedAt);

        // iterate through all available ranges and sum up their length
        int totalLength = 0;
        for (int i = 0; i < AvailableRanges.Count; i++)
        {
            TaskRange range = AvailableRanges[i];
            totalLength += range.End - range.Start;
        }

        double percentage = (double)totalLength / (double)(ProblemParameters.DEFAULT_RANGE.End - ProblemParameters.DEFAULT_RANGE.Start);
        Console.WriteLine("Percentage of all combinations tried: " + ((1 - percentage)*100).ToString("F2") + "%");
        return reservedRange;
    }

    /// <summary>
    /// MergeRanges merges all ranges that are adjacent to each other in order to save memory
    /// </summary>
    public void MergeRanges()
    {
        // merge all ranges that are adjacent to each other

        for (int i = 0; i < AvailableRanges.Count - 1; i++)
        {
            TaskRange range1 = AvailableRanges[i];
            TaskRange range2 = AvailableRanges[i + 1];
            if (range1.End + 1 == range2.Start)
            {
                AvailableRanges.RemoveAt(i);
                AvailableRanges.RemoveAt(i);
                AvailableRanges.Add(range1.Start, range2.End);
                i--;
            }
        }
        
        for (int i = 0; i < ReservedRanges.Count - 1; i++)
        {
            ReservedRange range1 = (ReservedRange)ReservedRanges[i];
            ReservedRange range2 = (ReservedRange)ReservedRanges[i + 1];
            if (range1.End + 1 == range2.Start && range1.ReservedAt == range2.ReservedAt)
            {
                ReservedRange newReservedRange = new ReservedRange(range1.Start, range2.End, range1.ReservedAt);
                ReservedRanges.RemoveAt(i);
                ReservedRanges.RemoveAt(i);
                ReservedRanges.Add(newReservedRange);
                i--;
            }
        }
    }

    /// <summary>
    /// ReserveRange reserves a range of numbers for a certain amount of time
    /// </summary>
    /// <param name="range">
    /// A Range object
    /// </param>
    public void ReserveRange(TaskRange range)
    {
        // find the available range that contains the start of this range
        // if it exists, split it into two ranges
        // and add the removed range to the reserved ranges
        // if it doesn't exist, throw an exception

        int start = range.Start;
        int end = range.End;

        for (int i = 0; i < AvailableRanges.Count; i++)
        {
            TaskRange availableRange = AvailableRanges[i];
            if (start >= availableRange.Start && start <= availableRange.End)
            {
                if (start == availableRange.Start && end == availableRange.End)
                {
                    AvailableRanges.RemoveAt(i);
                }
                else if (start == availableRange.Start)
                {
                    AvailableRanges.RemoveAt(i);
                    AvailableRanges.Add(end + 1, availableRange.End);
                }
                else if (end == availableRange.End)
                {
                    AvailableRanges.RemoveAt(i);
                    AvailableRanges.Add(availableRange.Start, start - 1);
                }
                else
                {
                    AvailableRanges.RemoveAt(i);
                    AvailableRanges.Add(availableRange.Start, start - 1);
                    AvailableRanges.Add(end + 1, availableRange.End);
                }

                DateTime reservedAt = DateTime.Now;
                ReservedRange reservedRange = new ReservedRange(start, end, reservedAt);
                ReservedRanges.Add(reservedRange);


                MergeRanges();
                return;
            }
        }
    }

    /// <summary>
    /// FreeExpiredReservedRanges frees all reserved ranges that have expired
    /// </summary>
    public void FreeExpiredReservedRanges()
    {
        // free all reserved ranges that have expired
        // if there are no reserved ranges, do nothing

        if (ReservedRanges.Count == 0)
        {
            return;
        }

        List<ReservedRange> toRemove = new List<ReservedRange>();

        DateTime now = DateTime.Now;
        for (int i = 0; i < ReservedRanges.Count; i++)
        {
            ReservedRange reservedRange = (ReservedRange)ReservedRanges[i];
            if (now - reservedRange.ReservedAt > ProblemParameters.EXPIRATION_DELTA)
            {
                toRemove.Add(reservedRange);
            }
        }

        foreach (ReservedRange reservedRange in toRemove)
        {
            FreeReservedRange(reservedRange);
        }
    }

    /// <summary>
    /// FreeReservedRange frees a reserved range and adds it to the available ranges
    /// </summary>
    public void FreeReservedRange(TaskRange range)
    {
        // find the reserved range that contains the start of this range
        // if it exists, split it into two ranges
        // and add the removed range to the available ranges
        // if it doesn't exist, throw an exception
        // do it like above

        int start = range.Start;
        int end = range.End;

        for (int i = 0; i < ReservedRanges.Count; i++)
        {
            ReservedRange reservedRange = (ReservedRange)ReservedRanges[i];
            if (start >= reservedRange.Start && start <= reservedRange.End)
            {
                if (start == reservedRange.Start && end == reservedRange.End)
                {
                    ReservedRanges.RemoveAt(i);
                }
                else if (start == reservedRange.Start)
                {
                    ReservedRanges.RemoveAt(i);
                    ReservedRange newReservedRange = new ReservedRange(end + 1, reservedRange.End, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange);
                }
                else if (end == reservedRange.End)
                {
                    ReservedRanges.RemoveAt(i);
                    ReservedRange newReservedRange = new ReservedRange(reservedRange.Start, start - 1, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange);
                }
                else
                {
                    ReservedRanges.RemoveAt(i);
                    ReservedRange newReservedRange1 = new ReservedRange(reservedRange.Start, start - 1, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange1);
                    ReservedRange newReservedRange2 = new ReservedRange(end + 1, reservedRange.End, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange2);
                }

                AvailableRanges.Add(range);

                MergeRanges();
                return;
            }
        }
    }

    /// <summary>
    /// RemoveReservedRange removes a reserved range when it was successfully processed
    /// </summary>
    public void RemoveReservedRange(TaskRange range)
    {
        // find the reserved range that contains the start of this range
        // if it exists, split it into two ranges
        // and add the removed range to the available ranges
        // if it doesn't exist, throw an exception

        int start = range.Start;
        int end = range.End;

        for (int i = 0; i < ReservedRanges.Count; i++)
        {
            ReservedRange reservedRange = (ReservedRange)ReservedRanges[i];
            if (start >= reservedRange.Start && start <= reservedRange.End)
            {
                if (start == reservedRange.Start && end == reservedRange.End)
                {
                    ReservedRanges.RemoveAt(i);
                }
                else if (start == reservedRange.Start)
                {
                    ReservedRanges.RemoveAt(i);
                    ReservedRange newReservedRange = new ReservedRange(end + 1, reservedRange.End, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange);
                }
                else if (end == reservedRange.End)
                {
                    ReservedRanges.RemoveAt(i);
                    ReservedRange newReservedRange = new ReservedRange(reservedRange.Start, start - 1, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange);
                }
                else
                {
                    ReservedRanges.RemoveAt(i);
                    ReservedRange newReservedRange1 = new ReservedRange(reservedRange.Start, start - 1, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange1);
                    ReservedRange newReservedRange2 = new ReservedRange(end + 1, reservedRange.End, reservedRange.ReservedAt);
                    ReservedRanges.Add(newReservedRange2);
                }
                
                MergeRanges();
                return;
            }
        }
    }

    /// <summary>
    /// Reset resets the iterator state
    /// </summary>
    public void Reset()
    {
        AvailableRanges.Clear();
        ReservedRanges.Clear();
        AvailableRanges.Add(ProblemParameters.DEFAULT_RANGE);
    }

    public void PrintState()
    {
        Console.WriteLine("Available ranges:");
        for (int i = 0; i < AvailableRanges.Count; i++)
        {
            TaskRange range = AvailableRanges[i];
            Console.WriteLine("Start: " + range.Start + " End: " + range.End);
        }

        Console.WriteLine("Reserved ranges:");
        for (int i = 0; i < ReservedRanges.Count; i++)
        {
            ReservedRange range = (ReservedRange)ReservedRanges[i];
            Console.WriteLine("Start: " + range.Start + " End: " + range.End + " ReservedAt: " + range.ReservedAt);
        }
    }
}
