using System.Security.Cryptography.X509Certificates;

namespace CoreClassLibrary.Mathematics.Cryptography.Certificates;

public record PrivateCertificateSettings(string FileName, string Password)
    {
    public X509Certificate2 LoadPrivateCertificate(X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
        => X509CertificateLoader.LoadPkcs12FromFile(this.FileName, this.Password, keyStorageFlags, loaderLimits);

    public X509Certificate2 LoadPublicCertificate(X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet, Pkcs12LoaderLimits? loaderLimits = null)
        {
        using X509Certificate2 privateCertificate = this.LoadPrivateCertificate(keyStorageFlags, loaderLimits);
        return X509CertificateLoader.LoadCertificate(privateCertificate.Export(X509ContentType.Cert));
        }
    }

public record PublicCertificateSettings(string FileName)
    {
    public X509Certificate2 LoadCertificate()
        => X509CertificateLoader.LoadCertificateFromFile(this.FileName);
    }
