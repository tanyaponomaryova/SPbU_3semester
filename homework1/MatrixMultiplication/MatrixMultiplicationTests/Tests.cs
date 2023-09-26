namespace MatrixMultiplicationTests
{
    public class Tests
    {
        [TestCase(-2, 0)]
        [TestCase(-2, -1)]
        [TestCase(-12, 90)]
        public void CreateRandomMatrixShouldThrowArgumentExceptionWhenNegativeOrZeroArguments(int rowsCount, int columnsCount)
        {
            Assert.Throws<ArgumentException>(() => MatrixMultiplication.Matrix.CreateRandomMatrix(rowsCount, columnsCount));
        }

        [TestCase(10, 20, 20, 13)]
        [TestCase(4, 8, 8, 7)]
        [TestCase(13, 40, 40, 2)]
        public void ParallelAndSequentialMultiolicationGiveSameResultsOnRandomMatrices(int firstRowsCount, int firstColumnsCount, int secondRowsCount, int secondColumnsCount)
        {
            var firstMatrix = MatrixMultiplication.Matrix.CreateRandomMatrix(firstRowsCount, firstColumnsCount);
            var secondMatrix = MatrixMultiplication.Matrix.CreateRandomMatrix(secondRowsCount, secondColumnsCount);

            var resultMatricSequential = MatrixMultiplication.Matrix.SequentialMatrixMultiplication(firstMatrix, secondMatrix);
            var resultMatricParallel = MatrixMultiplication.Matrix.ParallelMatrixMultiplication(firstMatrix, secondMatrix);

            Assert.IsTrue(MatrixMultiplication.Matrix.AreMatricesEqual(resultMatricParallel, resultMatricSequential));
        }

        [Test]
        public void GetMatrixFromFileWorksCorrectly()
        {
            int[,] expectedMatrix = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 }, { 10, 11, 12 } };
            var actualMatrix = MatrixMultiplication.Matrix.GetMatrixFromFile("../../../matrix4x3.txt");
            Assert.IsTrue(MatrixMultiplication.Matrix.AreMatricesEqual(expectedMatrix, actualMatrix));

            int[,] expectedMatrix2 = { { 5, 3, 12, 5, 16 }, { 12, 4, 3, 2, 1 }, { 8, 76, 12, 0, 4 } };
            actualMatrix = MatrixMultiplication.Matrix.GetMatrixFromFile("../../../matrix3x5.txt");
            Assert.IsTrue(MatrixMultiplication.Matrix.AreMatricesEqual(expectedMatrix2, actualMatrix));
        }
    }
}