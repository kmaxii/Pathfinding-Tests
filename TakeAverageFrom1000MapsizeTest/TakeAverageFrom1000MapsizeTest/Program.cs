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
                List<double[]> allNumericValues = new List<double[]>(); // To hold all numeric values for averaging
                string mapSize = ""; // To store the current map size or identifier

                // Generate new file name for averages
                string directory = Path.GetDirectoryName(filePath);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string newFilePath = Path.Combine(directory, $"{filenameWithoutExtension}_average.csv");

                using (var writer = new StreamWriter(newFilePath))
                {
                    foreach (var line in lines)
                    {
                        // Check if the line is numeric or text
                        var stringValues = line.Split(',');
                        if (stringValues.All(sv => double.TryParse(sv, NumberStyles.Any, CultureInfo.InvariantCulture, out _)))
                        {
                            // It's a numeric row
                            double[] numericValues = new double[stringValues.Length];
                            mapSize = stringValues[0]; // Always take the first value from the row as the map size

                            for (int i = 1; i < stringValues.Length; i++) // Start from 1 to skip the map size column
                            {
                                if (!double.TryParse(stringValues[i], NumberStyles.Any, CultureInfo.InvariantCulture, out numericValues[i]))
                                {
                                    Console.WriteLine($"Warning: Unable to parse '{stringValues[i]}' to a double.");
                                    numericValues[i] = 0; // Assign a default value or handle appropriately
                                }
                            }

                            allNumericValues.Add(numericValues);
                            currentBatch++;
                        }
                        else
                        {
                            // It's a text row, write it as-is
                            writer.WriteLine(line);
                            continue;
                        }

                        // Check if we've reached 1000 rows or the end of the file
                        if (currentBatch == batchSize || allNumericValues.Count == lines.Length)
                        {
                            WriteAverages(allNumericValues, writer, mapSize); // Write averages for the current batch to the new file
                            allNumericValues.Clear(); // Reset for the next batch
                            currentBatch = 0; // Reset batch counter
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void WriteAverages(List<double[]> values, StreamWriter writer, string mapSize)
        {
            if (values.Count == 0) return;

            int numberOfColumns = values[0].Length;
            var averages = new List<string> { mapSize }; // Start with the map size
            for (int i = 1; i < numberOfColumns; i++) // Start from 1 to skip the map size column
            {
                double columnAverage = values.Average(row => row[i]);
                averages.Add(columnAverage.ToString(CultureInfo.InvariantCulture));
            }

            // Write the averages as a new line in the output file
            writer.WriteLine(string.Join(",", averages));
        }
    }
}
