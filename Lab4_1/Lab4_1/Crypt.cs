using System.Collections;
using System.Numerics;
using static Lab4.Matrix;
using static System.Reflection.Metadata.BlobBuilder;

namespace Lab4 {
    internal class Crypt {
        private static readonly int m = 4;
        private static readonly int n = 8;
        private static readonly int t = 2;

        private static int[][] matrixG = new int[][] {
            new int[] { 1,0,0,0, 1,1,0 },
            new int[] { 0,1,0,0, 1,0,1 },
            new int[] { 0,0,1,0, 0,1,1 },
            new int[] { 0,0,0,1, 1,1,1 },
            };
        private static int[][] matrixS = new int[][] {
            new int[] { 1,0,0,0 },
            new int[] { 0,1,0,0 },
            new int[] { 0,0,1,0 },
            new int[] { 0,0,0,1 }
        };
        private static int[][] matrixP = new int[][] {
            new int[] { 1,0,0,0,0,0,0 },
            new int[] { 0,1,0,0,0,0,0 },
            new int[] { 0,0,1,0,0,0,0 },
            new int[] { 0,0,0,1,0,0,0 },
            new int[] { 0,0,0,0,1,0,0 },
            new int[] { 0,0,0,0,0,1,0 },
            new int[] { 0,0,0,0,0,0,1 },
        };
        private static int[][] matrixGHead;
        private static int[][] matrixH = new int[][] {
            new int[] { 1,1,0 },
            new int[] { 1,0,1 },
            new int[] { 0,1,1 },
            new int[] { 1,1,1 },

            new int[] { 1,0,0 },
            new int[] { 0,1,0 },
            new int[] { 0,0,1 },
            };
        private static int[][] matrixInverseP;
        private static int[][] matrixInverseS;
        private static int[] vectorZ;

        private static void PrepareAllInfo() {

            matrixGHead = MatrixProduct(matrixS, matrixG);
            matrixGHead = MatrixProduct(matrixGHead, matrixP);

            for (int i = 0; i < matrixGHead.Length; i++) {
                for (int j = 0; j < matrixGHead[i].Length; j++) {
                    if (matrixGHead[i][j] % 2 == 0) {
                        matrixGHead[i][j] = 0;
                    }
                }
            }
            matrixInverseP = MatrixInverse(matrixP);
            matrixInverseS = MatrixInverse(matrixS);
        }
        private static byte[] CryptByte(byte byteForEncryption) {
            BitArray bitArrayFull = new BitArray(new byte[] { byteForEncryption });
            byte[] byteArr = new byte[(n-1)*2];
            int index = 0;
            for (int k = 0; k < n; k += n/2) {
                BitArray bitArrayHalf = new BitArray(4);
                for (int j = 0; j < n/2; j++) {
                    bitArrayHalf[j] = bitArrayFull[j+k];
                }
                vectorZ = GetErrorVector(n-1, n/2);

                int[] arr = new int[bitArrayHalf.Length];
                for (int i = 0; i < bitArrayHalf.Length; i++) {
                    arr[i] = bitArrayHalf[i] ? 1 : 0;
                }

                int[] result = MultiplyMatrixOnVector(matrixGHead, arr);

                for (int i = 0; i < result.Length; i++) {

                    result[i] += vectorZ[i];
                    result[i] %= 2;

                }

                for (int i = index; i < index + result.Length; i++) {
                    byteArr[i] = (byte)result[i-index];
                }
                index += (n - 1);
            }
             

            return byteArr;
        }
        private static byte DectyptBytes(byte[] inputBytes) {
            byte[] bytes = new byte[2];
            bool[] boolsResult = new bool[n];
            int index = 0;
            for (int k = 0; k < (n - 1) * 2; k += (n - 1)) {
                int[] arr = new int[inputBytes.Length / 2];

                for (int i = k; i < k+inputBytes.Length/2; i++) {
                    arr[i-k] = (int)inputBytes[i];
                }

                int[] vectorCHead = MultiplyMatrixOnVector(matrixInverseP, arr);

                for (int i = 0; i < vectorCHead.Length; i++) {
                    vectorCHead[i] %= 2;
                }

                int[] syndrom = MultiplyMatrixOnVector(matrixH, vectorCHead);
                for (int i = 0; i < syndrom.Length; i++) {
                    syndrom[i] %= 2;

                }

                int resultIndex = -1;
                for (int i = 0; i < matrixH.Length; i++) {
                    int sumIndex = 0;
                    for (int j = 0; j < matrixH[0].Length; j++) {
                        if (syndrom[j] == matrixH[i][j]) {
                            sumIndex++;
                        }
                    }
                    if (sumIndex == matrixH[0].Length) {
                        resultIndex = i;
                        break;
                    }
                }
                if (resultIndex != -1) {
                    vectorCHead[resultIndex] = vectorCHead[resultIndex] == 0 ? 1 : 0;
                }

                vectorCHead = MultiplyMatrixOnVector(matrixInverseS, vectorCHead);
               
                for (int i = index; i < index+boolsResult.Length/2; i++) {
                    boolsResult[i] = vectorCHead[i-index] == 1 ? true : false;
                }
                index += (n / 2);
              
            }
            BitArray a = new BitArray(boolsResult);
            a.CopyTo(bytes, 0);

            return bytes[0];
        }


        public static void EncryptMessageFromFile(string inputFileName, string ouputFileName) {
            PrepareAllInfo();
            using (FileStream outputFile = File.OpenWrite(ouputFileName)) {
                using (FileStream inputFile = File.OpenRead(inputFileName)) {
                    using (BinaryReader binaryReader = new BinaryReader(inputFile)) {
                        using (BinaryWriter binaryWriter = new BinaryWriter(outputFile)) {

                            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) {
                                byte[] encryptedBytes = CryptByte(binaryReader.ReadByte());
                                binaryWriter.Write(encryptedBytes);
                            }
                        }
                    }
                }
            }
        }


        public static void DecryptMessageFromFile(string inputFileName, string ouputFileName) {
            using (FileStream outputFile = File.OpenWrite(ouputFileName)) {
                using (FileStream inputFile = File.OpenRead(inputFileName)) {
                    using (BinaryReader binaryReader = new BinaryReader(inputFile)) {
                        using (BinaryWriter binaryWriter = new BinaryWriter(outputFile)) {
                            byte[] inputVal;
                            while ((inputVal = binaryReader.ReadBytes((n-1)*2)).Length == (n-1)*2) {
                                binaryWriter.Write(DectyptBytes(inputVal));
                            }
                        }
                    }
                }
            }

        }
    }
}
