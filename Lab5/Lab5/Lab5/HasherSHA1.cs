using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lab5.Hasher3411;

namespace Lab5 {
    public class HasherSHA1 {
        private static uint h0 = 0x67452301;
        private static uint h1 = 0xEFCDAB89;
        private static uint h2 = 0x98BADCFE;
        private static uint h3 = 0x10325476;
        private static uint h4 = 0xC3D2E1F0;

        private static uint LeftRotate(uint x, int c)
            => (x << c) | (x >> (32 - c));

        private static uint GetUintFromBytes(byte[] bytes) {
            byte[] value = new byte[8];
            Array.Reverse(bytes);
            Array.Copy(bytes, value, bytes.Length);
            uint result = (uint)BitConverter.ToUInt64(value, 0);

            return result;
        }

        public static byte[] GetHash(byte[] bytes) {
            // message length always is a multiple of 8 bits
            var processedInputBuilder = new List<byte>(bytes) { 0x80 };
            while (processedInputBuilder.Count % 64 != 56)
                processedInputBuilder.Add(0x0);
            processedInputBuilder.AddRange(BitConverter.GetBytes((long)bytes.Length * 8).Reverse()); // bit converter returns big-endian
            byte[] processedInput = processedInputBuilder.ToArray();

            int blockAmount = processedInput.Length / 64;

            for (int i = 0; i < blockAmount; i++) {
                byte[] temp = new byte[64];
                Array.Copy(processedInput, i*64, temp, 0, 64);

                byte[][] w = new byte[80][];
                for (int j = 0; j < 16; j++) {
                    w[j] = new byte[4];
                    Array.Copy(temp, 4*j, w[j], 0, 4);
                }
                for (int j = 16; j < 79; j++) {
                    w[j] = new byte[4];
                    w[j] = Xor(w[j], w[j - 3]);
                    w[j] = Xor(w[j], w[j - 8]);
                    w[j] = Xor(w[j], w[j - 14]);
                    w[j] = Xor(w[j], w[j - 16]);

                    // Left Rotate 1 (differs from sha-0)
                    byte tempByte = w[j][0];
                    for (int p = 0; p < 3; p++) {
                        w[j][p] = w[j][p + 1];
                    }
                    w[j][3] = tempByte;
                }

                uint a = h0, b=h1, c=h2, d=h3, e=h4, f, k;

                for (int j = 0; j < 79; j++) {
                    if (i >= 0 && i <= 19) {
                        f = (b & c) | (~b & d);
                        k = 0x5A827999;
                    } else if (i >= 20 && i <= 39) {
                        f = b ^ c ^ d;
                        k = 0x6ED9EBA1;
                    } else if (i >= 40 && i <= 59) {
                        f = (b & c) | (b & d) | (c & d);
                        k = 0x8F1BBCDC;
                    } else {
                        f = b ^ c ^ d;
                        k = 0xCA62C1D6;
                    }
                    
                    uint tempUint = LeftRotate(a, 5) + f + e + k + GetUintFromBytes(w[j]);
                    e = d;
                    d = c;
                    c = LeftRotate(b, 30);
                    b = a;
                    a = tempUint;
                }
                h0 = h0 + a;
                h1 = h1 + b;
                h2 = h2 + c;
                h3 = h3 + d;
                h4 = h4 + e;
            }
            byte[] hh = new byte[20];

            Array.Copy(BitConverter.GetBytes(h0), 0, hh, 0, 4);
            Array.Copy(BitConverter.GetBytes(h1), 0, hh, 4, 4);
            Array.Copy(BitConverter.GetBytes(h2), 0, hh, 8, 4);
            Array.Copy(BitConverter.GetBytes(h3), 0, hh, 12, 4);
            Array.Copy(BitConverter.GetBytes(h4), 0, hh, 16, 4);

            return hh;

        }
        private static byte[] Xor(byte[] byteArr1, byte[] byteArr2) {
            if (byteArr1.Length != 4 || byteArr2.Length != 4) {
                throw new Exception("Wrong input parameters. Expecting byte array, length 64");
            }

            byte[] result = new byte[4];

            for (int i = 0; i < 4; i++) {
                result[i] = (byte)(byteArr1[i] ^ byteArr2[i]);
            }

            return result;
        }

        public static void WriteHashIntoFile(string inputFileName, string ouputFileName) {
            if (!File.Exists(inputFileName)) {
                File.WriteAllText(inputFileName, string.Empty);
            }

            byte[] message = File.ReadAllBytes(inputFileName);
            byte[] hash = GetHash(message);
            string hashedString = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            File.WriteAllText(ouputFileName, hashedString);
        }
    }
}
