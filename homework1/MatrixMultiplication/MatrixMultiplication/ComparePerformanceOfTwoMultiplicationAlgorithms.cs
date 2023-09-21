using System.Diagnostics;

namespace MatrixMultiplication
{
    internal class ComparePerformanceOfTwoMultiplicationAlgorithms
    {
        public static (double mathExpectationSequental, double mathExpectationParallel,
                double standartDeviationSequental, double standartDeviationParallel)
                MakeComparisonOnGivenMatrices(int numberOfTestRuns, int[,] firstMatrix, int[,] secondMatrix)
        {
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

        public static void ExperimentOnRandomMatricesOfDifferentSizes(int numberOfTestRuns, string filePathForResultTable)
        {
            using var streamWriter = new StreamWriter(filePathForResultTable);
            streamWriter.WriteLine("{0,11} {1,15} {2,15} {3,15} {4,15}", "Size:", "E (parallel):", "σ (parallel):", "E (sequential):", "σ (sequential):");
            for (int size = 100; size <= 500; size += 100)
            {
                var firstMatrix = Matrix.CreateRandomMatrix(size, size);
                var secondMatrix = Matrix.CreateRandomMatrix(size, size);
                (double mathExpectationSequental, double mathExpectationParallel,
                double standartDeviationSequental, double standartDeviationParallel) = MakeComparisonOnGivenMatrices(numberOfTestRuns, firstMatrix, secondMatrix);
                streamWriter.WriteLine(String.Format("{0,11} {1,15} {2,15} {3,15} {4,15}", size.ToString() + " x " + size.ToString(),
                    Math.Round(mathExpectationParallel, 5) + " sec", Math.Round(standartDeviationParallel, 5) + " sec",
                    Math.Round(mathExpectationSequental, 5) + " sec", Math.Round(standartDeviationSequental, 5) + " sec"));
            }
        }
    }
}