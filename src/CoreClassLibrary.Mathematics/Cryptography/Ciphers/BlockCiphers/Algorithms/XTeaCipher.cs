

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;

/// <summary>
/// Extended Tiny Encryption Algorithm (XTEA) block cipher.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/XTEA"/>
/// <see cref="https://en.wikipedia.org/wiki/Tiny_Encryption_Algorithm"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/XTEAEngine.cs"/>
/// <see cref="TeaCipher"/>
public class XTeaCipher: IBlockCipher
    {
    public int BlockSize => 8;
    //			key_size	= 16,

    private const int rounds = 32;
    private const int delta = unchecked((int)0x9E3779B9);

    // the expanded key array of 4 subkeys
    private readonly uint[] _S = new uint[4],
        _sum0 = new uint[32],
        _sum1 = new uint[32];

    public XTeaCipher(byte[] key)
        {
        this.SetKey(key);
        }

    private void SetKey(byte[] key)
        {
        int i, j;
        for (i = j = 0; i < 4; i++, j += 4)
            {
            _S[i] = Pack.BE_To_UInt32(key, j);
            }

        for (i = j = 0; i < rounds; i++)
            {
            _sum0[i] = (uint)j + _S[j & 3];
            j += delta;
            _sum1[i] = (uint)j + _S[j >> 11 & 3];
            }
        }

    public void Initialize() { }

    public void EncryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        // Pack bytes into integers
        uint v0 = Pack.BE_To_UInt32(input, inputOffset);
        uint v1 = Pack.BE_To_UInt32(input, inputOffset + 4);

        for (int i = 0; i < rounds; i++)
            {
            v0 += ((v1 << 4 ^ v1 >> 5) + v1) ^ _sum0[i];
            v1 += ((v0 << 4 ^ v0 >> 5) + v0) ^ _sum1[i];
            }

        Pack.UInt32_To_BE(v0, output, outputOffset);
        Pack.UInt32_To_BE(v1, output, outputOffset + 4);
        }

    public void DecryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        uint v0 = Pack.BE_To_UInt32(input, inputOffset);
        uint v1 = Pack.BE_To_UInt32(input, inputOffset + 4);

        for (int i = rounds - 1; i >= 0; i--)
            {
            v1 -= ((v0 << 4 ^ v0 >> 5) + v0) ^ _sum1[i];
            v0 -= ((v1 << 4 ^ v1 >> 5) + v1) ^ _sum0[i];
            }

        Pack.UInt32_To_BE(v0, output, outputOffset);
        Pack.UInt32_To_BE(v1, output, outputOffset + 4);
        }
    }
