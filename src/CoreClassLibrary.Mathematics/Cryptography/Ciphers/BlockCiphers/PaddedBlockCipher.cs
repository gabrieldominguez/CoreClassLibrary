// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

/// <summary>
/// Base class for block cipher implementations.
/// </summary>
/// <remarks>
/// Si la longitud del buffer a encriptar no es multiplo del tamaño del bloque se debe utilizar padding. 
/// Si la longitud del buffer a encriptar es multiplo del tamaño del bloque puede especificarse null para el padding en el constructor.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="PaddedBlockCipher"/> class.
/// </remarks>
/// <param name="key">The key.</param>
/// <param name="blockSize">Size of the block.</param>
/// <param name="mode">Cipher mode.</param>
/// <param name="padding">Cipher padding (null for no-padding).</param>
public class PaddedBlockCipher(IBlockCipher blockCipher, IPadding? padding) : IPaddedBlockCipher
    {
    public int BlockSize => blockCipher.BlockSize;

    public void Initialize() 
        => blockCipher.Initialize();

    /// <summary>
    /// Encrypts the specified region of the input byte array and copies the encrypted data to the specified region of the output byte array.
    /// </summary>
    /// <param name="inputBuffer">The input data to encrypt.</param>
    /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
    /// <param name="outputBuffer">The output to which to write encrypted data.</param>
    /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
    /// <returns>
    /// The number of bytes encrypted.
    /// </returns>
    /// <exception cref="IndexOutOfRangeException"><paramref name="inputBuffer"/> or <paramref name="outputBuffer"/> is too short.</exception>
    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int blockSize, byte[] outputBuffer, int outputOffset)
        {
        PaddedBlockCipher.Validate(inputBuffer, inputOffset, blockSize, outputBuffer, outputOffset);
        blockCipher.EncryptBlock(inputBuffer, inputOffset, blockSize, outputBuffer, outputOffset);
        }

    /// <summary>
    /// Procesa el bloque final teniendo en cuenta el padding.
    /// </summary>
    /// <param name="inputBuffer">The input for which to compute the transform.</param>
    /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
    public byte[] EncryptFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
        byte[] outputBlock = new byte[this.BlockSize];

        if(inputCount < this.BlockSize)
            {
            byte[] inputBlock = new byte[inputCount];

            Buffer.BlockCopy(inputBuffer, inputOffset, inputBlock, 0, inputCount);

            if(padding != null)
                {
                byte[] paddedBlock = padding.Pad(this.BlockSize, inputBlock);
                this.EncryptBlock(paddedBlock, 0, this.BlockSize, outputBlock, 0);
                }
            }
        else
            {
            this.EncryptBlock(inputBuffer, inputOffset, inputCount, outputBlock, 0);
            }

        return outputBlock;
        }


    /// <summary>
    /// Decrypts the specified region of the input byte array and copies the decrypted data to the specified region of the output byte array.
    /// </summary>
    /// <param name="inputBuffer">The input data to decrypt.</param>
    /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
    /// <param name="outputBuffer">The output to which to write decrypted data.</param>
    /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="inputBuffer"/> or <paramref name="outputBuffer"/> is too short.</exception>
    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int blockSize, byte[] outputBuffer, int outputOffset)
        {
        PaddedBlockCipher.Validate(inputBuffer, inputOffset, blockSize, outputBuffer, outputOffset);
        blockCipher.DecryptBlock(inputBuffer, inputOffset, blockSize, outputBuffer, outputOffset);
        }

    /// <summary>
    /// Procesa el bloque final teniendo en cuenta el padding.
    /// </summary>
    /// <param name="inputBuffer">The input for which to compute the transform.</param>
    /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
    public byte[] DecryptFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
        if (inputCount == 0)
            {
            return [];
            }

        if (inputCount != this.BlockSize)
            {
            throw new InvalidOperationException(String.Format("Invalid data length: {0}", inputCount));
            }

        byte[] outputBlock = new byte[this.BlockSize];

        this.DecryptBlock(inputBuffer, inputOffset, inputCount, outputBlock, 0);

        return padding?.Unpad(outputBlock) ?? outputBlock;
        }


    private static void Validate(byte[] inputBuffer, int inputOffset, int blockSize, byte[] outputBuffer, int outputOffset)
        {
        if (inputOffset + blockSize > inputBuffer.Length)
            {
            throw new IndexOutOfRangeException("input buffer too short");
            }

        if (outputOffset + blockSize > outputBuffer.Length)
            {
            throw new IndexOutOfRangeException("output buffer too short");
            }

        // if (blockSize != BlockSize)
        //    {
        //    throw new ArgumentException("inputCount");
        //    }
        }
    }
