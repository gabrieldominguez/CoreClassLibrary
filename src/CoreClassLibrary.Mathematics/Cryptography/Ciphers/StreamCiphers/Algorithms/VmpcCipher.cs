using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

public sealed class VmpcCipher(byte[] key, byte[] iv) : XorCipherBase(new VmpcByteProvider(key, iv))
    {
    }

public sealed class VmpcEngine(byte[] key, byte[] iv) : XorEngineBase(new VmpcByteProvider(key, iv))
    {
    }

/// <summary>
/// VMPC (Variably Modified Permutation Composition) stream cipher.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Variably_Modified_Permutation_Composition"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/VMPCEngine.cs"/>
public class VmpcByteProvider : CipherBase, ISampleProvider<byte>
    {
    /*
    * variables to hold the state of the VMPC engine during encryption and
    * decryption
    */
    protected byte n = 0;
    protected byte[] P = new byte[256];
    protected byte s = 0;

    protected byte[] iv;

    public VmpcByteProvider(byte[] key, byte[] iv) : base(key)
        {
        if (iv.Length < 1 || iv.Length > 768)
            {
            throw new ArgumentException("VMPC requires 1 to 768 bytes of IV");
            }

        this.iv = iv.CloneArray();

        this.Initialize();
        }

    public override void Initialize()
        {
        n = 0;
        s = 0;
        P = new byte[256];
        for (int i = 0; i < 256; i++)
            {
            P[i] = (byte)i;
            }

        this.Initialize(this.key);
        this.Initialize(this.iv);
        }

    protected void Initialize(byte[] bytes)
        {
        for (int m = 0; m < 768; m++)
            {
            s = P[(s + P[m & 0xff] + bytes[m % bytes.Length]) & 0xff];
            (P[s & 0xff], P[m & 0xff]) = (P[m & 0xff], P[s & 0xff]);
            }
        }

    public byte GetNext()
        {
        s = P[(s + P[n & 0xff]) & 0xff];
        byte z = P[(P[(P[s & 0xff]) & 0xff] + 1) & 0xff];

        (P[s & 0xff], P[n & 0xff]) = (P[n & 0xff], P[s & 0xff]);
        n = (byte)((n + 1) & 0xff);

        return z;
        }
    }
