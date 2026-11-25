using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CoreClassLibrary.Mathematics.Cryptography;

public record SecureData(
    /// <summary>
    /// Fima digital de EncryptedData, realizada con la clave privada del certificado propio.
    /// </summary>
    byte[] Signature,

    /// <summary>
    /// Datos encriptados con el algoritmo simetrico.
    /// </summary>
    byte[] EncryptedData,

    /// <summary>
    /// Clave del algoritmo simetrico con la que se encriptaron los datos, encriptada con la clave publica del certificado del destinatario.
    /// </summary>
    byte[] EncryptedKey,

    /// <summary>
    /// Vector de inicializacion (IV) del algoritmo simetrico con el que se encriptaron los datos, encriptado con la clave publica del certificado del destinatario.
    /// </summary>
    /// <remarks>Si el algoritmo simetrico no requiere IV (por usar el modo ECB por ej.) pasar null.</remarks>
    byte[] EncryptedIV
    )
    {

    /// <summary>
    /// Encripta los datos con el algoritmo simetrico para ser enviados,
    /// firmandolos con un certificado propio (privado) y 
    /// encriptando la clave y el IV con el certificado del destinatario (publico).
    /// </summary>
    /// <param name="data">Datos a encriptar</param>
    /// <param name="privateCertificate">Certificado propio (privado) </param>
    /// <param name="publicCertificate">Certificado del destinatario (publico)</param>
    /// <param name="symmetricAlgorithm">Algoritmo simetrico con el cual encriptar los datos.</param>
    /// <returns>Estructura con los datos encriptados.</returns>
    public static SecureData EncryptWithCertificates(byte[] data, X509Certificate2 privateCertificate, X509Certificate2 publicCertificate, SymmetricAlgorithm symmetricAlgorithm)
        {
        byte[] encryptedData = symmetricAlgorithm.Encrypt(data);

        return new SecureData(            
            // Firma digital de EncryptedData con la clave privada del certificado propio
            privateCertificate.SignData(encryptedData),

            // Encriptado de los datos con el algoritmo simetrico
            encryptedData,

            // Encriptado de la clave de la encriptacion simetrica con la clave publica del certificado del destinatario
            publicCertificate.Encrypt(symmetricAlgorithm.Key),

            // Encriptado del vector de inicializacion de la encriptacion simetrica con la clave publica del certificado del destinatario
            publicCertificate.Encrypt(symmetricAlgorithm.IV)
            );
        }

    /// <summary>
    /// Desencripta los datos recibidos con el algoritmo simetrico,
    /// desencriptando la clave y el IV con el certificado propio (privado)
    /// y verificando la firma con el certificado del emisor (publico).
    /// </summary>
    /// <param name="privateCertificate">Certificado propio (privado)</param>
    /// <param name="publicCertificate">Certificado del destinatario (publico)</param>
    /// <param name="symmetricAlgorithm">Algoritmo simetrico con el cual desencriptar los datos.</param>
    /// <returns>Datos desencriptados.</returns>
    public byte[] DecryptWithCertificates(X509Certificate2 privateCertificate, X509Certificate2 publicCertificate, SymmetricAlgorithm symmetricAlgorithm)
        {
        // Comprobación de la fima digital de EncryptedData con la clave pública del certificado del destinatario
        if (!publicCertificate.VerifyData(this.EncryptedData, this.Signature))
            {
            throw new CryptographicException($"The digital signature doesn´t match the certicate: {publicCertificate.FriendlyName}");
            }

        // Desencriptado de la clave de la encriptacion simetrica con la clave privada del certificado propio
        symmetricAlgorithm.Key = privateCertificate.Decrypt(this.EncryptedKey);            

        // Desencriptado del vector de inicializacion de la encriptacion simetrica con la clave pública del certificado del destinatario
        symmetricAlgorithm.IV = privateCertificate.Decrypt(this.EncryptedIV);

        // Desencriptado de los datos con el algoritmo simetrico                        
        return symmetricAlgorithm.Decrypt(this.EncryptedData);
        }

    }
