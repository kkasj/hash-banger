using design_patterns.Utils;

/// <summary>
/// This class is responsible for converting between data strings and their corresponding indices.
/// </summary>
public class DataIndexer
{
    /// <summary>
    /// Converts an index to a data string.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string FromIndex(int index)
    {
        // convert index to data string
        string data = "";

        for (int i = 0; i < ProblemParameters.DATA_LENGTH; i++)
        {
            data = (char)('a' + index % 26) + data;
            index /= 26;
        }

        return data;
    }

    /// <summary>
    /// Converts a data string to an index.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static int ToIndex(string data)
    {
        // convert data string to index
        // inverse of FromIndex
        int index = 0;

        for (int i = 0; i < ProblemParameters.DATA_LENGTH; i++)
        {
            index *= 26;
            index += data[i] - 'a';
        }

        return index;
    }
}