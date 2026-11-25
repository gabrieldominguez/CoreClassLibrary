
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Paddings;

/// <summary>
/// Pad with zero bytes to fill out the last block.
/// </summary>
public class ZerosPadding : IPadding
    {
    public byte[] Pad(byte[] input, int offset, int length, int paddingLength)
        => PaddingExtensions.Pad(input, offset, length, paddingLength);

    public int GetPaddingLength(byte[] paddedBuffer, int offset, int length)
        {
        int paddingLength = 0;

        for (int i = offset + length - 1; i >= offset; i--)
            {
            if (paddedBuffer[i] == 0)
                {
                paddingLength++;
                }
            else
                {
                return paddingLength;
                }
            }

        return paddingLength;
        }
    }
