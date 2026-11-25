
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;

public class StreamEngineCipher(IStreamEngine encriptionEngine, IStreamEngine decriptionEngine) : IStreamCipher
    {
    public StreamEngineCipher(IStreamEngine engine) : this(engine, engine) { }

    public void Initialize()
        {
        encriptionEngine.Initialize();
        decriptionEngine.Initialize();
        }

     public byte Encrypt(byte input)
        => encriptionEngine.ProcessByte(input);

    public byte Decrypt(byte input)
        => decriptionEngine.ProcessByte(input);
    }
