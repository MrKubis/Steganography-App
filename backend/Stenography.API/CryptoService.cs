using System;
using System.Security.Cryptography;
using System.Text;

namespace Stenography.API;

public class CryptoService
{
    private readonly byte[] _key;

    public CryptoService(string myKey)
    {
        _key = Convert.FromBase64String(myKey);
    }

    public string Encrypt(string plainText)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        int nonceSize = 12;
        int tagSize = 16;
        int cipherSize = plainBytes.Length;

        int encryptedDataLength = nonceSize + cipherSize + tagSize;
        byte[] encryptedData = new byte[encryptedDataLength];

        Span<byte> encryptedDataSpan = encryptedData.AsSpan();
        Span<byte> nonce = encryptedDataSpan.Slice(0, nonceSize);
        Span<byte> cipherText = encryptedDataSpan.Slice(nonceSize, cipherSize);
        Span<byte> tag = encryptedDataSpan.Slice(nonceSize + cipherSize, tagSize);

        RandomNumberGenerator.Fill(nonce);

        using (var aes = new AesGcm(_key, tagSize))
        {
            aes.Encrypt(nonce, plainBytes, cipherText, tag);
        }

        return Convert.ToBase64String(encryptedData);
    }

    public string Decrypt(string encryptedBase64)
    {
        byte[] encryptedData = Convert.FromBase64String(encryptedBase64);
        
        int nonceSize = 12;
        int tagSize = 16;
        int cipherSize = encryptedData.Length - nonceSize - tagSize;

        Span<byte> encryptedDataSpan = encryptedData.AsSpan();
        Span<byte> nonce = encryptedDataSpan.Slice(0, nonceSize);
        Span<byte> cipherText = encryptedDataSpan.Slice(nonceSize, cipherSize);
        Span<byte> tag = encryptedDataSpan.Slice(nonceSize + cipherSize, tagSize);

        byte[] plainBytes = new byte[cipherSize];

        using (var aes = new AesGcm(_key, tagSize))
        {
            aes.Decrypt(nonce, cipherText, tag, plainBytes);
        }

        return Encoding.UTF8.GetString(plainBytes);
    }

    public static string GenerateKey()
    {
        byte[] key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }
}