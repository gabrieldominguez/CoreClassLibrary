namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

/// <summary>
/// Encapsula un stream como destino de operaciones de encriptacion y desencriptacion.
/// </summary>
/// <see cref="PaddedBlockCipher"/>
/// <see cref="https://github.com/ORelio/Minecraft-Console-Client/blob/master/MinecraftClient/Crypto/Streams/BouncyAes/CipherStream.cs"/>
/// <see cref="https://referencesource.microsoft.com/#mscorlib/system/security/cryptography/cryptostream.cs"/>
public class BlockCipherStream : Stream
    {
    private readonly Stream stream;
    private readonly IPaddedBlockCipher blockCipher;
    private readonly bool forEncryption;

    public BlockCipherStream(Stream stream, IPaddedBlockCipher blockCipher, bool forEncryption)
        {
        this.stream = stream;
        (this.blockCipher = blockCipher).Initialize();
        this.forEncryption = forEncryption;
        }

    public override long Position { get => this.stream.Position; set => this.stream.Position = value; }
    public override bool CanSeek => this.stream.CanSeek;
    public override long Seek(long offset, SeekOrigin origin)
        => this.stream.Seek(offset, origin);

    public override long Length => this.stream.Length;
    public override void SetLength(long value)
        => this.stream.SetLength(value);

    public override bool CanRead => false;
    public override int Read(byte[] output, int outputOffset, int outputCount)
        => throw new InvalidOperationException("Stream does not support reading.");

    public override bool CanWrite => this.stream.CanWrite;

    public void Write(byte[] input)
        => this.Write(input, 0, input.Length);

    public void Write(byte[] input, int offset)
        => this.Write(input, offset, input.Length - offset);

    // Summary:
    //     Encrypt or decrypt a sequence of bytes and writes it to the current stream.
    //
    // Parameters:
    //   buffer:
    //     An array of bytes. This method encrypt or decrypt count bytes from buffer to the current stream.
    //
    //   offset:
    //     The zero-based byte offset in buffer at which to encrypting or decrypting copying bytes to the current stream.
    //
    //   count:
    //     The number of bytes to be written to the current stream.
    public override void Write(byte[] input, int offset, int count)
        {
        byte[] outputBlock = new byte[this.blockCipher.BlockSize];

        if (this.forEncryption)
            {
            int blockCount = (int)Math.Truncate((double)count / this.blockCipher.BlockSize);
            for (int i = 0; i < blockCount; i++)
                {
                this.blockCipher.EncryptBlock(input, offset, this.blockCipher.BlockSize, outputBlock, 0);
                this.stream.Write(outputBlock);

                offset += this.blockCipher.BlockSize;
                }

            byte[] finalOutput = this.blockCipher.EncryptFinalBlock(input, offset, count % this.blockCipher.BlockSize);
            this.stream.Write(finalOutput);
            }
        else
            {
            int blockCount = count / this.blockCipher.BlockSize - 1;
            for (int i = 0; i < blockCount; i++)
                {
                this.blockCipher.DecryptBlock(input, offset, this.blockCipher.BlockSize, outputBlock, 0);
                this.stream.Write(outputBlock);

                offset += this.blockCipher.BlockSize;
                }

            byte[] finalOutput = this.blockCipher.DecryptFinalBlock(input, offset, this.blockCipher.BlockSize);
            this.stream.Write(finalOutput);
            }
        }

    public override void Flush() 
        => this.stream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) 
        => this.stream.FlushAsync(cancellationToken);

    public override void Close() 
        => this.stream.Close();
    }
