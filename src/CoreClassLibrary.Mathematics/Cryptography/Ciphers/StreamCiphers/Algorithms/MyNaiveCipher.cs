
namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;

/// <summary>
/// Encripta usando un algoritmo propio (de cuando era estudiante).
/// </summary>
/// <remarks>
/// Corresponde a la 1er version donde no se usa vector de inicializacion ni los valores de los indices.
/// 
/// Para un uso productivo usar AES.
/// </remarks>
/// <see cref="System.Security.Cryptography.Aes"/>
public sealed class MyNaiveCipher(byte[] key) : CipherBase(key), IStreamCipher
    {
    private int iKey = 0;

    public override void Initialize() 
        => this.iKey = 0;

    public byte Encrypt(byte input)
        {
        int output = input + this.key[this.iKey++ % this.key.Length];    // cod% = ASC(MID$(texto$, indTexto %, 1)) + ASC(MID$(clave$, indClave %, 1))
        if (output > 255) { output -= 256; }                                                    // IF cod% > 255 THEN cod% = cod% -256

        return (byte)output;
        }

    public byte Decrypt(byte input)
        {
        int output = input - this.key[this.iKey++ % this.key.Length];    // cod% = ASC(MID$(texto$, indTexto %, 1)) - ASC(MID$(clave$, indClave %, 1))
        if (output < 0) { output += 256; }                                                      // IF cod% < 0 THEN cod% = cod% +256

        return (byte)output;
        }
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
