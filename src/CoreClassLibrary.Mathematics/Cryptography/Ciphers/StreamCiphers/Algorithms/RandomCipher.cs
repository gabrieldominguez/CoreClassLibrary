using CoreClassLibrary.Mathematics.Randomness;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

/// <summary>
/// Encripta usando IRandom.
/// </summary>
/// <remarks>
/// Solo como demostracion del uso de random para encriptado, para un uso productivo usar AES.
/// </remarks>
/// <see cref="System.Security.Cryptography.Aes"/>
public sealed class RandomCipher(Func<IRandom> randomFactory) : XorCipherBase(new RandomByteProvider(randomFactory));
public sealed class RandomEngine(Func<IRandom> randomFactory) : XorEngineBase(new RandomByteProvider(randomFactory));

public sealed class RandomByteProvider(Func<IRandom> randomFactory) : ISampleProvider<byte>
    {
    private IRandom random = randomFactory();
    
    public void Initialize()
        => this.random = randomFactory();

    public byte GetNext() 
        => this.random.Next<byte>();
    }
