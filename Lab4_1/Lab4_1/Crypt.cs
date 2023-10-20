using System.Collections;
using static Lab4.Matrix;

namespace Lab4 {
    internal class Crypt {
        private static readonly int m = 4;
        private static readonly int n = 12;
        private static readonly int t = 2;

        private static int[][] matrixG = new int[][] {
            new int[] { 0,1,1,0,1,0,1,0,0,1,0,0 },
            new int[] { 0,1,1,1,1,0,0,1,1,0,0,0 },
            new int[] { 1,1,0,1,1,0,0,0,0,0,0,1 },
            new int[] { 1,1,1,0,1,1,0,1,0,0,1,0 },
            };
        private static int[][] matrixS = new int[][] {
            new int[] { 1,0,0,1 },
            new int[] { 0,1,0,1 },
            new int[] { 0,1,0,0 },
            new int[] { 0,0,1,1 }
        };
        private static int[][] matrixP = new int[][] {
            new int[] { 1,0,0,0,0,0,0,0,0,0,0,0 },
            new int[] { 0,0,1,0,0,0,0,0,0,0,0,0 },
            new int[] { 0,0,0,0,0,0,0,0,1,0,0,0 },
            new int[] { 0,0,0,0,0,1,0,0,0,0,0,0 },
            new int[] { 0,0,0,0,1,0,0,0,0,0,0,0 },
            new int[] { 0,1,0,0,0,0,0,0,0,0,0,0 },
            new int[] { 0,0,0,1,0,0,0,0,0,0,0,0 },
            new int[] { 0,0,0,0,0,0,0,0,0,0,0,1 },
            new int[] { 0,0,0,0,0,0,0,1,0,0,0,0 },
            new int[] { 0,0,0,0,0,0,0,0,0,1,0,0 },
            new int[] { 0,0,0,0,0,0,0,0,0,0,1,0 },
            new int[] { 0,0,0,0,0,0,1,0,0,0,0,0 }
        };
        private static int[][] matrixGHead;
        private static int[][] matrixInverseP;
        private static int[] vectorZ;

        private static void PrepareAllInfo() {
            matrixGHead = MatrixProduct(matrixS, matrixG);
            matrixGHead = MatrixProduct(matrixGHead, matrixP);
            for (int i = 0; i < matrixGHead.Length; i++) {
                for (int j = 0; j < matrixGHead[i].Length; j++) {
                    if (matrixGHead[i][j] % 2 == 0) {
                        matrixGHead[i][j] = 0;
                    }
                    Console.Write(matrixGHead[i][j] + " ");
                }
                Console.WriteLine();
            }
            matrixInverseP = MatrixInverse(matrixP);
        }
        // private static readonly int k = 16; // > 16-4*2=52

        /*        private static int[][] matrixA;
                private static int[][] matrixP;
                private static int[][] matrixInverseP;
                private static int[][] matrixE;
                private static int[] vectorE;*/

        /*        private static int[][] matrixG = new int[][] {
                    new int[] { 1,0,0,1,0,1,0,0,0,0,0,0,1,0,1,1 },
                    new int[] { 1,0,0,0,0,1,0,0,0,1,1,1,1,1,1,0 },
                    new int[] { 0,0,0,1,0,1,1,0,0,0,0,1,1,0,0,0 },
                    new int[] { 0,0,0,1,0,1,0,1,0,0,1,0,1,1,1,0 },
                    new int[] { 1,0,1,0,0,1,0,0,0,0,0,0,0,1,1,0 },
                    new int[] { 0,0,0,0,1,1,0,0,0,0,1,1,0,1,0,0 },
                    new int[] { 1,0,0,0,0,1,0,0,1,0,1,0,1,0,0,0 },
                    new int[] { 1,1,0,1,0,1,0,0,0,0,0,1,0,1,0,0 },
                    new int[] { 1,0,0,1,0,1,0,0,0,0,0,0,1,0,1,1 },
                    new int[] { 1,0,0,0,0,1,0,0,0,1,1,1,1,1,1,0 },
                    new int[] { 0,0,0,1,0,1,1,0,0,0,0,1,1,0,0,0 },
                    new int[] { 0,0,0,1,0,1,0,1,0,0,1,0,1,1,1,0 },
                    new int[] { 1,0,1,0,0,1,0,0,0,0,0,0,0,1,1,0 },
                    new int[] { 0,0,0,0,1,1,0,0,0,0,1,1,0,1,0,0 },
                    new int[] { 1,0,0,0,0,1,0,0,1,0,1,0,1,0,0,0 },
                    new int[] { 1,1,0,1,0,1,0,0,0,0,0,1,0,1,0,0 }
                    };*/

        /*        private static void PrepareMatrixes() {
                    try {
                        int[][] m = MatrixRandom(k, k, 0, 1);
                        int det = MatrixDeterminant(m);
                        while (det == 0) {
                            m = MatrixRandom(k, k, 0, 1);
                            det = MatrixDeterminant(m);
                        }

                        Console.WriteLine(det);
                        for (int i = 0; i < k; i++) {
                            for (int j = 0; j < k; j++) {
                                Console.Write(m[i][j] + " ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                        matrixA = m;

                        int[][] p = MatrixCreatePermutations(n, n);
                        for (int i = 0; i < n; i++) {
                            for (int j = 0; j < n; j++) {
                                Console.Write(p[i][j] + " ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine(det);
                        matrixP = p;

                        matrixE = MatrixProduct(matrixA, matrixG);
                        matrixE = MatrixProduct(matrixE, matrixP);

                        for (int i = 0; i < n; i++) {
                            for (int j = 0; j < n; j++) {
                                Console.Write(matrixE[i][j] + " ");
                            }
                            Console.WriteLine();
                        }

                    } catch (Exception ex) {
                        PrepareMatrixes();
                    }
                }*/
        /*        public static void PrepareAllInfo() {
                    PrepareMatrixes();
                    vectorE = GenerateErrorVector(m - r - 1, n);
                    matrixInverseP = MatrixInverse(matrixP);
                }*/
        private static byte[] CryptByte(byte byteForEncryption) {
            BitArray bitArr = new BitArray(new bool[] { true, false, true, false });
            vectorZ = new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            int[] arr = new int[bitArr.Length];
            for (int i = 0; i < bitArr.Length; i++) {
                arr[i] = bitArr[i] ? 1 : 0;
            }
            int[] result = MultiplyMatrixOnVector(matrixGHead, arr);
            for (int i = 0; i < result.Length; i++) {
                if (result[i] % 2 == 0) {
                    result[i] = 0;
                }

                result[i] += vectorZ[i];

                if (result[i] % 2 == 0) {
                    result[i] = 0;
                }
            }
            Console.WriteLine();
            byte[] byteArr = new byte[result.Length];
            for (int i = 0; i < result.Length; i++) {
                byteArr[i] = (byte)result[i];
                Console.WriteLine(byteArr[i]);
            }

            return byteArr;
        }
        private static byte[] DectyptBytes(byte[] inputBytes) {
            int[] arr = new int[inputBytes.Length];

            for (int i = 0; i < inputBytes.Length; i++) {
                arr[i] = (int)inputBytes[i];
            }

            int[] vectorCHead = MultiplyMatrixOnVector(matrixInverseP, arr);
            
            Console.WriteLine();
            for (int i = 0; i < vectorCHead.Length; i++) {
               
                if (vectorCHead[i] % 2 == 0) {
                    vectorCHead[i] = 0;
                }
                
                Console.Write(vectorCHead[i] + " ");
            }

            Console.WriteLine();
            return new byte[] { };
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
                                return;
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
                            while ((inputVal = binaryReader.ReadBytes(n)).Length == n) {
                                binaryWriter.Write(DectyptBytes(inputVal));
                            }
                        }
                    }
                }
            }

        }
    }
}
