using System.Text;


namespace Lab6 {
    class Program{
        static void Main(string[] args) {
            BigInteger p = new BigInteger("6277101735386680763835789423207666416083908700390324961279", 10);
            BigInteger a = new BigInteger("-3", 10);
            BigInteger b = new BigInteger("64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1", 16);
            byte[] xG = fromHexStringToByte("03188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012");
            BigInteger n = new BigInteger("ffffffffffffffffffffffff99def836146bc9b1b4d22831", 16);
            
            Module34_10 DS = new Module34_10(p, a, b, n, xG);
            BigInteger d = DS.genPrivateKey(192);
            Point Q = DS.genPublicKey(d);
            Hasher3411 hash = new Hasher3411(256);


            String message = readFile("input.txt");
            byte[] H = hash.GetHash(Encoding.Default.GetBytes(message));
            string sign = DS.genDS(H, d);
            Console.WriteLine($"ЭЦП: {sign}");
            writeFile("output.txt", sign);

           
            String message2 = readFile("input.txt");
            byte[] H2 = hash.GetHash(Encoding.Default.GetBytes(message2));
           
            string signVer = readFile("output.txt");

            bool result = DS.verifDS(H2, signVer, Q);

            if (result) {
                Console.WriteLine("Верификация прошла успешно. Цифровая подпись верна.");
            } else {
                Console.WriteLine("Верификация не прошла! Цифровая подпись не верна.");
            }
        }

        private static byte[] fromHexStringToByte(string input) {
            byte[] data = new byte[input.Length / 2];
            string HexByte = "";
            for (int i = 0; i < data.Length; i++)
            {
                HexByte = input.Substring(i * 2, 2);
                data[i] = Convert.ToByte(HexByte, 16);
            }
            return data;
        }

        private static string readFile(string path) {
            string text = System.IO.File.ReadAllText(path);
            return (text);
        }

        private static void writeFile(string path, string text) {
            System.IO.File.WriteAllText(path, text);
        }
    }
}
