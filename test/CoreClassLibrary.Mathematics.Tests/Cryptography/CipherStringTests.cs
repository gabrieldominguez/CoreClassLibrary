
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Cryptography;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Paddings;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;
using CoreClassLibrary.Mathematics.Randomness;
using System.Security.Cryptography;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

/// <summary>
/// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography tests
/// </summary>
public class CipherStringTests(ITestOutputHelper output)
    {
    private void OutputString(string message, string encrypted, string decrypted)
        {
        output.WriteLine("message");
        output.WriteLine(message);
        output.WriteLine("");
        output.WriteLine("encrypted");
        output.WriteLine(encrypted);
        output.WriteLine("");
        output.WriteLine("decrypted");
        output.WriteLine(decrypted);
        output.WriteLine("");

        //Assert.NotEqual(data, encrypted);
        //Assert.Equal(message.Length, decryptedMessage.Length);
        Assert.Equal(message, decrypted);
        }

    #region BlockCipher
    private void BlockEncryptString(int blockSize, Func<bool, byte[], IBlockEngine> engineFactory, string message) => this.BlockEncryptString(blockSize, blockSize, engineFactory, message);

    private void BlockEncryptString(int keySize, int blockSize, Func<bool, byte[], IBlockEngine> engineFactory, string message) => this.BlockEncryptString(keySize, blockSize, key => new BlockEngineCipher(engineFactory.Invoke(true, key), engineFactory.Invoke(false, key)), message);

    private void BlockEncryptString(int blockSize, Func<byte[], IBlockCipher> blockCipherFactory, string message) => this.BlockEncryptString(blockSize, blockSize, blockCipherFactory, message);

    private void BlockEncryptString(int keySize, int blockSize, Func<byte[], IBlockCipher> blockCipherFactory, string message)
        {
        byte[] key = CryptographyExtensions.GetRandomBytes(keySize);
        byte[] iv = CryptographyExtensions.GetRandomBytes(blockSize);

        this.EncryptString(
            new PaddedBlockCipher(new CBCCipherMode(blockCipherFactory.Invoke(key), iv), new PKCS7Padding()),
            message);

        this.EncryptString(
            new PaddedBlockCipher(new CBCCipherMode(blockCipherFactory.Invoke(key), iv), new PKCS7Padding()),
            new PaddedBlockCipher(new CBCCipherMode(blockCipherFactory.Invoke(key), iv), new PKCS7Padding()),
            message);
        }

    private void EncryptString(IPaddedBlockCipher blockCipher, string message)
        {
        this.EncryptStringWithStream(blockCipher, message);
        this.EncryptStringWithCipherBlock(blockCipher, message);
        this.EncryptStringWithSymmetricAlgorithm(blockCipher, message);
        }

    private void EncryptString(IPaddedBlockCipher encBlockCipher, IPaddedBlockCipher decBlockCipher, string message)
        {
        this.EncryptStringWithStream(encBlockCipher, decBlockCipher, message);
        this.EncryptStringWithCipherBlock(encBlockCipher, decBlockCipher, message);
        this.EncryptStringWithSymmetricAlgorithm(encBlockCipher, decBlockCipher, message);
        }

    private void EncryptStringWithCipherBlock(IPaddedBlockCipher blockCipher, string message)
        {
        output.WriteLine("CipherBlock 1 TEST");

        this.EncryptString(
            new PaddedBlockCipherCipher(blockCipher, true),
            message);
        }

    private void EncryptStringWithCipherBlock(IPaddedBlockCipher encBlockCipher, IPaddedBlockCipher decBlockCipher, string message)
        {
        output.WriteLine("CipherBlock 2 TEST");

        this.EncryptString(
            new PaddedBlockCipherCipher(encBlockCipher, true),
            new PaddedBlockCipherCipher(decBlockCipher, true),
            message);
        }

    private void EncryptStringWithStream(IPaddedBlockCipher blockCipher, string message)
        {
        output.WriteLine("CipherStream 1 TEST");

        this.EncryptString(
            new BlockCipherStreamCipher(blockCipher),
            message);
        }

    private void EncryptStringWithStream(IPaddedBlockCipher encBlockCipher, IPaddedBlockCipher decBlockCipher, string message)
        {
        output.WriteLine("CipherStream 2 TEST");

        this.EncryptString(
            new BlockCipherStreamCipher(encBlockCipher),
            new BlockCipherStreamCipher(decBlockCipher),
            message);
        }

    #region SymmetricAlgorithm
    private void EncryptStringWithSymmetricAlgorithm(IPaddedBlockCipher blockCipher, string message)
        {
        blockCipher.Initialize();
        using SymmetricAlgorithm cipherSymAlg = new SymmetricAlgorithmAdapter(blockCipher);
        string encrypted2 = this.EncryptString(cipherSymAlg, message);
        }

    private void EncryptStringWithSymmetricAlgorithm(IPaddedBlockCipher encBlockCipher, IPaddedBlockCipher decBlockCipher, string message)
        {
        encBlockCipher.Initialize();
        decBlockCipher.Initialize();
        using SymmetricAlgorithm encCipherSymAlg = new SymmetricAlgorithmAdapter(encBlockCipher);
        using SymmetricAlgorithm decCipherSymAlg = new SymmetricAlgorithmAdapter(decBlockCipher);
        string encrypted2 = this.EncryptString(encCipherSymAlg, decCipherSymAlg, message);
        }

    private string EncryptString(SymmetricAlgorithm cipher, string message)
        {
        string encrypted = cipher.Encrypt(message);
        string decrypted = cipher.Decrypt(encrypted);

        output.WriteLine("SymmetricAlgorithm 1 TEST");
        output.WriteLine("message");
        output.WriteLine(message);
        output.WriteLine("");
        output.WriteLine("encrypted");
        output.WriteLine(encrypted);
        output.WriteLine("");
        output.WriteLine("decrypted");
        output.WriteLine(decrypted);
        output.WriteLine("");

        //Assert.NotEqual(message, encrypted);
        //Assert.Equal(message.Length, decryptedMessage.Length);
        //Assert.Equal(message, decryptedMessage);
        //Assert.True(message.Length <= decrypted.Length);
        Assert.StartsWith(message, decrypted);

        return encrypted;
        }

    private string EncryptString(SymmetricAlgorithm encCipher, SymmetricAlgorithm decCipher, string message)
        {
        string encrypted = encCipher.Encrypt(message);
        string decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("SymmetricAlgorithm 2 TEST");
        output.WriteLine("message");
        output.WriteLine(message);
        output.WriteLine("");
        output.WriteLine("encrypted");
        output.WriteLine(encrypted);
        output.WriteLine("");
        output.WriteLine("decrypted");
        output.WriteLine(decrypted);
        output.WriteLine("");

        //Assert.NotEqual(message, encrypted);
        //Assert.Equal(message.Length, decryptedMessage.Length);
        //Assert.Equal(message, decryptedMessage);
        //Assert.True(message.Length <= decrypted.Length);
        Assert.StartsWith(message, decrypted);

        return encrypted;
        }
    #endregion
    #endregion

    #region StreamCipher
    /*
    private void StreamEncryptString(int keySize, Func<bool, byte[], IStreamEngine> engineFactory, string message)
        {
        this.StreamEncryptString(keySize, key => new StreamEngineCipher(engineFactory.Invoke(true, key), engineFactory.Invoke(false, key)), message);
        }
    */
    private void StreamEncryptString(int keySize, Func<byte[], IStreamCipher> streamCipherFactory, string message)
        {
        byte[] key = CryptographyExtensions.GetRandomBytes(keySize);

        this.EncryptString(
            new StreamCipher(streamCipherFactory.Invoke(key)),
            message);

        this.EncryptString(
            new StreamCipher(streamCipherFactory.Invoke(key)),
            new StreamCipher(streamCipherFactory.Invoke(key)),
            message);
        }

    private void EncryptString(IStreamCipher streamCipher, string message)
        {
        this.EncryptStringWithStream(streamCipher, message);
        this.EncryptStringWithCipherStream(streamCipher, message);
        }

    private void EncryptString(IStreamCipher encStreamCipher, IStreamCipher decStreamCipher, string message)
        {
        this.EncryptStringWithStream(encStreamCipher, decStreamCipher, message);
        this.EncryptStringWithCipherStream(encStreamCipher, decStreamCipher, message);
        }

    private void EncryptStringWithCipherStream(IStreamCipher streamCipher, string message)
        {
        output.WriteLine("CipherStream 1 TEST");

        this.EncryptString(
            new StreamCipherStreamCipher(streamCipher),
            message);
        }

    private void EncryptStringWithCipherStream(IStreamCipher encStreamCipher, IStreamCipher decStreamCipher, string message)
        {
        output.WriteLine("CipherStream 2 TEST");

        this.EncryptString(
            new StreamCipherStreamCipher(encStreamCipher),
            new StreamCipherStreamCipher(decStreamCipher),
            message);
        }

    private void EncryptStringWithStream(IStreamCipher streamCipher, string message)
        {
        output.WriteLine("CipherStream 1 TEST");

        this.EncryptString(
            new StreamCipherStreamCipher(streamCipher),
            message);
        }

    private void EncryptStringWithStream(IStreamCipher encStreamCipher, IStreamCipher decStreamCipher, string message)
        {
        output.WriteLine("CipherStream 2 TEST");

        this.EncryptString(
            new StreamCipherStreamCipher(encStreamCipher),
            new StreamCipherStreamCipher(decStreamCipher),
            message);
        }
    #endregion
    /*
    private void EncryptString(int keySize, Func<byte[], ICipher> cipherFactory, string message)
        {
        byte[] key = CryptographyExtensions.GetRandomBytes(keySize);

        this.EncryptString(
            cipherFactory.Invoke(key),
            message);

        this.EncryptString(
            cipherFactory.Invoke(key),
            cipherFactory.Invoke(key),
            message);
        }
    */
    private void EncryptString(ICipher cipher, string message)
        {
        cipher.Initialize();
        string encrypted = cipher.Encrypt(message);
        cipher.Initialize();
        string decrypted = cipher.Decrypt(encrypted);

        output.WriteLine("Cipher 1 TEST");
        this.OutputString(message, encrypted, decrypted);
        }

    private void EncryptString(ICipher encCipher, ICipher decCipher, string message)
        {
        encCipher.Initialize();
        string encrypted = encCipher.Encrypt(message);
        decCipher.Initialize();
        string decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("Cipher 2 TEST");
        this.OutputString(message, encrypted, decrypted);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void AesCipherStringPadded(string message)
        {
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];

        IPadding pkcs7Padding = new PKCS7Padding();

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), pkcs7Padding), true);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);

        byte[] data = StringExtensions.UTF8Encoding.GetBytes(message);

        byte[] encrypted = encCipher.Encrypt(data);
        byte[] decryptedPadded = decCipher.Decrypt(encrypted);

        string decyptedMessage = StringExtensions.UTF8Encoding.GetString(decryptedPadded, 0, decryptedPadded.Length - pkcs7Padding.GetPaddingLength(decryptedPadded));

        output.WriteLine(message);
        output.WriteLine("");
        output.WriteLine(StringExtensions.UTF8Encoding.GetString(encrypted));
        output.WriteLine("");
        output.WriteLine(decyptedMessage);
        output.WriteLine("");

        Assert.NotEqual(data, encrypted);
        Assert.Equal(message.Length, decyptedMessage.Length);
        Assert.Equal(message, decyptedMessage);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void AesCipher(string message) => this.BlockEncryptString(16, key => new AesCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Blowfish(string message) => this.BlockEncryptString(8, key => new BlowfishCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Camellia(string message) => this.BlockEncryptString(16, (forEncryption, key) => new CamelliaEngine(forEncryption, key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Cast5CipherString(string message) => this.BlockEncryptString(8, key => new Cast5Cipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Cast6(string message) => this.BlockEncryptString(16, key => new Cast6Cipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Des(string message) => this.BlockEncryptString(8, key => new DesCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Idea(string message) => this.BlockEncryptString(8, (forEncryption, key) => new IdeaEngine(forEncryption, key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Isaac(string message) => this.StreamEncryptString(8, key => new IsaacCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Vmpc(string message)
        {
        byte[] iv = CryptographyExtensions.GetRandomBytes(16);
        this.StreamEncryptString(8, key => new VmpcCipher(key, iv), message);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void NoekeonCipher(string message) => this.BlockEncryptString(16, key => new NoekeonCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void RC2Cipher(string message) => this.BlockEncryptString(8, key => new RC2Cipher(key), message);

    [Theory]
    [InlineData("\0\0\0\0")]
    [InlineData("\0\0\0\0\0\0\0\0")]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void RC5Cipher(string message) => this.BlockEncryptString(8, key => new RC5Cipher(key), message);

    [Theory]
    [InlineData("\0\0\0\0")]
    [InlineData("\0\0\0\0\0\0\0\0")]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void RC6Cipher(string message) => this.BlockEncryptString(16, key => new RC6Cipher(key), message);

    [Theory]
    [InlineData("\0\0\0\0")]
    [InlineData("\0\0\0\0\0\0\0\0")]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void SEEDCipher(string message) => this.BlockEncryptString(16, key => new SEEDCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Kalyna(string message) => this.BlockEncryptString(32, key => new KalynaCipher(32 * 8, key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void TripleDes(string message)
        {
        byte[] key = CryptographyExtensions.GetRandomBytes(16);
        byte[] iv = CryptographyExtensions.GetRandomBytes(8);

        this.EncryptString(
            new PaddedBlockCipher(new CBCCipherMode(new TripleDesCipher(key), iv), new PKCS7Padding()),
            message);

        this.EncryptString(
            new PaddedBlockCipher(new CBCCipherMode(new TripleDesCipher(key), iv), new PKCS7Padding()),
            new PaddedBlockCipher(new CBCCipherMode(new TripleDesCipher(key), iv), new PKCS7Padding()),
            message);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Serpent(string message) => this.BlockEncryptString(16, key => new SerpentCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Tnepres(string message) => this.BlockEncryptString(16, key => new TnepresCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void SkipjackCipher(string message) => this.BlockEncryptString(10, 8, key => new SkipjackCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void TeaCipher(string message) => this.BlockEncryptString(16, 8, key => new TeaCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void XTeaCipher(string message) => this.BlockEncryptString(16, 8, key => new XTeaCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]    // el tamaño del buffer es multiplo del tamaño del bloque
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Twofish(string message) => this.BlockEncryptString(16, key => new TwofishCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Arc4(string message) => this.StreamEncryptString(16, key => new Arc4Cipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void HC128Cipher(string message)
        {
        byte[] iv = CryptographyExtensions.GetRandomBytes(16);
        this.StreamEncryptString(16, key => new HC128Cipher(key, iv), message);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void HC256Cipher(string message)
        {
        byte[] iv = CryptographyExtensions.GetRandomBytes(16);
        this.StreamEncryptString(16, key => new HC256Cipher(key, iv), message);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Xor(string message) => this.StreamEncryptString(16, key => new XorCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Random(string message)
        {
        int key = RandomWrapper.Shared.Next<int>();

        this.EncryptString(
            new RandomCipher(() => new RandomWrapper(key)),
            message);

        this.EncryptString(
            new RandomCipher(() => new RandomWrapper(key)),
            new RandomCipher(() => new RandomWrapper(key)),
            message);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void MyNaive(string message) => this.StreamEncryptString(16, key => new MyNaiveCipher(key), message);

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void MyNaive2(string message)
        {
        byte[] key = CryptographyExtensions.GetRandomBytes(32);
        byte[] iv = CryptographyExtensions.GetRandomBytes(32);

        this.EncryptString(
            new MyNaiveCipher2(key, iv),
            message);

        this.EncryptString(
            new MyNaiveCipher2(key, iv),
            new MyNaiveCipher2(key, iv),
            message);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void Composite(string message)
        {
        byte[] key1 = CryptographyExtensions.GetRandomBytes(32);
        byte[] key2 = CryptographyExtensions.GetRandomBytes(32);

        byte[] iv = CryptographyExtensions.GetRandomBytes(32);

        // Obvio
        this.EncryptString(
            new CompositeCipher([new StreamCipher(new Arc4Cipher(key1))]),
            new CompositeCipher([new StreamCipher(new Arc4Cipher(key1))]),
            message);

        // Obvio 2
        this.EncryptString(
            new CompositeCipher([new StreamCipher(new Arc4Cipher(key1))]),
            message);

        // Directo
        this.EncryptString(
            new CompositeCipher([
                new StreamCipher(new Arc4Cipher(key1)),
                new StreamCipher(new Arc4Cipher(key2))]),
            new CompositeCipher([
                new StreamCipher(new Arc4Cipher(key1)),
                new StreamCipher(new Arc4Cipher(key2))]),
            message);

        // Reversos, solo para arc4 and xor 
        this.EncryptString(
            new CompositeCipher([
                new StreamCipher(new Arc4Cipher(key1)),
                new StreamCipher(new Arc4Cipher(key2))]),
            new CompositeCipher([
                new StreamCipher(new Arc4Cipher(key2)),
                new StreamCipher(new Arc4Cipher(key1))]),
            message);

        // Con diferente algoritmo
        this.EncryptString(
            new CompositeCipher([
                new StreamCipher(new Arc4Cipher(key1)),
                new MyNaiveCipher2(key2, iv)]),
            new CompositeCipher([
                new StreamCipher(new Arc4Cipher(key1)),
                new MyNaiveCipher2(key2, iv)]),
            message);
        }

    [Theory]
    [InlineData("6EQUJ5")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("  6EQUJ5 11  1 ")] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
    [InlineData("Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola")]
    [InlineData("Hola mundo Hola mundo Hola mundo")]
    [InlineData("Hola mundo Hola mundo Hola mundo___________________________________________________________________________________________Hola mundo Hola mundo Hola mundo Hola mundo")]
    public void CompositeBlockCiphe(string message)
        {
        const int blockSize = 16;
        byte[] key1 = CryptographyExtensions.GetRandomBytes(blockSize);
        byte[] iv1 = CryptographyExtensions.GetRandomBytes(blockSize);
        byte[] key2 = CryptographyExtensions.GetRandomBytes(blockSize);
        byte[] iv2 = CryptographyExtensions.GetRandomBytes(blockSize);

        this.EncryptString(
            new PaddedBlockCipher(
                new CompositeBlockCipher([
                    new CBCCipherMode(new AesCipher(key1), iv1),
                    new CBCCipherMode(new AesCipher(key2), iv2)]),
                new PKCS7Padding()),
            message);

        this.EncryptString(
            new PaddedBlockCipher(
                new CompositeBlockCipher([
                    new CBCCipherMode(new AesCipher(key1), iv1),
                    new CBCCipherMode(new AesCipher(key2), iv2)]),
                new PKCS7Padding()),
            new PaddedBlockCipher(
                new CompositeBlockCipher([
                    new CBCCipherMode(new AesCipher(key1), iv1),
                    new CBCCipherMode(new AesCipher(key2), iv2)]),
                new PKCS7Padding()),
            message);

        this.EncryptString(
            new PaddedBlockCipher(
                new CompositeBlockCipher([
                    new CBCCipherMode(new AesCipher(key1), iv1),
                    new PCBCCipherMode(new TwofishCipher(key2), iv2)]),
                new PKCS7Padding()),
            new PaddedBlockCipher(
                new CompositeBlockCipher([
                    new CBCCipherMode(new AesCipher(key1), iv1),
                    new PCBCCipherMode(new TwofishCipher(key2), iv2)]),
                new PKCS7Padding()),
            message);
        }
    }
