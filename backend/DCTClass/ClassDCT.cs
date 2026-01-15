using System.Text;
using ImageMagick;

namespace DCTClass;

public class ClassDCT
{
    private static int UParam = 4;
    private static int VParam = 3;
    private static string Terminator = "<END>";
    private static double QScale = 30.0;

    private static List<int> StringToBits(string message)
    {
        List<int> bits = new List<int>();
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        foreach (byte b in messageBytes)
        {
            for (int i = 7; i >= 0; i--)
            {
                bits.Add((b >> i) & 1);
            }
        }
        return bits;
    }

    private static bool CheckForTerminator(List<byte> bytes)
    {
        byte[] termBytes = Encoding.UTF8.GetBytes(Terminator);
        if (bytes.Count < termBytes.Length) return false;

        for (int i = 0; i < termBytes.Length; i++)
        {
            if (bytes[bytes.Count - termBytes.Length + i] != termBytes[i])
                return false;
        }
        return true;
    }

    private static void GetBlocks(IPixelCollection<byte> pixels, int width, int height, int startX, int startY, double[,] blkY, double[,] blkCr, double[,] blkCb)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (startX + x >= width || startY + y >= height) continue;

                var pixelColor = pixels.GetPixel(startX + x, startY + y).ToColor();

                double r = ((double)pixelColor.R / Quantum.Max) * 255.0;
                double g = ((double)pixelColor.G / Quantum.Max) * 255.0;
                double b = ((double)pixelColor.B / Quantum.Max) * 255.0;

                blkY[x, y] = 0.299 * r + 0.587 * g + 0.114 * b;
                blkCb[x, y] = 128 - 0.168736 * r - 0.331264 * g + 0.5 * b;
                blkCr[x, y] = 128 + 0.5 * r - 0.418688 * g - 0.081312 * b;
            }
        }
    }

    private static void SetBlocks(IPixelCollection<byte> pixels, int width, int height, int startX, int startY, double[,] blkY, double[,] blkCr, double[,] blkCb)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (startX + x >= width || startY + y >= height) continue;

                double yValue = blkY[x, y];
                double cbValue = blkCb[x, y];
                double crValue = blkCr[x, y];

                int r = (int)Math.Round(yValue + 1.402 * (crValue - 128));
                int g = (int)Math.Round(yValue - 0.344136 * (cbValue - 128) - 0.714136 * (crValue - 128));
                int b = (int)Math.Round(yValue + 1.772 * (cbValue - 128));

                r = Math.Max(0, Math.Min(255, r));
                g = Math.Max(0, Math.Min(255, g));
                b = Math.Max(0, Math.Min(255, b));

                pixels.SetPixel(startX + x, startY + y, new byte[] { (byte)r, (byte)g, (byte)b });
            }
        }
    }

    private static double[,] dct8x8(double[,] block)
    {
        double[,] output = new double[8, 8];
        for (int u = 0; u < 8; u++)
        {
            for (int v = 0; v < 8; v++)
            {
                double sum = 0.0;
                double cu = (u == 0) ? 1.0 / Math.Sqrt(2) : 1.0;
                double cv = (v == 0) ? 1.0 / Math.Sqrt(2) : 1.0;

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        sum += block[x, y] *
                               Math.Cos(((2 * x + 1) * u * Math.PI) / 16.0) *
                               Math.Cos(((2 * y + 1) * v * Math.PI) / 16.0);
                    }
                }
                output[u, v] = 0.25 * cu * cv * sum;
            }
        }
        return output;
    }

    private static double[,] idct8x8(double[,] block)
    {
        double[,] output = new double[8, 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                double sum = 0.0;
                for (int u = 0; u < 8; u++)
                {
                    for (int v = 0; v < 8; v++)
                    {
                        double cu = (u == 0) ? 1.0 / Math.Sqrt(2) : 1.0;
                        double cv = (v == 0) ? 1.0 / Math.Sqrt(2) : 1.0;
                        sum += cu * cv * block[u, v] * Math.Cos(((2 * x + 1) * u * Math.PI) / 16.0) * Math.Cos(((2 * y + 1) * v * Math.PI) / 16.0);
                    }
                }
                output[x, y] = 0.25 * sum;
            }
        }
        return output;
    }

    private static void HideBit(double[,] dctBlock, int bit)
    {
        double val = dctBlock[UParam, VParam];
        int quantized = (int)Math.Round(val / QScale);
        if (Math.Abs(quantized) % 2 != bit)
        {
            if (quantized >= 0) quantized++;
            else quantized--;
        }
        dctBlock[UParam, VParam] = quantized * QScale;
    }

    private static int GetHiddenBit(double[,] dctBlock)
    {
        double val = dctBlock[UParam, VParam];
        int quantized = (int)Math.Round(val / QScale);
        return Math.Abs(quantized) % 2;
    }

    public static byte[] Encrypt(Stream inputStream, string message)
    {
        try
        {
            inputStream.Position = 0;
            using var testImage = new MagickImage(inputStream);
            {
                
            }
        }
        catch (Exception e)
        {
            throw;
        }

        inputStream.Position = 0;
        using var image = new MagickImage(inputStream);
        image.Format = MagickFormat.Png;
        message += Terminator;
        List<int> bits = StringToBits(message);
        int bitIndex = 0;
        int bitMessageLength = bits.Count;
        int width = (int)image.Width - ((int)image.Width % 8);
        int height = (int)image.Height - ((int)image.Height % 8);

        double[,] blockY = new double[8, 8];
        double[,] blockCb = new double[8, 8];
        double[,] blockCr = new double[8, 8];

        bool finished = false;

        using (var pixels = image.GetPixels())
        {
            for (int by = 0; by < height; by += 8)
            {
                for (int bx = 0; bx < width; bx += 8)
                {
                    if (finished) break;

                    GetBlocks(pixels, (int)image.Width, (int)image.Height, bx, by, blockY, blockCr, blockCb);
                    double[,] dctY = dct8x8(blockY);

                    if (bitIndex < bitMessageLength)
                    {
                        int bit = bits[bitIndex];
                        HideBit(dctY, bit);
                        bitIndex++;
                        double[,] idctY = idct8x8(dctY);
                        SetBlocks(pixels, (int)image.Width, (int)image.Height, bx, by, idctY, blockCr, blockCb);
                    }
                    else
                    {
                        finished = true;
                    }
                }
                if (finished) break;
            }
        }
        return image.ToByteArray();
    }

    public static string Decrypt(Stream inputStream)
    {
        using var image = new MagickImage(inputStream);

        int width = (int)image.Width - ((int)image.Width % 8);
        int height = (int)image.Height - ((int)image.Height % 8);

        double[,] blockY = new double[8, 8];
        double[,] blockCb = new double[8, 8];
        double[,] blockCr = new double[8, 8];

        byte currentByte = 0;
        int bitCount = 0;
        List<byte> collectedBytes = new List<byte>();
        bool foundTerminator = false;

        using (var pixels = image.GetPixels())
        {
            for (int by = 0; by < height; by += 8)
            {
                for (int bx = 0; bx < width; bx += 8)
                {
                    GetBlocks(pixels, (int)image.Width, (int)image.Height, bx, by, blockY, blockCr, blockCb);
                    double[,] dctY = dct8x8(blockY);
                    int bit = GetHiddenBit(dctY);

                    currentByte = (byte)((currentByte << 1) | bit);
                    bitCount++;

                    if (bitCount == 8)
                    {
                        collectedBytes.Add(currentByte);
                        bitCount = 0;
                        currentByte = 0;
                        if (CheckForTerminator(collectedBytes))
                        {
                            foundTerminator = true;
                            break;
                        }
                    }
                }
                if (foundTerminator) break;
            }
        }

        if (foundTerminator)
        {
            int termLen = Encoding.UTF8.GetByteCount(Terminator);
            collectedBytes.RemoveRange(collectedBytes.Count - termLen, termLen);
        }

        return Encoding.UTF8.GetString(collectedBytes.ToArray());
    }
}
