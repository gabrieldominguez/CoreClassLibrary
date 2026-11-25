
// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;

/// <summary>
/// Implements CTR cipher mode
/// </summary>
/// <remarks>
/// Counter (CTR)
/// Note: CTR mode(CM) is also known as integer counter mode(ICM) and segmented integer counter(SIC) mode
/// Like OFB, Counter mode turns a block cipher into a stream cipher.It generates the next keystream block 
/// by encrypting successive values of a "counter". The counter can be any function which produces a sequence 
/// which is guaranteed not to repeat for a long time, although an actual increment-by-one counter is the simplest 
/// and most popular.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Block_cipher_mode_of_operation"/>
public class CTRCipherMode(IBlockCipher cipher, byte[] iv) : CipherModeIV(cipher, iv), IBlockCipher
    {
    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.cipher.EncryptBlock(this.iv, 0, this.iv.Length, outputBuffer, outputOffset);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
            }

        // increment IV
        int j = this.iv.Length; while (--j >= 0 && ++this.iv[j] == 0);
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.cipher.EncryptBlock(this.iv, 0, this.iv.Length, outputBuffer, outputOffset);

        for (int i = 0; i < this.iv.Length; i++)
            {
            outputBuffer[outputOffset + i] ^= inputBuffer[inputOffset + i];
            }

        // increment IV
        int j = this.iv.Length; while (--j >= 0 && ++this.iv[j] == 0);
        }
    }
