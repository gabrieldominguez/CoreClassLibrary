
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;

public class CipherModeIV: CipherMode
    {
    private readonly byte[] iv0;
    protected byte[] iv;

    public CipherModeIV(IBlockCipher cipher, byte[] iv): base(cipher)
        {
        if (cipher.BlockSize != iv.Length)
            {
            throw new InvalidOperationException(String.Format("Cipher.BlockSize ({0}) not equal IV size ({1})", cipher.BlockSize, iv.Length));
            }

        base.Initialize();
        this.iv0 = iv.CloneArray();
        this.iv = iv0.CloneArray();
        }

    public sealed override void Initialize()
        {
        base.Initialize();
        this.iv = iv0.CloneArray();
        }
    }

public class CipherMode(IBlockCipher cipher)
    {
    protected readonly IBlockCipher cipher = cipher;

    public int BlockSize => this.cipher.BlockSize;

    public virtual void Initialize()
        => this.cipher.Initialize();
    }
