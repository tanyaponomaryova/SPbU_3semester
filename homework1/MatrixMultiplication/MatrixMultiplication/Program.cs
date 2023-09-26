using MatrixMultiplication;

Console.WriteLine("This program performs multiplication of " +
    "two matrices given in files \"firstMatrix.txt\" and \"secondMatrix.txt\".");

try
{
    var firstMatrix = Matrix.GetMatrixFromFile("../../../firstMatrix.txt");
    var secondMatrix = Matrix.GetMatrixFromFile("../../../secondMatrix.txt");
    var resultMatrixSeq = Matrix.SequentialMatrixMultiplication(firstMatrix, secondMatrix);
    var resultMatrixPar = Matrix.ParallelMatrixMultiplication(firstMatrix, secondMatrix);

    if (!Matrix.AreMatricesEqual(resultMatrixPar, resultMatrixSeq))
    {
        Console.WriteLine("Sorry, multiplication went wrong.");
        return;
    }

    Matrix.WriteMatrixToFile(resultMatrixPar, "../../../resultMatrix.txt");
    Console.WriteLine("Result of multiplication was written to file \"resultMatrix.txt\".");
    Console.WriteLine("To run a comparison of sequential and parallel multiplication, " +
        "enter the number of runs for each test case, to exit press any other key.");

}
catch (Exception e)
{
    Console.WriteLine("An error occurred during program execution:");
    Console.WriteLine(e.Message);
    return;
}

if (int.TryParse(Console.ReadLine(), out int numberOfTestRuns))
{
    try
    {
        ComparePerformanceOfTwoMultiplicationAlgorithms.ExperimentOnRandomMatricesOfDifferentSizes(numberOfTestRuns, "../../../experimentResults.txt");
        Console.WriteLine("Results of experiment was written to file \"experimentResults.txt\".");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}