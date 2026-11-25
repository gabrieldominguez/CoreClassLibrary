
using CoreClassLibrary.Streams;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;

/// <summary>
/// Implementa un ICipher con un CipherStream que encapsula un IBlockCipher.
/// </summary>
public class StreamCipherStreamCipher(IStreamCipher streamCipher) : ICipher
    {
    public void Initialize()
        => streamCipher.Initialize();

    public byte[] Encrypt(byte[] input, int offset, int length)
        => this.Process(input, offset, length, true);

    public byte[] Decrypt(byte[] input, int offset, int length)
        => this.Process(input, offset, length, false);

    public byte[] Process(byte[] input, int offset, int length, bool forEncryption)
        {
        using MemoryStream memoryStream = MemoryStreamManager.Instance.GetStream("StreamCipherStreamCipher.Process", length);
        using StreamCipherStream cipherStream = new StreamCipherStream(memoryStream, streamCipher, forEncryption);
        cipherStream.Write(input, offset, length);
        cipherStream.Flush();
        return memoryStream.ToArray();  //.GetBuffer()
        }
    }
