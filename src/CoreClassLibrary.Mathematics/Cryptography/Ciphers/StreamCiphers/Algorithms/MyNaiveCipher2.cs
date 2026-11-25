
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

/// <summary>
/// Encripta usando un algoritmo propio (de cuando era estudiante).
/// </summary>
/// <remarks>
/// Corresponde a la 2da version donde se usa el byte anterior como vector de inicializacion y los valores de los indices.
/// El vector de inicializacion se usa de forma inversa que en el estandar CBC, es decir que en la encriptacion se captura (para usarse en el proximo bloque) antes de cifrar y en la desencriptacion despues.
/// No sumar inputOffset o outputOffset en el calculo porque SymmetricAlgorithm cambia la estrategia de buffer entre al encriptacion y la desencriptacion.
/// 
/// Para un uso productivo usar AES.
/// </remarks>
/// <see cref="System.Security.Cryptography.Aes"/>
public sealed class MyNaiveStreamCipher2(byte[] key, byte[] iv) : MyNaiveCipher2Base(key, iv), IStreamCipher
    {
    public byte Encrypt(byte input)
        {
        byte output = (byte)((input + this.key[this.iKey] + this.iv[this.iIV] + this.iKey + this.iIV) % 256);    // cod% = (car% + ASC(MID$(clave$, indClave%, 1)) + indTexto% + indClave% + finClave% + carAnt%) MOD 256

        this.iv[this.iIV] = input; // carAnt% = car%

        this.iKey = (this.iKey + 1) % this.key.Length;
        this.iIV = (this.iIV + 1) % this.iv.Length;

        return output;
        }

    public byte Decrypt(byte input)
        {
        int output = 256 - (256 - input + this.key[this.iKey] + this.iv[this.iIV] + this.iKey + this.iIV) % 256;        // cod% = 256 - (-(ASC(MID$(texto$, indTexto%, 1)) - ASC(MID$(clave$, indClave%, 1)) - indTexto% - indClave% - finClave% - cod%) + 256) MOD 256

        if (output == 256) { output = 0; }  // IF cod% = 256 THEN cod% = 0

        byte outputByte = (byte)output;
        this.iv[this.iIV] = outputByte;

        this.iKey = (this.iKey + 1) % this.key.Length;
        this.iIV = (this.iIV + 1) % this.iv.Length;

        return outputByte;
        }
    }

public sealed class MyNaiveCipher2(byte[] key, byte[] iv) : MyNaiveCipher2Base(key, iv), ICipher
    {

    /// <summary>
    /// Encrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin encrypting.</param>
    /// <param name="length">The number of bytes to encrypt from <paramref name="input"/>.</param>
    /// <returns>
    /// Encrypted data.
    /// </returns>
    public byte[] Encrypt(byte[] input, int offset, int count)
        {
        byte[] output = new byte[count];

        for (int i = 0; i < count; i++)
            {
            byte inputByte = input[i + offset];
            byte outputByte = (byte)((inputByte + this.key[this.iKey] + this.iv[this.iIV] + i + offset + count + this.iKey + this.iIV) % 256);    // cod% = (car% + ASC(MID$(clave$, indClave%, 1)) + indTexto% + indClave% + finClave% + carAnt%) MOD 256

            this.iv[this.iIV] = inputByte; // carAnt% = car%

            output[i] = outputByte;

            this.iKey = (this.iKey + 1) % this.key.Length;
            this.iIV = (this.iIV + 1) % this.iv.Length;
            }

        return output;
        }

    /// <summary>
    /// Decrypts the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="offset">The zero-based offset in <paramref name="input"/> at which to begin decrypting.</param>
    /// <param name="count">The number of bytes to decrypt from <paramref name="input"/>.</param>
    /// <returns>
    /// The decrypted data.
    /// </returns>
    public byte[] Decrypt(byte[] input, int offset, int count)
        {
        byte[] output = new byte[count];

        for (int i = 0; i < count; i++)
            {
            int outputByte = 256 - (256 - input[i + offset] + this.key[this.iKey] + this.iv[this.iIV] + i + offset + count + this.iKey + this.iIV) % 256;        // cod% = 256 - (-(ASC(MID$(texto$, indTexto%, 1)) - ASC(MID$(clave$, indClave%, 1)) - indTexto% - indClave% - finClave% - cod%) + 256) MOD 256

            if (outputByte == 256) { outputByte = 0; }  // IF cod% = 256 THEN cod% = 0

            output[i] = this.iv[this.iIV] = (byte)outputByte;

            this.iKey = (this.iKey + 1) % this.key.Length;
            this.iIV = (this.iIV + 1) % this.iv.Length;
            }

        return output;
        }
    }

public class MyNaiveCipher2Base 
    {
    protected int iKey = 0;
    protected int iIV = 0;

    protected readonly byte[] key;
    protected readonly byte[] iv0;
    protected byte[] iv;

    public MyNaiveCipher2Base(byte[] key, byte[] iv)
        {
        this.key = key.CloneArray();
        this.iv0 = iv.CloneArray();
        this.iv = iv0.CloneArray();
        }

    public void Initialize()
        {
        this.iv = iv0.CloneArray();
        this.iKey = this.iIV = 0;
        }
    }

/*

============================================ Funciones originales en QuickBasic (08/12/1996) ============================================ 

FUNCTION encriptar$ (texto$, clave$)
'Encripta la variable pasada como texto$ con la pasada como clave$

'texto$= texto a encriptar
'clave$= clave con la cual encriptar el texto

IF NOT clave$ = "" THEN
    textoEncrip$ = ""
    finClave% = LEN(clave$)
    clave$ = codificar$(clave$)
    indClave% = 1
    FOR indTexto% = 1 TO LEN(texto$)
            'El puntero de la clave corre de derecha a izquierda
            indClave% = indClave% - 1
            IF indClave% = 0 THEN indClave% = finClave%     'El puntero de la clave da una vuelta
            car% = ASC(MID$(texto$, indTexto%, 1))
            cod% = car% + ASC(MID$(clave$, indClave%, 1)) + indTexto% + indClave% + finClave% + carAnt%
            cod% = cod% MOD 256
            textoEncrip$ = textoEncrip$ + CHR$(cod%)
            carAnt% = car%
    NEXT
    encriptar$ = textoEncrip$
END IF
END FUNCTION


FUNCTION desencriptar$ (texto$, clave$)
'Desencripta la variable pasada como texto$  (la cual ha sido encriptada con encriptar$) con la pasada como clave$

'texto$= texto a desencriptar
'clave$= clave con la cual desencriptar el texto
IF NOT clave$ = "" THEN
    textoEncrip$ = ""
    finClave% = LEN(clave$)
    clave$ = codificar$(clave$)
    indClave% = 1
    FOR indTexto% = 1 TO LEN(texto$)
            indClave% = indClave% - 1
            IF indClave% = 0 THEN indClave% = finClave%
            cod% = ASC(MID$(texto$, indTexto%, 1)) - ASC(MID$(clave$, indClave%, 1)) - indTexto% - indClave% - finClave% - cod%
            cod% = 256 - (-cod% + 256) MOD 256
            IF cod% = 256 THEN cod% = 0
            textoEncrip$ = textoEncrip$ + CHR$(cod%)
    NEXT
    desencriptar$ = textoEncrip$
END IF
END FUNCTION

*/
