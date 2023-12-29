using System.Diagnostics;

namespace MatrixMultiplication
{
    /// <summary>
    /// Class for comparing sequential and parallel matrix multiplication.
    /// </summary>
    public class ComparisonPerformanceOfTwoMultiplicationAlgorithms
    {
        /// <summary>
        /// Conducts the specified number of test runs of each multiplication algorithm and returns statistics on them.
        /// </summary>
        /// <param name="numberOfTestRuns"></param>
        /// <param name="firstMatrix">first matrix as a two-dimensional array.</param>
        /// <param name="secondMatrix">second matrix as a two-dimensional array.</param>
        /// <returns>a tuple containing the mathematical expectation and standard deviation 
        /// for parallel multiplication and the mathematical expectation and standard deviation 
        /// for sequential multiplication.</returns>
        /// <exception cref="ArgumentException">if number of test runs are negative or zero.</exception>
        public static (double mathExpectationSequental, double mathExpectationParallel,
                double standartDeviationSequental, double standartDeviationParallel)
                CompareOnGivenMatrices(int numberOfTestRuns, int[,] firstMatrix, int[,] secondMatrix)
        {
            if (numberOfTestRuns <= 0)
            {
                throw new ArgumentException("The number of test runs must be a positive number.");
            }

            double mathExpectationSequental = 0;
            double mathExpectationParallel = 0;
            double standartDeviationSequental = 0;
            double standartDeviationParallel = 0;

            Stopwatch stopwatch = new Stopwatch();

            for (int i = 0; i < numberOfTestRuns; i++)
            {
                stopwatch.Start();
                Matrix.SequentialMatrixMultiplication(firstMatrix, secondMatrix);
                stopwatch.Stop();
                mathExpectationSequental += stopwatch.Elapsed.TotalSeconds;
                standartDeviationSequental += Math.Pow(stopwatch.Elapsed.TotalSeconds, 2);
                stopwatch.Reset();

                stopwatch.Start();
                Matrix.ParallelMatrixMultiplication(firstMatrix, secondMatrix);
                stopwatch.Stop();
                mathExpectationParallel += stopwatch.Elapsed.TotalSeconds;
                standartDeviationParallel += Math.Pow(stopwatch.Elapsed.TotalSeconds, 2);
                stopwatch.Reset();
            }

            mathExpectationSequental /= numberOfTestRuns;
            mathExpectationParallel /= numberOfTestRuns;
            standartDeviationSequental = Math.Sqrt(standartDeviationSequental / numberOfTestRuns - Math.Pow(mathExpectationSequental, 2));
            standartDeviationParallel = Math.Sqrt(standartDeviationParallel / numberOfTestRuns - Math.Pow(mathExpectationParallel, 2));

            return (mathExpectationSequental, mathExpectationParallel,
                 standartDeviationSequental, standartDeviationParallel);
        }

        /// <summary>
        /// Сonducts an experiment on sequential and parallel matrix
        /// multiplication and also writes the results to a file.
        /// </summary>
        /// <param name="numberOfTestRuns"></param>
        /// <param name="filePathForResultTable"></param>
        public static void RunExperiment(int numberOfTestRuns, string filePathForResultTable)
        {
            using var streamWriter = new StreamWriter(filePathForResultTable);
            streamWriter.WriteLine("{0,11} {1,15} {2,15} {3,15} {4,15}",
                "Size:", "E (parallel):", "σ (parallel):", "E (sequential):", "σ (sequential):");
            for (int size = 100; size <= 500; size += 100)
            {
                var firstMatrix = Matrix.CreateRandomMatrix(size, size);
                var secondMatrix = Matrix.CreateRandomMatrix(size, size);
                (double mathExpectationSequental, double mathExpectationParallel,
                double standartDeviationSequental, double standartDeviationParallel) = CompareOnGivenMatrices(numberOfTestRuns, firstMatrix, secondMatrix);
                streamWriter.WriteLine(String.Format("{0,11} {1,15} {2,15} {3,15} {4,15}", size.ToString() + " x " + size.ToString(),
                    Math.Round(mathExpectationParallel, 5) + " sec", Math.Round(standartDeviationParallel, 5) + " sec",
                    Math.Round(mathExpectationSequental, 5) + " sec", Math.Round(standartDeviationSequental, 5) + " sec"));
            }
        }
    }
}