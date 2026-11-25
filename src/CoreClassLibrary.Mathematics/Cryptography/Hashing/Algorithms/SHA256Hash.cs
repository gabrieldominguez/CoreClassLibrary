// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

/// <summary>
/// Double SHA-2 256 implementation
/// </summary>
/// <see cref="https://en.bitcoin.it/wiki/Hashcash#Double_Hash"/>
/// <see cref="https://bitcoin.stackexchange.com/questions/6037/why-are-hashes-in-the-bitcoin-protocol-typically-computed-twice-double-computed"/>
public class DoubleSHA256HashAlgorithm : DoubleHashAlgorithm
    {
    public DoubleSHA256HashAlgorithm() : base(new SHA256Hash(), new SHA256Hash()) { }
    }

/// <summary>
/// Implements SHA-2 256.
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/SHA-2"/>
public class SHA256Hash : IHashAlgorithm
    {
    private const int DigestSize = 32;

    private uint _h1, _h2, _h3, _h4, _h5, _h6, _h7, _h8;

    /// <summary>
    /// The word buffer.
    /// </summary>
    private readonly uint[] _x;

    private int _offset;

    private readonly byte[] _buffer;

    private int _bufferOffset;

    private long _byteCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="SHA256Hash"/> class.
    /// </summary>
    public SHA256Hash()
        {
        _buffer = new byte[4];
        _x = new uint[64];

        InitializeHashValue();
        }

    /// <summary>
    /// Gets the size, in bits, of the computed hash code.
    /// </summary>
    /// <returns>
    /// The size, in bits, of the computed hash code.
    /// </returns>
    public int HashSize => SHA256Hash.DigestSize * 8;

    /// <summary>
    /// Gets the block size.
    /// </summary>
    /// <returns>
    /// The block size.
    /// </returns>
    public int BlockSize => 64;

    /// <summary>
    /// Routes data written to the object into the hash algorithm for computing the hash.
    /// </summary>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="offset">The offset into the byte array from which to begin using data.</param>
    /// <param name="size">The number of bytes in the byte array to use as data.</param>
    public void HashCore(byte[] array, int offset, int size)
        {
        //  Fill the current word
        while (_bufferOffset != 0 && size > 0)
            {
            Update(array[offset]);
            offset++;
            size--;
            }

        //  Process whole words.
        while (size > _buffer.Length)
            {
            ProcessWord(array, offset);

            offset += _buffer.Length;
            size -= _buffer.Length;
            _byteCount += _buffer.Length;
            }

        //  Load in the remainder.
        while (size > 0)
            {
            Update(array[offset]);

            offset++;
            size--;
            }
        }

    /// <summary>
    /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    public byte[] HashFinal()
        {
        byte[] output = new byte[DigestSize];
        long bitLength = _byteCount << 3;

        //
        // add the pad bytes.
        //
        Update(128);

        while (_bufferOffset != 0)
            Update(0);

        if (_offset > 14)
            {
            ProcessBlock();
            }

        _x[14] = (uint)((ulong)bitLength >> 32);
        _x[15] = (uint)(ulong)bitLength;

        ProcessBlock();

        UInt32_To_BE(_h1, output, 0);
        UInt32_To_BE(_h2, output, 4);
        UInt32_To_BE(_h3, output, 8);
        UInt32_To_BE(_h4, output, 12);
        UInt32_To_BE(_h5, output, 16);
        UInt32_To_BE(_h6, output, 20);
        UInt32_To_BE(_h7, output, 24);
        UInt32_To_BE(_h8, output, 28);

        return output;
        }

    /// <summary>
    /// Resets <see cref="SHA256Hash"/> to its initial state.
    /// </summary>
    public void Initialize()
        {
        InitializeHashValue();

        _byteCount = 0;
        _bufferOffset = 0;
        for (int i = 0; i < _buffer.Length; i++)
            {
            _buffer[i] = 0;
            }

        _offset = 0;
        for (int i = 0; i < _x.Length; i++)
            {
            _x[i] = 0;
            }
        }

    private void InitializeHashValue()
        {
        _h1 = 0x6a09e667;
        _h2 = 0xbb67ae85;
        _h3 = 0x3c6ef372;
        _h4 = 0xa54ff53a;
        _h5 = 0x510e527f;
        _h6 = 0x9b05688c;
        _h7 = 0x1f83d9ab;
        _h8 = 0x5be0cd19;
        }

    private void Update(byte input)
        {
        _buffer[_bufferOffset++] = input;

        if (_bufferOffset == _buffer.Length)
            {
            ProcessWord(_buffer, 0);
            _bufferOffset = 0;
            }

        _byteCount++;
        }

    private static uint BE_To_UInt32(byte[] bs, int off)
        {
        uint n = (uint)bs[off] << 24;
        n |= (uint)bs[++off] << 16;
        n |= (uint)bs[++off] << 8;
        n |= bs[++off];
        return n;
        }

    private static void UInt32_To_BE(uint n, byte[] bs, int off)
        {
        bs[off] = (byte)(n >> 24);
        bs[++off] = (byte)(n >> 16);
        bs[++off] = (byte)(n >> 8);
        bs[++off] = (byte)n;
        }

    private void ProcessWord(byte[] input, int inOff)
        {
        _x[_offset] = BE_To_UInt32(input, inOff);

        if (++_offset == 16)
            {
            ProcessBlock();
            }
        }

    private void ProcessBlock()
        {
        //
        // expand 16 word block into 64 word blocks.
        //
        for (int ti = 16; ti <= 63; ti++)
            {
            _x[ti] = Theta1(_x[ti - 2]) + _x[ti - 7] + Theta0(_x[ti - 15]) + _x[ti - 16];
            }

        //
        // set up working variables.
        //
        uint a = _h1;
        uint b = _h2;
        uint c = _h3;
        uint d = _h4;
        uint e = _h5;
        uint f = _h6;
        uint g = _h7;
        uint h = _h8;

        int t = 0;
        for (int i = 0; i < 8; ++i)
            {
            // t = 8 * i
            h += Sum1Ch(e, f, g) + K[t] + _x[t];
            d += h;
            h += Sum0Maj(a, b, c);
            ++t;

            // t = 8 * i + 1
            g += Sum1Ch(d, e, f) + K[t] + _x[t];
            c += g;
            g += Sum0Maj(h, a, b);
            ++t;

            // t = 8 * i + 2
            f += Sum1Ch(c, d, e) + K[t] + _x[t];
            b += f;
            f += Sum0Maj(g, h, a);
            ++t;

            // t = 8 * i + 3
            e += Sum1Ch(b, c, d) + K[t] + _x[t];
            a += e;
            e += Sum0Maj(f, g, h);
            ++t;

            // t = 8 * i + 4
            d += Sum1Ch(a, b, c) + K[t] + _x[t];
            h += d;
            d += Sum0Maj(e, f, g);
            ++t;

            // t = 8 * i + 5
            c += Sum1Ch(h, a, b) + K[t] + _x[t];
            g += c;
            c += Sum0Maj(d, e, f);
            ++t;

            // t = 8 * i + 6
            b += Sum1Ch(g, h, a) + K[t] + _x[t];
            f += b;
            b += Sum0Maj(c, d, e);
            ++t;

            // t = 8 * i + 7
            a += Sum1Ch(f, g, h) + K[t] + _x[t];
            e += a;
            a += Sum0Maj(b, c, d);
            ++t;
            }

        _h1 += a;
        _h2 += b;
        _h3 += c;
        _h4 += d;
        _h5 += e;
        _h6 += f;
        _h7 += g;
        _h8 += h;

        // 
        // reset the offset and clean out the word buffer.
        //
        _offset = 0;
        for (int i = 0; i < _x.Length; i++)
            {
            _x[i] = 0;
            }
        }

    private static uint Sum1Ch(uint x, uint y, uint z) => (((x >> 6) | (x << 26)) ^ ((x >> 11) | (x << 21)) ^ ((x >> 25) | (x << 7)))
               + ((x & y) ^ ((~x) & z));

    private static uint Sum0Maj(uint x, uint y, uint z) => (((x >> 2) | (x << 30)) ^ ((x >> 13) | (x << 19)) ^ ((x >> 22) | (x << 10)))
               + ((x & y) ^ (x & z) ^ (y & z));

    private static uint Theta0(uint x) => ((x >> 7) | (x << 25)) ^ ((x >> 18) | (x << 14)) ^ (x >> 3);

    private static uint Theta1(uint x) => ((x >> 17) | (x << 15)) ^ ((x >> 19) | (x << 13)) ^ (x >> 10);

    /// <summary>
    /// The SHA-256 Constants (represent the first 32 bits of the fractional parts of the cube roots of the first sixty-four prime numbers)
    /// </summary>
    private static readonly uint[] K =
    [
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
        0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
        0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
        0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
        0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
        0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
        0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
        0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
        0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
        0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
        0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
        0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
        0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
    ];
    }
