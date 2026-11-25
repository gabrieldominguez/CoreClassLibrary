

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;

/// <summary>
/// Tiny Encryption Algorithm (TEA) block cipher.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Tiny_Encryption_Algorithm"/>
/// <see cref="https://crypto.stackexchange.com/questions/16186/is-tea-considered-secure"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/TEAEngine.cs"/>
/// <see cref="XTeaCipher"/>
public class TeaCipher: IBlockCipher
    {
    public int BlockSize => 8;
    //			key_size	= 16,

    private const int rounds = 32;

    private const uint
        delta = 0x9E3779B9,
        d_sum = 0xC6EF3720; // sum on decrypt

		// the expanded key array of 4 subkeys
    private uint _a, _b, _c, _d;

    public TeaCipher(byte[] key)
        {
        this.SetKey(key);
        }

    private void SetKey(byte[] key)
        {
        _a = Pack.BE_To_UInt32(key, 0);
        _b = Pack.BE_To_UInt32(key, 4);
        _c = Pack.BE_To_UInt32(key, 8);
        _d = Pack.BE_To_UInt32(key, 12);
        }

    public void Initialize() { }

    public void EncryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        // Pack bytes into integers
        uint v0 = Pack.BE_To_UInt32(input, inputOffset);
        uint v1 = Pack.BE_To_UInt32(input, inputOffset + 4);

        uint sum = 0;

        for (int i = 0; i != rounds; i++)
            {
            sum += delta;
            v0 += ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >> 5) + _b);
            v1 += ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >> 5) + _d);
            }

        Pack.UInt32_To_BE(v0, output, outputOffset);
        Pack.UInt32_To_BE(v1, output, outputOffset + 4);
        }

    public void DecryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        // Pack bytes into integers
        uint v0 = Pack.BE_To_UInt32(input, inputOffset);
        uint v1 = Pack.BE_To_UInt32(input, inputOffset + 4);

        uint sum = d_sum;

        for (int i = 0; i != rounds; i++)
            {
            v1 -= ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >> 5) + _d);
            v0 -= ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >> 5) + _b);
            sum -= delta;
            }

        Pack.UInt32_To_BE(v0, output, outputOffset);
        Pack.UInt32_To_BE(v1, output, outputOffset + 4);
        }
    }
