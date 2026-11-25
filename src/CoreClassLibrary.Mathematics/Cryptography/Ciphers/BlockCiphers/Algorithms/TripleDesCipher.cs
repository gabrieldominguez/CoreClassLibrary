// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;

/// <summary>
/// Implements 3DES cipher algorithm.
/// </summary>
/// <remarks>
/// Triple DES (3DES), is a symmetric-key block cipher, which applies the DES cipher algorithm three times to each data block.
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Triple_DES"/>
/// <see cref="DesCipher"/>
public sealed class TripleDesCipher : CipherBase, IBlockCipher
    {
    private readonly int[][] encryptionKeys = new int[3][];
    private readonly int[][] decryptionKeys = new int[3][];

    private readonly byte[] buffer = new byte[8];

    public int BlockSize => 8;

    public TripleDesCipher(byte[] key): base(key)
        {
        if (key.Length != 16 && key.Length != 24)
            {
            throw new ArgumentException(string.Format("Key length '{0}' is not valid for this algorithm.", key.Length));
            }

        this.MakeWorkingKeys();
        }

    public void MakeWorkingKeys()
        {
        this.MakeWorkingKeys(this.key, 0);
        this.MakeWorkingKeys(this.key, 1);

        if (this.key.Length == 24)
            {
            this.MakeWorkingKeys(this.key, 2);
            }
        else
            {
            this.encryptionKeys[2] = this.encryptionKeys[0];
            this.decryptionKeys[2] = this.decryptionKeys[0];
            }
        }

    private void MakeWorkingKeys(byte[] key, int i)
        {
        byte[] part = new byte[8];
        Buffer.BlockCopy(key, i * 8, part, 0, 8);
        this.encryptionKeys[i] = DesCipher.MakeWorkingKey(part, i != 1);
        this.decryptionKeys[i] = DesCipher.MakeWorkingKey(part, i == 1);
        }

    public void EncryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        DesCipher.DesFunc(this.encryptionKeys[0], inputBuffer, inputOffset, this.buffer, 0);
        DesCipher.DesFunc(this.encryptionKeys[1], this.buffer, 0, this.buffer, 0);
        DesCipher.DesFunc(this.encryptionKeys[2], this.buffer, 0, outputBuffer, outputOffset);
        }

    public void DecryptBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        DesCipher.DesFunc(this.decryptionKeys[2], inputBuffer, inputOffset, this.buffer, 0);
        DesCipher.DesFunc(this.decryptionKeys[1], this.buffer, 0, this.buffer, 0);
        DesCipher.DesFunc(this.decryptionKeys[0], this.buffer, 0, outputBuffer, outputOffset);
        }
    }
