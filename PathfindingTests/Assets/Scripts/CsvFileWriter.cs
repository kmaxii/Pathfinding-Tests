using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class CsvFileWriter
{
    private const string FileName = "DataLog.csv";

    private static readonly List<string> Headers = new List<string>
        {"Sphere Count"};


    private static readonly string PathToFile =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), FileName);


    public static void WriteHeaders(List<DetectionBehaviour.DetectionBehaviour> detectionBehaviour)
    {
        
        Headers.AddRange(detectionBehaviour.Select((behaviour => behaviour.name)));
        WriteToFile(Headers);
    }

    public static void WriteToFile(List<string> elements)
    {
        
        File.AppendAllText(PathToFile, ListToCsvString(elements) + Environment.NewLine);
    }


    public static void CreateNewFile()
    {
        File.Create(PathToFile).Close();
    }


    private static string ListToCsvString(List<string> inputList)
    {
        string toBeReturned = "";

        for (var i = 0; i < inputList.Count; i++)
        {
            toBeReturned += inputList[i] + ",";
        }

        //Remove last comma
        return toBeReturned.Remove(toBeReturned.Length - 1, 1);
    }
}