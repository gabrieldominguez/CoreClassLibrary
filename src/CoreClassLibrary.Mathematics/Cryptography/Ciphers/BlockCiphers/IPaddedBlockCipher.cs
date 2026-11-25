
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

public interface IPaddedBlockCipher: IBlockCipher
    {
    /// <summary>
    /// Encripta el bloque final teniendo en cuenta el padding.
    /// </summary>
    /// <remarks>
    /// Si la longitud del buffer a encriptar es multiplo del tamaño del bloque y se usa padding, 
    /// se debe agregar un bloque mas, por lo si inputCount == 0 se crea un bloque completamente con padding, se encripta y retorna. 
    /// </remarks>
    byte[] EncryptFinalBlock(byte[] input, int offset, int count);

    /// <summary>
    /// Desencripta el bloque final teniendo en cuenta el padding.
    /// </summary>
    /// <remarks>
    /// El bloque final puede ser vacio, si la buffer desencriptado es multiplo del tamaño del bloque.
    /// </remarks>
    byte[] DecryptFinalBlock(byte[] input, int offset, int count);
    }
