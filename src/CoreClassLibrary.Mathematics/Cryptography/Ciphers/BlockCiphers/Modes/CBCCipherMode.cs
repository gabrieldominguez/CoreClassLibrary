// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;

/// <summary>
/// Implements CBC cipher mode
/// </summary>
/// <remarks>
///    The Cipher Block Chaining (CBC) mode introduces feedback. Before each plain text
//     block is encrypted, it is combined with the cipher text of the previous block
//     by a bitwise exclusive OR operation. This ensures that even if the plain text
//     contains many identical blocks, they will each encrypt to a different cipher
//     text block. The initialization vector is combined with the first plain text block
//     by a bitwise exclusive OR operation before the block is encrypted. If a single
//     bit of the cipher text block is mangled, the corresponding plain text block will
//     also be mangled. In addition, a bit in the subsequent block, in the same position
//     as the original mangled bit, will be mangled.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation"/>
public class CBCCipherMode(IBlockCipher cipher, byte[] iv) : CipherModeIV(cipher, iv), IBlockCipher
    {
    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        for (int i = 0; i < this.iv.Length; i++)
            {
            this.iv[i] ^= inputBuffer[inputOffset + i];
            }

        this.cipher.EncryptBlock(this.iv, 0, inputCount, outputBuffer, outputOffset);

        Buffer.BlockCopy(outputBuffer, outputOffset, this.iv, 0, this.iv.Length);
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.cipher.DecryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= this.iv[i];
            }

        Buffer.BlockCopy(inputBuffer, inputOffset, this.iv, 0, this.iv.Length);
        }
    }
