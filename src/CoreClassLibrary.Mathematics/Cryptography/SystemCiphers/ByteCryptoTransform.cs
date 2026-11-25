using System.Security.Cryptography;
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Cryptography.SystemCiphers;

/// <summary>
/// Clase base para encriptar byte a byte.
/// </summary>
/// <remarks>
/// Soporta Cipher Block Chaining (CBC).
/// </remarks>
public abstract class ByteEncryptTransform : ByteCryptoTransform
    {
    protected ByteEncryptTransform(byte[] key, byte[]? iv, CipherMode cipherMode = CipherMode.CBC): base(key, iv,  cipherMode) { }

    /// <summary>
    /// Transforma un byte.
    /// </summary>
    /// <param name="input">Byte a transformar.</param>
    /// <param name="i">Indice del byte dentro del bloque.</param>
    /// <returns>El byte de entrada transformado.</returns>
    protected sealed override byte TransformByte(byte input, int i)
        {
        if (this.cipherMode == CipherMode.CBC)
            {
            input ^= this.iv![i];
            }

        byte output = this.EncryptByte(input, this.key[i]);

        if (this.cipherMode == CipherMode.CBC)
            {
            this.iv![i] = output;
            }

        return output;
        }

    /// <summary>
    /// Encripta un byte.
    /// </summary>
    /// <param name="input">Byte de entrada a encriptar.</param>
    /// <param name="i">Byte de la clave con la que encriptar el byte de entrada.</param>
    /// <returns>El byte de entrada encriptado.</returns>
    protected abstract byte EncryptByte(byte input, byte key);
    }

/// <summary>
/// Clase base para desencriptar byte a byte.
/// </summary>
/// <remarks>
/// Soporta Cipher Block Chaining (CBC).
/// </remarks>
public abstract class ByteDecryptTransform : ByteCryptoTransform
    {
    protected ByteDecryptTransform(byte[] key, byte[]? iv, CipherMode cipherMode = CipherMode.CBC): base(key, iv,  cipherMode) { }

    /// <summary>
    /// Transforma un byte.
    /// </summary>
    /// <param name="input">Byte a transformar.</param>
    /// <param name="i">Indice del byte dentro del bloque.</param>
    /// <returns>El byte de entrada transformado.</returns>
    protected sealed override byte TransformByte(byte input, int i)
        {
        byte output = this.DecryptByte(input, this.key[i]);

        if (this.cipherMode == CipherMode.CBC)
            {
            output ^= this.iv![i];
            this.iv[i] = input;
            }

        return output;
        }

    /// <summary>
    /// Desencripta un byte.
    /// </summary>
    /// <param name="input">Byte de entrada a desencriptar.</param>
    /// <param name="i">Byte de la clave con la que desencriptar el byte de entrada.</param>
    /// <returns>El byte de entrada desencriptado.</returns>
    protected abstract byte DecryptByte(byte input, byte key);
    }

/// <summary>
/// Clase base para transformaciones criptográficas.
/// </summary>
/// <remarks>
/// Ofrece métodos para transformaciones byte a byte.
///
/// Ni CryptoStream, ni SymmetricAlgorithm saben nada del modo de cifrado, ni del padding, por lo que
/// esta funcionalidad debe ser provista por las clases que implementen ICryptoTransform
///
/// Al operar byte a byte y no en bloque, esta clase no necesita padding.
/// </remarks>
public abstract class ByteCryptoTransform : ICryptoTransform
    {
    public bool CanTransformMultipleBlocks => false;
    public int InputBlockSize { get; }
    public int OutputBlockSize { get; }

    /// <summary>
    /// Esta clase solo se puede reutilizar si no se usa un vector de inicializacion (IV),
    /// porque este ultimo queda sobreescrito luego de la transformación.
    /// </summary>
    public bool CanReuseTransform => this.cipherMode == CipherMode.ECB;

    /// <summary>
    /// Modo de cifrado.
    /// </summary>
    /// <remarks>
    /// CBC = 1,
    ///     El modo Cipher Block Chaining (CBC) incluye información de cifrados anteriores.Antes
    ///     de cifrarse cada bloque de texto sin formato, cada uno de ellos se combina con
    ///     el texto de cifrado del bloque anterior mediante una operación OR exclusiva bit
    ///     a bit.Esto garantiza que, aunque el texto sin formato contenga muchos bloques
    ///     idénticos, cada uno de ellos se cifrará en un bloque de texto de cifrado distinto.El
    ///     vector de inicialización se combina con el primer bloque de texto sin formato
    ///     mediante una operación OR exclusiva bit a bit antes de cifrar el bloque.Si se
    ///     trastoca un único bit del bloque de texto de cifrado, se trastoca también el
    ///     bloque de texto sin formato correspondiente.Asimismo, se trastocará el bit del
    ///     bloque siguiente que se encuentra en la misma posición que el bit trastocado
    ///     original.
    ///
    /// ECB = 2,
    ///     El modo Electronic Codebook (ECB) cifra cada bloque por separado.Los bloques
    ///     de texto sin formato que sean idénticos y estén en el mismo mensaje, o en uno
    ///     diferente cifrado con la misma clave, se convertirán en bloques de texto de cifrado
    ///     idénticos.Importante: no se recomienda este modo porque representa un riesgo
    ///     para la seguridad.Si el texto sin formato que se va a cifrar contiene muchas
    ///     repeticiones, es posible que el texto de cifrado se divida en un bloque cada
    ///     vez.También es posible usar el análisis de bloques para determinar la clave de
    ///     cifrado.Un adversario activo también puede sustituir e intercambiar bloques individuales
    ///     sin ser detectado, lo que permite guardar los bloques e insertarlos en la secuencia
    ///     en otros puntos sin que se advierta.
    ///
    ///
    /// CTS = 5
    ///     El modo Cipher Text Stealing (CTS) controla el texto sin formato de cualquier
    ///     tamaño y produce texto de cifrado cuya longitud coincide con la del texto sin
    ///     formato.Este modo se comporta igual que el modo CBC para todos los bloques excepto
    ///     para los dos últimos del texto sin formato.
    /// </remarks>
    /// <see cref="https://de.wikipedia.org/wiki/Cipher_Block_Chaining_Mode#Integrit.C3.A4tssicherung_mit_CBC.2C_CBC-MAC"/>
    protected readonly CipherMode cipherMode;

    protected readonly byte[] key;
    protected readonly byte[]? iv;

    protected ByteCryptoTransform(byte[] key, byte[]? iv, CipherMode cipherMode = CipherMode.CBC)
        {
        if ((this.cipherMode = cipherMode) == CipherMode.CTS)
            {
            throw new ArgumentException("Cipher Text Stealing (CTS) not supported.");
            }

        this.InputBlockSize = this.OutputBlockSize = (this.key = key).Length;

        if (cipherMode == CipherMode.CBC)
            {
            /*
            if (key.Length != iv.Length)
                {
                throw new ArgumentException("Key and IV must have the same size.");
                }
            */

            this.iv = iv?.CloneArray<byte>() ?? throw new InvalidOperationException("In CBC mode and IV must be provided.");
            }
        }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
        byte[] outputBuffer = new byte[inputCount];

        this.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0);

        return outputBuffer;
        }

    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
        for (int i = 0; i < inputCount; i++)
            {
            byte input = inputBuffer[inputOffset + i];

            byte output = this.TransformByte(input, i);

            outputBuffer[outputOffset + i] = output;
            }

        return inputCount;
        }

    /// <summary>
    /// Transforma un byte.
    /// </summary>
    /// <param name="input">Byte a transformar.</param>
    /// <param name="i">Indice del byte dentro del bloque.</param>
    /// <returns>El byte de entrada transformado.</returns>
    protected abstract byte TransformByte(byte input, int i);

    public virtual void Dispose()
        {
        GC.SuppressFinalize(this);
        this.iv?.Clear();
        }
    }
