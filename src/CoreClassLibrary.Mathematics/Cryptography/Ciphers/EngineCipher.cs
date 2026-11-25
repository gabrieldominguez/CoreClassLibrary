namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers;

public class EngineCipher(IEngine encriptionEngine, IEngine decriptionEngine) : ICipher
    {
    public EngineCipher(IEngine engine) : this(engine, engine) { }

    public void Initialize()
        {
        encriptionEngine.Initialize();
        decriptionEngine.Initialize();
        }

    public byte[] Encrypt(byte[] input, int offset, int length)
        => encriptionEngine.Process(input, offset, length);

    public byte[] Decrypt(byte[] input, int offset, int length)
        => decriptionEngine.Process(input, offset, length);
    }
