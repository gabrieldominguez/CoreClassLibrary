using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Cryptography;
using Xunit;
using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

public class AsymCryptographyRSATests(ITestOutputHelper output)
    {
    private static RSAParameters GetPrivateRSAKey()
        {
        using RSA rsa = RSA.Create();
        return rsa.ExportParameters(true);
        }

    private static RSAParameters GetPublicRSAKey(RSAParameters rsaParameters)
        {
        using RSA rsa = RSA.Create(rsaParameters);
        return rsa.ExportParameters(false);
        }

    [Fact]
    public void AsymmetricSigning()
        {
        byte[] dataFromBob = StringExtensions.UTF8Encoding.GetBytes("Hola");

        byte[] signature;

        RSAParameters privateBobKey = AsymCryptographyRSATests.GetPrivateRSAKey();
        RSAParameters publicBobKey = AsymCryptographyRSATests.GetPublicRSAKey(privateBobKey);

        // Bob firma los datos con su clave privada y los envia junto con su firma a Alice
        using (RSA rsa = RSA.Create(privateBobKey))
            {
            signature = rsa.SignData(dataFromBob);
            }

        // Alice recive los datos planos y la firma, para comprobar su autenticidad con la clave publica de Bob
        using (RSA rsa = RSA.Create(publicBobKey))
            {
            Assert.True(rsa.VerifyData(dataFromBob, signature));
            }
        }

    [Fact]
    public void AsymmetricEncription()
        {
        byte[] dataToAlice = StringExtensions.UTF8Encoding.GetBytes("Hola");
        byte[] encriptedData;
        byte[] decriptedData;

        RSAParameters privateAliceKey = AsymCryptographyRSATests.GetPrivateRSAKey();
        RSAParameters publicAliceKey = AsymCryptographyRSATests.GetPublicRSAKey(privateAliceKey);

        // Bob encripta los datos con la clave publica de Alice y se los envia encriptados ella.
        using (RSA rsa = RSA.Create(publicAliceKey))
            {
            encriptedData = rsa.Encrypt(dataToAlice);
            }

        // Alice recive los datos encriptados y los desencripta con su clave privada
        using (RSA rsa = RSA.Create(privateAliceKey))
            {
            decriptedData = rsa.Decrypt(encriptedData);
            }

        Assert.Equal(decriptedData, dataToAlice);
        }

    // Public key: modulus n and exponent e
    // Private key: modulus n and the exponent d.
    [Fact]
    public void RSAParameters()
        {
        //certificate.PrivateKey is RSACryptoServiceProvider privateRsaKey
        using RSA rsa = RSA.Create();
        RSAParameters publicKey = rsa.ExportParameters(false);
        output.WriteLine("PublicKey:");
        output.WriteLine(String.Concat("byte[] Modulus = new byte[] ", publicKey.Modulus?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] Exponent = new byte[] ", publicKey.Exponent?.ToByteArrayLiteral(), ";"));

        output.WriteLine("");

        RSAParameters privateKey = rsa.ExportParameters(true);
        output.WriteLine("PrivateKey:");
        //this.output.WriteLine(String.Concat("byte[] Modulus = new byte[] ", publicKey.Modulus.ToByteArrayLiteral(), ";"));
        //this.output.WriteLine(String.Concat("byte[] Exponent = new byte[] ", publicKey.Exponent.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] D = new byte[] ", privateKey.D?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] DP = new byte[] ", privateKey.DP?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] DQ= new byte[] ", privateKey.DQ?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] InverseQ = new byte[] ", privateKey.InverseQ?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] P = new byte[] ", privateKey.P?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] Q = new byte[] ", privateKey.Q?.ToByteArrayLiteral(), ";"));
        }
    }
