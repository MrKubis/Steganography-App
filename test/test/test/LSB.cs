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
                        bmp.SetPixel(x, y, Color.FromArgb(pixel.A,r,g,b));
                    }
                }
                bmp.Save(output_path);
            }
        }
        public BitArray DecryptImage(string input_path)
        {
            bool[] lengthArray = new bool[64];
            int i = 0;
            Bitmap bmp = new Bitmap(input_path);
            List<bool> bits_list = new List<bool>();
            string result = "";

            //UWAGA - ZAKŁADAMY ZE ZDJĘCIE MA MINIMUM 64 PIXELI SZEROKOSCI
            //PIERWSZE 64 PIXELE - ZAPIS SEGMENTU DLUGOSCI

            while (i < 64)
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
                        bits_list.Add(readLastBit(bmp.GetPixel(x, y).R));
                        jumpCounter = 0;
                        i++;
                    }
                }
            }
            return new BitArray(bits_list.ToArray());
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
