using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class EnhancedCsvFileWriter
{
    private static string _filePath;
    private static List<string> _headers = new List<string>();
    private static List<List<string>> _rows = new List<List<string>>();

    public static void Initialize(string fileName, List<string> headers)
    {
        _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        _headers = headers;
        _rows.Clear(); // Ensure that previous data is cleared when re-initializing.

        // Write the header row immediately to the file.
        WriteRow(_headers, true);
    }

    public static void AddRow(List<string> row)
    {
        if(row.Count != _headers.Count)
        {
            Debug.LogError("Row column count does not match header column count. Row not added.");
            return;
        }

        _rows.Add(row);
    }

    // Call this method to write all accumulated rows to the CSV file.
    public static void FlushDataToFile()
    {
        foreach (var row in _rows)
        {
            WriteRow(row, false);
        }

        // Clear the rows after writing to file to avoid duplicate data on next flush.
        _rows.Clear();
    }

    private static void WriteRow(List<string> row, bool isHeader)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < row.Count; i++)
        {
            sb.Append(row[i]);
            if (i < row.Count - 1)
                sb.Append(","); // Delimit columns with comma
        }
        sb.AppendLine(); // New line at the end of the row

        try
        {
            // Append to the file if it's row data, overwrite if it's the header.
            File.AppendAllText(_filePath, sb.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to CSV file: {e.Message}");
        }
    }
}