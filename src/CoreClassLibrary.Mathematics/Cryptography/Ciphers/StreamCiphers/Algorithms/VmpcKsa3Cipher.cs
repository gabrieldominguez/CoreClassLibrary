
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

public sealed class VmpcKsa3Cipher(byte[] key, byte[] iv) : XorCipherBase(new VmpcKsa3ByteProvider(key, iv))
    {
    }

public sealed class VmpcKsa3Engine(byte[] key, byte[] iv) : XorEngineBase(new VmpcKsa3ByteProvider(key, iv))
    {
    }

/// <summary>
/// VmpcKsa3 (Variably Modified Permutation Composition) stream cipher.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Variably_Modified_Permutation_Composition"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/VmpcKsa3Engine.cs"/>
public class VmpcKsa3ByteProvider(byte[] key, byte[] iv) : VmpcByteProvider(key, iv), ISampleProvider<byte>
    {
    public override void Initialize()
        {
        base.Initialize();

        this.Initialize(this.key);
        }
    }
