
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

/// <summary>
/// Interface for cipher padding implementations
/// </summary>
/// <see cref="https://www.di-mgt.com.au/cryptopad.html"/>
/// <see cref="http://www.crypto-it.net/eng/theory/padding.html"/>
/// <see cref="https://crypto.stackexchange.com/questions/24130/why-can-the-last-block-contain-a-full-block-of-padding-in-cbc-encryption"/>
public interface IPadding
    {
    /// <summary>
    /// Pads the specified input with a given number of bytes filled with zero (like with ZerosPadding).
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which the data to pad starts.</param>
    /// <param name="length">The number of bytes in <paramref name="input"/> to take into account.</param>
    /// <param name="paddingLength">The number of bytes to pad the input with.</param>
    /// <remarks>Todos los metodos (Pad/Unpad) deben retornar una copia del buffer de entrada.</remarks>
    /// <returns>
    /// The padded data array.
    /// </returns>
    byte[] Pad(byte[] input, int offset, int length, int paddingLength);

    /// <summary>
    /// Get the padding length.
    /// </summary>
    int GetPaddingLength(byte[] paddedBuffer, int offset, int length);
    }
