using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CoreClassLibrary.Extensions;
using System.Numerics;
using CoreClassLibrary.Streams;

namespace CoreClassLibrary.Mathematics.Cryptography;

/// <summary>
/// Cryptography Extensions
/// </summary>
/// <see cref="https://emvlab.org/encoding/"/>
/// <see cref="https://emvlab.org/descalc/"/>
/// <see cref="https://emvlab.org/pinblock/"/>
public static class CryptographyExtensions
    {
    public static BigInteger GetRandomBigInteger(int length)
        {
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        return randomNumberGenerator.GetBigInteger(length);
        }

    public static BigInteger GetRandomPositiveBigInteger(int length)
        {
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        return randomNumberGenerator.GetPositiveBigInteger(length);
        }

    public static BigInteger GetBigInteger(this RandomNumberGenerator rng, int length)
        {
        Span<byte> buffer = stackalloc byte[length];
        rng.GetBytes(buffer);
        return new BigInteger(buffer);
        }

    public static BigInteger GetPositiveBigInteger(this RandomNumberGenerator rng, int length)
        {
        Span<byte> buffer = stackalloc byte[length];
        rng.GetBytes(buffer);
        buffer[^1] = (byte)(buffer[^1] & 0x7F);   //  Ensure not a negative value
        return new BigInteger(buffer);
        }
    
    
    /// <summary>
    /// Fills an array of bytes with a cryptographically strong sequence of random values.  
    /// </summary>
    public static byte[] GetRandomBytes(int length)
        {
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        return randomNumberGenerator.GetBytes(length);
        }

    public static byte[] GetBytes(this RandomNumberGenerator rng, int length)
        {
        byte[] data = new byte[length];
        rng.GetBytes(data);
        return data;
        }

    public static byte[] DeriveBytes(string password, int saltLength = 8, int iterations = 1000, int length = 16)
        => CryptographyExtensions.DeriveBytes(password, HashAlgorithmName.SHA256, saltLength, iterations, length);

    public static byte[] DeriveBytes(string password, HashAlgorithmName hashAlgorithm, int saltLength = 8, int iterations = 1000, int length = 16)
        => CryptographyExtensions.DeriveBytes(password, CryptographyExtensions.GetRandomBytes(saltLength), hashAlgorithm, iterations, length);

    public static byte[] DeriveBytes(string password, byte[] salt, HashAlgorithmName hashAlgorithm, int iterations = 1000, int length = 16)
        => Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, length);
    
    /// <summary>
    /// Calculate the hash of the string.
    /// </summary>
    /// <remarks>
    /// Hash functions are fundamental to modern cryptography. 
    /// These functions map binary strings of an arbitrary length to small binary strings of a fixed length, known as hash values. 
    /// A cryptographic hash function has the property that it is computationally infeasible to find two distinct inputs that hash to the same value. 
    /// Hash functions are commonly used with digital signatures and for data integrity.
    /// 
    /// The hash is used as a unique value of fixed size representing a 
    /// large amount of data. Hashes of two sets of data should match 
    /// if and only if the corresponding data also matches. Small changes 
    /// to the data result in large unpredictable changes in the hash.
    /// </remarks>
    /// <see cref="https://www.fileformat.info/tool/hash.htm"/>
    /// <see cref="https://www.tools4noobs.com/online_tools/hash/"/>
    /// <see cref="https://www.browserling.com/tools/all-hashes"/>
    public static string ComputeHash(string data)
        {
        using SHA256 sha = SHA256.Create();
        return CryptographyExtensions.ComputeHash(sha, data);
        }

    public static string ComputeHash(this HashAlgorithm hashAlgorithm, string data)
        => hashAlgorithm.ComputeHash(data, StringExtensions.UTF8Encoding);

    public static string ComputeHash(this HashAlgorithm hashAlgorithm, string data, Encoding encoding)
        => hashAlgorithm.ComputeHash(encoding.GetBytes(data)).ToHexString();

    /// <summary>
    /// Firma la cadena encodeada a UTF8 sin BOM de forma simetrica.
    /// </summary>
    /// <param name="key">Clave.</param>
    /// <param name="data">Datos a encodear a UTF8 y luego firmar.</param>
    /// <returns>Cadena con la firma digital en hexadecimal luego de encodear la cadena de entrada a UTF8.</returns>
    public static string Sign(byte[] key, string data)
        => CryptographyExtensions.Sign(key, data, StringExtensions.UTF8Encoding);

    /// <summary>
    /// Firma la cadena encodeada de forma simetrica.
    /// </summary>
    /// <param name="key">Clave.</param>
    /// <param name="data">Datos a encodear a UTF8 y luego firmar.</param>
    /// <param name="encoding">Encoding con el cual encodear la cadena.</param>
    /// <returns>Cadena con la firma digital en hexadecimal luego de encodear la cadena de entrada a UTF8.</returns>
    public static string Sign(byte[] key, string data, Encoding encoding)
        => CryptographyExtensions.Sign(key, encoding.GetBytes(data)).ToHexString();

    /// <summary>
    /// Firma el buffer de forma simetrica.
    /// </summary>
    /// <remarks>
    /// A hash-based message authentication code (HMAC) can be used as a signature.
    /// </remarks>
    public static byte[] Sign(byte[] key, byte[] buffer)
        {
        using HMACSHA256 hmac = new HMACSHA256(key);
        return hmac.ComputeHash(buffer);
        }

    /// <summary>
    /// Comprueba la firma de la cadena de forma simetrica luego de encodearla en UTF8 sin BOM.
    /// </summary>
    /// <param name="data">Datos a verificar.</param>
    /// <param name="key">Clave.</param>
    /// <param name="signature">Firma digital en hexadecimal.</param>
    /// <returns>True si la firma digital se corresponde con los datos.</returns>
    public static bool Verify(byte[] key, string data, string signature)
        => CryptographyExtensions.Verify(key, data, signature, StringExtensions.UTF8Encoding);

    /// <summary>
    /// Comprueba la firma de la cadena de forma simetrica luego de encodearla.
    /// </summary>
    /// <param name="data">Datos a verificar.</param>
    /// <param name="key">Clave.</param>
    /// <param name="signature">Firma digital en hexadecimal.</param>
    /// <param name="encoding">Encoding con el cual encodear la cadena.</param>
    /// <returns>True si la firma digital se corresponde con los datos.</returns>
    public static bool Verify(byte[] key, string data, string signature, Encoding encoding)
        => CryptographyExtensions.Verify(key, encoding.GetBytes(data), signature.FromHexString());

    /// <summary>
    /// Comprueba la firma del buffer de forma simetrica.
    /// </summary>
    public static bool Verify(byte[] key, byte[] buffer, byte[] signature)
        => CryptographyExtensions.Sign(key, buffer).SequenceEqual(signature);

    public static (string Key, string IV, string Data) ToHexString(this (byte[] Key, byte[] IV, byte[] Data) encrypted)
        => (encrypted.Key.ToHexString(), encrypted.IV.ToHexString(), encrypted.Data.ToHexString());

    /// <summary>
    /// Encodea la cadena a UTF8 sin BOM y la encripta con una clave y vector de inicializacion aleatorios usando AES.
    /// </summary>
    /// <returns>La clave de encriptacion, el vector de inicializacion y el buffer encriptado en cadenas hexadecimales.</returns>
    public static (string Key, string IV, string Data) Encrypt(string data)
        => CryptographyExtensions.Encrypt(data, StringExtensions.UTF8Encoding);

    /// <summary>
    /// Encodea la cadena y la encripta con una clave y vector de inicializacion aleatorios usando AES.
    /// </summary>
    /// <returns>La clave de encriptacion, el vector de inicializacion y el buffer encriptado en cadenas hexadecimales.</returns>
    public static (string Key, string IV, string Data) Encrypt(string data, Encoding encoding)
        => CryptographyExtensions.Encrypt(encoding.GetBytes(data)).ToHexString();

    /// <summary>
    /// Encripta el buffer con una clave y vector de inicializacion aleatorios usando AES.
    /// </summary>
    /// <returns>La clave de encriptacion, el vector de inicializacion y el buffer encriptado.</returns>
    public static (byte[] Key, byte[] IV, byte[] Data) Encrypt(byte[] data)
        {
        using Aes aes = Aes.Create();
        return (aes.Key, aes.IV, CryptographyExtensions.Encrypt(aes, data));
        }

    public static string Encrypt(this SymmetricAlgorithm symmetricAlgorithm, string data)
        => CryptographyExtensions.Encrypt(symmetricAlgorithm, data, StringExtensions.UTF8Encoding);

    public static string Encrypt(this SymmetricAlgorithm symmetricAlgorithm, string data, Encoding encoding)
        => CryptographyExtensions.Encrypt(symmetricAlgorithm, encoding.GetBytes(data)).ToHexString();

    public static byte[] Encrypt(this SymmetricAlgorithm symmetricAlgorithm, byte[] data)
        {
        using MemoryStream inStream = new MemoryStream(data);
        using MemoryStream outStream = MemoryStreamManager.Instance.GetStream();
        CryptographyExtensions.Encrypt(symmetricAlgorithm, inStream, outStream);

        return outStream.ToArray();
        }

    public static void Encrypt(this SymmetricAlgorithm symmetricAlgorithm, Stream inStream, Stream outStream)
        {
        using ICryptoTransform encryptor = symmetricAlgorithm.CreateEncryptor();
        CryptographyExtensions.Encrypt(encryptor, inStream, outStream);
        }

    public static void Encrypt(this ICryptoTransform encryptor, Stream inStream, Stream outStream)
        {
        using CryptoStream encryptStream = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write, true);
        inStream.CopyTo(encryptStream);
        }

    public static (byte[] Key, byte[] IV, byte[] Data) FromHexString(this (string Key, string IV, string Data) encrypted)
        => (encrypted.Key.FromHexString(), encrypted.IV.FromHexString(), encrypted.Data.FromHexString());

    public static string Decrypt((string Key, string IV, string Data) encrypted)
        => CryptographyExtensions.Decrypt(encrypted, StringExtensions.UTF8Encoding);

    public static string Decrypt((string Key, string IV, string Data) encrypted, Encoding encoding)
        => encoding.GetString(CryptographyExtensions.Decrypt(encrypted.FromHexString()));

    public static byte[] Decrypt((byte[] Key, byte[] IV, byte[] Data) encrypted)
        => CryptographyExtensions.Decrypt(encrypted.Key, encrypted.IV, encrypted.Data);

    /// <summary>
    /// Desencripta el buffer usando AES
    /// </summary>
    /// <returns>El buffer desencriptado.</returns>
    public static byte[] Decrypt(byte[] key, byte[] iv, byte[] buffer)
        {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        return CryptographyExtensions.Decrypt(aes, buffer);
        }

    public static string Decrypt(this SymmetricAlgorithm symmetricAlgorithm, string data)
        => CryptographyExtensions.Decrypt(symmetricAlgorithm, data, StringExtensions.UTF8Encoding);

    public static string Decrypt(this SymmetricAlgorithm symmetricAlgorithm, string data, Encoding encoding)
        => encoding.GetString(CryptographyExtensions.Decrypt(symmetricAlgorithm, data.FromHexString()));

    public static byte[] Decrypt(this SymmetricAlgorithm symmetricAlgorithm, byte[] buffer)
        {
        using MemoryStream inStream = new MemoryStream(buffer);
        using MemoryStream outStream = MemoryStreamManager.Instance.GetStream();
        CryptographyExtensions.Decrypt(symmetricAlgorithm, inStream, outStream);

        return outStream.ToArray();
        }

    public static void Decrypt(this SymmetricAlgorithm symmetricAlgorithm, Stream inStream, Stream outStream)
        {
        using ICryptoTransform decryptor = symmetricAlgorithm.CreateDecryptor();
        CryptographyExtensions.Decrypt(decryptor, inStream, outStream);
        }

    public static void Decrypt(this ICryptoTransform decryptor, Stream inStream, Stream outStream)
        {
        using CryptoStream decryptStream = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read, true);
        decryptStream.CopyTo(outStream);
        }

    /// <summary>
    /// Firma la cadena encodeada a UTF8 sin BOM con la clave privada de un certificado propio.
    /// </summary>
    /// <remarks>
    /// Sign means: Hash the data and Encrypt the hash with a given key.
    /// The reason the return value of that SignHash call does match the value returned by rsa.SignData(data, sha256) is that the latter method does the same thing, except it does the hashing and hash signing as one operation, so you don't have to compute the hash as a separate step if you don't need it for any purpose other than signing it.
    /// </remarks>
    /// <param name="privateCertificate">Certificado con clave privada propia.</param>
    /// <param name="data">Datos a encodear a UTF8 y luego firmar.</param>
    /// <returns>Cadena con la firma digital en hexadecimal luego de encodear la cadena de entrada a UTF8 sin BOM.</returns>
    /// <see cref="https://stackoverflow.com/questions/41072042/digital-signatures-encrypting-the-hash-vs-signing-the-hash/41072090#41072090"/>
    public static string SignData(this X509Certificate2 privateCertificate, string data)
        => CryptographyExtensions.SignData(privateCertificate, data, StringExtensions.UTF8Encoding);

    public static string SignData(this X509Certificate2 privateCertificate, string data, Encoding encoding)
        => CryptographyExtensions.SignData(privateCertificate, encoding.GetBytes(data)).ToHexString();

    /// <summary>
    /// Firma el contenido del buffer con la clave privada de un certificado propio.
    /// </summary>
    /// <param name="privateCertificate">Certificado propio.</param>
    /// <param name="data">Datos a firmar.</param>
    /// <returns>Firma digital.</returns>
    public static byte[] SignData(this X509Certificate2 privateCertificate, byte[] data)
        {
        using RSA rsa = privateCertificate.GetCertificateRSAPrivateKey();
        return CryptographyExtensions.SignData(rsa, data);
        }

    public static byte[] SignData(this RSA rsa, byte[] data)
        => rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    /// <summary>
    /// Verifica que la cadena haya sido firmada con un certificado ajeno, usando su clave pública.
    /// </summary>
    /// <param name="data">Data a verificar.</param>
    /// <param name="publicCertificate">Certificado con clave publica del firmante.</param>
    /// <param name="signature">Firma digital en hexadecimal.</param>
    /// <returns>True si la firma digital se corresponde con los datos.</returns>
    public static bool VerifyData(this X509Certificate2 publicCertificate, string data, string signature)
        => CryptographyExtensions.VerifyData(publicCertificate, data, signature, StringExtensions.UTF8Encoding);

    public static bool VerifyData(this X509Certificate2 publicCertificate, string data, string signature, Encoding encoding)
        => CryptographyExtensions.VerifyData(publicCertificate, encoding.GetBytes(data), signature.FromHexString());

    /// <summary>
    /// Verifica que el contenido del buffer haya sido firmado con un certificado ajeno, usando su clave pública.
    /// </summary>
    /// <param name="data">Data a verificar.</param>
    /// <param name="publicCertificate">Certificado del firmante.</param>
    /// <param name="signature">Firma digital.</param>
    /// <returns>True si la firma digital se corresponde con los datos.</returns>
    public static bool VerifyData(this X509Certificate2 publicCertificate, byte[] data, byte[] signature)
        {
        using RSA? rsa = publicCertificate.GetCertificateRSAPublicKey();
        return CryptographyExtensions.VerifyData(rsa, data, signature);
        }

    public static bool VerifyData(this RSA rsa, byte[] data, byte[] signature)
        => rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    public static string SignHash(this X509Certificate2 privateCertificate, string hash)
        => CryptographyExtensions.SignHash(privateCertificate, hash.FromHexString()).ToHexString();

    /// <summary>
    /// Encripta un hash con la clave privada del certificado.
    /// </summary>
    /// <returns>Firma digital.</returns>
    public static byte[] SignHash(this X509Certificate2 privateCertificate, byte[] hash)
        {
        using RSA rsa = privateCertificate.GetCertificateRSAPrivateKey();
        return CryptographyExtensions.SignHash(rsa, hash);
        }

    public static byte[] SignHash(this RSA rsa, byte[] hash)
        => rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    public static bool VerifyHash(this X509Certificate2 publicCertificate, string hash, string signature)
        => CryptographyExtensions.VerifyHash(publicCertificate, hash.FromHexString(), signature.FromHexString());

    public static bool VerifyHash(this X509Certificate2 publicCertificate, byte[] hash, byte[] signature)
        {
        using RSA rsa = publicCertificate.GetCertificateRSAPublicKey();
        return CryptographyExtensions.VerifyHash(rsa, hash, signature);
        }

    /// <summary>
    /// Verifica que un hash haya sido firmado con un certificado ajeno, usando su clave pública.
    /// </summary>
    /// <returns>True si la firma digital se corresponde con los datos.</returns>
    public static bool VerifyHash(this RSA rsa, byte[] hash, byte[] signature)
        => rsa.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    public static string Encrypt(this X509Certificate2 publicCertificate, string data)
        => CryptographyExtensions.Encrypt(publicCertificate, data, StringExtensions.UTF8Encoding);

    public static string Encrypt(this X509Certificate2 publicCertificate, string data, Encoding encoding)
        => CryptographyExtensions.Encrypt(publicCertificate, encoding.GetBytes(data)).ToHexString();

    /// <summary>
    /// Encripta el contenido del buffer con un certificado ajeno, usando su clave pública.
    /// </summary>
    /// <param name="data">Datos a encriptar.</param>
    /// <param name="publicCertificate">Certificado ajeno.</param>
    /// <returns>Datos encriptados.</returns>
    public static byte[] Encrypt(this X509Certificate2 publicCertificate, byte[] data)
        {
        using RSA rsa = publicCertificate.GetCertificateRSAPublicKey();
        return CryptographyExtensions.Encrypt(rsa, data);
        }

    public static byte[] Encrypt(this RSA rsa, byte[] data)
        => rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);

    public static string Decrypt(this X509Certificate2 privateCertificate, string data)
        => CryptographyExtensions.Decrypt(privateCertificate, data, StringExtensions.UTF8Encoding);

    public static string Decrypt(this X509Certificate2 privateCertificate, string data, Encoding encoding)
        => encoding.GetString(CryptographyExtensions.Decrypt(privateCertificate, data.FromHexString()));

    /// <summary>
    /// Desencripta el contenido del buffer con un certificado propio, usando su clave privada.
    /// </summary>
    /// <param name="data">Datos a encriptar.</param>
    /// <param name="privateCertificate">Certificado propio.</param>
    /// <returns>Datos desencriptados.</returns>
    public static byte[] Decrypt(this X509Certificate2 privateCertificate, byte[] data)
        {
        using RSA rsa = privateCertificate.GetCertificateRSAPrivateKey();
        return CryptographyExtensions.Decrypt(rsa, data);
        }

    public static byte[] Decrypt(this RSA rsa, byte[] data)
        => rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);

    public static RSA GetCertificateRSAPrivateKey(this X509Certificate2 certificate)
        => certificate.GetRSAPrivateKey() ?? throw new InvalidOperationException("Cannot get RSA private key from certificate.");

    public static RSA GetCertificateRSAPublicKey(this X509Certificate2 certificate)
        => certificate.GetRSAPublicKey() ?? throw new InvalidOperationException("Cannot get RSA public key from the certificate.");
    }
