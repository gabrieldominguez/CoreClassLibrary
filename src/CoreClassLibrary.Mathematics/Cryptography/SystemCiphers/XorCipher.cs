
using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.SystemCiphers;

/// <summary>
/// Encripta usando XOR.
/// </summary>
/// <remarks>
/// Para un uso productivo usar AES.
/// </remarks>
/// <see cref="System.Security.Cryptography.Aes"/>
public sealed class XorCipher : BlockCipher
    {
    private class XorEncryptorTransform(byte[] key, byte[]? iv, CipherMode cipherMode) : ByteEncryptTransform(key, iv, cipherMode)
        {
        protected override byte EncryptByte(byte input, byte key)
            => (byte)(input ^ key);
        }

    private class XorDecryptorTransform(byte[] key, byte[]? iv, CipherMode cipherMode) : ByteDecryptTransform(key, iv, cipherMode)
        {
        protected override byte DecryptByte(byte input, byte key)
            => (byte)(input ^ key);
        }

    protected override ICryptoTransform CreateEncryptorCore(byte[] key, byte[]? iv)
        => new XorEncryptorTransform(key, iv, this.Mode);

    protected override ICryptoTransform CreateDecryptorCore(byte[] key, byte[]? iv)
        => new XorDecryptorTransform(key, iv, this.Mode);
    }
