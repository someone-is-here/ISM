using SkiaSharp;

namespace Lab8;

class Program {
    static void Main() {
        var loadedText = File.ReadAllText("input.txt");

        var modifiedImage = Steganography.EncryptMessage("input2.jpg", loadedText);
        using (FileStream stream = File.OpenWrite("output.jpeg")) {
            // Encode the SKImage and save it to the stream
            modifiedImage.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(stream);
        }

        var extractedText = Steganography.DecryptMessage("output.jpeg");
        SaveTextToFile(extractedText, "output.txt");

    }   

    private static void SaveTextToFile(string textToSave, string fileName) {
        File.WriteAllText(fileName, textToSave);
    }
}