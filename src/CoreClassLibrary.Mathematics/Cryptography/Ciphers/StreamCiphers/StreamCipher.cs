
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;

public class StreamCipher(IStreamCipher streamCipher) : ICipher
    {
    public void Initialize() => streamCipher.Initialize();

    public byte[] Encrypt(byte[] input, int offset, int count)
        {
        byte[] output = new byte[count];

        for (int i = 0; i < count; i++)
            {
            output[i] = streamCipher.Encrypt(input[offset + i]);
            }

        return output;
        }

    public byte[] Decrypt(byte[] input, int offset, int count)
        {
        byte[] output = new byte[count];

        for (int i = 0; i < count; i++)
            {
            output[i] = streamCipher.Decrypt(input[offset + i]);
            }

        return output;
        }
    }
