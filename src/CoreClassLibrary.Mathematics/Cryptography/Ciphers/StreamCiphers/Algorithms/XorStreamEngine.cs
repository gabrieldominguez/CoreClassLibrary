
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

public sealed class XorCipher(byte[] key) : XorCipherBase(new CircularProvider<byte>(key.CloneArray()))
    {
    }

public sealed class XorEngine(byte[] key) : XorEngineBase(new CircularProvider<byte>(key.CloneArray()))
    {
    }

public class XorCipherBase(ISampleProvider<byte> byteProvider) : StreamEngineCipher(new XorStreamEngine(byteProvider))
    {
    }

public class XorEngineBase(ISampleProvider<byte> byteProvider) : StreamEngine(new XorStreamEngine(byteProvider))
    {
    }

/// <summary>
/// Implements XOR cipher algorithm
/// </summary>
public class XorStreamEngine(ISampleProvider<byte> byteProvider) : IStreamEngine
    {
    public void Initialize() => byteProvider.Initialize();

    public byte ProcessByte(byte input)
        => (byte)(input ^ byteProvider.GetNext());
    }
