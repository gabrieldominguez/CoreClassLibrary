
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

public class CompositeBlockCipher : IBlockCipher
    {
    private readonly IReadOnlyList<IBlockCipher> blockCiphers;

    public int BlockSize { get; }

    public CompositeBlockCipher(IReadOnlyList<IBlockCipher> blockCiphers)
        {
        this.BlockSize = (this.blockCiphers = blockCiphers)[0].BlockSize;

        foreach (IBlockCipher blockCipher in blockCiphers)
            {
            if (blockCipher.BlockSize != this.BlockSize)
                {
                throw new InvalidOperationException("All block ciphers must have the same block size.");
                }
            }
        }

    public void Initialize()
        {
        foreach (IBlockCipher blockCipher in this.blockCiphers)
            {
            blockCipher.Initialize();
            }
        }

    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.blockCiphers[0].EncryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);

        byte[] tempBuffer = new byte[inputCount];

        for (int i = 1; i < this.blockCiphers.Count; i++)
            {
            Buffer.BlockCopy(outputBuffer, outputOffset, tempBuffer, 0, inputCount);
            this.blockCiphers[i].EncryptBlock(tempBuffer, 0, inputCount, outputBuffer, outputOffset);
            }
        }

    /// <summary>
    /// Decrypts the specified region of the input byte array and copies the decrypted data to the specified region of the output byte array.
    /// </summary>
    /// <param name="inputBuffer">The input data to decrypt.</param>
    /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
    /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
    /// <param name="outputBuffer">The output to which to write decrypted data.</param>
    /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.blockCiphers[^1].DecryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);

        byte[] tempBuffer = new byte[inputCount];

        for (int i = this.blockCiphers.Count - 2; i >= 0; i--)
            {
            Buffer.BlockCopy(outputBuffer, outputOffset, tempBuffer, 0, inputCount);
            this.blockCiphers[i].DecryptBlock(tempBuffer, 0, inputCount, outputBuffer, outputOffset);
            }
        }
    }
