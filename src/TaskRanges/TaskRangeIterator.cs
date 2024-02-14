using System;
using System.Collections;
using design_patterns.Utils;

namespace design_patterns.TaskRanges;

/// <summary>
/// TaskRangeIterator is a class that is used to iterate through the collection of task ranges.
/// It is used to find a range that is available.
/// </summary>
public abstract class TaskRangeIterator
{
    public TaskRangeCollection TaskRangeCollection { get; set; }

    /// <summary>
    /// GetNext returns the next available range.
    /// </summary>
    /// <returns></returns>
    public abstract TaskRange GetNext();
}


/// <summary>
/// Implements an iterator that iterates through the available ranges in a random order.
/// </summary>
public class RandomTaskRangeIterator : TaskRangeIterator
{
    public RandomTaskRangeIterator(TaskRangeCollection taskRangeCollection)
    {
        TaskRangeCollection = taskRangeCollection;
    }

    /// <summary>
    /// GetNext returns a random range from the available ranges
    /// </summary>
    /// <returns>
    /// A TaskRange object
    /// </returns>
    public override TaskRange GetNext()
    {
        if (TaskRangeCollection.AvailableRanges.Count == 0)
        {
            throw new Exception("No available ranges");
        }

        // get random available range
        Random random = new Random();
        int index = random.Next(TaskRangeCollection.AvailableRanges.Count);
        TaskRange availableRange = TaskRangeCollection.AvailableRanges[index];

        // availableRange always has length of at least CHUNK_LENGTH
        DateTime reservedAt = DateTime.Now;

        // reserve random chunk from the available range
        random = new Random();
        int nr_chunks = (int)(availableRange.End - availableRange.Start)/ProblemParameters.CHUNK_LENGTH;
        int start_chunk = random.Next(0, nr_chunks);
        ReservedRange reservedRange = new ReservedRange(availableRange.Start + start_chunk*ProblemParameters.CHUNK_LENGTH, availableRange.Start + (start_chunk+1)*ProblemParameters.CHUNK_LENGTH - 1, reservedAt);

        // iterate through all available ranges and sum up their length
        int totalLength = 0;
        for (int i = 0; i < TaskRangeCollection.AvailableRanges.Count; i++)
        {
            TaskRange range = TaskRangeCollection.AvailableRanges[i];
            totalLength += range.End - range.Start;
        }

        double percentage = (double)totalLength / (double)(ProblemParameters.DEFAULT_RANGE.End - ProblemParameters.DEFAULT_RANGE.Start);
        Console.WriteLine("Percentage of all combinations tried: " + ((1 - percentage)*100).ToString("F2") + "%");
        return reservedRange;
    }
}