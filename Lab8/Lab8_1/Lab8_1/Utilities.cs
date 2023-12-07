using System.Numerics;

namespace Lab8;

public static class Utilities {
    internal static Complex EmbedBit(Complex pixelValue, byte bit) {
        double real = pixelValue.Real;

        if (bit == 0) {
            real = Math.Floor(real / 2) * 2;
        } else {
            real = Math.Floor(real / 2) * 2 + 1;
        }

        return new Complex(real, pixelValue.Imaginary);
    }

    internal static byte ExtractBit(Complex pixelValue) {
        double real = pixelValue.Real;

        // Извлекаем младший бит из реальной и мнимой частей
        byte bitReal = (byte)((int)real & 1);

        return bitReal;
    }

    public static void Copy(Complex[,] matrixTo, Complex[,] matrixFrom) {
        for (int i = 0; i < matrixTo.GetLength(0); i++) {
            for (int j = 0; j < matrixTo.GetLength(1); j++) {
                matrixTo[i, j] = matrixFrom[i, j];
            }
        }
    }
}