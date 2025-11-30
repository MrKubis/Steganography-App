using System.Drawing;
using System.Text;

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

    private static void GetBlocks(Bitmap bmp, int startX, int startY, double[,] blkY, double[,] blkCr, double[,] blkCb)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (startX + x >= bmp.Width || startY + y >= bmp.Height) continue;
                
                Color c = bmp.GetPixel(startX + x, startY + y);
                blkY[x, y] = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
                blkCb[x, y] = 128 - 0.168736 * c.R - 0.331264 * c.G + 0.5 * c.B;
                blkCr[x, y] = 128 + 0.5 * c.R - 0.418688 * c.G - 0.081312 * c.B;
            }
        }
    }

    private static void SetBlocks(Bitmap bmp, int startX, int startY, double[,] blkY, double[,] blkCr, double[,] blkCb)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (startX + x >= bmp.Width || startY + y >= bmp.Height) continue;

                double yValue = blkY[x, y];
                double cbValue = blkCb[x, y];
                double crValue = blkCr[x, y];

                int r = (int)Math.Round(yValue + 1.402 * (crValue - 128));
                int g = (int)Math.Round(yValue - 0.344136 * (cbValue - 128) - 0.714136 * (crValue - 128));
                int b = (int)Math.Round(yValue + 1.772 * (cbValue - 128));
                
                r = Math.Max(0, Math.Min(255, r));
                g = Math.Max(0, Math.Min(255, g));
                b = Math.Max(0, Math.Min(255, b));

                bmp.SetPixel(startX + x, startY + y, Color.FromArgb(r, g, b));
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
    
    private static void HideBit(double [,] dctBlock, int bit)
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
    
    private static int GetHiddenBit(double [,] dctBlock)
    {
        double val = dctBlock[UParam, VParam];
        int quantized = (int)Math.Round(val / QScale);
        return Math.Abs(quantized) % 2;
    }
    
    public static Image Encrypt(Image input, string message)
    {
        Bitmap bmpIn = new Bitmap(input);
        Bitmap bmpOut = new Bitmap(bmpIn); 

        message += Terminator;
        List<int> bits = StringToBits(message);
        int bitIndex = 0;
        int bitMessageLength = bits.Count;
        
        int width = bmpIn.Width - (bmpIn.Width % 8);
        int height = bmpIn.Height - (bmpIn.Height % 8);
        
        double[,] blockY = new double[8, 8];
        double[,] blockCb = new double[8, 8];
        double[,] blockCr = new double[8, 8];

        bool finished = false;

        for (int by = 0; by < height; by += 8)
        {
            for (int bx = 0; bx < width; bx += 8)
            {
                if (finished) break;

                GetBlocks(bmpIn, bx, by, blockY,  blockCr,  blockCb);
                double [,] dctY = dct8x8(blockY);
                
                if (bitIndex < bitMessageLength)
                {
                    int bit = bits[bitIndex];
                    HideBit(dctY, bit);
                    bitIndex++;
                    double [,] idctY = idct8x8(dctY);
                    SetBlocks(bmpOut, bx, by, idctY, blockCr, blockCb);
                }
                else
                {
                    finished = true;
                }
            }
            if (finished) break;
        }

        return (Image)bmpOut;
    }

    public static string Decrypt(Image input)
    {
        Bitmap bmpIn = new Bitmap(input);
        int width = bmpIn.Width - (bmpIn.Width % 8);
        int height = bmpIn.Height - (bmpIn.Height % 8);
        double[,] blockY = new double[8, 8];
        double[,] blockCb = new double[8, 8];
        double[,] blockCr = new double[8, 8];
        
        byte currentByte = 0;
        int bitCount = 0;
        List<byte> collectedBytes = new List<byte>();
        bool foundTerminator = false;

        for (int by = 0; by < height; by += 8)
        {
            for (int bx = 0; bx < width; bx += 8)
            {
                GetBlocks(bmpIn, bx, by, blockY,  blockCr,  blockCb);
                double [,] dctY = dct8x8(blockY);
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

        if (foundTerminator)
        {
            int termLen = Encoding.UTF8.GetByteCount(Terminator);
            collectedBytes.RemoveRange(collectedBytes.Count - termLen, termLen);
        }
        
        return Encoding.UTF8.GetString(collectedBytes.ToArray());
    }
}