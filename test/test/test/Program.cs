// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Text;
using System.Linq;
using test;
using System.ComponentModel;

//MESSAGE PART


string message = "piwo";
string input = "auto.png";
string output = "result.png";
/*
LSB.EncryptPNGImage(input, output,message);
*/

BitArray bits = LSB.DecryptPNGImage(output);
byte[] bytes = LSB.ToByteArray(bits);
string text = Encoding.UTF8.GetString(bytes);
Console.WriteLine(text);
