namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

public class BlockEngineCipher : IBlockCipher
    {
    public int BlockSize => this.encriptionEngine.BlockSize;

    private readonly IBlockEngine encriptionEngine;
    private readonly IBlockEngine decriptionEngine;

    public BlockEngineCipher(IBlockEngine engine) : this(engine, engine) { }

    public BlockEngineCipher(IBlockEngine encriptionEngine, IBlockEngine decriptionEngine)
        {
        if((this.encriptionEngine = encriptionEngine).BlockSize != (this.decriptionEngine = decriptionEngine).BlockSize)
            {
            throw new InvalidOperationException("Engine must have equals BlockSize");
            }
        }

    public void Initialize()
        {
        this.encriptionEngine.Initialize();
        this.decriptionEngine.Initialize();
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        => this.decriptionEngine.ProcessBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);

    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        => this.encriptionEngine.ProcessBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
    }    
