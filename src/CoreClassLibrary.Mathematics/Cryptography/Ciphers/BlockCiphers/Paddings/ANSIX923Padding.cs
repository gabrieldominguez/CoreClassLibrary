
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Paddings;

/// <summary>
/// Implements ANSIX923 cipher padding: Fills the bytes with zeros and puts the number of padding bytes in the last byte.
/// </summary>
public class ANSIX923Padding : IPadding
    {
    public byte[] Pad(byte[] input, int offset, int length, int paddingLength)
        {
        byte[] output = PaddingExtensions.Pad(input, offset, length, paddingLength);

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
