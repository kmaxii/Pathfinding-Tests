using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CSVRowAverage {
    class Program {
        // Flags to control which metrics to calculate and write
        static readonly bool calculateAverage = true;
        static readonly bool calculateMin = true;
        static readonly bool calculateMax = true;
        static readonly bool calculateDeviation = true;

        static void Main(string[] args) {
            string filePath =
                @"D:\Examensarbete\Pathfinding-Tests\TakeAverageFrom1000MapsizeTest\TakeAverageFrom1000MapsizeTest\bin\Debug\net5.0\DEMIANSDetectionData.csv"; // Update this path to your CSV file location
            CalculateAndWriteAverages(filePath);
        }

        static void CalculateAndWriteAverages(string filePath) {
            try {
                var lines = File.ReadAllLines(filePath);
                int batchSize = 1000; // Number of rows per map size
                List<double[]> allNumericValues = new List<double[]>(); // To hold all numeric values for processing

                // Generate new file name for averages
                string directory = Path.GetDirectoryName(filePath);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string newFilePath = Path.Combine(directory, $"{filenameWithoutExtension}_AVG_MIN_MAX_SDV.csv");

                using (var writer = new StreamWriter(newFilePath)) {
                    string[] headers = lines.FirstOrDefault()?.Split(',');
                    if (headers != null && headers.Length > 0) {
                        // Write headers for the new file, adjusting for selected metrics
                        WriteHeaders(writer, headers);
                    }

                    foreach (var line in lines.Skip(1)) // Skip header row
                    {
                        var stringValues = line.Split(',');
                        if (stringValues.All(sv => sv == stringValues.First() || double.TryParse(sv, NumberStyles.Any,
                                CultureInfo.InvariantCulture, out double parsedValue))) {
                            // It's a numeric row
                            double[] numericValues = stringValues.Select(val =>
                                double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out double num)
                                    ? num
                                    : double.NaN).ToArray();
                            allNumericValues.Add(numericValues);
                        }

                        // Check if we've reached 1000 rows or the end of the file
                        if (allNumericValues.Count == batchSize || lines.Last().Equals(line)) {
                            WriteMetrics(allNumericValues,
                                writer); // Write selected metrics for the current batch to the new file
                            allNumericValues.Clear(); // Reset for the next batch
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void WriteHeaders(StreamWriter writer, string[] headers) {
            var newHeaders = new List<string> {headers[0]}; // Keep the first header as is
            for (int i = 1; i < headers.Length; i++) {
                if (calculateAverage) newHeaders.Add($"AVG({headers[i]})");
                if (calculateMin) newHeaders.Add($"MIN({headers[i]})");
                if (calculateMax) newHeaders.Add($"MAX({headers[i]})");
                if (calculateDeviation) {
                    newHeaders.Add($"DEV_MIN({headers[i]})");
                    newHeaders.Add($"DEV_MAX({headers[i]})");
                }
            }

            writer.WriteLine(string.Join(",", newHeaders));
        }

        static void WriteMetrics(List<double[]> values, StreamWriter writer) {
            if (values.Count == 0) return;

            int numberOfColumns = values[0].Length;
            var resultRow = new List<string> {
                values.Select(v => v[0]).FirstOrDefault().ToString(CultureInfo.InvariantCulture)
            }; // First column value (map size or identifier)
            for (int i = 1; i < numberOfColumns; i++) {
                var columnValues = values.Select(row => row[i]).Where(val => !double.IsNaN(val) && val > 0).ToArray();
                if (columnValues.Length > 0) {
                    double average = columnValues.Average();
                    double stdDeviation = CalculateStandardDeviation(columnValues);
                    double deviationMin = Math.Max(0, average - stdDeviation); // Ensure min deviation is not negative
                    double deviationMax =
                        average + stdDeviation +
                        (average - stdDeviation - deviationMin); // Adjust max deviation accordingly

                    if (calculateAverage) resultRow.Add(average.ToString(CultureInfo.InvariantCulture));
                    if (calculateMin) resultRow.Add(columnValues.Min().ToString(CultureInfo.InvariantCulture));
                    if (calculateMax) resultRow.Add(columnValues.Max().ToString(CultureInfo.InvariantCulture));
                    if (calculateDeviation) {
                        resultRow.Add(deviationMin.ToString(CultureInfo.InvariantCulture)); // Adjusted Deviation Min
                        resultRow.Add(deviationMax.ToString(CultureInfo.InvariantCulture)); // Adjusted Deviation Max
                    }
                }
                else {
                    // If no valid data, write placeholders based on selected metrics
                    if (calculateAverage) resultRow.Add("NaN");
                    if (calculateMin) resultRow.Add("NaN");
                    if (calculateMax) resultRow.Add("NaN");
                    if (calculateDeviation) {
                        resultRow.Add("NaN"); // Deviation Min
                        resultRow.Add("NaN"); // Deviation Max
                    }
                }
            }

            writer.WriteLine(string.Join(",", resultRow));
        }


        public static double CalculateStandardDeviation(IEnumerable<double> values) {
            double[] validValues = values.Where(val => !double.IsNaN(val)).ToArray();
            if (validValues.Length <= 1) {
                return 0; // Standard deviation is not defined for 0 or 1 value
            }

            double average = validValues.Average();
            double sumOfSquaresOfDifferences = validValues.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation =
                Math.Sqrt(sumOfSquaresOfDifferences /
                          (validValues.Length - 1)); // Using sample standard deviation formula
            return standardDeviation;
        }
    }
}