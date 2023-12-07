using SkiaSharp;
using System.Numerics;
using System.Text;
using Accord.Math;

namespace Lab8;

public static class Steganography {

    private static Complex[,]? _data;

    public static SKBitmap EncryptMessage(string imagePath, string message) {
        using var image = SKBitmap.Decode(imagePath);
        int messageLength = message.Length;

        // Check message length
        if (messageLength > (image.Width * image.Height) / 8 - 1) {
            throw new Exception("Слишком длинное сообщение для данного изображения.");
        }

        // Add end-marker to the text
        message += '\0';

        // Text -> binary
        byte[] binaryMessage = Encoding.UTF8.GetBytes(message);

        // FFT
        Complex[,] fftData = ApplyFFT(image);

        int currentX = 0;
        int currentY = 0;

        int bitIndex = 0;
        int messageIndex = 0;

        while (messageIndex < binaryMessage.Length) {
            Complex pixelValue = fftData[currentX, currentY];

            byte bit = (byte)((binaryMessage[messageIndex] >> bitIndex) & 1);
            pixelValue = Utilities.EmbedBit(pixelValue, bit);
            fftData[currentX, currentY] = pixelValue;

            // Обнуление индекса бита при достижение 8
            if (++bitIndex == 8) {
                // Console.WriteLine(binaryMessage[messageIndex]);
                bitIndex = 0;
                messageIndex++;
            }

            // Переход к следующему пикселю с учетом шага
            currentX += 1;
            if (currentX >= image.Width) {
                currentX = 0;
                currentY += 1;
            }

            if (currentY >= image.Height) {
                break; // Изображение закончилось
            }
        }

        // ~FFT
        SKBitmap encodedImage = InverseFFT(fftData);

        return encodedImage;
    }

    public static string DecryptMessage(string modifiedImagePath) {
        // Get modified image bitmap
        var encodedImage = SKBitmap.Decode(modifiedImagePath);

        // Extract coefficients from image
        Complex[,] fftData = ApplyFFT(encodedImage);

        int currentX = 0;
        int currentY = 0;

        int bitIndex = 0;
        byte binaryChar = byte.MinValue;
        List<byte> binaryChars = new();

        bool endFound = false;

        while (currentY < encodedImage.Height && !endFound) {
            Complex pixelValue = fftData[currentX, currentY];

            byte bit = Utilities.ExtractBit(pixelValue);
            if (bitIndex == 0) {
                binaryChar = bit;
            } else {
                binaryChar |= (byte)(bit << bitIndex);
            }

            if (++bitIndex == 8) {
                bitIndex = 0;
                if (binaryChar != '\0') {
                    binaryChars.Add(binaryChar);
                } else {
                    // Marker found
                    endFound = true;
                }
                binaryChar = byte.MinValue;
            }

            // Next pixel
            currentX += 1;
            if (currentX >= encodedImage.Width) {
                currentX = 0;
                currentY += 1;
            }
        }

        // Text from bytes
        var extractedMessage = Encoding.UTF8.GetString(binaryChars.ToArray());

        return endFound ? extractedMessage : "ERROR_EXTRACTING";
    }

    private static Complex[,] ApplyFFT(SKBitmap image) {
        int width = image.Width;
        int height = image.Height;

        Complex[,] fftData = new Complex[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                SKColor pixel = image.GetPixel(x, y);
                byte red = pixel.Red;
                byte green = pixel.Green;
                byte blue = pixel.Blue;

                int combined = (red << 16) | (green << 8) | blue;

                fftData[x, y] = new Complex(combined, 0);
            }
        }

        FourierTransform.FFT2(fftData, FourierTransform.Direction.Forward);
        if (_data != null) {
            Utilities.Copy(fftData, _data);
        }

        return fftData;
    }

    private static SKBitmap InverseFFT(Complex[,] fftData) {
        int width = fftData.GetLength(0);
        int height = fftData.GetLength(1);

        _data = new Complex[width, height];

        Utilities.Copy(_data, fftData);

        FourierTransform.FFT2(fftData, FourierTransform.Direction.Backward);

        SKBitmap resultImage = new SKBitmap(width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Complex pixelValue = fftData[x, y];

                int combined = (int)pixelValue.Real;
                byte red = (byte)((combined >> 16) & 0xFF);
                byte green = (byte)((combined >> 8) & 0xFF);
                byte blue = (byte)(combined & 0xFF);

                SKColor pixel = new SKColor(red, green, blue);
                resultImage.SetPixel(x, y, pixel);
            }
        }

        return resultImage;
    }
}