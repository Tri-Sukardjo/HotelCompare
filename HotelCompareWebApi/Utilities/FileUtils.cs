using Microsoft.SemanticKernel.SemanticFunctions;
using System.Text.Json;

namespace HotelCompareWebApi.Utilities;

public class FileUtils
{
    public string ReadFromFile(string fileName)
    {
        string? result = null;

        try
        {
            using (var sr = new StreamReader(fileName))
            {
                result = sr.ReadToEnd();
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        return result;
    }

    public T ReadFromJSONFile<T>(string fileName)
    {
        string? result = ReadFromFile(fileName);
        return JsonSerializer.Deserialize<T>(result);
    }
}
