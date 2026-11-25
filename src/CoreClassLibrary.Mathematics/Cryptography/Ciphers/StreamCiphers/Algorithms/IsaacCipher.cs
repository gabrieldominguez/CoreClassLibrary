namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

public sealed class IsaacCipher(byte[] key) : XorCipherBase(new IsaacByteProvider(key))
    {
    }

public sealed class IsaacEngine(byte[] key) : XorEngineBase(new IsaacByteProvider(key))
    {
    }

/// <summary>
/// Bob Jenkin's ISAAC (Indirection Shift Accumulate Add and Count).
/// </summary>
/// <see cref="http://www.burtleburtle.net/bob/rand/isaacafa.html"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/ISAACEngine.cs"/>
public sealed class IsaacByteProvider : CipherBase, ISampleProvider<byte>
    {
    // Constants
    private static readonly int sizeL = 8,
                                stateArraySize = sizeL << 5; // 256

    // Cipher's internal state
    private uint[]? engineState = null, // mm                
                   results = null; // randrsl

    private uint a = 0, b = 0, c = 0;

    // Engine state
    private int index = 0;
    private byte[] keyStream = new byte[stateArraySize << 2]; // results expanded into bytes

    public IsaacByteProvider(byte[] key): base(key)
        {
        this.Initialize();
        }

    public byte GetNext()
        {
        if (index == 0)
            {
            this.Isaac();
            keyStream = Pack.UInt32_To_BE(results!);
            }

        byte output = keyStream[index];
        index = (index + 1) & 1023;

        return output;
        }

    public override void Initialize()
        {
        engineState ??= new uint[stateArraySize];
        results ??= new uint[stateArraySize];

        int i, j, k;

        // Reset state
        for (i = 0; i < stateArraySize; i++)
            {
            engineState[i] = results[i] = 0;
            }

        a = b = c = 0;

        // Reset index counter for output
        index = 0;

        // Convert the key bytes to ints and put them into results[] for initialization
        byte[] t = new byte[key.Length + (key.Length & 3)];
        Array.Copy(key, 0, t, 0, key.Length);
        for (i = 0; i < t.Length; i += 4)
            {
            results[i >> 2] = Pack.LE_To_UInt32(t, i);
            }

        // It has begun?
        uint[] abcdefgh = new uint[sizeL];

        for (i = 0; i < sizeL; i++)
            {
            abcdefgh[i] = 0x9e3779b9; // Phi (golden ratio)
            }

        for (i = 0; i < 4; i++)
            {
            IsaacByteProvider.Mix(abcdefgh);
            }

        for (i = 0; i < 2; i++)
            {
            for (j = 0; j < stateArraySize; j += sizeL)
                {
                for (k = 0; k < sizeL; k++)
                    {
                    abcdefgh[k] += (i < 1) ? results[j + k] : engineState[j + k];
                    }

                IsaacByteProvider.Mix(abcdefgh);

                for (k = 0; k < sizeL; k++)
                    {
                    engineState[j + k] = abcdefgh[k];
                    }
                }
            }

        this.Isaac();
        }

    private void Isaac()
        {
        uint x, y;

        b += ++c;
        for (int i = 0; i < stateArraySize; i++)
            {
            x = engineState![i];
            switch (i & 3)
                {
                case 0: a ^= a << 13; break;
                case 1: a ^= a >> 6; break;
                case 2: a ^= a << 2; break;
                case 3: a ^= a >> 16; break;
                }
            a += engineState[(i + 128) & 0xFF];
            engineState[i] = y = engineState[(int)((uint)x >> 2) & 0xFF] + a + b;
            results![i] = b = engineState[(int)((uint)y >> 10) & 0xFF] + x;
            }
        }

    private static void Mix(uint[] x)
        {
        x[0] ^= x[1] << 11; x[3] += x[0]; x[1] += x[2];
        x[1] ^= x[2] >> 2; x[4] += x[1]; x[2] += x[3];
        x[2] ^= x[3] << 8; x[5] += x[2]; x[3] += x[4];
        x[3] ^= x[4] >> 16; x[6] += x[3]; x[4] += x[5];
        x[4] ^= x[5] << 10; x[7] += x[4]; x[5] += x[6];
        x[5] ^= x[6] >> 4; x[0] += x[5]; x[6] += x[7];
        x[6] ^= x[7] << 8; x[1] += x[6]; x[7] += x[0];
        x[7] ^= x[0] >> 9; x[2] += x[7]; x[0] += x[1];
        }
    }
