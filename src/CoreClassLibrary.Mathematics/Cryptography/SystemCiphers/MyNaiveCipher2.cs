using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Cryptography.SystemCiphers;

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
public class MyNaiveCipher2 : BlockCipher
    {
    private class NaiveEncryptorTransform(byte[] key, byte[] iv) : ByteCryptoTransform(key, iv, CipherMode.CBC)
        {
        protected override byte TransformByte(byte input, int i)
            {
            byte output = (byte)((input + this.key[i] + this.iv![i] + i) % 256);    // cod% = (car% + ASC(MID$(clave$, indClave%, 1)) + indTexto% + indClave% + finClave% + carAnt%) MOD 256

            this.iv[i] = input;                                                    // carAnt% = car%

            return output;
            }
        }

    private class NaiveDecryptorTransform(byte[] key, byte[] iv) : ByteCryptoTransform(key, iv, CipherMode.CBC)
        {
        protected override byte TransformByte(byte input, int i)
            {
            int output = 256 - (256 - input + this.key[i] + this.iv![i] + i) % 256;        // cod% = 256 - (-(ASC(MID$(texto$, indTexto%, 1)) - ASC(MID$(clave$, indClave%, 1)) - indTexto% - indClave% - finClave% - cod%) + 256) MOD 256
                
            if(output == 256) { output = 0; }                                             // IF cod% = 256 THEN cod% = 0

            return this.iv[i] = (byte) output;
            }
        }


    public override CipherMode Mode => CipherMode.CBC;

    protected override ICryptoTransform CreateEncryptorCore(byte[] key, byte[]? iv)
        => new NaiveEncryptorTransform(key, iv ?? throw new InvalidOperationException("IV must be provided."));

    protected override ICryptoTransform CreateDecryptorCore(byte[] key, byte[]? iv)
        => new NaiveDecryptorTransform(key, iv ?? throw new InvalidOperationException("IV must be provided."));
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
