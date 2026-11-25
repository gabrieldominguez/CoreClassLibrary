#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Mathematics.Cryptography.SystemCiphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Cryptography;
using System.Security.Cryptography;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

public class CryptographyTests(ITestOutputHelper output)
    {
    [Fact]
    public void Random()
        {
        for (int i = 0; i < 10; i++)
            {
            output.WriteLine(CryptographyExtensions.GetRandomBytes(10).Select(b => b.ToString("000")).ToDelimitedString("-"));
            }
        }

    [Theory]
    [InlineData("Some test data", "password")]
    public void PasswordDeriveBytes(string data, string password)
        {
        byte[] salt = CryptographyExtensions.GetRandomBytes(8);
        using PasswordDeriveBytes deriveBytes = new PasswordDeriveBytes(password, salt, HashAlgorithmName.SHA256.Name!, 1000);
        byte[] key = deriveBytes.GetBytes(16);
        
        this.AesEncryptString(data, key);
        }

    [Theory]
    [InlineData("Some test data", "password")]
    public void Rfc2898DeriveBytesTest(string data, string password)
        {
        byte[] salt = CryptographyExtensions.GetRandomBytes(8);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, 1000, HashAlgorithmName.SHA256, 16);

        this.AesEncryptString(data, key);
        }

    [Theory]
    [InlineData("Some test data", "password")]
    public void DefaultDeriveBytes(string data, string password)
        => this.AesEncryptString(data, CryptographyExtensions.DeriveBytes(password));
    
    [Theory]
    [InlineData("Some test data", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 })]
    public void AesEncryptString(string data, byte[] key)
        {
        using Aes aes = Aes.Create();
        aes.Key = key;
        this.EncryptString(aes, data);
        }

    [Fact]
    public void Hashing()
        {
        string hash1 = CryptographyExtensions.ComputeHash("Hola mundo Hola mundo Hola mundo Hola mundo Hola mundo Hola mundo Hola mundo");
        string hash2 = CryptographyExtensions.ComputeHash("Hola mundo Hola mundo Hola mundo Hola Mundo Hola mundo Hola mundo Hola mundo");
        /////////////////////////////////////////////////////////////////////////////////////////^

        Assert.NotEqual(hash1, hash2);
        }

    [Theory]
    [InlineData("Hola mundo", "ca8f60b2cc7f05837d98b208b57fb6481553fc5f1219d59618fd025002a66f5c")]
    public void SHA256Hashing(string str, string hash) => Assert.Equal(hash, CryptographyExtensions.ComputeHash(str));

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void SymmetricSigning(string text)
        {
        byte[] key = CryptographyExtensions.GetRandomBytes(10);
        string signature = CryptographyExtensions.Sign(key, text);

        Assert.True(CryptographyExtensions.Verify(key, text, signature));
        }

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void SymmetricEncription(string text)
        {
        (string Key, string IV, string Data) encrypted = CryptographyExtensions.Encrypt(text);

        Assert.NotEqual(text, encrypted.Data);
        Assert.Equal(text, CryptographyExtensions.Decrypt(encrypted));
        }

    [Fact]
    public void XOR1()
        {
        const byte data = 10;
        const byte key = 150;
        const byte encryptedData = (byte)(data ^ key);

        Assert.Equal(data, (byte)(encryptedData ^ key));
        }

    [Fact]
    public void XOR2()
        {
        const byte data = 10;
        const byte key = 5;
        const byte iv = 2;
        const byte encryptedData = (byte)(data ^ iv ^ key);

        Assert.Equal(data, (byte)(encryptedData ^ key ^ iv));
        }

    [Fact]
    public void SimmAlgKeySize()
        {
        XorCipher xorCipher = new XorCipher();

        // el cambio no tiene efecto, porque la propiedad LegalBlockSizes retorna una copia.
        xorCipher.LegalBlockSizes[0] = new KeySizes(10, 15, 3);

        output.WriteLine(xorCipher.LegalBlockSizes[0].MinSize.ToString());
        }

    [Fact]
    public void GenerateIVKey()
        {
        XorCipher xorCipher = new XorCipher();

        byte[] originalIV = xorCipher.IV;
        byte[] originalKey = xorCipher.Key;

        xorCipher.GenerateIV();
        xorCipher.GenerateKey();

        Assert.NotNull(originalIV);
        Assert.NotNull(originalKey);

        Assert.NotNull(xorCipher.IV);
        Assert.NotNull(xorCipher.Key);

        Assert.False(originalIV.SequenceEqual(xorCipher.IV));
        Assert.False(originalKey.SequenceEqual(xorCipher.Key));
        }

    private void EncryptString(SymmetricAlgorithm cipher, string message)
        {
        string encrypted = cipher.Encrypt(message);
        string decrypted = cipher.Decrypt(encrypted);

        output.WriteLine("message:");
        output.WriteLine(message);
        output.WriteLine("");
        output.WriteLine("encrypted:");
        output.WriteLine(encrypted);
        output.WriteLine("");
        output.WriteLine("decrypted:");
        output.WriteLine(decrypted);
        output.WriteLine("");

        Assert.NotEqual(message, encrypted);
        Assert.Equal(message.Length, decrypted.Length);
        Assert.Equal(message, decrypted);
        }

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    public void XorCipherECB_CBC(string message)
        {
        using SymmetricAlgorithm cipherECB = new XorCipher() { Mode = System.Security.Cryptography.CipherMode.ECB };
        using SymmetricAlgorithm cipherCBC = new XorCipher() { Mode = System.Security.Cryptography.CipherMode.CBC };
        string encryptedECB = cipherECB.Encrypt(message);
        string encryptedCBC = cipherCBC.Encrypt(message);

        Assert.NotEqual(encryptedECB, encryptedCBC);
        }

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    public void XorCipherFix(string message)
        {
        byte[] key = [0xa, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf];
        byte[] iv = [5, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf];

        using SymmetricAlgorithm cipher = new XorCipher() { Key = key, IV = iv };
        this.EncryptString(cipher, message);
        }

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    public void XorCipher(string message)
        {
        using SymmetricAlgorithm cipher = new XorCipher();
        this.EncryptString(cipher, message);
        }

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    public void NaiveCipher(string message)
        {
        using SymmetricAlgorithm cipher = new MyNaiveCipher();
        this.EncryptString(cipher, message);
        }

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    public void NaiveCipher2(string message)
        {
        using SymmetricAlgorithm cipher = new MyNaiveCipher2();
        this.EncryptString(cipher, message);
        }

    [Fact]
    public void NaiveCipher2a()
        {
        byte[] data = CryptographyExtensions.GetRandomBytes(100000);

        using SymmetricAlgorithm cipher = new MyNaiveCipher2();
        byte[] encrypted = cipher.Encrypt(data);
        output.WriteLine(cipher.Decrypt(encrypted).SequenceEqual(data).ToString());
        }

    [Fact]
    public void NaiveCipher2b()
        {
        for (int i = 0; i < 100; i++)
            {
            byte[] data = CryptographyExtensions.GetRandomBytes(32);

            using SymmetricAlgorithm cipher = new MyNaiveCipher2();
            byte[] encrypted = cipher.Encrypt(data);
            output.WriteLine(cipher.Decrypt(encrypted).SequenceEqual(data).ToString());
            }
        }

    [Fact]
    public void NaiveCipher2c()
        {
        byte[] data = Enumerable.Range(0, 32).Select(b => (byte) b).ToArray();
        byte[] key = [0xa, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf];
        byte[] iv = [5, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf];

        using SymmetricAlgorithm cipher = new MyNaiveCipher2() { Key = key, IV = iv };
        byte[] encrypted = cipher.Encrypt(data);
        output.WriteLine(cipher.Decrypt(encrypted).SequenceEqual(data).ToString());
        }

    [Theory]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    public void TestAes(string message)
        {
        using SymmetricAlgorithm cipher = Aes.Create();
        this.EncryptString(cipher, message);
        }
    }
