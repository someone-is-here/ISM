using System.Numerics;

namespace Lab7 {
    public class Crypt {
        private static Random rand = new Random();

        //message
        private static BigInteger m;

        // open key
        private static int p = 257;
        private static BigInteger g = 3;

        // close key
        private static int x = rand.Next(1, p - 1);

        private static int k = rand.Next(1, p - 1);

        private static BigInteger y = BigInteger.ModPow(g, x, p);


        public static byte[] Encrypt(byte[] message) {
            m = new BigInteger(message, true);

            BigInteger a = BigInteger.ModPow(g, k, p);
            BigInteger temp = BigInteger.Pow(y, k) * m;
            BigInteger b = BigInteger.ModPow(temp, 1, p);

            byte[] aByte = a.ToByteArray(true);
            byte[] bByte = b.ToByteArray(true);
            byte[] result = new byte[aByte.Length + bByte.Length];

            Array.Copy(aByte, 0, result, 0, aByte.Length);
            Array.Copy(bByte, 0, result, aByte.Length, bByte.Length);

            return result;
        }

        public static byte Decrypt(byte[] encrypted) {
            byte[] aByte = new byte[encrypted.Length / 2];
            byte[] bByte = new byte[encrypted.Length / 2];

            Array.Copy(encrypted, 0, aByte, 0, encrypted.Length / 2);
            Array.Copy(encrypted, encrypted.Length / 2, bByte, 0, encrypted.Length / 2);

            BigInteger a = new BigInteger(aByte, true);
            BigInteger b = new BigInteger(bByte, true);

            BigInteger temp = BigInteger.Pow(a, p - 1 - x);

            BigInteger message = BigInteger.ModPow(b * temp, 1, p);

            byte[] messageByte = message.ToByteArray();

            return messageByte[0];
        }

        public static void EncryptMessageFromFile(string inputFileName, string ouputFileName) {
            using (FileStream outputFile = File.OpenWrite(ouputFileName)) {
                using (FileStream inputFile = File.OpenRead(inputFileName)) {
                    using (BinaryReader binaryReader = new BinaryReader(inputFile)) {
                        using (BinaryWriter binaryWriter = new BinaryWriter(outputFile)) {

                            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) {
                                byte[] encryptedBytes = Encrypt(new byte[] { binaryReader.ReadByte() });
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
                            while ((inputVal = binaryReader.ReadBytes(2)).Length == 2) {
                                binaryWriter.Write(Decrypt(inputVal));
                            }
                        }
                    }
                }
            }

        }
    }
}
