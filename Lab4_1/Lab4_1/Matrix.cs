using System.ComponentModel.DataAnnotations;

namespace Lab4 {
    internal class Matrix {
        int m; // количество строк
        int n; // количество столбцов
        int[][] data; // значения

        public static int[] GetErrorVector(int length, int maxIndex) {
            Random rand = new Random();
            int[] arr = new int[length];

            int index = rand.Next(0, maxIndex);

            for (int i = 0; i < length; i++) {
                arr[i] = 0;
            }
            arr[index] = 1;

            return arr;
        }

        public static int[][] MatrixCreate(int rows, int cols) {
            // Создаем матрицу, полностью инициализированную
            // значениями 0.0. Проверка входных параметров опущена.
            int[][] result = new int[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new int[cols]; // автоинициализация в 0.0
            return result;
        }
        public static int[][] MatrixCreatePermutations(int rows, int cols) {
            // Создаем матрицу, полностью инициализированную
            // значениями 0.0. Проверка входных параметров опущена.
            Random rand = new Random();

            int[][] result = new int[rows][];
            List<int> indexes = new List<int>();

            for (int i = 0; i < rows; ++i) {
                result[i] = new int[cols]; // автоинициализация в 0.0
            }

            for (int i = 0; i < rows; i++) {
                int index = rand.Next(cols);
                while (indexes.Contains(index)) {
                    index = rand.Next(cols);
                }
                result[i][index] = 1;
                indexes.Add(index);
            }

            return result;
        }
        public static int[][] MatrixProduct(int[][] matrixA,
                                        int[][] matrixB) {

            int aRows = matrixA.Length; int aCols = matrixA[0].Length;
            int bRows = matrixB.Length; int bCols = matrixB[0].Length;

            if (aCols != bRows) {
                throw new Exception("Non-conformable matrices in MatrixProduct");
            }

            int[][] result = MatrixCreate(aRows, bCols);
            for (int i = 0; i < aRows; ++i) // каждая строка A
                for (int j = 0; j < bCols; ++j) // каждый столбец B
                    for (int k = 0; k < aCols; ++k)
                        result[i][j] += matrixA[i][k] * matrixB[k][j];

            return result;
        }
        public static int[][] MatrixRandom(int rows, int cols,
                                       int minVal, int maxVal) {

            // Возвращаем матрицу со значениями
            // в диапазоне от minVal до maxVal
            Random ran = new Random();
            int[][] result = MatrixCreate(rows, cols);
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i][j] = ran.Next(minVal, maxVal + 1);

            return result;
        }

        public static int[][] MatrixIdentity(int n) {
            int[][] result = MatrixCreate(n, n);

            for (int i = 0; i < n; ++i)
                result[i][i] = 1;

            return result;
        }
        public static bool MatrixAreEqual(int[][] matrixA,
                                   int[][] matrixB, int epsilon) {
            // True, если все значения в A равны
            // соответствующим значениям в B
            int aRows = matrixA.Length;
            int bCols = matrixB[0].Length;
            for (int i = 0; i < aRows; ++i) // каждая строка A и B
                for (int j = 0; j < bCols; ++j) // каждый столбец A и B
                    if (Math.Abs(matrixA[i][j] - matrixB[i][j]) > epsilon)
                        return false;
            return true;
        }
        public static int[][] MatrixDuplicate(int[][] matrix) {
            // Предполагается, что матрица не нулевая
            int[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (int i = 0; i < matrix.Length; ++i) // Копирование значений
                for (int j = 0; j < matrix[i].Length; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        public static int[][] MatrixDecompose(int[][] matrix,
                                          out int[] perm, out int toggle) {
            // Разложение LUP Дулитла. Предполагается,
            // что матрица квадратная.
            int n = matrix.Length; // для удобства
            int[][] result = MatrixDuplicate(matrix);
            perm = new int[n];
            for (int i = 0; i < n; ++i) { perm[i] = i; }
            toggle = 1;
            for (int j = 0; j < n - 1; ++j) // каждый столбец
            {
                int colMax = Math.Abs(result[j][j]); // Наибольшее значение в столбце j
                int pRow = j;
                for (int i = j + 1; i < n; ++i) {
                    if (result[i][j] > colMax) {
                        colMax = result[i][j];
                        pRow = i;
                    }
                }
                if (pRow != j) // перестановка строк
                {
                    int[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;
                    int tmp = perm[pRow]; // Меняем информацию о перестановке
                    perm[pRow] = perm[j];
                    perm[j] = tmp;
                    toggle = -toggle; // переключатель перестановки строк
                }
                if (Math.Abs(result[j][j]) < 1.0E-20)
                    return null;
                for (int i = j + 1; i < n; ++i) {
                    result[i][j] /= result[j][j];
                    for (int k = j + 1; k < n; ++k)
                        result[i][k] -= result[i][j] * result[j][k];
                }
            } // основной цикл по столбцу j
            return result;
        }
        public static int[] HelperSolve(int[][] luMatrix,
                                    int[] b) {
            // Решаем luMatrix * x = b
            int n = luMatrix.Length;
            int[] x = new int[n];
            b.CopyTo(x, 0);
            for (int i = 1; i < n; ++i) {
                int sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }
            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i) {
                int sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }
            return x;
        }
        public static int[][] MatrixInverse(int[][] matrix) {
            int n = matrix.Length;
            int[][] result = MatrixDuplicate(matrix);
            int[] perm;
            int toggle;
            int[][] lum = MatrixDecompose(matrix, out perm, out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");
            int[] b = new int[n];
            for (int i = 0; i < n; ++i) {
                for (int j = 0; j < n; ++j) {
                    if (i == perm[j])
                        b[j] = 1;
                    else
                        b[j] = 0;
                }
                int[] x = HelperSolve(lum, b);
                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }
        public static int[] MultiplyMatrixOnVector(int[][] matrix, int[] vector) {

            int[] result = new int[matrix[0].Length];
            for (int i = 0; i < matrix[0].Length; i++) {
                for (int j = 0; j < matrix.Length; j++) {

                    result[i] += vector[j] * matrix[j][i];
                }
            }
            return result;
        }
        public static int[] MultiplyVectorOnMatrix(int[] vector, int[][] matrix) {
            if (matrix[0].Length != vector.Length) {
                throw new Exception("Умножение не возможно! Количество столбцов матрицы должно совпадать с количеством элементов вектора.");
            }
            int[] result = new int[matrix[0].Length];
            for (int i = 0; i < matrix.Length; i++) {
                for (int j = 0; j < matrix[0].Length; j++) {

                    result[i] += vector[j] * matrix[i][j];
                }
            }
            return result;
        }

        public static int MatrixDeterminant(int[][] matrix) {
            int[] perm;
            int toggle;
            int[][] lum = MatrixDecompose(matrix, out perm, out toggle);
            if (lum == null)
                throw new Exception("Unable to compute MatrixDeterminant");
            int result = toggle;
            for (int i = 0; i < lum.Length; ++i)
                result *= lum[i][i];
            return result;
        }
        public static double[][] MatrixTranform(int[][] matrix) {
            double[][] newMatrix = new double[matrix[0].Length][];
            for (int i = 0; i < matrix[0].Length; i++) {
                newMatrix[i] = new double[matrix.Length];
            }

            for (int i = 0; i < newMatrix.Length; i++) {
                for (int j = 0; j < newMatrix[0].Length; j++) {
                    newMatrix[i][j] = matrix[j][i];
                }
            }

            return newMatrix;
        }
    }
}
