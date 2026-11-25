// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;

/// <summary>
/// Implements CFB cipher mode
/// </summary>
/// <remarks>
///    The Cipher Feedback (CFB) mode processes small increments of plain text into
//     cipher text, instead of processing an entire block at a time. This mode uses
//     a shift register that is one block in length and is divided into sections. For
//     example, if the block size is 8 bytes, with one byte processed at a time, the
//     shift register is divided into eight sections. If a bit in the cipher text is
//     mangled, one plain text bit is mangled and the shift register is corrupted. This
//     results in the next several plain text increments being mangled until the bad
//     bit is shifted out of the shift register. The default feedback size can vary
//     by algorithm, but is typically either 8 bits or the number of bits of the block
//     size. You can alter the number of feedback bits by using the System.Security.Cryptography.SymmetricAlgorithm.FeedbackSize
//     property. Algorithms that support CFB use this property to set the feedback.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation"/>
public class CFBCipherMode(IBlockCipher cipher, byte[] iv) : CipherModeIV(cipher, iv), IBlockCipher
    {
    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {            
        this.cipher.EncryptBlock(this.iv, 0, this.iv.Length, outputBuffer, outputOffset);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
            }

        Buffer.BlockCopy(outputBuffer, outputOffset, this.iv, 0, this.iv.Length);
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.cipher.EncryptBlock(this.iv, 0, this.iv.Length, outputBuffer, outputOffset);

        Buffer.BlockCopy(inputBuffer, inputOffset, this.iv, 0, this.iv.Length);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
            }
        }
    }
