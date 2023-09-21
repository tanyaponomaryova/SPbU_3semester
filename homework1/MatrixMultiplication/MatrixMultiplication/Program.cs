using MatrixMultiplication;

Console.WriteLine("This program performs multiplication of two matrices given in files \"firstMatrix.txt\" and \"secondMatrix.txt\".");
var firstMatrix = Matrix.GetMatrixFromFile("../../../firstMatrix.txt");
var secondMatrix = Matrix.GetMatrixFromFile("../../../secondMatrix.txt");
var resultMatrixSeq = Matrix.SequentialMatrixMultiplication(firstMatrix, secondMatrix);
var resultMatrixPar = Matrix.ParallelMatrixMultiplication(firstMatrix, secondMatrix);
if (!Matrix.AreMatricesEqual(resultMatrixPar, resultMatrixSeq))
{
    Console.WriteLine("Sorry, multiplication went wrong.");
    return;
}
Matrix.WriteResultMatrixToNewFile(resultMatrixPar, "../../../resultMatrix.txt");
Console.WriteLine("Result of multiplication was written to file \"resultMatrix.txt\".");
Console.WriteLine("To run a comparison of sequential and parallel multiplication, enter the number of runs for each test case, to exit press any other key.");

if (int.TryParse(Console.ReadLine(), out int numberOfTetsRuns))
{
    ComparePerformanceOfTwoMultiplicationAlgorithms.ExperimentOnRandomMatricesOfDifferentSizes(numberOfTetsRuns, "../../../experimentResults.txt");
    Console.WriteLine("Results of experiment was written to file \"experimentResults.txt\".");
}
return;