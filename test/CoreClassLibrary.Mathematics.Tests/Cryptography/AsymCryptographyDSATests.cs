using CoreClassLibrary.Extensions;
using Xunit;
using System.Security.Cryptography;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

public class AsymCryptographyDSATests(ITestOutputHelper output)
    {
    private static DSAParameters GetPrivateKey()
        {
        using DSA dsa = DSA.Create();
        return dsa.ExportParameters(true);
        }

    private static DSAParameters GetPublicKey(DSAParameters parameters)
        {
        using DSA dsa = DSA.Create(parameters);
        return dsa.ExportParameters(false);
        }

    [Fact]
    public void AsymmetricSigning()
        {
        byte[] dataFromBob = StringExtensions.UTF8Encoding.GetBytes("Hola");

        byte[] signature;

        DSAParameters privateBobKey = AsymCryptographyDSATests.GetPrivateKey();
        DSAParameters publicBobKey = AsymCryptographyDSATests.GetPublicKey(privateBobKey);

        // Bob firma los datos con su clave privada y los envia junto con su firma a Alice
        using (DSA dsa = DSA.Create(privateBobKey))
            {
            signature = dsa.CreateSignature(dataFromBob);
            }

        // Alice recive los datos planos y la firma, para comprobar su autenticidad con la clave publica de Bob
        using (DSA dsa = DSA.Create(publicBobKey))
            {
            Assert.True(dsa.VerifySignature(dataFromBob, signature));
            }
        }

    // Public key: p, q, g, y
    // Private key: x.
    [Fact]
    public void DSAParameters()
        {
        using DSA dsa = DSA.Create();
        DSAParameters publicKey = dsa.ExportParameters(false);
        output.WriteLine("PublicKey:");
        output.WriteLine(String.Concat("byte[] G = new byte[] ", publicKey.G?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] P = new byte[] ", publicKey.P?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] Q = new byte[] ", publicKey.Q?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] Seed = new byte[] ", publicKey.Seed?.ToByteArrayLiteral(), ";"));
        output.WriteLine(String.Concat("byte[] Y = new byte[] ", publicKey.Y?.ToByteArrayLiteral(), ";"));

        output.WriteLine("");

        DSAParameters privateKey = dsa.ExportParameters(true);
        output.WriteLine("PrivateKey:");
        output.WriteLine(String.Concat("byte[] X = new byte[] ", privateKey.X?.ToByteArrayLiteral(), ";"));
        }
    }
