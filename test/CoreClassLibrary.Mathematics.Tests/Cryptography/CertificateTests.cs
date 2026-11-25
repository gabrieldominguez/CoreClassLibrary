using CoreClassLibrary.Mathematics.Cryptography;
using CoreClassLibrary.Mathematics.Cryptography.Certificates;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

// makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=Alice -sky exchange -pe
// makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=Bob -sky exchange -pe

// https://docs.microsoft.com/en-us/powershell/module/pkiclient/new-selfsignedcertificate?view=win10-ps
// New-SelfSignedCertificate -Subject "CN=Alice" -KeyAlgorithm RSA -KeyExportPolicy Exportable
// New-SelfSignedCertificate -Subject "CN=Bob" -KeyAlgorithm RSA -KeyExportPolicy Exportable
public class CertificateTests(ITestOutputHelper output)
    {
    private readonly PrivateCertificateSettings privateCertificateSettings = new PrivateCertificateSettings("Cryptography\\Certificados\\localhost.pfx", "YourSecurePassword");
    private readonly PublicCertificateSettings publicCertificateSettings = new PublicCertificateSettings("Cryptography\\Certificados\\localhost.cer");

    [Fact]
    public void AsymmetricDataSigning()
        {
        const string dataFromBob = "Hola";

        string signature;

        // Bob firma los datos con su clave privada y los envia junto con su firma a Alice
        using (X509Certificate2 privateCertificate = this.privateCertificateSettings.LoadPrivateCertificate())
            {
            signature = privateCertificate.SignData(dataFromBob);
            }

        // Alice recibe los datos planos y la firma, para comprobar su autenticidad con la clave publica de Bob
        using X509Certificate2 publicCertificate = this.publicCertificateSettings.LoadCertificate();
        Assert.True(publicCertificate.VerifyData(dataFromBob, signature));
        }

    [Fact]
    public void AsymmetricHashSigning()
        {
        const string dataFromBob = "Hola";

        string signature;
        string hash = CryptographyExtensions.ComputeHash(dataFromBob);

        // Bob firma los datos con su clave privada y los envia junto con su firma a Alice
        using (X509Certificate2 privateCertificate = this.privateCertificateSettings.LoadPrivateCertificate())
            {
            signature = privateCertificate.SignHash(hash);
            }

        // Alice recibe los datos planos y la firma, para comprobar su autenticidad con la clave publica de Bob
        using X509Certificate2 publicCertificate = this.publicCertificateSettings.LoadCertificate();
        Assert.True(publicCertificate.VerifyHash(hash, signature));
        }

    [Fact]
    public void AsymmetricEncription()
        {
        const string dataToAlice = "Hola";
        string encriptedData;
        string decriptedData;

        // Bob encripta los datos con la clave publica de Alice y se los envia encriptados ella.
        using (X509Certificate2 publicCertificate = this.publicCertificateSettings.LoadCertificate())
            {
            encriptedData = publicCertificate.Encrypt(dataToAlice);
            }

        // Alice recibe los datos encriptados y los desencripta con su clave privada
        using (X509Certificate2 privateCertificate = this.privateCertificateSettings.LoadPrivateCertificate())
            {
            decriptedData = privateCertificate.Decrypt(encriptedData);
            }

        Assert.Equal(dataToAlice, decriptedData);
        }

    [Fact]
    //https://stackoverflow.com/questions/43928064/export-private-public-keys-from-x509-certificate-to-pem
    public void X509CertificateToPEM()
        {
        using X509Certificate2 certificate = this.privateCertificateSettings.LoadPrivateCertificate();
        output.WriteLine(certificate.ToPublicPEM());
        }

    [Fact]
    public void PublicCertificateLoading()
        {
        using X509Certificate2 publicCertificate1 = this.publicCertificateSettings.LoadCertificate();
        using X509Certificate2 publicCertificate2 = this.privateCertificateSettings.LoadPrivateCertificate();
        
        Assert.Equal(publicCertificate1, publicCertificate2);
        }

    [Fact]
    public void LocalStoreCertificates()
        {
        foreach (X509Certificate2 certificate in CertificateExtensions.GetCertificatesFromStore())
            {
            this.WriteCertificateDetail(certificate);
            output.WriteLine("");
            output.WriteLine("");
            }
        }

    [Theory]
    [InlineData("CN=BancoSantaFe_330")]
    public void LocalStoreCertificatesBySubject(string subject)
        {
        this.WriteCertificateDetail(
            CertificateExtensions.GetCertificatesFromStore()
                .Single(certificate => certificate.Subject.StartsWith(subject)));
        }

    [Fact]
    public void X509Certificate2Detail()
        {
        using X509Certificate2 certificate = this.privateCertificateSettings.LoadPrivateCertificate();
        this.WriteCertificateDetail(certificate);
        }

    private void WriteCertificateDetail(X509Certificate2 certificate)
        {
        output.WriteLine("ToString(true)");
        output.WriteLine(certificate.ToString(true));
        output.WriteLine("RawCertData");
        output.WriteLine(certificate.GetRawCertData().ToByteArrayLiteral());
        output.WriteLine("RawCertData base64");
        output.WriteLine(Convert.ToBase64String(certificate.GetRawCertData()));

        output.WriteLine("");
        X509Certificate2Detail certDetail = new X509Certificate2Detail(certificate);

        output.WriteLine("Archived: " + certDetail.Archived);
        output.WriteLine("FriendlyName: " + certDetail.FriendlyName);
        output.WriteLine("HasPrivateKey: " + certDetail.HasPrivateKey);
        output.WriteLine("NotAfter: " + certDetail.NotAfter);
        output.WriteLine("NotBefore: " + certDetail.NotBefore);
        output.WriteLine("SerialNumber: " + certDetail.SerialNumber);
        output.WriteLine("Thumbprint: " + certDetail.Thumbprint);
        output.WriteLine("Version: " + certDetail.Version);
        output.WriteLine("SignatureAlgorithm: " + certDetail.SignatureAlgorithm);
        output.WriteLine("IssuerName: " + certDetail.IssuerName);
        output.WriteLine("SubjectName: " + certDetail.SubjectName);
        output.WriteLine("SubjectKeyIdentifier: " + certDetail.SubjectKeyIdentifier);
        }

    [Fact]
    public void GetCurrentDirectory()
        {
        output.WriteLine(Directory.GetCurrentDirectory());
        }

    [Fact]
    public void CertificateToBytesString()
        {
        using X509Certificate2 certificate = this.privateCertificateSettings.LoadPrivateCertificate(X509KeyStorageFlags.Exportable);
        byte[] bytes = certificate.Export(X509ContentType.Pkcs12, this.privateCertificateSettings.Password);

        output.WriteLine("base64");
        output.WriteLine(Convert.ToBase64String(bytes));
        output.WriteLine("");
        output.WriteLine("hex");
        output.WriteLine(bytes.ToByteArrayLiteral());

        X509Certificate2 certificate2 = X509CertificateLoader.LoadPkcs12(bytes, this.privateCertificateSettings.Password);

        Assert.True(certificate.HasPrivateKey);
        Assert.True(certificate2.HasPrivateKey);
        Assert.Equal(certificate, certificate2);
        }

    [Fact]
    public void X509ChainDoc()
        {
        using X509Certificate2 certificate = this.privateCertificateSettings.LoadPrivateCertificate();
        using X509Chain chain = new X509Chain();
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.Build(certificate);

        output.WriteLine("Chain Information:");
        output.WriteLine("     Chain revocation flag: {0}", chain.ChainPolicy.RevocationFlag);
        output.WriteLine("     Chain revocation mode: {0}", chain.ChainPolicy.RevocationMode);
        output.WriteLine("     Chain verification flag: {0}", chain.ChainPolicy.VerificationFlags);
        output.WriteLine("     Chain verification time: {0}", chain.ChainPolicy.VerificationTime);
        output.WriteLine("     Chain status length: {0}", chain.ChainStatus.Length);
        output.WriteLine("     Chain application policy count: {0}", chain.ChainPolicy.ApplicationPolicy.Count);
        output.WriteLine("     Chain certificate policy count: {0} {1}", chain.ChainPolicy.CertificatePolicy.Count, Environment.NewLine);

        output.WriteLine("Chain Element Information:");
        output.WriteLine("     Number of chain elements: {0}", chain.ChainElements.Count);
        output.WriteLine("");

        foreach (X509ChainElement element in chain.ChainElements)
            {
            output.WriteLine("     Element issuer name: {0}", element.Certificate.Issuer);
            output.WriteLine("     Element certificate subject: {0}", element.Certificate.Subject);
            output.WriteLine("     Element certificate valid until: {0}", element.Certificate.NotAfter);
            output.WriteLine("     Element error status length: {0}", element.ChainElementStatus.Length);
            output.WriteLine("     Element information: {0}", element.Information);
            output.WriteLine("     Number of element extensions: {0}{1}", element.Certificate.Extensions.Count, Environment.NewLine);

            for (int index = 0; index < element.Certificate.Extensions.Count; index++)
                {
                output.WriteLine("         Element extension: {0}", element.Certificate.Extensions[index].Oid?.FriendlyName ?? "");
                output.WriteLine("         Element extension is critical: {0}", element.Certificate.Extensions[index].Critical);
                }

            if (chain.ChainStatus.Length > 1)
                {
                for (int index = 0; index < element.ChainElementStatus.Length; index++)
                    {
                    output.WriteLine(element.ChainElementStatus[index].Status.ToString());
                    output.WriteLine(element.ChainElementStatus[index].StatusInformation);
                    }
                }
            }
        }

    [Fact]
    public void CertificateTrustChain()
        {
        using X509Certificate2 parentCertificate = X509CertificateLoader.LoadCertificateFromFile("Cryptography\\Certificados\\ca.crt");
        using X509Certificate2? certificate = CertificateExtensions.GetCertificatesFromStore().FirstOrDefault(cer => cer.Subject.StartsWith("CN=bancosantafe"));

        if (certificate != null) 
            {
            output.WriteLine(parentCertificate.ToString(true));
            output.WriteLine("");
            output.WriteLine(certificate.ToString(true));
            output.WriteLine("");
            output.WriteLine("Certificate is valid? {0}", certificate.Verify());

            Assert.True(certificate.Verify());
            }
        }
    }    
