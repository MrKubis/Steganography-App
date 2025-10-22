// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Text;
using System.Linq;
using test;

//MESSAGE PART

string message = "Siemanko!!!";
string input = "test.jpg";
string output = "result.jpg";
LSB lsb = new LSB();
lsb.EncryptImage(input, output,message);
lsb.DecryptImage(output);
