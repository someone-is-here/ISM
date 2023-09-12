using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace ConsoleApp1 {
    public class Encryption {
        private static readonly byte[] key = {
                0x11, 0x12, 0x13, 0x14,
                0x22, 0x23, 0x24, 0x28,
                0x30, 0x35, 0x33, 0x39,
                0x62, 0x6e, 0x33, 0x73,
                0x79, 0x67, 0x61, 0x20,
                0x74, 0x74, 0x67, 0x69,
                0x65, 0x68, 0x65, 0x73,
                0x73, 0x3d, 0x2C, 0x20
        };
        private static readonly byte[][] replacementTable = {
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 },
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 },
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 },
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 },
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 },
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 },
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 },
              new byte[] { 0xF,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0x0 }
        };

        private static readonly int sizeOfAccumulator = 4;
        private static byte[] n1 = new byte[sizeOfAccumulator];
        private static byte[] n2 = new byte[sizeOfAccumulator];

        private static byte[] mod2_32( byte[] a, byte[] b) {
            byte[] res;

            UInt32 A = BitConverter.ToUInt32(a, 0);
            UInt32 B = BitConverter.ToUInt32(b, 0);

            A += B;

            res = BitConverter.GetBytes(A);

            return res;
        }

        private static BitArray CyclicShift11(BitArray bitArr) {
            for (int k = 0; k < 11; k++) {
                if (bitArr.Count > 1) {
                    var tmp = bitArr[bitArr.Count - 1];

                    for (var i = bitArr.Count - 1; i != 0; --i) bitArr[i] = bitArr[i - 1];

                    bitArr[0] = tmp;
                }
            }

            return bitArr;
        }

        private static byte[] ConvertBitsArrayIntoByteArray(BitArray bitsArr) {
            int byteArraySize = bitsArr.Count / 8;
            byte[] byteArr = new byte[byteArraySize];

            for (int l = 0; l < byteArraySize; l++) {
                byte sByte = 0x00;
                int bitIndex = 0;

                for (int m = 0; m < 8; m++) {
                    if (bitsArr[l*8 + m]) {
                        sByte |= (byte)(1 << bitIndex);
                    }
                    bitIndex++;
                }
                byteArr[l] = sByte;
            }

            return byteArr;
        }
        private static byte Convert4BitsIntoByte(BitArray bitsArr, int bitArrayIndex) {
            byte sByte = 0x00;
            int bitIndex = 0;

            for (int k = 0; k < sizeOfAccumulator ; k++) {
                if (bitsArr[bitArrayIndex + k]) {
                    sByte |= (byte)(1 << bitIndex);
                }
                bitIndex++;
            }

            return sByte;
        }
        private static BitArray ConvertBitArrayToNormal(BitArray bitArr) {
            BitArray resultBitArr = new BitArray(bitArr.Count);

            for (int m = 0; m < resultBitArr.Count; m += (sizeOfAccumulator*2)) {
                for (int k = sizeOfAccumulator * 2 - 1; k >= 0; k--) {
                    resultBitArr[m + sizeOfAccumulator * 2 - 1 - k] = bitArr[m + k];
                } 

            }
            return resultBitArr;
        }

        private static void CryptBytes(BinaryWriter binaryWriter, byte[] n1, byte[] n2, bool status=true) {
            byte[] keyLine = new byte[sizeOfAccumulator];
            byte[] sum32 = new byte[sizeOfAccumulator];

            int keyIndex = 0;
            for (int i = 0; i < 32; i++) {
                if (status) {
                    if (i == 24) keyIndex = 7;
                } else {
                    if (i == 8) keyIndex = 7;
                }
            
                // срезаем нужную часть ключа, размером в 4 байта
                Array.Copy(key, keyIndex, keyLine, 0, sizeOfAccumulator);

                //суммируем по модулю
                sum32 = mod2_32(n1, keyLine);

                    BitArray sum32BitsArray = new BitArray(sum32);
                   // sum32BitsArray = ConvertBitArrayToNormal(sum32BitsArray);

                    BitArray bitArrayResult = new BitArray(sum32BitsArray.Count, false);

                    int index = 0, resultIndex = 0;
                    int index_i = 0;

                    for (int j = sum32BitsArray.Count - 1; j >= 0; j -= sizeOfAccumulator) {
                        // 7
                        index = (j + 1) / 4 - 1;
                        //размер 4 бита
                        //s0
                        int bitArrayIndex = index * sizeOfAccumulator;
                        byte sByte = Convert4BitsIntoByte(sum32BitsArray, bitArrayIndex);

                        byte replacementValue = replacementTable[index_i][Convert.ToInt16(sByte)];
                        
                        BitArray bitReplacementArray = new BitArray( new byte[] { replacementValue });
                       // bitReplacementArray = ConvertBitArrayToNormal(bitReplacementArray);

                        for (int k = 0; k < sizeOfAccumulator; k++) {
                            bitArrayResult[resultIndex + k] = bitReplacementArray[k];
                        }

                        resultIndex += sizeOfAccumulator;
                        index_i++;
                    }

                    BitArray bitsAfterShift = CyclicShift11(bitArrayResult);

                    BitArray bitsFromN2 = new BitArray(n2);
                   // bitsFromN2 = ConvertBitArrayToNormal(bitsFromN2);

                    for (int k = 0; k < bitsAfterShift.Count; k++) {
                        sum32BitsArray[k] = bitsAfterShift[k] ^ bitsFromN2[k];
                    }

                    sum32 = ConvertBitsArrayIntoByteArray(sum32BitsArray);

                    if (i < 31) {
                        n2 = n1;
                        n1 = sum32;
                    } else {
                        // n1 сохраняет старое значение
                        n2 = sum32;
                    }

                    if (status) {
                        if (i < 24) {
                            keyIndex++;
                            if (keyIndex > 7) keyIndex = 0;
                        } else {
                            keyIndex--;
                            if (keyIndex < 0) keyIndex = 7;
                        }
                    } else {
                        if (i < 8) {
                            keyIndex++;
                            if (keyIndex > 7) keyIndex = 0;
                        } else{
                            keyIndex--;
                            if (keyIndex < 0) keyIndex = 7;
                        }
                    }              
            }
            binaryWriter.Write(n1);
            binaryWriter.Write(n2);

            return;
        }

        public static void СryptMessage(string fileInputName, string fileOutputName, bool status=true) {
            using (FileStream outputFile = File.OpenWrite(fileOutputName)) {
                using (FileStream inputFile = File.OpenRead(fileInputName)) {
                   
                    long fileSize = inputFile.Length; // размер файла в байтах
                    Console.WriteLine(fileSize);
                    long fileSizeBlock = fileSize;
                    // делаем размер кратным 8 (чтобы можно было разделить на блоки)
                    if (fileSize % 8 != 0) {
                        fileSizeBlock = fileSize + (8 - fileSize % 8);
                    }

                   using (BinaryReader binaryReader = new BinaryReader(inputFile)) {
                        using (BinaryWriter binaryWriter = new BinaryWriter(outputFile)) {
                            for (int p = 0; p < fileSizeBlock / 8; p++) {
                                // заполнение накопителей
                                n1 = binaryReader.ReadBytes(sizeOfAccumulator);
                                n2 = binaryReader.ReadBytes(sizeOfAccumulator);


                                if (n1.Length < sizeOfAccumulator) {
                                    byte[] n1Copy = new byte[sizeOfAccumulator];
                                    for (int i = 0; i < n1.Length; i++) {
                                        n1Copy[i] = n1[i];
                                    }
                                    for (int i = n1Copy.Length - 1; i < sizeOfAccumulator; i++) {
                                        if (i < 0) {
                                            i = 0;
                                        }
                                        n1Copy[i] = 0x00;
                                    }
                                    n1 = n1Copy;
                                }

                                if (n2.Length < sizeOfAccumulator) {
                                    byte[] n2Copy = new byte[sizeOfAccumulator];
                                    for (int i = 0; i < n2.Length; i++) {
                                        n2Copy[i] = n2[i];
                                    }
                                    for (int i = n2Copy.Length - 1; i < sizeOfAccumulator; i++) {
                                        if (i < 0) {
                                            i = 0;
                                        }
                                        n2Copy[i] = 0x00;
                                    }
                                    n2 = n2Copy;
                                }
                                CryptBytes(binaryWriter, n1, n2, status);
                            }
                        }
                    }
                }
            }

            return;
        }
    }
}
