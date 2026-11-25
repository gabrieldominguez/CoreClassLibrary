// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Hashing;

/// <summary>
/// Implements SHA-384
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/SHA-2"/>
/// <see cref="http://csrc.nist.gov/groups/ST/toolkit/documents/Examples/SHA_All.pdf"/>
public class SHA384Hash : SHA2HashBase
    {
    protected override int DigestSize => 48;

    public SHA384Hash()
        {
        InitializeHashValue();
        }

    /// <summary>
    /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
    /// </summary>
    /// <returns>
    /// The computed hash code.
    /// </returns>
    public override byte[] HashFinal()
        {
        byte[] output = new byte[DigestSize];

        Finish();

        UInt64_To_BE(H1, output, 0);
        UInt64_To_BE(H2, output, 8);
        UInt64_To_BE(H3, output, 16);
        UInt64_To_BE(H4, output, 24);
        UInt64_To_BE(H5, output, 32);
        UInt64_To_BE(H6, output, 40);

        return output;
        }

    /// <summary>
    /// Resets <see cref="SHA384Hash"/> to its initial state.
    /// </summary>
    public override void Initialize()
        {
        base.Initialize();

        InitializeHashValue();
        }

    private void InitializeHashValue()
        {
        /*
         * SHA-384 initial hash value
         * The first 64 bits of the fractional parts of the square roots
         * of the 9th through 16th prime numbers
         */
        H1 = 0xcbbb9d5dc1059ed8;
        H2 = 0x629a292a367cd507;
        H3 = 0x9159015a3070dd17;
        H4 = 0x152fecd8f70e5939;
        H5 = 0x67332667ffc00b31;
        H6 = 0x8eb44a8768581511;
        H7 = 0xdb0c2e0d64f98fa7;
        H8 = 0x47b5481dbefa4fa4;
        }
    }
