namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;

/// <summary>
/// Noekeon cipher, using direct-key mode.
/// </summary>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/NoekeonEngine.cs"/>
public sealed class NoekeonCipher : CipherBase, IBlockCipher
    {
    public int BlockSize => 16; // Block and key size, as well as the amount of rounds.

    public NoekeonCipher(byte[] key): base(key)
        {
        this.Initialize();
        }
    
    private static readonly uint[] nullVector =
    [
        0x00, 0x00, 0x00, 0x00 // Used in decryption
		];

    private static readonly uint[] roundConstants =
    [
        0x80, 0x1b, 0x36, 0x6c,
        0xd8, 0xab, 0x4d, 0x9a,
        0x2f, 0x5e, 0xbc, 0x63,
        0xc6, 0x97, 0x35, 0x6a,
        0xd4
    ];

    private readonly uint[] state = new uint[4], // a
                    subKeys = new uint[4], // k
                    decryptKeys = new uint[4];


    public override void Initialize()
        {
        subKeys[0] = Pack.BE_To_UInt32(key, 0);
        subKeys[1] = Pack.BE_To_UInt32(key, 4);
        subKeys[2] = Pack.BE_To_UInt32(key, 8);
        subKeys[3] = Pack.BE_To_UInt32(key, 12);
        }


    public void EncryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        state[0] = Pack.BE_To_UInt32(input, inputOffset);
        state[1] = Pack.BE_To_UInt32(input, inputOffset + 4);
        state[2] = Pack.BE_To_UInt32(input, inputOffset + 8);
        state[3] = Pack.BE_To_UInt32(input, inputOffset + 12);

        int i;
        for (i = 0; i < BlockSize; i++)
            {
            state[0] ^= roundConstants[i];
            theta(state, subKeys);
            pi1(state);
            gamma(state);
            pi2(state);
            }

        state[0] ^= roundConstants[i];
        theta(state, subKeys);

        Pack.UInt32_To_BE(state[0], output, outputOffset);
        Pack.UInt32_To_BE(state[1], output, outputOffset + 4);
        Pack.UInt32_To_BE(state[2], output, outputOffset + 8);
        Pack.UInt32_To_BE(state[3], output, outputOffset + 12);
        }

    public void DecryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        state[0] = Pack.BE_To_UInt32(input, inputOffset);
        state[1] = Pack.BE_To_UInt32(input, inputOffset + 4);
        state[2] = Pack.BE_To_UInt32(input, inputOffset + 8);
        state[3] = Pack.BE_To_UInt32(input, inputOffset + 12);

        Array.Copy(subKeys, 0, decryptKeys, 0, subKeys.Length);
        theta(decryptKeys, nullVector);

        int i;
        for (i = BlockSize; i > 0; i--)
            {
            theta(state, decryptKeys);
            state[0] ^= roundConstants[i];
            pi1(state);
            gamma(state);
            pi2(state);
            }

        theta(state, decryptKeys);
        state[0] ^= roundConstants[i];

        Pack.UInt32_To_BE(state[0], output, outputOffset);
        Pack.UInt32_To_BE(state[1], output, outputOffset + 4);
        Pack.UInt32_To_BE(state[2], output, outputOffset + 8);
        Pack.UInt32_To_BE(state[3], output, outputOffset + 12);
        }

    private void gamma(uint[] a)
        {
        a[1] ^= ~a[3] & ~a[2];
        a[0] ^= a[2] & a[1];

        uint tmp = a[3];
        a[3] = a[0];
        a[0] = tmp;
        a[2] ^= a[0] ^ a[1] ^ a[3];

        a[1] ^= ~a[3] & ~a[2];
        a[0] ^= a[2] & a[1];
        }

    private void theta(uint[] a, uint[] k)
        {
        uint tmp;
        tmp = a[0] ^ a[2];
        tmp ^= rotl(tmp, 8) ^ rotl(tmp, 24);
        a[1] ^= tmp;
        a[3] ^= tmp;

        for (int i = 0; i < 4; i++)
            {
            a[i] ^= k[i];
            }

        tmp = a[1] ^ a[3];
        tmp ^= rotl(tmp, 8) ^ rotl(tmp, 24);
        a[0] ^= tmp;
        a[2] ^= tmp;
        }

    private void pi1(uint[] a)
        {
        a[1] = rotl(a[1], 1);
        a[2] = rotl(a[2], 5);
        a[3] = rotl(a[3], 2);
        }

    private void pi2(uint[] a)
        {
        a[1] = rotl(a[1], 31);
        a[2] = rotl(a[2], 27);
        a[3] = rotl(a[3], 30);
        }

    // Helpers

    private static uint rotl(uint x, int y) => (x << y) | (x >> (32 - y));
    }
