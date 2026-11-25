// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

/// <summary>
/// Implementa un ICipher con un cifrador de bloques especificado.
/// </summary>
/// <remarks>
/// Usar CipherStream en lugar de esta clase.
/// 
/// Si la longitud del buffer a encriptar no es multiplo del tamaño del bloque se debe utilizar padding. 
/// Si cuando se desencripte se opta por eliminar el padding, esto implicará una copia de todo el buffer desencriptado (menos el padding).
/// Para evitarlo se puede no especificar CipherPadding en el constructor de BlockCipher al desencriptar y 
/// luego con CipherPadding.GetPaddingLength() obtener la longitud del padding para no procesarlo.
/// </remarks>
public class PaddedBlockCipherCipher(IPaddedBlockCipher blockCipher, bool usePadding) : ICipher
    {
    public void Initialize() 
        => blockCipher.Initialize();

    /// <summary>
    /// Devuelve cuanto será la longitud del padding de acuerdo al tamaño del bloque.
    /// </summary>
    /// <remarks>
    /// Si la longitud del buffer a encriptar es multiplo del tamaño del bloque y se usa padding, 
    /// se debe agregar un bloque mas, por lo que paddingLength == this.blockCipher.BlockSize. 
    /// </remarks>
    private int GetPaddingLength(int inputCount)
        {
        int paddingLength = blockCipher.BlockSize - inputCount % blockCipher.BlockSize;

        if (!usePadding)
            {
            if (paddingLength == blockCipher.BlockSize)
                {
                return 0;
                }

            throw new InvalidOperationException(String.Format("Invalid data length ({0}) and no padding provided.", inputCount));
            }

        return paddingLength;
        }

    /// <summary>
    /// Encrypts the specified data.
    /// </summary>
    /// <param name="inputBuffer">The data.</param>
    /// <param name="inputOffset">The zero-based offset in <paramref name="inputBuffer"/> at which to begin encrypting.</param>
    /// <param name="inputCount">The number of bytes to encrypt from <paramref name="inputBuffer"/>.</param>
    /// <returns>Encrypted data</returns>
    public byte[] Encrypt(byte[] inputBuffer, int inputOffset, int inputCount)
        {
        byte[] output = new byte[inputCount + this.GetPaddingLength(inputCount)];

        int outputOffset = 0;
        int blockCount = (int)Math.Truncate((double)inputCount / blockCipher.BlockSize);
        for (int i = 0; i < blockCount; i++)
            {
            blockCipher.EncryptBlock(inputBuffer, inputOffset, blockCipher.BlockSize, output, outputOffset);

            inputOffset += blockCipher.BlockSize;
            outputOffset += blockCipher.BlockSize;
            }

        if (usePadding)
            {
            byte[] finalOutput = blockCipher.EncryptFinalBlock(inputBuffer, inputOffset, inputCount % blockCipher.BlockSize);
            Buffer.BlockCopy(finalOutput, 0, output, outputOffset, finalOutput.Length);
            }

        return output;
        }


    /// <summary>
    /// Decrypts the specified input.
    /// </summary>
    /// <remarks>
    /// Si se usó padding al encriptar, todo el buffer de salida es copiado al quitarselo.
    /// </remarks>
    /// <param name="inputBuffer">The input.</param>
    /// <param name="inputOffset">The zero-based offset in <paramref name="inputBuffer"/> at which to begin decrypting.</param>
    /// <param name="inputCount">The number of bytes to decrypt from <paramref name="inputBuffer"/>.</param>
    /// <returns>
    /// The decrypted data.
    /// </returns>
    public byte[] Decrypt(byte[] inputBuffer, int inputOffset, int inputCount)
        {
        byte[] output = new byte[inputCount];

        int outputOffset = 0;
        int blockCount = inputCount / blockCipher.BlockSize;

        if (usePadding)
            {
            blockCount--;
            }

        for (int i = 0; i < blockCount; i++)
            {
            blockCipher.DecryptBlock(inputBuffer, inputOffset, blockCipher.BlockSize, output, outputOffset);

            inputOffset += blockCipher.BlockSize;
            outputOffset += blockCipher.BlockSize;
            }

        if (usePadding)
            {
            byte[] finalOutput = blockCipher.DecryptFinalBlock(inputBuffer, inputOffset, blockCipher.BlockSize);
            byte[] paddedOutput = new byte[outputOffset + finalOutput.Length];

            Buffer.BlockCopy(output, 0, paddedOutput, 0, outputOffset);
            Buffer.BlockCopy(finalOutput, 0, paddedOutput, outputOffset, finalOutput.Length);

            return paddedOutput;
            }

        return output;
        }
    }
