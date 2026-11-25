
using CoreClassLibrary.Streams;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

/// <summary>
/// Implementa un ICipher con un CipherStream que encapsula un IBlockCipher.
/// </summary>
public class BlockCipherStreamCipher(IPaddedBlockCipher blockCipher) : ICipher
    {
    public void Initialize()
        => blockCipher.Initialize();

    public byte[] Encrypt(byte[] input, int offset, int length)
        => this.Process(input, offset, length, true);

    public byte[] Decrypt(byte[] input, int offset, int length)
        => this.Process(input, offset, length, false);

    public byte[] Process(byte[] input, int offset, int length, bool forEncryption)
        {
        using MemoryStream memoryStream = MemoryStreamManager.Instance.GetStream("BlockCipherStreamCipher.Process", length);
        using BlockCipherStream cipherStream = new BlockCipherStream(memoryStream, blockCipher, forEncryption);
        cipherStream.Write(input, offset, length);
        cipherStream.Flush();
        return memoryStream.ToArray();  //.GetBuffer()
        }
    }
