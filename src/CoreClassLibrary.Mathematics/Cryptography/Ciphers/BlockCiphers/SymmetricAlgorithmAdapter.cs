using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

/// <summary>
/// Adaptador para usar BlockCipher con CryptoStream.
/// </summary>
/// <remarks>
/// Al desencriptar el padding debe quitarse manualmente.
/// </remarks>
public class SymmetricAlgorithmAdapter(IPaddedBlockCipher blockCipher) : SymmetricAlgorithm
    {

    // public sealed override int BlockSize => this.blockCipher.BlockSize * MathExtensions.BitsPerByte;

    public override void GenerateIV() { }
    public override void GenerateKey() { }

    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
        => new BlockCipherDecryptTransform(blockCipher);

    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV)
        => new BlockCipherEncryptTransform(blockCipher);
    }

public class BlockCipherEncryptTransform(IPaddedBlockCipher blockCipher) : CryptoTransformAdapter(blockCipher)
    {
    public override int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.blockCipher.EncryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        return inputCount;
        }

    public override byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        => this.blockCipher.EncryptFinalBlock(inputBuffer, inputOffset, inputCount);
    }

public class BlockCipherDecryptTransform(IPaddedBlockCipher blockCipher) : CryptoTransformAdapter(blockCipher)
    {
    public override int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        this.blockCipher.DecryptBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        return inputCount;
        }

    public override byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) => this.blockCipher.DecryptFinalBlock(inputBuffer, inputOffset, inputCount); // new byte[0]; 
    }

/// <summary>
/// Adaptador para usar BlockCipher con CryptoStream.
/// </summary>
public abstract class CryptoTransformAdapter : ICryptoTransform
    {
    protected readonly IPaddedBlockCipher blockCipher;

    protected CryptoTransformAdapter(IPaddedBlockCipher blockCipher)
        {
        (this.blockCipher = blockCipher).Initialize();
        }

    public bool CanReuseTransform => false;
    public bool CanTransformMultipleBlocks => false;

    public int InputBlockSize => this.blockCipher.BlockSize;
    public int OutputBlockSize => this.blockCipher.BlockSize;

    public void Dispose() => GC.SuppressFinalize(this);

    public abstract int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);
    public abstract byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
    }
