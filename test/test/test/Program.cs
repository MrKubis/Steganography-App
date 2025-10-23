// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Text;
using System.Linq;
using test;
using System.ComponentModel;

//MESSAGE PART

string message = "Siemanko!!!";
string input = "test.png";
string output = "result.png";
LSB lsb = new LSB();
//lsb.EncryptImage(input, output,message);
BitArray bits = lsb.DecryptImage(output);
byte[] bytes = LSB.ToByteArray(bits);
string text = Encoding.UTF8.GetString(bytes);
Console.WriteLine(text);
