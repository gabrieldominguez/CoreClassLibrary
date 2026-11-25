
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;

/// <summary>
/// The specification for RC5 came from the RC5 Encryption Algorithm publication in RSA CryptoBytes, Spring of 1995.
/// </summary>
/// <remarks>
/// RFC 2040 "The RC5, RC5-CBC, RC5-CBC-Pad, and RC5-CTS Algorithms"
/// This implementation has a word size of 32 bits.
/// </remarks>
/// <see cref="http://www.rsasecurity.com/rsalabs/cryptobytes"/>
/// <see cref="ftp://ftp.nordu.net/rfc/rfc2040.txt"/>
/// <see cref="https://www.cosic.esat.kuleuven.be/nessie/testvectors/bc/rc5/Rc5-128-64.verified.test-vectors"/>
/// <see cref="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/engines/RC532Engine.cs"/>
public sealed class RC5Cipher : CipherBase, IBlockCipher
    {
    private const int BLOCK_SIZE = 8;

    public int BlockSize => BLOCK_SIZE;

    /*
    * the number of rounds to perform
    */
    private readonly int _noRounds;

    /*
    * the expanded key array of size 2*(rounds + 1)
    */
    private readonly int[] _S;

    /*
    * our "magic constants" for 32 32
    *
    * Pw = Odd((e-2) * 2^wordsize)
    * Qw = Odd((o-2) * 2^wordsize)
    *
    * where e is the base of natural logarithms (2.718281828...)
    * and o is the golden ratio (1.61803398...)
    */
    private static readonly int P32 = unchecked((int)0xb7e15163);
    private static readonly int Q32 = unchecked((int)0x9e3779b9);

    public RC5Cipher(byte[] key, int rounds = 12): base(key)
        {
        _S = new int[2 * ((_noRounds = rounds) + 1)];
        this.Initialize();
        }

    public override void Initialize()
        {
        //
        // KEY EXPANSION:
        //
        // There are 3 phases to the key expansion.
        //
        // Phase 1:
        //   Copy the secret key K[0...b-1] into an array L[0..c-1] of
        //   c = ceil(b/u), where u = 32/8 in little-endian order.
        //   In other words, we fill up L using u consecutive key bytes
        //   of K. Any unfilled byte positions in L are zeroed. In the
        //   case that b = c = 0, set c = 1 and L[0] = 0.
        //
        int[] L = new int[(key.Length + (4 - 1)) / 4];

        for (int i = 0; i != key.Length; i++)
            {
            L[i / 4] += (key[i] & 0xff) << (8 * (i % 4));
            }

        //
        // Phase 2:
        //   Initialize S to a particular fixed pseudo-random bit pattern
        //   using an arithmetic progression modulo 2^wordsize determined
        //   by the magic numbers, Pw & Qw.
        //
        _S[0] = P32;
        for (int i = 1; i < _S.Length; i++)
            {
            _S[i] = _S[i - 1] + Q32;
            }

        //
        // Phase 3:
        //   Mix in the user's secret key in 3 passes over the arrays S & L.
        //   The max of the arrays sizes is used as the loop control
        //
        int iter = L.Length > _S.Length ? 3 * L.Length : 3 * _S.Length;

        int A = 0, B = 0;
        int ii = 0, jj = 0;

        for (int k = 0; k < iter; k++)
            {
            A = _S[ii] = RotateLeft(_S[ii] + A + B, 3);
            B = L[jj] = RotateLeft(L[jj] + A + B, A + B);
            ii = (ii + 1) % _S.Length;
            jj = (jj + 1) % L.Length;
            }
        }

    public void EncryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        int A = BytesToWord(input, inputOffset) + _S[0];
        int B = BytesToWord(input, inputOffset + 4) + _S[1];

        for (int i = 1; i <= _noRounds; i++)
            {
            A = RotateLeft(A ^ B, B) + _S[2 * i];
            B = RotateLeft(B ^ A, A) + _S[2 * i + 1];
            }

        WordToBytes(A, output, outputOffset);
        WordToBytes(B, output, outputOffset + 4);
        }

    public void DecryptBlock(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
        int A = BytesToWord(input, inputOffset);
        int B = BytesToWord(input, inputOffset + 4);

        for (int i = _noRounds; i >= 1; i--)
            {
            B = RotateRight(B - _S[2 * i + 1], A) ^ A;
            A = RotateRight(A - _S[2 * i], B) ^ B;
            }

        WordToBytes(A - _S[0], output, outputOffset);
        WordToBytes(B - _S[1], output, outputOffset + 4);
        }


    //////////////////////////////////////////////////////////////
    //
    // PRIVATE Helper Methods
    //
    //////////////////////////////////////////////////////////////

    /**
    * Perform a left "spin" of the word. The rotation of the given
    * word <em>x</em> is rotated left by <em>y</em> bits.
    * Only the <em>lg(32)</em> low-order bits of <em>y</em>
    * are used to determine the rotation amount. Here it is
    * assumed that the wordsize used is a power of 2.
    *
    * @param  x  word to rotate
    * @param  y    number of bits to rotate % 32
    */
    private int RotateLeft(int x, int y) => (int) ((uint) (x << (y & (32 - 1))) |
                          ((uint) x >> (32 - (y & (32 - 1)))))
               ;

    /**
    * Perform a right "spin" of the word. The rotation of the given
    * word <em>x</em> is rotated left by <em>y</em> bits.
    * Only the <em>lg(32)</em> low-order bits of <em>y</em>
    * are used to determine the rotation amount. Here it is
    * assumed that the wordsize used is a power of 2.
    *
    * @param  x  word to rotate
    * @param  y    number of bits to rotate % 32
    */
    private int RotateRight(int x, int y) => (int) (((uint) x >> (y & (32 - 1))) |
                            (uint) (x << (32 - (y & (32 - 1)))))
               ;

    private int BytesToWord(
        byte[] src,
        int srcOff) => (src[srcOff] & 0xff) | ((src[srcOff + 1] & 0xff) << 8)
            | ((src[srcOff + 2] & 0xff) << 16) | ((src[srcOff + 3] & 0xff) << 24);

    private void WordToBytes(
        int word,
        byte[] dst,
        int dstOff)
        {
        dst[dstOff] = (byte)word;
        dst[dstOff + 1] = (byte)(word >> 8);
        dst[dstOff + 2] = (byte)(word >> 16);
        dst[dstOff + 3] = (byte)(word >> 24);
        }
    }
