using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Threading.Channels;
using System.Drawing.Imaging;
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
                Bitmap bmp = new Bitmap(input_path);
                long jump = CalculateJump(bmp.Width * bmp.Height, Message.Bits.Count - 64);
                long jumpCounter = 0;
                for (int y=0; y < bmp.Height; y++)
                {
                    for(int x=0; x < bmp.Width; x++)
                    {
                        Color pixel = bmp.GetPixel(x, y);
                        byte r = pixel.R;
                        byte g = pixel.G;
                        byte b = pixel.B;

                        //Pierwsze 64 bity to segment długości - go k
                        if(i < 64)
                        {                      
                            r = SetLastBit(r, Message.Bits[i]);
                            i++;
                        }

                        else if (i <= Message.Bits.Count)
                        {
                            if(jumpCounter <jump)
                            {
                                jumpCounter++;
                            }
                            else
                            {
                                r = SetLastBit(r, Message.Bits[i]);
                                i++;
                                jumpCounter = 0;
                            }
                        }
                        bmp.SetPixel(x, y, Color.FromArgb(pixel.A,r,g,b));
                    }
                }
                Console.WriteLine("printed");
                bmp.Save(output_path);
            }
        }
        public static BitArray DecryptPNGImage(string input_path)
        {
            Bitmap bmp = new Bitmap(input_path);
            bool[] lengthArray = new bool[64];
            List<bool> bits_list = new List<bool>();

            //UWAGA - ZAKŁADAMY ZE ZDJĘCIE MA MINIMUM 64 PIXELI SZEROKOSCI
            //PIERWSZE 64 PIXELE - ZAPIS SEGMENTU DLUGOSCI

            for(int i = 0; i < 64;i++)
            {
                lengthArray[i] = (readLastBit(bmp.GetPixel(i, 0).R));
                i++;
            }
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
                        bits_list.Add(readLastBit(bmp.GetPixel(x, y).R));
                        jumpCounter = 0;
                    }
                }
            }
            return new BitArray(bits_list.ToArray());
        }

        public static void EncryptGIFImage(string input_path, string output_path, string message)
        {
            if (!File.Exists(input_path)) { Console.Write("File " + input_path + "doesn't exist"); return; }
            if (Path.GetExtension(input_path).ToLower() != ".gif") { Console.Write("Wrong extention!!"); return; }
            using (var imageCollection = new MagickImageCollection(input_path))
            {
                Message = new LSBMessage(message);
                //Liczenie skoku
                int totalPixels = 0;
                foreach (var frame in imageCollection)
                {
                    totalPixels += (int)(frame.Width * frame.Height);
                }

                int jump = (int)CalculateJump(totalPixels, Message.Bits.Count - 64);
                int jumpcounter = 0;
                int i = 0; //Wskazuje na którym bicie wiadomości jesteśmy

                //PIERWSZY FRAME - TAM KODUJEMY SEGMENT DŁUGOŚCI
                var firstframe = imageCollection[0];
                var firstpixels = firstframe.GetPixels();

                //Pierwsze 64 pixele w Y=0
                while (i < 64)
                {
                    //Tutaj robię konwersję z ushort na byte
                    ushort channel16 = firstpixels.GetPixel(i, 0).GetChannel(0);
                    byte channel8 = (byte)(channel16 / 257);
                    byte newChannel8 = SetLastBit(channel8, Message.Bits[i]);
                    ushort newChannel16 = (ushort)(newChannel8 * 257);
                    Console.Write(newChannel16 + " ");

                    firstpixels.SetPixel(i, 0, [newChannel16]);
                    i++;
                }

                for (int x = 64; x < firstframe.Width; x++)
                {
                    ushort r = firstpixels.GetPixel(x, 0).GetChannel(0);

                    if (jumpcounter >= jump && i < Message.Bits.Length)
                    {
                        byte channel8 = (byte)(r / 257);
                        byte newChannel8 = SetLastBit(channel8, Message.Bits[i]);
                        r = (ushort)(newChannel8 * 257);
                        jumpcounter = 0;
                        i++;
                    }
                    else
                    {
                        jumpcounter++;
                    }
                    firstpixels.SetPixel(x, 0, [r]);
                }

                for (int y = 1; y < firstframe.Height; y++)
                {
                    for (int x = 0; x < firstframe.Width; x++)
                    {
                        ushort r = firstpixels.GetPixel(x, y).GetChannel(0);

                        if (jumpcounter >= jump && i < Message.Bits.Length)
                        {
                            byte channel8 = (byte)(r / 257);
                            byte newChannel8 = SetLastBit(channel8, Message.Bits[i]);
                            r = (ushort)(newChannel8 * 257);
                            jumpcounter = 0;
                            i++;
                        }
                        else
                        {

                            jumpcounter++;
                        }
                        firstpixels.SetPixel(x, 0, [r]);
                    }
                }

                int frameCount = imageCollection.Count;

                //ITERUJEMY TERAZ OD FRAME1 AZ DO KONCA
                for (int frameIndex = 1; frameIndex < frameCount; frameIndex++)
                {
                    {
                        var frame = imageCollection[frameIndex];
                        var pixels = frame.GetPixels();
                        for (int y = 0; y < frame.Height; y++)
                        {
                            for (int x = 0; x < frame.Width; x++)
                            {
                                ushort r = pixels.GetPixel(x, y).GetChannel(0);
                                if (jumpcounter >= jump && i < Message.Bits.Length)
                                {
                                    byte channel8 = (byte)(r / 257);
                                    byte newChannel8 = SetLastBit(channel8, Message.Bits[i]);
                                    r = (ushort)(newChannel8 * 257);
                                    i++;
                                }
                                else
                                {
                                    jumpcounter++;
                                }

                                pixels.SetPixel(x, y, [r]);
                            }
                        }
                    }
                }
                imageCollection.Write(output_path);
            }
        }
        public static BitArray? DecryptGIFImage(string input_path)
        {
            if (!File.Exists(input_path)) { Console.Write("File " + input_path + "doesn't exist"); return null; }
            if (Path.GetExtension(input_path).ToLower() != ".gif") { Console.Write("Wrong extention!!"); return null; }

            List<bool> bits_list = new List<bool>();

            using (var imageCollection = new MagickImageCollection(input_path))
            {
                var firstframe = imageCollection[0];
                var firstpixels = firstframe.GetPixels();

                bool[] lengthArray = new bool[64];
                for (int i = 0; i < 64; i++)
                {
                    ushort r = firstpixels.GetPixel(i, 0).GetChannel(0);
                    Console.Write(r + " ");
                    byte channel8 = (byte)(r / 257);
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
