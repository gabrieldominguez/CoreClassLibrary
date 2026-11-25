
// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;

/// <summary>
/// Implements ECB cipher mode
/// </summary>
/// <remarks>
/// <para>    Este modo es equivalente a no usar ninguno, se provee solo como referencia.</para>
/// <para>
///     The Electronic Codebook (ECB) mode encrypts each block individually. Any blocks
///     of plain text that are identical and in the same message, or that are in a different
///     message encrypted with the same key, will be transformed into identical cipher
///     text blocks. Important: This mode is not recommended because it opens the door
///     for multiple security exploits. If the plain text to be encrypted contains substantial
///     repetition, it is feasible for the cipher text to be broken one block at a time.
///     It is also possible to use block analysis to determine the encryption key. Also,
///     an active adversary can substitute and exchange individual blocks without detection,
///     which allows blocks to be saved and inserted into the stream at other points
///     without detection.
/// </para>
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation"/>
public class ECBCipherMode(IBlockCipher cipher) : CipherMode(cipher), IBlockCipher
    {
    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        => this.cipher.EncryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        => this.cipher.DecryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
    }
