// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Text;
using System.Linq;
using test;
using System.ComponentModel;

//MESSAGE PART


string message = "jaja";
string input = "test.png";

string output = "result.png";
LSB.EncryptPNGImage(input, output,message);

BitArray bits = LSB.DecryptPNGImage(output);
byte[] bytes = LSB.ToByteArray(bits);
string text = Encoding.UTF8.GetString(bytes);
Console.WriteLine(text);


/*
input = "nyan-cat.gif";
output = "nyan-cat-result.gif";

LSB.EncryptGIFImage(input,output, message);

Console.WriteLine("DECRYPTING...");

BitArray gifbits = LSB.DecryptGIFImage(output);
byte[] gifbytes = LSB.ToByteArray(gifbits);
string giftext = Encoding.UTF8.GetString(gifbytes);
Console.WriteLine(giftext);
*/
