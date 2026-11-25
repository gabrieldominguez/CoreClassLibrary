
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers;

public class CompositeCipher(IReadOnlyList<ICipher> ciphers) : ICipher
    {
    public void Initialize()
        {
        foreach (ICipher cipher in ciphers)
            {
            cipher.Initialize();
            }
        }


    /// <summary>
    /// Encrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin encrypting.</param>
    /// <param name="length">The number of bytes to encrypt from <paramref name="input"/>.</param>
    /// <returns>
    /// The encrypted data.
    /// </returns>
    public byte[] Encrypt(byte[] input, int offset, int length)
        {
        byte[] temp = ciphers[0].Encrypt(input, offset, length);

        for (int i = 1; i < ciphers.Count; i++)
            {
            temp = ciphers[i].Encrypt(temp, 0, temp.Length);
            }

        return temp;
        }

    /// <summary>
    /// Decrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin decrypting.</param>
    /// <param name="length">The number of bytes to decrypt from <paramref name="input"/>.</param>
    /// <returns>
    /// The decrypted data.
    /// </returns>
    public byte[] Decrypt(byte[] input, int offset, int length)
        {
        byte[] temp = ciphers[^1].Decrypt(input, offset, length);

        for (int i = ciphers.Count - 2; i >= 0; i--)
            {
            temp = ciphers[i].Decrypt(temp, 0, temp.Length);
            }

        return temp;
        }
    }
