
using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Paddings;

/// <summary>
/// Implements ISO 10126 cipher padding.
///     Same as PKCS7 except that it fills the bytes before the last one with random values.
///     or
///     Same as ANSIX923 except that it fills the bytes with random values instead of zeros. 
/// </summary>
public class ISO10126Padding(RandomNumberGenerator randomNumberGenerator) : IPadding
    {
    public byte[] Pad(byte[] input, int offset, int length, int paddingLength)
        {
        byte[] output = PaddingExtensions.Pad(input, offset, length, paddingLength);

        randomNumberGenerator.GetBytes(output, length, paddingLength - 1);

        output[output.Length - 1] = (byte)paddingLength;

        return output;
        }

    public int GetPaddingLength(byte[] paddedBuffer, int offset, int length)
        {
        byte paddingLength = paddedBuffer[offset + length - 1];

        if (paddingLength > length)
            {
            return 0;     // there is nothing to unpad
            }

        return paddingLength;
        }
    }
