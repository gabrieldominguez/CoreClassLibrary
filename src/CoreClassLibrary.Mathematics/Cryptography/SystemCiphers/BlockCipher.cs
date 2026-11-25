
using CoreClassLibrary.Extensions;
using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.SystemCiphers;

/// <summary>
/// Clase base para algoritmos criptográficos simétricos.
/// </summary>
/// <remarks>
/// Para transformaciones criptográficas donde el tamaño del bloque y la clave son iguales.
/// </remarks>    
public abstract class BlockCipher : SymmetricAlgorithm
    {
    protected static bool UsesIv(CipherMode cipherMode)
        => cipherMode != CipherMode.ECB;

    public static bool IsLegalSize(int size, KeySizes[] legalSizes)
        {
        foreach (KeySizes currentSizes in legalSizes)
            {
            // If a cipher has only one valid key size, MinSize == MaxSize and SkipSize will be 0
            if (currentSizes.SkipSize == 0)
                {
                if (currentSizes.MinSize == size)
                    {
                    return true;
                    }
                }
            else if (size >= currentSizes.MinSize && size <= currentSizes.MaxSize)
                {
                // If the number is in range, check to see if it's a legal increment above MinSize
                int delta = size - currentSizes.MinSize;

                // While it would be unusual to see KeySizes { 10, 20, 5 } and { 11, 14, 1 }, it could happen.
                // So don't return false just because this one doesn't match.
                if (delta % currentSizes.SkipSize == 0)
                    {
                    return true;
                    }
                }                    
            }

        return false;
        }

    private readonly RandomNumberGenerator randomNumberGenerator;

    /// <summary>
    /// Contructor para bloques de 128 bits (16 bytes)
    /// </summary>
    protected BlockCipher(): this(128) { }

    /// <summary>
    /// Contructor 
    /// </summary>
    /// <param name="blockSize">Tamaño del bloque en bits.</param>
    protected BlockCipher(int blockSize): this(blockSize, RandomNumberGenerator.Create()) { }

    protected BlockCipher(int blockSize, RandomNumberGenerator randomNumberGenerator)
        {
        this.randomNumberGenerator = randomNumberGenerator;

        if(blockSize % Binary.BitsPerByte != 0)
            {
            throw new ArgumentException("Invalid block size.", nameof(blockSize));
            }

        this.LegalKeySizesValue = [new KeySizes(blockSize, blockSize, 0)];
        this.LegalBlockSizesValue = [new KeySizes(blockSize, blockSize, 0)];

        this.KeySize = this.BlockSize = blockSize;
        }

    public sealed override void GenerateIV()
        // => this.randomNumberGenerator.GetBytes(this.IV);    // StackOverflowException
        {
        byte[] buffer = new byte[this.BlockSize / Binary.BitsPerByte];
        this.randomNumberGenerator.GetBytes(buffer);
        this.IV = buffer;   // el arreglo es clonado
        }

    public sealed override void GenerateKey()
        // => this.randomNumberGenerator.GetBytes(this.Key);   // StackOverflowException
        {
        byte[] buffer = new byte[this.KeySize / Binary.BitsPerByte];
        this.randomNumberGenerator.GetBytes(buffer);
        this.Key = buffer;   // el arreglo es clonado
        }

    public sealed override ICryptoTransform CreateEncryptor(byte[] key, byte[]? iv)
        {
        this.ValidateSizes(key, iv);
        return this.CreateEncryptorCore(key, iv?.CloneArray<byte>());
        }

    protected abstract ICryptoTransform CreateEncryptorCore(byte[] key, byte[]? iv);

    public sealed override ICryptoTransform CreateDecryptor(byte[] key, byte[]? iv)
        {
        this.ValidateSizes(key, iv);
        return this.CreateDecryptorCore(key, iv?.CloneArray<byte>());
        }

    protected abstract ICryptoTransform CreateDecryptorCore(byte[] key, byte[]? iv);

    protected void ValidateSizes(byte[] key, byte[]? iv)
        {
        long keySize = key.Length * (long)Binary.BitsPerByte;
        if (keySize > int.MaxValue || 
            !BlockCipher.IsLegalSize((int)keySize, this.LegalKeySizes))
            {
            throw new ArgumentException("Invalid Key Size", nameof(key));
            }

        if (iv != null)
            {
            long ivSize = iv.Length * (long)Binary.BitsPerByte;
            if (ivSize != this.BlockSize)
                {
                throw new ArgumentException("Invalid IV Size", nameof(iv));
                }
            }
        }
    }
