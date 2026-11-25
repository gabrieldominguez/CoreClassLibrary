namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

/// <summary>
/// HC-128 is a software-efficient stream cipher created by Hongjun Wu. It
/// generates keystream from a 128-bit secret key and a 128-bit initialization vector.
/// 
/// It is a third phase candidate in the eStream contest, and is patent-free.
/// No attacks are known as of today (April 2007). 
/// </summary>
/// <see cref="http://www.ecrypt.eu.org/stream/hcp3.html"/>
/// <see cref="http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc128_p3.pdf"/>
/// <see cref="http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc256_p3.pdf"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/HC128Engine.cs"/>
public sealed class HC128Cipher(byte[] key, byte[] iv) : XorCipherBase(new HC128ByteProvider(key, iv))
    {
    }

public sealed class HC128Engine(byte[] key, byte[] iv) : XorEngineBase(new HC128ByteProvider(key, iv))
    {
    }

public sealed class HC128ByteProvider : CipherBase, ISampleProvider<byte>
    {
    private readonly uint[] p = new uint[512];
    private readonly uint[] q = new uint[512];
    private uint cnt = 0;

    private static uint F1(uint x) => RotateRight(x, 7) ^ RotateRight(x, 18) ^ (x >> 3);

    private static uint F2(uint x) => RotateRight(x, 17) ^ RotateRight(x, 19) ^ (x >> 10);

    private uint G1(uint x, uint y, uint z) => (RotateRight(x, 10) ^ RotateRight(z, 23)) + RotateRight(y, 8);

    private uint G2(uint x, uint y, uint z) => (RotateLeft(x, 10) ^ RotateLeft(z, 23)) + RotateLeft(y, 8);

    private static uint RotateLeft(uint x, int bits) => (x << bits) | (x >> -bits);

    private static uint RotateRight(uint x, int bits) => (x >> bits) | (x << -bits);

    private uint H1(uint x) => q[x & 0xFF] + q[((x >> 16) & 0xFF) + 256];

    private uint H2(uint x) => p[x & 0xFF] + p[((x >> 16) & 0xFF) + 256];

    private static uint Mod1024(uint x) => x & 0x3FF;

    private static uint Mod512(uint x) => x & 0x1FF;

    private static uint Dim(uint x, uint y) => Mod512(x - y);

    private uint Step()
        {
        uint j = Mod512(cnt);
        uint ret;
        if (cnt < 512)
            {
            p[j] += G1(p[Dim(j, 3)], p[Dim(j, 10)], p[Dim(j, 511)]);
            ret = H1(p[Dim(j, 12)]) ^ p[j];
            }
        else
            {
            q[j] += G2(q[Dim(j, 3)], q[Dim(j, 10)], q[Dim(j, 511)]);
            ret = H2(q[Dim(j, 12)]) ^ q[j];
            }
        cnt = Mod1024(cnt + 1);
        return ret;
        }

    private readonly byte[] iv;

    public HC128ByteProvider(byte[] key, byte[] iv) : base(key)
        {
        if (key.Length != 16)
            {
            throw new ArgumentException("The key must be 128 bits long");
            }

        this.iv = iv;
        this.Initialize();
        }

    public override void Initialize() 
        {
        idx = 0;
        cnt = 0;

        uint[] w = new uint[1280];

        for (int i = 0; i < 16; i++)
            {
            w[i >> 2] |= (uint)key[i] << (8 * (i & 0x3));
            }
        Array.Copy(w, 0, w, 4, 4);

        for (int i = 0; i < iv.Length && i < 16; i++)
            {
            w[(i >> 2) + 8] |= (uint)iv[i] << (8 * (i & 0x3));
            }
        Array.Copy(w, 8, w, 12, 4);

        for (uint i = 16; i < 1280; i++)
            {
            w[i] = F2(w[i - 2]) + w[i - 7] + F1(w[i - 15]) + w[i - 16] + i;
            }

        Array.Copy(w, 256, p, 0, 512);
        Array.Copy(w, 768, q, 0, 512);

        for (int i = 0; i < 512; i++)
            {
            p[i] = Step();
            }
        for (int i = 0; i < 512; i++)
            {
            q[i] = Step();
            }

        cnt = 0;
        }

    private readonly byte[] buf = new byte[4];
    private int idx = 0;

    public byte GetNext() 
        {
        if (idx == 0)
            {
            Pack.UInt32_To_LE(Step(), buf);
            }

        byte ret = buf[idx];
        idx = idx + 1 & 0x3;
        return ret;
        }
    }
