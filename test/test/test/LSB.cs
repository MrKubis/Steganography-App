using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading.Channels;
using System.Runtime.InteropServices;
using ImageMagick;
namespace test
{
    internal static class LSB
    {
        private static LSBMessage _message;
        private static LSBMessage Message { get { return _message; } set { _message = value; } }
        public static void EncryptPNGImage(string input_path,string output_path,string message)
        {
            if (File.Exists(input_path))
            {
                int i = 0;
                Message = new LSBMessage(message);
                var bmp = new MagickImage(input_path);

                long jump = CalculateJump(bmp.Width * bmp.Height, Message.Bits.Count - 64);
                long jumpCounter = 0;
                var pixels = bmp.GetPixelsUnsafe();
                while (i < 64)
                {
                    var pixel = pixels.GetPixel(i, 0);
                    var r = pixel.GetChannel(0);
                    r = SetLastBit(r, Message.Bits[i]);

                    pixel.SetChannel(0, r);
                    Console.Write(Message.Bits[i] ? "1" : "0");
                    i++;
                    pixels.SetPixel(pixel);
                }
                Console.Write("|");
                for (int x = 64; x < bmp.Width; x++)
                {
                    var pixel = pixels.GetPixel(x, 0);
                    var r = pixel.GetChannel(0);
                    if (i <= Message.Bits.Count)
                    {
                        if (jumpCounter < jump)
                        {
                            jumpCounter++;
                        }
                        else
                        {
                            r = SetLastBit(r, Message.Bits[i]);
                            Console.Write(Message.Bits[i] ? "1" : "0");

                            i++;
                            jumpCounter = 0;
                        }
                    }
                    pixel.SetChannel(0, r);
                    pixels.SetPixel(pixel);
                }
                for (int y=1; y < bmp.Height; y++)
                {
                    for(int x=0; x < bmp.Width; x++)
                    {
                        var pixel = pixels.GetPixel(x, y);
                        var r = pixel.GetChannel(0);
                        if (jumpCounter < jump)
                        {
                            jumpCounter++;

                        }
                        
                        else if (i <= Message.Bits.Count)
                        {
                            r = SetLastBit(r, Message.Bits[i]);

                            Console.Write(Message.Bits[i] ? "1" : "0");

                            i++;
                            jumpCounter = 0;

                        }
                        pixel.SetChannel(0, r);
                        pixels.SetPixel(pixel);
                    }
                }
                bmp.Write(output_path);
                Console.WriteLine("printed");
            }
        }
        public static BitArray DecryptPNGImage(string input_path)
        {
            bool[] lengthArray = new bool[64];
            int i = 0;
            var bmp = new MagickImage(input_path);
            List<bool> bits_list = new List<bool>();

            //UWAGA - ZAKŁADAMY ZE ZDJĘCIE MA MINIMUM 64 PIXELI SZEROKOSCI
            //PIERWSZE 64 PIXELE - ZAPIS SEGMENTU DLUGOSCI

            var pixels = bmp.GetPixels();

            while (i < 64)
            {
                var pixel = pixels.GetPixel(i, 0);
                var r = pixel.GetChannel(0);
                lengthArray[i] = readLastBit(r);
                Console.Write(readLastBit(r) ? "1" : "0");
                i++;
            }
            Console.Write("|");
            long rawmessagelength = readBitsToLong(lengthArray);
            //TUTAJ RAZY 8 BO BITY
            long jump = CalculateJump(bmp.Width * bmp.Height, rawmessagelength * 8);
            long jumpCounter = 0;
            for (int x = 64; x < bmp.Width; x++)
                {
                if (jumpCounter < jump)
                {
                    jumpCounter++;
                }
                else
                {
                    var pixel = pixels.GetPixel(x, 0);
                    var r = pixel.GetChannel(0);
                    bits_list.Add(readLastBit(r));
                    Console.Write(readLastBit(r) ? "1" : "0");
                    jumpCounter = 0;
                }
            }
            for (int y = 1; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (jumpCounter < jump)
                    {
                        jumpCounter++;
                    }
                    else
                    {
                        var pixel = pixels.GetPixel(x, y);
                        var r = pixel.GetChannel(0);
                        bits_list.Add(readLastBit(r));
                        Console.Write(readLastBit(r) ? "1" : "0");

                        jumpCounter = 0;
                        i++;
                    }
                }
            }
            return new BitArray(bits_list.ToArray());
        }

        private static byte SetLastBit(byte value, bool bit)
        {
            return (byte)((value & 0b_11111110) | (bit ? 1 : 0));
        }
        private static long CalculateJump(long pixelCount,long rawmessagelength)
        {
            return (long)(pixelCount - 64)/(rawmessagelength +1);
        }

        private static bool readLastBit(byte b)
        {
            return b % 2 != 0;
        }
        private static long readBitsToLong(bool[] bits)
        {
            Array.Reverse(bits);
            long result = 0;
            for(int i = 0 ;  i < bits.Length; i ++)
            {
                if (bits[i])
                {
                    result += (long)Math.Pow(2,i);
                }
            }
            return result;
        }
        public static byte[] ToByteArray(BitArray bits)
        {
            byte[] reversed_bytes = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(reversed_bytes, 0);
            byte[] result = new byte[reversed_bytes.Length];
            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < reversed_bytes.Length;i++)
                {
                    result[i] = ReverseByte(reversed_bytes[i]);   
                }
            }
            return result;
        }
        public static byte ReverseByte(byte b)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                result <<= 1;
                result |= (byte)(b & 1);
                b >>= 1;
            }
            return result;
        }
    }
}
