namespace MatrixMultiplication
{
    public class Matrix
    {
        public static int[,] GetMatrixFromFile(string filePath)
        {
            var linesArray = File.ReadAllLines(filePath);
            var line = linesArray[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int rowsCount = linesArray.Length;
            int columnsCount = line.Length;
            int[,] matrix = new int[rowsCount, columnsCount];

            for (int i = 0; i < line.Length; i++)
            {
                if (int.TryParse(line[i], out int number))
                {
                    matrix[0, i] = number;
                }
                else
                {
                    throw new ArgumentException("Matrix cannot contain anything other than numbers.");
                }
            }

            for (int i = 1; i < linesArray.Length; i++)
            {
                line = linesArray[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length != columnsCount)
                {
                    throw new ArgumentException("The rows of the matrix do not have the same number of elements.");
                }

                for (int j = 0; j < columnsCount; j++)
                {
                    if (int.TryParse(line[j], out int number))
                    {
                        matrix[i, j] = number;
                    }
                    else
                    {
                        throw new ArgumentException("Matrix cannot contain anything other than numbers.");
                    }
                }
            }

            return matrix;
        }

        private static bool CheckConsistencyOfMatrixDimensions(int[,] firstMatrix, int[,] secondMatrix)
        {
            return firstMatrix.GetLength(1) == secondMatrix.GetLength(0);
        }

        public static int[,] SequentialMatrixMultiplication(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (!CheckConsistencyOfMatrixDimensions(firstMatrix, secondMatrix))
            {
                throw new ArgumentException("Matrices do not correspond to each other's dimensions.");
            }

            var resultMatrix = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];
            for (int i = 0; i < firstMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < secondMatrix.GetLength(1); j++)
                {
                    for (int k = 0; k < firstMatrix.GetLength(1); k++)
                    {
                        int firstElement = firstMatrix[i, k];
                        int secondElement = secondMatrix[k, j];

                        resultMatrix[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                    }
                }
            }

            return resultMatrix;
        }

        public static int[,] ParallelMatrixMultiplication(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (!CheckConsistencyOfMatrixDimensions(firstMatrix, secondMatrix))
            {
                throw new ArgumentException("Matrices do not correspond to each other's dimensions.");
            }

            var resultMatrix = new int[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];
            var threadsArray = new Thread[Environment.ProcessorCount];
            var matrixElementsPerThread = resultMatrix.GetLength(0) * resultMatrix.GetLength(1) / threadsArray.Length + 1;
            for (int i = 0; i < threadsArray.Length; i++)
            {
                var localI = i;
                threadsArray[i] = new Thread(() =>
                {
                    for (int j = localI * matrixElementsPerThread; j < (localI + 1) * matrixElementsPerThread && j < resultMatrix.Length; j++)
                    {
                        int rowOfCurrentElement = j / resultMatrix.GetLength(1); // находим ряд, в котором находится элемент, который сейчас будем считать
                        int columnOfCurrentElement = j % resultMatrix.GetLength(1); // находим столбец

                        for (int k = 0; k < firstMatrix.GetLength(1); k++)
                        {
                            resultMatrix[rowOfCurrentElement, columnOfCurrentElement] += firstMatrix[rowOfCurrentElement, k] * secondMatrix[k, columnOfCurrentElement];
                        }
                    }
                });
            }

            foreach (var thread in threadsArray)
            {
                thread.Start();
            }

            foreach (var thread in threadsArray)
            {
                thread.Join();
            }

            return resultMatrix;
        }

        public static void WriteResultMatrixToNewFile(int[,] matrix, string filePath)
        {
            using var streamWriter = new StreamWriter(filePath);
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    streamWriter.Write(matrix[i, j] + " ");
                }
                streamWriter.WriteLine();
            }
        }

        public static int[,] CreateRandomMatrix(int rowsCount, int columnsCount)
        {
            if (rowsCount <= 0 || columnsCount <= 0)
            {
                throw new ArgumentException("Matrix dimensions can not be negative or zero.");
            }

            var random = new Random();
            var resultMatrix = new int[rowsCount, columnsCount];
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    resultMatrix[i, j] = random.Next(-1000, 1000);
                }
            }

            return resultMatrix;
        }

        public static void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }


        public static bool AreMatricesEqual(int[,] firstMatrix, int[,] secondMatrix)
        {
            if (firstMatrix.GetLength(0) != secondMatrix.GetLength(0) || firstMatrix.GetLength(1) != secondMatrix.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < firstMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < firstMatrix.GetLength(1); j++)
                {
                    if (firstMatrix[i, j] != secondMatrix[i, j])
                    {
                        return false;
                    }
                }

            }

            return true;
        }
    }
}