// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Paddings;

/// <summary>
/// Implements PKCS7 cipher padding: append that many bytes as needed to fill the given block size (but at least one), and each of them should have the padding length as value.
/// </summary>
/// <remarks>
/// The padding scheme itself is given in section 6.3. Content-encryption Process. It essentially says: append that many bytes as needed to fill the given block size (but at least one), and each of them should have the padding length as value.
/// Thus, looking at the last decrypted byte we know how many bytes to strip off. (One could also check that they all have the same value.)
/// 
/// </remarks>
/// <see cref="https://stackoverflow.com/questions/7314901/how-to-add-remove-pkcs7-padding-from-an-aes-encrypted-string"/>
/// <see cref="http://tools.ietf.org/html/rfc5652#section-6.3"/>
public class PKCS7Padding : IPadding
    {
    public byte[] Pad(byte[] input, int offset, int length, int paddingLength)
        {
        byte[] output = PaddingExtensions.Pad(input, offset, length, paddingLength);

        byte paddingLengthByte = (byte)paddingLength;

        for (int i = length; i < output.Length; i++)
            {
            output[i] = paddingLengthByte;
            }

        return output;
        }

    /// <summary>
    /// Get the padding length.
    /// </summary>
    /// <see cref="https://gist.github.com/bitbeans/fb2bf80a47b1af31b07c"/>
    public int GetPaddingLength(byte[] paddedBuffer, int offset, int length)
        {
        byte paddingLength = paddedBuffer[offset + length - 1];

        if (paddingLength > length)
            {
            return 0;     // there is nothing to unpad
            }

        int paddingStart = offset + length - paddingLength;

        // checks all the padding bytes for correct value
        for (int i = offset + length - 2; i >= paddingStart; i--)
            {
            if (paddedBuffer[i] != paddingLength)
                {
                return 0;     // there is nothing to unpad
                }
            }

        return paddingLength;
        }
    }
