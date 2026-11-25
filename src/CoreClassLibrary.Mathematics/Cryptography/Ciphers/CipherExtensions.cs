
using CoreClassLibrary.Extensions;
using System.Text;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers;

public static class CipherExtensions
    {
    public static string Encrypt(this ICipher cipher, string input)
        => cipher.Encrypt(input, StringExtensions.UTF8Encoding);

    public static string Decrypt(this ICipher cipher, string input)
        => cipher.Decrypt(input, StringExtensions.UTF8Encoding);

    public static string Encrypt(this ICipher cipher, string input, Encoding encoding)
        {
        byte[] inputBytes = encoding.GetBytes(input);

        string output = cipher.Encrypt(inputBytes).ToHexString();

        inputBytes.Clear();

        return output;
        }

    public static string Decrypt(this ICipher cipher, string input, Encoding encoding)
        {
        byte[] ouputBytes = cipher.Decrypt(input.FromHexString());

        string output = encoding.GetString(ouputBytes);

        ouputBytes.Clear();

        return output;
        }

    public static byte[] Encrypt(this ICipher cipher, byte[] input)
        => cipher.Encrypt(input, 0, input.Length);

    public static byte[] Decrypt(this ICipher cipher, byte[] input)
        => cipher.Decrypt(input, 0, input.Length);
    }
