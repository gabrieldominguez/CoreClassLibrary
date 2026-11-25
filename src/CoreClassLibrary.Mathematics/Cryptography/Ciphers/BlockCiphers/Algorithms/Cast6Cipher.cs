namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;

/// <summary>
/// Implements CAST 6 cipher algorithm
/// </summary>
/// <remarks>
/// RFC2144 - Cast5Cipher(64bit block, 40-128bit key)
/// RFC2612 - CAST6(128bit block, 128-256bit key)
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/CAST-256"/>
/// <see cref="http://www.ietf.org/rfc/rfc2612.txt"/>
/// <see cref="https://tools.ietf.org/html/rfc2612"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/Cast6Engine.cs"/>
public sealed class Cast6Cipher : CipherBase, IBlockCipher
    {
    public int BlockSize => 16; //bytes = 128 bits

    private const int ROUNDS = 12;

    /*
    * Put the round and mask keys into an array.
    * Kr0[i] => _Kr[i*4 + 0]
    */
    private readonly int[] _Kr = new int[ROUNDS * 4]; // the rotating round key(s)
    private readonly uint[] _Km = new uint[ROUNDS * 4]; // the masking round key(s)

    /*
    * Key setup
    */
    private readonly int[] _Tr = new int[24 * 8];
    private readonly uint[] _Tm = new uint[24 * 8];

    public Cast6Cipher(byte[] key) : base(key)
        {
        this.ValidateKeySize();
        this.MakeWorkingKey();
        }

    private void ValidateKeySize()
        {
        int keySize = key.Length * 8;

        if (!(128 <= keySize && keySize <= 256 && keySize % 8 == 0))
            {
            throw new ArgumentException(string.Format("KeySize '{0}' is not valid for this algorithm.", keySize));
            }
        }

    private void MakeWorkingKey()
        {
        uint[] workingKey = new uint[8];

        uint Cm = 0x5a827999;
        const uint Mm = 0x6ed9eba1;
        int Cr = 19;
        const int Mr = 17;
        /*
        * Determine the key size here, if required
        *
        * if keysize < 256 bytes, pad with 0
        *
        * Typical key sizes => 128, 160, 192, 224, 256
        */
        for (int i = 0; i < 24; i++)
            {
            for (int j = 0; j < 8; j++)
                {
                _Tm[i * 8 + j] = Cm;
                Cm += Mm; //mod 2^32;
                _Tr[i * 8 + j] = Cr;
                Cr = (Cr + Mr) & 0x1f;            // mod 32
                }
            }

        byte[] tmpKey = new byte[64];
        key.CopyTo(tmpKey, 0);

        // now create ABCDEFGH
        for (int i = 0; i < 8; i++)
            {
            workingKey[i] = Pack.BigEndianToUInt32(tmpKey, i * 4);
            }

        // Generate the key schedule
        for (int i = 0; i < 12; i++)
            {
            // KAPPA <- W2i(KAPPA)
            int i2 = i * 2 * 8;
            workingKey[6] ^= Cast5Cipher.F1(workingKey[7], _Tm[i2], _Tr[i2]);
            workingKey[5] ^= Cast5Cipher.F2(workingKey[6], _Tm[i2 + 1], _Tr[i2 + 1]);
            workingKey[4] ^= Cast5Cipher.F3(workingKey[5], _Tm[i2 + 2], _Tr[i2 + 2]);
            workingKey[3] ^= Cast5Cipher.F1(workingKey[4], _Tm[i2 + 3], _Tr[i2 + 3]);
            workingKey[2] ^= Cast5Cipher.F2(workingKey[3], _Tm[i2 + 4], _Tr[i2 + 4]);
            workingKey[1] ^= Cast5Cipher.F3(workingKey[2], _Tm[i2 + 5], _Tr[i2 + 5]);
            workingKey[0] ^= Cast5Cipher.F1(workingKey[1], _Tm[i2 + 6], _Tr[i2 + 6]);
            workingKey[7] ^= Cast5Cipher.F2(workingKey[0], _Tm[i2 + 7], _Tr[i2 + 7]);
            // KAPPA <- W2i+1(KAPPA)
            i2 = (i * 2 + 1) * 8;
            workingKey[6] ^= Cast5Cipher.F1(workingKey[7], _Tm[i2], _Tr[i2]);
            workingKey[5] ^= Cast5Cipher.F2(workingKey[6], _Tm[i2 + 1], _Tr[i2 + 1]);
            workingKey[4] ^= Cast5Cipher.F3(workingKey[5], _Tm[i2 + 2], _Tr[i2 + 2]);
            workingKey[3] ^= Cast5Cipher.F1(workingKey[4], _Tm[i2 + 3], _Tr[i2 + 3]);
            workingKey[2] ^= Cast5Cipher.F2(workingKey[3], _Tm[i2 + 4], _Tr[i2 + 4]);
            workingKey[1] ^= Cast5Cipher.F3(workingKey[2], _Tm[i2 + 5], _Tr[i2 + 5]);
            workingKey[0] ^= Cast5Cipher.F1(workingKey[1], _Tm[i2 + 6], _Tr[i2 + 6]);
            workingKey[7] ^= Cast5Cipher.F2(workingKey[0], _Tm[i2 + 7], _Tr[i2 + 7]);
            // Kr_(i) <- KAPPA
            _Kr[i * 4] = (int)(workingKey[0] & 0x1f);
            _Kr[i * 4 + 1] = (int)(workingKey[2] & 0x1f);
            _Kr[i * 4 + 2] = (int)(workingKey[4] & 0x1f);
            _Kr[i * 4 + 3] = (int)(workingKey[6] & 0x1f);
            // Km_(i) <- KAPPA
            _Km[i * 4] = workingKey[7];
            _Km[i * 4 + 1] = workingKey[5];
            _Km[i * 4 + 2] = workingKey[3];
            _Km[i * 4 + 3] = workingKey[1];
            }
        }

    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        // process the input block
        // batch the units up into 4x32 bit chunks and go for it
        uint A = Pack.BE_To_UInt32(inputBuffer, inputOffset);
        uint B = Pack.BE_To_UInt32(inputBuffer, inputOffset + 4);
        uint C = Pack.BE_To_UInt32(inputBuffer, inputOffset + 8);
        uint D = Pack.BE_To_UInt32(inputBuffer, inputOffset + 12);

        uint[] result = new uint[4];
        this.Encipher(A, B, C, D, result);

        // now stuff them into the destination block
        Pack.UInt32_To_BE(result[0], outputBuffer, outputOffset);
        Pack.UInt32_To_BE(result[1], outputBuffer, outputOffset + 4);
        Pack.UInt32_To_BE(result[2], outputBuffer, outputOffset + 8);
        Pack.UInt32_To_BE(result[3], outputBuffer, outputOffset + 12);
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        // process the input block
        // batch the units up into 4x32 bit chunks and go for it
        uint A = Pack.BE_To_UInt32(inputBuffer, inputOffset);
        uint B = Pack.BE_To_UInt32(inputBuffer, inputOffset + 4);
        uint C = Pack.BE_To_UInt32(inputBuffer, inputOffset + 8);
        uint D = Pack.BE_To_UInt32(inputBuffer, inputOffset + 12);

        uint[] result = new uint[4];
        this.Decipher(A, B, C, D, result);

        // now stuff them into the destination block
        Pack.UInt32_To_BE(result[0], outputBuffer, outputOffset);
        Pack.UInt32_To_BE(result[1], outputBuffer, outputOffset + 4);
        Pack.UInt32_To_BE(result[2], outputBuffer, outputOffset + 8);
        Pack.UInt32_To_BE(result[3], outputBuffer, outputOffset + 12);
        }

    /**
    * Does the 12 quad rounds rounds to encrypt the block.
    *
    * @param A    the 00-31  bits of the plaintext block
    * @param B    the 32-63  bits of the plaintext block
    * @param C    the 64-95  bits of the plaintext block
    * @param D    the 96-127 bits of the plaintext block
    * @param result the resulting ciphertext
    */
    private void Encipher(
        uint A,
        uint B,
        uint C,
        uint D,
        uint[] result)
        {
        for (int i = 0; i < 6; i++)
            {
            int x = i * 4;
            // BETA <- Qi(BETA)
            C ^= Cast5Cipher.F1(D, _Km[x], _Kr[x]);
            B ^= Cast5Cipher.F2(C, _Km[x + 1], _Kr[x + 1]);
            A ^= Cast5Cipher.F3(B, _Km[x + 2], _Kr[x + 2]);
            D ^= Cast5Cipher.F1(A, _Km[x + 3], _Kr[x + 3]);
            }
        for (int i = 6; i < 12; i++)
            {
            int x = i * 4;
            // BETA <- QBARi(BETA)
            D ^= Cast5Cipher.F1(A, _Km[x + 3], _Kr[x + 3]);
            A ^= Cast5Cipher.F3(B, _Km[x + 2], _Kr[x + 2]);
            B ^= Cast5Cipher.F2(C, _Km[x + 1], _Kr[x + 1]);
            C ^= Cast5Cipher.F1(D, _Km[x], _Kr[x]);
            }
        result[0] = A;
        result[1] = B;
        result[2] = C;
        result[3] = D;
        }

    /**
    * Does the 12 quad rounds rounds to decrypt the block.
    *
    * @param A    the 00-31  bits of the ciphertext block
    * @param B    the 32-63  bits of the ciphertext block
    * @param C    the 64-95  bits of the ciphertext block
    * @param D    the 96-127 bits of the ciphertext block
    * @param result the resulting plaintext
    */
    private void Decipher(
        uint A,
        uint B,
        uint C,
        uint D,
        uint[] result)
        {
        for (int i = 0; i < 6; i++)
            {
            int x = (11 - i) * 4;
            // BETA <- Qi(BETA)
            C ^= Cast5Cipher.F1(D, _Km[x], _Kr[x]);
            B ^= Cast5Cipher.F2(C, _Km[x + 1], _Kr[x + 1]);
            A ^= Cast5Cipher.F3(B, _Km[x + 2], _Kr[x + 2]);
            D ^= Cast5Cipher.F1(A, _Km[x + 3], _Kr[x + 3]);
            }
        for (int i = 6; i < 12; i++)
            {
            int x = (11 - i) * 4;
            // BETA <- QBARi(BETA)
            D ^= Cast5Cipher.F1(A, _Km[x + 3], _Kr[x + 3]);
            A ^= Cast5Cipher.F3(B, _Km[x + 2], _Kr[x + 2]);
            B ^= Cast5Cipher.F2(C, _Km[x + 1], _Kr[x + 1]);
            C ^= Cast5Cipher.F1(D, _Km[x], _Kr[x]);
            }
        result[0] = A;
        result[1] = B;
        result[2] = C;
        result[3] = D;
        }
    }
