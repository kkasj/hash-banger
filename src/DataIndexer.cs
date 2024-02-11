public class DataIndexer
{
    const int DATA_LENGTH = 4;

    public static string FromIndex(int index)
    {
        // convert index to data string
        string data = "";

        for (int i = 0; i < DATA_LENGTH; i++)
        {
            data = (char)('a' + index % 26) + data;
            index /= 26;
        }

        return data;
    }

    public static int ToIndex(string data)
    {
        // convert data string to index
        // inverse of FromIndex
        int index = 0;

        for (int i = 0; i < DATA_LENGTH; i++)
        {
            index *= 26;
            index += data[i] - 'a';
        }

        return index;
    }
}