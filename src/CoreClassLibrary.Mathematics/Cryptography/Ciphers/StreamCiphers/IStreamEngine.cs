
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;

public interface IStreamEngine
    {
    void Initialize();

    /// <summary>
    /// Process (Encrypts/Decrypts) the specified input.
    /// </summary>
    byte ProcessByte(byte input);
    }
