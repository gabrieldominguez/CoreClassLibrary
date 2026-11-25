// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;

/// <summary>
/// Implements PCBC cipher mode
/// </summary>
/// <remarks>
/// The Propagating Cipher Block Chaining or plaintext cipher-block chaining mode was designed 
/// to cause small changes in the ciphertext to propagate indefinitely when decrypting, 
/// as well as when encrypting.In PCBC mode, each block of plaintext is XORed with both 
/// the previous plaintext block and the previous ciphertext block before being encrypted.
/// As with CBC mode, an initialization vector is used in the first block.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation"/>
public class PCBCCipherMode(IBlockCipher cipher, byte[] iv) : CipherModeIV(cipher, iv), IBlockCipher
    {
    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        for (int i = 0; i < this.iv.Length; i++)
            {
            this.iv[i] ^= inputBuffer[inputOffset + i];
            }

        this.cipher.EncryptBlock(this.iv, 0, inputCount, outputBuffer, outputOffset);

        Buffer.BlockCopy(outputBuffer, outputOffset, this.iv, 0, this.iv.Length);

        for (int i = 0; i < this.iv.Length; i++)
            {
            this.iv[i] ^= inputBuffer[inputOffset + i];
            }
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.cipher.DecryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= this.iv[i];
            }

        Buffer.BlockCopy(inputBuffer, inputOffset, this.iv, 0, this.iv.Length);

        for (int i = 0; i < this.iv.Length; i++)
            {
            this.iv[i] ^= outputBuffer[outputOffset + i];
            }
        }
    }
