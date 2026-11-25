using System.Security.Cryptography.X509Certificates;

namespace CoreClassLibrary.Mathematics.Cryptography.Certificates;

/// <summary>
/// Certificate Extensions.
/// </summary>
/// <example>
/// Generate a Root Certificate
/// makecert -sky exchange -r -n “CN=NetworkingDemoRootCert2” -pe -a sha1 -len 2048 -ss My “NetworkingDemoRootCert2.cer”
///
/// Generate Client Certificate
/// makecert.exe -n “CN= NetworkingDemoClientCert2” -pe -sky exchange -m 96 -ss My -in “NetworkingDemoRootCert2” -is my -a sha1
/// 
/// 
/// makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=Alice -sky exchange -pe
/// makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=Bob -sky exchange -pe
/// New-SelfSignedCertificate -Subject "CN=Alice" -KeyAlgorithm RSA -KeyExportPolicy Exportable
/// New-SelfSignedCertificate -Subject "CN=Bob" -KeyAlgorithm RSA -KeyExportPolicy Exportable
/// </example>
/// <see cref="https://docs.microsoft.com/en-us/powershell/module/pkiclient/new-selfsignedcertificate?view=win10-ps"/>
public static class CertificateExtensions
    {
    public static IEnumerable<X509Certificate2> GetCertificatesFromStore(StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.LocalMachine)
        {
        using X509Store store = new X509Store(storeName, storeLocation);
        store.Open(OpenFlags.ReadOnly);

        foreach (X509Certificate2 cert in store.Certificates)
            {
            yield return cert;
            }
        }

    /// <summary>
    /// Extrae el Identificador de Clave de Asunto (SKI: SubjectKeyIdentifier) del certificado.
    /// </summary>
    /// <param name="certificate"></param>
    /// <returns>El SKI o "" si no posee.</returns>
    public static string GetSubjectKeyIdentifier(this X509Certificate2 certificate)
        => certificate.Extensions.OfType<X509SubjectKeyIdentifierExtension>().FirstOrDefault()?.SubjectKeyIdentifier ?? "";

    /// <summary>
    /// Verifica la validez de un certificado, recorriendo la cadena de confianza hasta el certificado raiz.
    /// </summary>
    /// <remarks>
    /// La raiz de confianza es una certificado auto-firmado que debe encontrarse en el store "Entidades de certificación de raiz de confianza" de la máquina.
    /// 
    /// Si hay certificados intermedios que no están incluidos en el store, pueden especificarse con 
    /// chain.ChainPolicy.ExtraStore.AddRange() (no disponible en este método). Se recomienda incluir todos los certificados en el store.
    /// 
    /// Para determinar la causa de un validación fallida debe examinarse la propiedad chain.ChainStatus (no disponible en este método).
    /// </remarks>
    /// <param name="certificate">Certificado a verificar.</param>
    /// <returns>True si la validación fue exitosa.</returns>
    /// <see cref="https://technet.microsoft.com/en-us/library/ee619754%28v=ws.10%29.aspx?f=255&MSPPError=-2147217396"/>
    /// <see cref="http://stackoverflow.com/questions/6126388/how-to-verify-an-x509certificate2-against-an-x509certificate2collection-chain"/>
    /// <see cref="https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509chain(v=vs.110).aspx"/>
    public static bool Verify(X509Certificate2 certificate, X509RevocationMode revocationMode = X509RevocationMode.NoCheck)
        {
        using X509Chain chain = new X509Chain();
        chain.ChainPolicy.RevocationMode = revocationMode;

        return CertificateExtensions.Verify(certificate, chain);
        }

    public static bool Verify(X509Certificate2 certificate, X509Chain chain)
        => certificate != null && chain != null && chain.Build(certificate);

    /// <summary>
    /// Export private/public keys from X509 certificate to PEM
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/43928064/export-private-public-keys-from-x509-certificate-to-pem"/>
    public static string ToPublicPEM(this X509Certificate2 certificate)
        => String.Concat(
        "-----BEGIN CERTIFICATE-----", Environment.NewLine,
        Convert.ToBase64String(certificate.RawData, Base64FormattingOptions.InsertLineBreaks),
        "-----END CERTIFICATE-----", Environment.NewLine
        );
    }
