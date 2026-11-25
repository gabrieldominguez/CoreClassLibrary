
using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.SystemCiphers;

/// <summary>
/// Encripta usando un algoritmo propio (de cuando era estudiante).
/// </summary>
/// <remarks>
/// Corresponde a la 1er version donde no se usa vector de inicializacion ni los valores de los indices.
/// 
/// Para un uso productivo usar AES.
/// </remarks>
/// <see cref="System.Security.Cryptography.Aes"/>
public class MyNaiveCipher : BlockCipher
    {
    private class NaiveEncryptorTransform(byte[] key, byte[]? iv) : ByteEncryptTransform(key, iv, CipherMode.ECB)
        {
        protected override byte EncryptByte(byte input, byte key)
            {
            int output = input + key;               // cod% = ASC(MID$(texto$, indTexto %, 1)) + ASC(MID$(clave$, indClave %, 1))
            if (output > 255) { output -= 256; }    // IF cod% > 255 THEN cod% = cod% -256

            return (byte) output;
            }
        }

    private class NaiveDecryptorTransform(byte[] key, byte[]? iv) : ByteDecryptTransform(key, iv, CipherMode.ECB)
        {
        protected override byte DecryptByte(byte input, byte key)
            {
            int output = input - key;               // cod% = ASC(MID$(texto$, indTexto %, 1)) - ASC(MID$(clave$, indClave %, 1))
            if (output < 0) { output += 256; }      // IF cod% < 0 THEN cod% = cod% +256

            return (byte)output;
            }
        }

    public override CipherMode Mode => CipherMode.ECB;

    protected override ICryptoTransform CreateEncryptorCore(byte[] key, byte[]? iv)
        => new NaiveEncryptorTransform(key, iv);

    protected override ICryptoTransform CreateDecryptorCore(byte[] key, byte[]? iv)
        => new NaiveDecryptorTransform(key, iv);
    }

/*

============================================ Funciones originales en QuickBasic (29/05/1995) ============================================ 

FUNCTION encriptar$ (texto$, clave$)
'Encripta la variable pasada como texto$ con la pasada como clave$

'texto$= texto a encriptar
'clave$= clave con la cual encriptar el texto

IF NOT clave$ = "" THEN
    textoEncrip$ = ""
    fintexto% = LEN(texto$)
    finClave% = LEN(clave$)
    clave$ = codificar$(clave$)
    indClave% = 1
    FOR indTexto% = 1 TO fintexto%
            indClave% = indClave% - 1
            IF indClave% = 0 THEN indClave% = finClave%
            cod% = ASC(MID$(texto$, indTexto%, 1)) + ASC(MID$(clave$, indClave%, 1))
            IF cod% > 255 THEN cod% = cod% - 256
            textoEncrip$ = textoEncrip$ + CHR$(cod%)
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
    fintexto% = LEN(texto$)
    finClave% = LEN(clave$)
    clave$ = codificar$(clave$)
    indClave% = 1
    FOR indTexto% = 1 TO fintexto%
            indClave% = indClave% - 1
            IF indClave% = 0 THEN indClave% = finClave%
            cod% = ASC(MID$(texto$, indTexto%, 1)) - ASC(MID$(clave$, indClave%, 1))
            IF cod% < 0 THEN cod% = cod% + 256
            textoEncrip$ = textoEncrip$ + CHR$(cod%)
    NEXT
    desencriptar$ = textoEncrip$
END IF
END FUNCTION

*/
