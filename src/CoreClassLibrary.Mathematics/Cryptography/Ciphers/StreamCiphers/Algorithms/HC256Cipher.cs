namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

/// <summary>
/// HC-256 is a software-efficient stream cipher created by Hongjun Wu. 
/// It generates keystream from a 256-bit secret key and a 256-bit initialization vector.
/// 
/// Its brother, HC-128, is a third phase candidate in the eStream contest.
///
/// The algorithm is patent-free. 
/// No attacks are known as of today (April 2007). 
/// </summary>
/// <see cref="http://www.ecrypt.eu.org/stream/hcp3.html"/>
/// <see cref="http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc128_p3.pdf"/>
/// <see cref="http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc256_p3.pdf"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/HC256Engine.cs"/>
public sealed class HC256Cipher(byte[] key, byte[] iv) : XorCipherBase(new HC256ByteProvider(key, iv))
    {
    }

public sealed class HC256Engine(byte[] key, byte[] iv) : XorEngineBase(new HC256ByteProvider(key, iv))
    {
    }

public sealed class HC256ByteProvider : CipherBase, ISampleProvider<byte>
    {
    private readonly uint[] p = new uint[1024];
    private readonly uint[] q = new uint[1024];
    private uint cnt = 0;

    private uint Step()
        {
        uint j = cnt & 0x3FF;
        uint ret;
        if (cnt < 1024)
            {
            uint x = p[j - 3 & 0x3FF];
            uint y = p[j - 1023 & 0x3FF];
            p[j] += p[j - 10 & 0x3FF]
                + (RotateRight(x, 10) ^ RotateRight(y, 23))
                + q[(x ^ y) & 0x3FF];

            x = p[j - 12 & 0x3FF];
            ret = (q[x & 0xFF] + q[((x >> 8) & 0xFF) + 256]
                + q[((x >> 16) & 0xFF) + 512] + q[((x >> 24) & 0xFF) + 768])
                ^ p[j];
            }
        else
            {
            uint x = q[j - 3 & 0x3FF];
            uint y = q[j - 1023 & 0x3FF];
            q[j] += q[j - 10 & 0x3FF]
                + (RotateRight(x, 10) ^ RotateRight(y, 23))
                + p[(x ^ y) & 0x3FF];

            x = q[j - 12 & 0x3FF];
            ret = (p[x & 0xFF] + p[((x >> 8) & 0xFF) + 256]
                + p[((x >> 16) & 0xFF) + 512] + p[((x >> 24) & 0xFF) + 768])
                ^ q[j];
            }
        cnt = cnt + 1 & 0x7FF;
        return ret;
        }

    private readonly byte[] iv;

    public HC256ByteProvider(byte[] key, byte[] iv) : base(HC256ByteProvider.InitializeKey(key))
        {
        this.iv = HC256ByteProvider.InitializeIV(iv);

        this.Initialize();
        }

    private static byte[] InitializeKey(byte[] key)
        {
        if (key.Length != 32 && key.Length != 16)
            {
            throw new ArgumentException("The key must be 128/256 bits long");
            }

        if (key.Length == 16)
            {
            byte[] k = new byte[32];

            Array.Copy(key, 0, k, 0, key.Length);
            Array.Copy(key, 0, k, 16, key.Length);

            return k;
            }

        return key;
        }

    private static byte[] InitializeIV(byte[] iv)
        {
        if (iv.Length < 16)
            {
            throw new ArgumentException("The IV must be at least 128 bits long");
            }

        if (iv.Length < 32)
            {
            byte[] newIV = new byte[32];

            Array.Copy(iv, 0, newIV, 0, iv.Length);
            Array.Copy(iv, 0, newIV, iv.Length, newIV.Length - iv.Length);

            return newIV;
            }

        return iv;
        }

    public override void Initialize()
        {
        idx = 0;
        cnt = 0;

        uint[] w = new uint[2560];

        for (int i = 0; i < 32; i++)
            {
            w[i >> 2] |= (uint)key[i] << (8 * (i & 0x3));
            }

        for (int i = 0; i < 32; i++)
            {
            w[(i >> 2) + 8] |= (uint)iv[i] << (8 * (i & 0x3));
            }

        for (uint i = 16; i < 2560; i++)
            {
            uint x = w[i - 2];
            uint y = w[i - 15];
            w[i] = (RotateRight(x, 17) ^ RotateRight(x, 19) ^ (x >> 10))
                + w[i - 7]
                + (RotateRight(y, 7) ^ RotateRight(y, 18) ^ (y >> 3))
                + w[i - 16] + i;
            }

        Array.Copy(w, 512, p, 0, 1024);
        Array.Copy(w, 1536, q, 0, 1024);

        for (int i = 0; i < 4096; i++)
            {
            Step();
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

    private static uint RotateRight(uint x, int bits)
        => (x >> bits) | (x << -bits);
    }
