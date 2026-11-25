namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;

public static class PaddingExtensions
    {
    /// <summary>
    /// Pad the buffer.
    /// </summary>
    /// <remarks>
    /// Todos los metodos (Pad/Unpad) deben retornar una copia del buffer de entrada, 
    /// por lo que este debe ser pequeño.
    /// </remarks>
    public static byte[] Pad(this IPadding padding, int blockSize)
        => padding.Pad(blockSize, [], 0, 0);

    /// <summary>
    /// Pads the specified input to match the block size.
    /// </summary>
    /// <param name="blockSize">Size of the block.</param>
    /// <param name="input">The input.</param>
    /// <returns>
    /// Padded data array.
    /// </returns>
    public static byte[] Pad(this IPadding padding, int blockSize, byte[] input)
        => padding.Pad(blockSize, input, 0, input.Length);

    /// <summary>
    /// Pads the specified input to match the block size.
    /// </summary>
    /// <param name="blockSize">Size of the block.</param>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which the data to pad starts.</param>
    /// <param name="length">The number of bytes in <paramref name="input"/> to take into account.</param>
    /// <returns>
    /// The padded data array.
    /// </returns>
    public static byte[] Pad(this IPadding padding, int blockSize, byte[] input, int offset, int length)
        => padding.Pad(input, offset, length, blockSize - length % blockSize);

    /// <summary>
    /// Pads the specified input with a given number of bytes.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="paddingLength">The number of bytes to pad the input with.</param>
    /// <returns>
    /// The padded data array.
    /// </returns>
    public static byte[] Pad(this IPadding padding, byte[] input, int paddingLength)
        => padding.Pad(input, 0, input.Length, paddingLength);

    /// <summary>
    /// Pads the specified input with a given number of bytes filled with zero (like with ZerosPadding).
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which the data to pad starts.</param>
    /// <param name="length">The number of bytes in <paramref name="input"/> to take into account.</param>
    /// <param name="paddingLength">The number of bytes to pad the input with.</param>
    /// <returns>
    /// The padded data array.
    /// </returns>
    public static byte[] Pad(byte[] input, int offset, int length, int paddingLength)
        {
        byte[] output = new byte[length + paddingLength];
        Buffer.BlockCopy(input, offset, output, 0, length);
        return output;
        }

    /// <summary>
    /// Remove padding from the specified input.
    /// </summary>
    /// <remarks>
    /// Retorna siempre una copia de la entrada, aunque no haya ningún padding que quitar.
    /// Cuando sea posible es preferible procesar el buffer hasta el padding en lugar de hacer una copia con este metodo.
    /// Aplicar solo al ultimo bloque en lugar de a todo el buffer, para evitar una copia excesiva de datos.
    /// </remarks>
    /// <param name="input">The input.</param>
    /// <returns>
    /// The padded data array.
    /// </returns>
    public static byte[] Unpad(this IPadding padding, byte[] paddedBuffer)
        {
        int paddingLength = padding.GetPaddingLength(paddedBuffer);

        byte[] output = new byte[paddedBuffer.Length - paddingLength];
        Buffer.BlockCopy(paddedBuffer, 0, output, 0, output.Length);

        return output;
        }

    public static int GetPaddingLength(this IPadding padding, byte[] paddedBuffer)
        => padding.GetPaddingLength(paddedBuffer, 0, paddedBuffer.Length);
    }
