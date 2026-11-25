using CoreClassLibrary.Mathematics.Cryptography.Certificates;
using System.Security.Cryptography.X509Certificates;

namespace CoreClassLibrary.Mathematics.Cryptography;

/// <summary>
/// Permite serializar los datos de un certificado.
/// </summary>
/// <remarks>
/// Esta información puede obtenerse como string con certificate.ToString(true)
/// </remarks>
public class X509Certificate2Detail(X509Certificate2 certificate)
    {
    public bool Archived { get; } = certificate.Archived;
    public string FriendlyName { get; } = certificate.FriendlyName;
    public bool HasPrivateKey { get; } = certificate.HasPrivateKey;
    public DateTime NotAfter { get; } = certificate.NotAfter;
    public DateTime NotBefore { get; } = certificate.NotBefore;
    public string SerialNumber { get; } = certificate.SerialNumber;
    public string Thumbprint { get; } = certificate.Thumbprint;
    public int Version { get; } = certificate.Version;

    public string SignatureAlgorithm { get; } = certificate.SignatureAlgorithm.FriendlyName ?? "";

    public string IssuerName { get; } = certificate.IssuerName.Name;
    public string SubjectName { get; } = certificate.SubjectName.Name;
    public string SubjectKeyIdentifier { get; } = certificate.GetSubjectKeyIdentifier();
    }
