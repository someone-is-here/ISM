using System.Collections;
using System.Numerics;

namespace Lab3 {
    internal class Crypt {
        //Closed key
        private static readonly int p = 107;
        private static readonly int q = 223;

        //Open key
        private static readonly int n = p * q;

        //Decryption helper
        private static readonly byte key = 0x80;

        private static byte[] ConvertToByteArray(BitArray bits) {
           
            byte[] bytes = new byte[2];
            bits.CopyTo(bytes, 0);

            return bytes;
        }

        private static List<BigInteger> ExtendedEuclideanAlgorithm(BigInteger r_0, BigInteger r_1,
                                                                   BigInteger s_0, BigInteger s_1,
                                                                   BigInteger t_0, BigInteger t_1) {
            BigInteger q = r_0 / r_1;
            BigInteger r_next = r_0 - r_1 * q;
            BigInteger s_next = s_0 - s_1 * q;
            BigInteger t_next = t_0 - t_1 * q;

            if (r_next == 0) {
                return new List<BigInteger> () { s_1, t_1 };
            }
            return ExtendedEuclideanAlgorithm(r_1, r_next, s_1, s_next, t_1, t_next);
        }

        public static byte[] CryptByte(byte value) {
            byte[] cryptedArr = new byte[2];
            cryptedArr[1] = value;
            cryptedArr[0] = key;
            BitArray bitsArr = new BitArray(cryptedArr);
            var reversedBitArr = new BitArray(bitsArr.Cast<bool>().Reverse().ToArray());
            int[] array = new int[1];
            reversedBitArr.CopyTo(array, 0);

            int cryptedValue = array[0];

            BigInteger val = cryptedValue;
            BigInteger resultVal = BigInteger.ModPow(val, 2, n);

            int res = (int)resultVal;

            byte[] result = BitConverter.GetBytes(res);

            return result;
        }

        public static void EncryptMessageFromFile(string inputFileName, string ouputFileName) {
            using (FileStream outputFile = File.OpenWrite(ouputFileName)) {
                using (FileStream inputFile = File.OpenRead(inputFileName)) {
                    using (BinaryReader binaryReader = new BinaryReader(inputFile)) {
                        using (BinaryWriter binaryWriter = new BinaryWriter(outputFile)) {

                            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) {
                                byte[] arr = CryptByte(binaryReader.ReadByte());
                                binaryWriter.Write(arr);
                            }                             
                        }
                    }
                }
            }
        }

        public static bool IsRightByte(BigInteger value, out byte byteRes) {
            byteRes = 0x00;

            int res = (int)value;

            byte[] bytesVal = BitConverter.GetBytes(res);
            byte[] cutRes = new byte[2];

            Array.Copy(bytesVal, cutRes, 2);

            BitArray bitsArr = new BitArray(cutRes);
            BitArray reversedBitArr = new BitArray(bitsArr.Cast<bool>().Reverse().ToArray());

            byte[] result = ConvertToByteArray(reversedBitArr);

            if (result[0] == key) {
                byteRes = result[1];

                return true;
            }

            return false;
        }

        public static byte GetRightByte(BigInteger r_1, BigInteger r_2, BigInteger r_3, BigInteger r_4) {
            byte decryptedByte = 0x00;

            if (IsRightByte(r_1,  out decryptedByte)) {
                return decryptedByte;
            }
            if (IsRightByte(r_2, out decryptedByte)) {
                return decryptedByte;
            }
            if (IsRightByte(r_3, out decryptedByte)) {
                return decryptedByte;
            }
            if (IsRightByte(r_4, out decryptedByte)) {
                return decryptedByte;
            }

            throw new Exception("Something went wrong while decrypting message!");
        }

        private static byte DectyptByte(byte[] inputVal) {
            int value = BitConverter.ToInt32(inputVal, 0);

            BigInteger m_p = BigInteger.ModPow(value, (BigInteger)(0.25 * (p + 1)), p);
            BigInteger m_q = BigInteger.ModPow(value, (BigInteger)(0.25 * (q + 1)), q);


            List<BigInteger> items = Crypt.ExtendedEuclideanAlgorithm(Math.Max(p, q), Math.Min(p, q), 1, 0, 0, 1);
            BigInteger y_p = 0, y_q = 0;
            if (Math.Max(p, q) == p) {
                y_p = items[0];
                y_q = items[1];
            } else {
                y_p = items[1];
                y_q = items[0];
            }

            BigInteger r_1 = (y_p * p * m_q + y_q * q * m_p) % n;
            if (r_1 < 0) {
                r_1 = n + r_1;
            }
            BigInteger r_2 = n - r_1;
            if (r_2 < 0) {
                r_2 = n + r_2;
            }
            BigInteger r_3 = (y_p * p * m_q - y_q * q * m_p) % n;
            if (r_3 < 0) {
                r_3 = n + r_3;
            }
            BigInteger r_4 = n - r_3;
            if (r_4 < 0) {
                r_4 = n + r_4;
            }

            byte result = GetRightByte(r_1, r_2, r_3, r_4);

            return result;
        }

        public static void DecryptMessageFromFile(string inputFileName, string ouputFileName) {
            using (FileStream outputFile = File.OpenWrite(ouputFileName)) {
                using (FileStream inputFile = File.OpenRead(inputFileName)) {
                    using (BinaryReader binaryReader = new BinaryReader(inputFile)) {
                        using (BinaryWriter binaryWriter = new BinaryWriter(outputFile)) {
                            byte[] inputVal;
                            while ((inputVal = binaryReader.ReadBytes(4)).Length == 4) {
                                binaryWriter.Write(DectyptByte(inputVal));
                            }
                        }
                    }
                }
            }

        }
    }
}
