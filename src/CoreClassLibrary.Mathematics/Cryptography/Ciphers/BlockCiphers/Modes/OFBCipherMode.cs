// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;

/// <summary>
/// Implements OFB cipher mode
/// </summary>
/// <remarks>
///    The Output Feedback (OFB) mode processes small increments of plain text into
//     cipher text instead of processing an entire block at a time. This mode is similar
//     to CFB; the only difference between the two modes is the way that the shift register
//     is filled. If a bit in the cipher text is mangled, the corresponding bit of plain
//     text will be mangled. However, if there are extra or missing bits from the cipher
//     text, the plain text will be mangled from that point on.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation"/>
public class OFBCipherMode(IBlockCipher cipher, byte[] iv) : CipherModeIV(cipher, iv), IBlockCipher
    {
    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.cipher.EncryptBlock(this.iv, 0, this.iv.Length, outputBuffer, outputOffset);

        Buffer.BlockCopy(outputBuffer, outputOffset, this.iv, 0, this.iv.Length);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
            }
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.cipher.EncryptBlock(this.iv, 0, this.iv.Length, outputBuffer, outputOffset);

        Buffer.BlockCopy(outputBuffer, outputOffset, this.iv, 0, this.iv.Length);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
            }
        }
    }
