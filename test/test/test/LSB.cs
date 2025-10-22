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
namespace test
{
    //TUTAJ CAŁY ALGORYTM

    internal class LSB
    {
        private LSBMessage _message;
        public LSBMessage Message { get { return _message; } set { _message = value; } }
        public LSB() { }

        public void EncryptImage(string input_path,string output_path,string message)
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
                            BitArray bits = new BitArray(r);
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

                        //JPG

                        bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                        //PNG
                        //bmp.SetPixel(x, y, Color.FromArgb(pixel.A,r,g,b));
                    }
                }
                Console.WriteLine("Encoding statistics:");
                Console.WriteLine("Image length: " + bmp.Height * bmp.Width);
                Console.WriteLine("Maximum message length (bytes): " + bmp.Height * bmp.Width / 8);
                Console.WriteLine("Jump length: " + jump);
                Console.Write("Message: ");
                Console.WriteLine(Message.ToString());
                Console.WriteLine("Raw message length: " + (i - 64 + 1) / 8);
                Console.WriteLine("Bits encoded: " + i);
                bmp.Save(output_path);
            }
        }
        public string DecryptImage(string input_path)
        {
            bool[] lengthArray = new bool[64];
            int i = 0;
            Bitmap bmp = new Bitmap(input_path);

            string result = "";
            //UWAGA - ZAKŁADAMY ZE ZDJĘCIE MA MINIMUM 64 PIXELI SZEROKOSCI
            //PIERWSZE 64 PIXELE - ZAPIS SEGMENTU DLUGOSCI

                while (i < 64)
                {
                    lengthArray[i] = (readLastBit(bmp.GetPixel(i, 0).R));
                    result += readLastBit(bmp.GetPixel(i, 0).R) ? "1" : "0";
                    i++;
                }
            long rawmessagelength = readBitsToLong(lengthArray);
            //TUTAJ RAZY 8 BO BITY
            long jump = CalculateJump(bmp.Width * bmp.Height, rawmessagelength * 8);
            long jumpCounter = 0;
            Console.WriteLine("Message: " + result);
            Console.WriteLine("Raw message length: " + rawmessagelength);

            for (int x = 64; x < bmp.Width; x++)
                {
                if (jumpCounter < jump)
                {
                    jumpCounter++;
                    i++;
                }
                else
                {
                    jumpCounter = 0;
                    i++;
                }
            }
            for (int y = 1; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                        if (jumpCounter < jump)
                        {
                            jumpCounter++;
                            i++;
                        }
                        else
                        {
                            result += readLastBit(bmp.GetPixel(x, y).R) ? "1" : "0";
                            jumpCounter = 0;
                            i++;
                        }               
                }
            }
            Console.WriteLine("Decoding statistics:");
            Console.WriteLine("Jump length: " + jump);
            Console.WriteLine("Message: " + result);
            return "";
        }
        private byte SetLastBit(byte value, bool bit)
        {
            return (byte)((value & 0b_11111110) | (bit ? 1 : 0));
        }
        private long CalculateJump(long pixelCount,long rawmessagelength)
        {
            return (long)(pixelCount - 64)/(rawmessagelength +1);
        }

        private bool readLastBit(byte b)
        {
            return b % 2 != 0;

        }
        private long readBitsToLong(bool[] bits)
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

    }
}
