using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CSVRowAverage
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"D:\Examensarbete\Pathfinding-Tests\TakeAverageFrom1000MapsizeTest\TakeAverageFrom1000MapsizeTest\bin\Debug\net5.0\DEMIANSDetectionData.csv"; // Update this path to your CSV file location
            CalculateAndWriteAverages(filePath);
        }

        static void CalculateAndWriteAverages(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                int batchSize = 1000; // Number of rows per map size
                int currentBatch = 0;
                List<double[]> allNumericValues = new List<double[]>(); // To hold all numeric values for processing
                bool isFirstBatch = true; // To track if we're processing the first batch

                // Generate new file name for averages
                string directory = Path.GetDirectoryName(filePath);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string newFilePath = Path.Combine(directory, $"{filenameWithoutExtension}_average.csv");

                using (var writer = new StreamWriter(newFilePath))
                {
                    string[] headers = lines.FirstOrDefault()?.Split(',');
                    if (headers != null && headers.Length > 0)
                    {
                        // Write headers for the new file, adjusting for average, min, max
                        WriteHeaders(writer, headers);
                    }

                    foreach (var line in lines.Skip(1)) // Skip header row
                    {
                        var stringValues = line.Split(',');
                        if (stringValues.All(sv => sv == stringValues.First() || double.TryParse(sv, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue)))
                        {
                            // It's a numeric row
                            double[] numericValues = stringValues.Select(val => double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out double num) ? num : double.NaN).ToArray();
                            allNumericValues.Add(numericValues);
                            currentBatch++;
                        }

                        // Check if we've reached 1000 rows or the end of the file
                        if (currentBatch == batchSize || lines.Last().Equals(line))
                        {
                            WriteAverages(allNumericValues, writer); // Write averages, min, max for the current batch to the new file
                            allNumericValues.Clear(); // Reset for the next batch
                            currentBatch = 0; // Reset batch counter
                            isFirstBatch = false; // Update the flag as the first batch has been processed
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void WriteHeaders(StreamWriter writer, string[] headers)
        {
            // Assuming the first column does not require avg, min, max
            var newHeaders = new List<string> { headers[0] }; // Keep the first header as is
            for (int i = 1; i < headers.Length; i++)
            {
                newHeaders.Add($"Avg({headers[i]})");
                newHeaders.Add($"Min({headers[i]})");
                newHeaders.Add($"Max({headers[i]})");
            }
            writer.WriteLine(string.Join(",", newHeaders));
        }

        static void WriteAverages(List<double[]> values, StreamWriter writer)
        {
            if (values.Count == 0) return;

            int numberOfColumns = values[0].Length;
            var resultRow = new List<string> { values.Select(v => v[0]).FirstOrDefault().ToString(CultureInfo.InvariantCulture) }; // First column value (map size or identifier)
            for (int i = 1; i < numberOfColumns; i++)
            {
                var columnValues = values.Select(row => row[i]).Where(val => !double.IsNaN(val) && val != 0).ToArray();
                if (columnValues.Length > 0)
                {
                    resultRow.Add(columnValues.Average().ToString(CultureInfo.InvariantCulture));
                    resultRow.Add(columnValues.Min().ToString(CultureInfo.InvariantCulture));
                    resultRow.Add(columnValues.Max().ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    // If all values are zero or NaN, write NaN or a placeholder to indicate no valid data was found
                    resultRow.Add("NaN");
                    resultRow.Add("NaN");
                    resultRow.Add("NaN");
                }
            }

            // Write the result row for averages, mins, and maxes
            writer.WriteLine(string.Join(",", resultRow));
        }
    }
}
