
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Cryptography;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Algorithms;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Modes;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Paddings;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers.Algorithms;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

/// <summary>
/// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography tests
/// </summary>
public class CipherVectorTests(ITestOutputHelper output)
    {
    [Fact]
    public void AesCipher_Input_128_WithoutPadding()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0x01];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new AesCipher(key), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new AesCipher(key), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void AesCipher_Input_128_CBC_WithoutPadding()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0x01];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0x8c, 0xed, 0xf1, 0x44, 0x31, 0xcd, 0xbd, 0xaa, 0x6b, 0xaf, 0xa6, 0x44, 0x55, 0x80, 0xc1, 0x97];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        }


    [Fact]
    public void AesCipher_input_ends_like_Padding_in_complete_block()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0x01];   // 0x01 termina como un PKCS7 Padding
        byte[] inputPadded = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0x01, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10];     // se agrega un bloque entero de padding
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0x8c, 0xed, 0xf1, 0x44, 0x31, 0xcd, 0xbd, 0xaa, 0x6b, 0xaf, 0xa6, 0x44, 0x55, 0x80, 0xc1, 0x97, 0xb3, 0x13, 0x8e, 0x4d, 0xa4, 0x79, 0x8c, 0xe5, 0xc7, 0x36, 0x2e, 0x11, 0xaa, 0xea, 0x1a, 0x49];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()), true);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()), true);
        ICipher decCipherNoUnpadding = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);
        byte[] decryptedPadded = decCipherNoUnpadding.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("expected");
        output.WriteLine(expected.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());
        output.WriteLine("input");
        output.WriteLine(input.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("decryptedPadded");
        output.WriteLine(decryptedPadded.ToByteArrayLiteral());
        output.WriteLine("inputPadded");
        output.WriteLine(inputPadded.ToByteArrayLiteral());

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        Assert.Equal(inputPadded, decryptedPadded);
        Assert.Equal(iv.Length, new PKCS7Padding().GetPaddingLength(decryptedPadded));
        }

    [Fact]
    public void AesCipher_input_ends_like_Padding_in_complete_block_Stream()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0x01];   // 0x01 termina como un PKCS7 Padding
        byte[] inputPadded = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0x01, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10];     // se agrega un bloque entero de padding
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0x8c, 0xed, 0xf1, 0x44, 0x31, 0xcd, 0xbd, 0xaa, 0x6b, 0xaf, 0xa6, 0x44, 0x55, 0x80, 0xc1, 0x97, 0xb3, 0x13, 0x8e, 0x4d, 0xa4, 0x79, 0x8c, 0xe5, 0xc7, 0x36, 0x2e, 0x11, 0xaa, 0xea, 0x1a, 0x49];

        ICipher encCipher = new BlockCipherStreamCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()));
        ICipher decCipher = new BlockCipherStreamCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()));
        ICipher decCipherNoUnpadding = new BlockCipherStreamCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null));

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);
        byte[] decryptedPadded = decCipherNoUnpadding.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("expected");
        output.WriteLine(expected.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());
        output.WriteLine("input");
        output.WriteLine(input.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("decryptedPadded");
        output.WriteLine(decryptedPadded.ToByteArrayLiteral());
        output.WriteLine("inputPadded");
        output.WriteLine(inputPadded.ToByteArrayLiteral());

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        //Assert.Equal(inputPadded, decryptedPadded);
        Assert.Equal(iv.Length, new PKCS7Padding().GetPaddingLength(decryptedPadded));
        }

    [Fact]
    public void AesCipher_input_ends_like_Padding_in_partial_block()
        {
        byte[] input =       [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0x01];   // 0x01 termina como un PKCS7 Padding, pero en un bloque incompleto no es problema
        byte[] inputPadded = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0x01, 0x01];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0xf4, 0xb0, 0x47, 0xd9, 0x29, 0x69, 0x5e, 0x89, 0x9d, 0x43, 0x53, 0xcf, 0x9e, 0x87, 0x56, 0xda];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()), true);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()), true);
        ICipher decCipherNoUnpadding = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);
        byte[] decryptedPadded = decCipherNoUnpadding.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("expected");
        output.WriteLine(expected.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());
        output.WriteLine("input");
        output.WriteLine(input.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("decryptedPadded");
        output.WriteLine(decryptedPadded.ToByteArrayLiteral());
        output.WriteLine("inputPadded");
        output.WriteLine(inputPadded.ToByteArrayLiteral());

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        Assert.Equal(inputPadded, decryptedPadded);
        Assert.Equal(1, new PKCS7Padding().GetPaddingLength(decryptedPadded));
        }

    [Fact]
    public void AesCipher_Input_128_CBC_OneCipher()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0x15, 0xa5, 0x10, 0x92, 0x4a, 0x37, 0x9e, 0xa9, 0x80, 0x1c, 0x14, 0x83, 0xa3, 0x39, 0x45, 0x28];

        PaddedBlockCipherCipher cipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = cipher.Encrypt(input);
        byte[] decryptedWrong = cipher.Decrypt(encrypted);

        Assert.NotEqual(input, decryptedWrong);

        cipher.Initialize();

        byte[] decrypted = cipher.Decrypt(encrypted);

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void AesCipher_Input_128_CBC()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0x15, 0xa5, 0x10, 0x92, 0x4a, 0x37, 0x9e, 0xa9, 0x80, 0x1c, 0x14, 0x83, 0xa3, 0x39, 0x45, 0x28];

        PaddedBlockCipherCipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);
        PaddedBlockCipherCipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void AesCipher_InputAndOffsetAndLength_128_CBC()
        {
        // 2 dummy bytes before and 3 after the input from test case above
        byte[] input = [0x05, 0x07, 0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5, 0x0f, 0x0d, 0x03];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0x15, 0xa5, 0x10, 0x92, 0x4a, 0x37, 0x9e, 0xa9, 0x80, 0x1c, 0x14, 0x83, 0xa3, 0x39, 0x45, 0x28];

        PaddedBlockCipherCipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);
        PaddedBlockCipherCipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), null), false);

        const int offset = 2;
        int length = input.Length - 5;

        byte[] encrypted = encCipher.Encrypt(input, offset, length);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(expected, encrypted);
        Assert.Equal(input.Skip(offset).Take(length), decrypted);
        }

    [Fact]
    public void AesCipher_InputAndOffsetAndLength_128_CBC_PKCS7Padding()
        {
        // 2 dummy bytes before and 3 after the input from test case above
        byte[] input = [0x05, 0x07, 0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5, 0x0f, 0x0d, 0x03];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54, 0xec, 0x77, 0x92, 0xf3, 0x15, 0xa0, 0x41, 0xa8];
        byte[] expected = [0x19, 0x7f, 0x80, 0xd8, 0xc9, 0x89, 0xc4, 0xa7, 0xc6, 0xc6, 0x3f, 0x9f, 0x1e, 0x00, 0x1f, 0x72, 0xa7, 0x5e, 0xde, 0x40, 0x88, 0xa2, 0x72, 0xf2, 0xed, 0x3f, 0x81, 0x45, 0xb6, 0xbd, 0x45, 0x87, 0x07, 0xba, 0x0e, 0xc0, 0xe9, 0xd8, 0x5a, 0x13, 0x09, 0x42, 0xd7, 0x11, 0xf3, 0xc6, 0xb4, 0x61];

        PaddedBlockCipherCipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()), true);
        PaddedBlockCipherCipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new AesCipher(key), iv), new PKCS7Padding()), true);

        const int offset = 2;
        int length = input.Length - 9;

        byte[] encrypted = encCipher.Encrypt(input, offset, length);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());

        Assert.Equal(expected, encrypted);
        Assert.Equal(input.Skip(offset).Take(length), decrypted);
        }

    [Fact]
    public void AesCipher_Input_128_CTR()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0xb0, 0x74, 0x21, 0x87, 0x16, 0xb9, 0x69, 0x48, 0x33, 0xce, 0xb3, 0xe7, 0xdc, 0x3f, 0x50, 0xdc, 0xcc, 0xd5, 0x27, 0xb7, 0xfe, 0x7a, 0x78, 0x22, 0xae, 0xc8];
        byte[] key = [0x17, 0x78, 0x56, 0xe1, 0x3e, 0xbd, 0x3e, 0x50, 0x1d, 0x79, 0x3f, 0x0f, 0x55, 0x37, 0x45, 0x54];
        byte[] iv = [0xe6, 0x65, 0x36, 0x0d, 0xdd, 0xd7, 0x50, 0xc3, 0x48, 0xdb, 0x48, 0x07, 0xa1, 0x30, 0xd2, 0x38];
        byte[] expected = [0xca, 0xfb, 0x1c, 0x49, 0xbf, 0x82, 0x2a, 0xbb, 0x1c, 0x52, 0xc7, 0x86, 0x22, 0x8a, 0xe5, 0xa4, 0xf3, 0xda, 0x4e, 0x1c, 0x3a, 0x87, 0x41, 0x1c, 0xd2, 0x6e, 0x76, 0xdc, 0xc2, 0xe9, 0xc2, 0x0e, 0xf5, 0xc7, 0xbd, 0x12, 0x85, 0xfa, 0x0e, 0xda, 0xee, 0x50, 0xd7, 0xfd, 0x81, 0x34, 0x25, 0x6d];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CTRCipherMode(new AesCipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CTRCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void AesCipher_Input_128_OFB()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0xb0, 0x74, 0x21, 0x87, 0x16, 0xb9, 0x69, 0x48, 0x33, 0xce, 0xb3, 0xe7, 0xdc, 0x3f, 0x50, 0xdc, 0xcc, 0xd5, 0x27, 0xb7, 0xfe, 0x7a, 0x78, 0x22, 0xae, 0xc8];
        byte[] key = [0x17, 0x78, 0x56, 0xe1, 0x3e, 0xbd, 0x3e, 0x50, 0x1d, 0x79, 0x3f, 0x0f, 0x55, 0x37, 0x45, 0x54];
        byte[] iv = [0xe6, 0x65, 0x36, 0x0d, 0xdd, 0xd7, 0x50, 0xc3, 0x48, 0xdb, 0x48, 0x07, 0xa1, 0x30, 0xd2, 0x38];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new OFBCipherMode(new AesCipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new OFBCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());

        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void AesCipher_Input_128_PCBC()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0xb0, 0x74, 0x21, 0x87, 0x16, 0xb9, 0x69, 0x48, 0x33, 0xce, 0xb3, 0xe7, 0xdc, 0x3f, 0x50, 0xdc, 0xcc, 0xd5, 0x27, 0xb7, 0xfe, 0x7a, 0x78, 0x22, 0xae, 0xc8];
        byte[] key = [0x17, 0x78, 0x56, 0xe1, 0x3e, 0xbd, 0x3e, 0x50, 0x1d, 0x79, 0x3f, 0x0f, 0x55, 0x37, 0x45, 0x54];
        byte[] iv = [0xe6, 0x65, 0x36, 0x0d, 0xdd, 0xd7, 0x50, 0xc3, 0x48, 0xdb, 0x48, 0x07, 0xa1, 0x30, 0xd2, 0x38];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new PCBCCipherMode(new AesCipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new PCBCCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());

        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void AesCipher_Input_128_CFB()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0xb0, 0x74, 0x21, 0x87, 0x16, 0xb9, 0x69, 0x48, 0x33, 0xce, 0xb3, 0xe7, 0xdc, 0x3f, 0x50, 0xdc, 0xcc, 0xd5, 0x27, 0xb7, 0xfe, 0x7a, 0x78, 0x22, 0xae, 0xc8];
        byte[] key = [0x17, 0x78, 0x56, 0xe1, 0x3e, 0xbd, 0x3e, 0x50, 0x1d, 0x79, 0x3f, 0x0f, 0x55, 0x37, 0x45, 0x54];
        byte[] iv = [0xe6, 0x65, 0x36, 0x0d, 0xdd, 0xd7, 0x50, 0xc3, 0x48, 0xdb, 0x48, 0x07, 0xa1, 0x30, 0xd2, 0x38];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CFBCipherMode(new AesCipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CFBCipherMode(new AesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());

        Assert.Equal(input, decrypted);
        }

    // see https://en.wikipedia.org/wiki/RC4
    [Theory]
    [InlineData("Key", "Plaintext", new byte[] { 0xBB, 0xF3, 0x16, 0xE8, 0xD9, 0x40, 0xAF, 0x0A, 0xD3 })]
    [InlineData("Wiki", "pedia", new byte[] { 0x10, 0X21, 0xBF, 0x04, 0x20 })]
    [InlineData("Secret", "Attack at dawn", new byte[] { 0x45, 0xA0, 0x1F, 0x64, 0x5F, 0xC3, 0x5B, 0x38, 0x35, 0x52, 0x54, 0x4B, 0x9B, 0xF5 })]
    public void Arc4Cipher_DischargeFirstBytes_False(string key, string expectedPlainText, byte[] cipherText)
        {
        ICipher cipher = new StreamCipher(new Arc4Cipher(StringExtensions.UTF8Encoding.GetBytes(key), false));
        ICipher cipher2 = new StreamCipher(new Arc4Cipher(StringExtensions.UTF8Encoding.GetBytes(key), false));

        byte[] actualPlainText = cipher.Decrypt(cipherText);
        byte[] encrypted = cipher2.Encrypt(actualPlainText);

        output.WriteLine(expectedPlainText);
        output.WriteLine(StringExtensions.UTF8Encoding.GetString(actualPlainText));

        Assert.Equal(expectedPlainText, StringExtensions.UTF8Encoding.GetString(actualPlainText));
        Assert.Equal(cipherText, encrypted);
        }

    [Theory]
    [InlineData("Key", "Plaintext")]
    [InlineData("Wiki", "pedia")]
    [InlineData("Secret", "Attack at dawn")]
    public void Arc4Cipher_DischargeFirstBytes_True(string key, string plainText)
        {
        ICipher cipher = new StreamCipher(new Arc4Cipher(StringExtensions.UTF8Encoding.GetBytes(key)));
        ICipher cipher2 = new StreamCipher(new Arc4Cipher(StringExtensions.UTF8Encoding.GetBytes(key)));

        byte[] encrypted = cipher.Encrypt(StringExtensions.UTF8Encoding.GetBytes(plainText));
        string actualPlainText = StringExtensions.UTF8Encoding.GetString(cipher2.Decrypt(encrypted));

        output.WriteLine(plainText);
        output.WriteLine(actualPlainText);

        Assert.Equal(plainText, actualPlainText);
        }

    [Fact]
    public void Arc4Cipher_InputAndOffsetAndLength()
        {
        const string key = "Key";
        const string expectedPlainText = "Plaintext";

        StreamCipher cipher = new StreamCipher(new Arc4Cipher(StringExtensions.UTF8Encoding.GetBytes(key), false));
        byte[] cipherText = [0x0A, 0x0f, 0xBB, 0xF3, 0x16, 0xE8, 0xD9, 0x40, 0xAF, 0x0A, 0xD3, 0x0d, 0x05];

        byte[] actualPlainText = cipher.Decrypt(cipherText, 2, cipherText.Length - 4);

        Assert.Equal(expectedPlainText, StringExtensions.UTF8Encoding.GetString(actualPlainText));
        }

    [Fact]
    public void Blowfish_128_CBC()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54];
        byte[] expected = [0x50, 0x49, 0xe0, 0xce, 0x98, 0x93, 0x8b, 0xec, 0x82, 0x7d, 0x14, 0x1b, 0x3e, 0xdc, 0xca, 0x63, 0xef, 0x36, 0x20, 0x67, 0x58, 0x63, 0x1f, 0x9c, 0xd2, 0x12, 0x6b, 0xca, 0xea, 0xd0, 0x78, 0x8b, 0x61, 0x50, 0x4f, 0xc4, 0x5b, 0x32, 0x91, 0xd6, 0x65, 0xcb, 0x74, 0xe5, 0x6e, 0xf5, 0xde, 0x14];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new BlowfishCipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new BlowfishCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void Blowfish_128()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new BlowfishCipher(key), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new BlowfishCipher(key), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(input, decrypted);
        }


    /// <summary>
    /// Blowfish tester - vectors from http://www.counterpane.com/vectors.txt
    /// </summary>
    [Theory]
    [InlineData("0000000000000000", "0000000000000000", "4EF997456198DD78")]
    [InlineData("FFFFFFFFFFFFFFFF", "FFFFFFFFFFFFFFFF", "51866FD5B85ECB8A")]
    [InlineData("3000000000000000", "1000000000000001", "7D856F9A613063F2")]
    [InlineData("1111111111111111", "1111111111111111", "2466DD878B963C9D")]
    [InlineData("0123456789ABCDEF", "1111111111111111", "61F9C3802281B096")]
    [InlineData("FEDCBA9876543210", "0123456789ABCDEF", "0ACEAB0FC6A0A28D")]
    [InlineData("7CA110454A1A6E57", "01A1D6D039776742", "59C68245EB05282B")]
    [InlineData("0131D9619DC1376E", "5CD54CA83DEF57DA", "B1B8CC0B250F09A0")]
    public void Blowfish(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new BlowfishCipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "000102030405060708090A0B0C0D0E0F", "8ef0272c42db838bcf7b07af0ec30f38")]
    [InlineData("000102030405060708090a0b0c0d0e0f1011121314151617", "000102030405060708090A0B0C0D0E0F", "95accc625366547617f8be4373d10cd7")]
    [InlineData("000102030405060708090a0b0c0d0e0f", "000102030405060708090A0B0C0D0E0F", "9fb63337151be9c71306d159ea7afaa4")]
    public void Twofish(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new TwofishCipher(key), hexKey, hexInput, hexOutput);

    /// <summary>
    /// Camellia tester - vectors from https://www.cosic.esat.kuleuven.be/nessie/testvectors/ and RFC 3713
    /// </summary>
    [Theory]
    [InlineData("00000000000000000000000000000000", "80000000000000000000000000000000", "07923A39EB0A817D1C4D87BDB82D1F1C")]
    [InlineData("80000000000000000000000000000000", "00000000000000000000000000000000", "6C227F749319A3AA7DA235A9BBA05A2C")]
    [InlineData("0123456789abcdeffedcba9876543210", "0123456789abcdeffedcba9876543210", "67673138549669730857065648eabe43")]

    [InlineData("0123456789abcdeffedcba98765432100011223344556677", "0123456789abcdeffedcba9876543210", "b4993401b3e996f84ee5cee7d79b09b9")]
    [InlineData("000000000000000000000000000000000000000000000000", "00040000000000000000000000000000", "9BCA6C88B928C1B0F57F99866583A9BC")]
    [InlineData("949494949494949494949494949494949494949494949494", "636EB22D84B006381235641BCF0308D2", "94949494949494949494949494949494")]

    [InlineData("0123456789abcdeffedcba987654321000112233445566778899aabbccddeeff", "0123456789abcdeffedcba9876543210", "9acc237dff16d76c20ef7c919e3a7509")]
    [InlineData("4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A", "057764FE3A500EDBD988C5C3B56CBA9A", "4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A4A")]
    [InlineData("0303030303030303030303030303030303030303030303030303030303030303", "7968B08ABA92193F2295121EF8D75C8A", "03030303030303030303030303030303")]
    public void Camellia(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector((forEncryption, key) => new CamelliaEngine(forEncryption, key), hexKey, hexInput, hexOutput);

    /// <summary>
    /// Cast tester - vectors from http://www.ietf.org/rfc/rfc2144.txt
    /// </summary>
    [Theory]
    [InlineData("0123456712345678234567893456789A", "0123456789ABCDEF", "238B4FE5847E44B2")]
    [InlineData("01234567123456782345", "0123456789ABCDEF", "EB6A711A2C02271B")]
    [InlineData("0123456712", "0123456789ABCDEF", "7Ac816d16E9B302E")]
    public void Cast5(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new Cast5Cipher(key), hexKey, hexInput, hexOutput);

    /// <summary>
    /// Cast6 tester - vectors from http://www.ietf.org/rfc/rfc2612.txt
    /// </summary>
    [Theory]
    [InlineData("2342bb9efa38542c0af75647f29f615d", "00000000000000000000000000000000", "c842a08972b43d20836c91d1b7530f6b")]
    [InlineData("2342bb9efa38542cbed0ac83940ac298bac77a7717942863", "00000000000000000000000000000000", "1b386c0210dcadcbdd0e41aa08a7a7e8")]
    [InlineData("2342bb9efa38542cbed0ac83940ac2988d7c47ce264908461cc1b5137ae6b604", "00000000000000000000000000000000", "4f6a2038286897b9c9870136553317fa")]
    public void Cast6(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new Cast6Cipher(key), hexKey, hexInput, hexOutput);

    [Fact]
    public void CastCipher_128()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new Cast5Cipher(key), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new Cast5Cipher(key), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void CastCipher_128_CBC()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x2c, 0x1a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x30, 0x9e, 0xe0, 0x9c, 0x12, 0xee, 0x3a, 0x30, 0x03, 0x52, 0x1c, 0x1a, 0xe7, 0x3e, 0x0b, 0x9a, 0xcf, 0x9a, 0x57, 0x42, 0x0b, 0x4f, 0x4a, 0x15, 0xa0, 0xf5];
        byte[] key = [0xe4, 0x94, 0xf9, 0xb1, 0x00, 0x4f, 0x16, 0x2a, 0x80, 0x11, 0xea, 0x73, 0x0d, 0xb9, 0xbf, 0x64];
        byte[] iv = [0x74, 0x8b, 0x4f, 0xe6, 0xc1, 0x29, 0xb3, 0x54];
        byte[] expected = [0x32, 0xef, 0xbd, 0xac, 0xb6, 0xfd, 0x1f, 0xae, 0x1b, 0x13, 0x5f, 0x31, 0x6d, 0x38, 0xcd, 0xb0, 0xe3, 0xca, 0xe1, 0xbc, 0xf8, 0xa7, 0xc2, 0x31, 0x62, 0x14, 0x3a, 0x9a, 0xda, 0xe3, 0xf8, 0xc8, 0x70, 0x87, 0x53, 0x21, 0x5d, 0xb7, 0x94, 0xb7, 0xe8, 0xc6, 0x9d, 0x46, 0x0c, 0x6d, 0x64, 0x6d];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new Cast5Cipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new Cast5Cipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void DES_CBC_Encrypt()
        {
        byte[] expectedEncrypted =
            [
                0x15, 0x43, 0x3e, 0x97, 0x65, 0x66, 0xea, 0x81,
                0x22, 0xab, 0xe3, 0x11, 0x0f, 0x7d, 0xcb, 0x78,
                0x56, 0x91, 0x22, 0x3d, 0xd6, 0xca, 0xe3, 0xbd,
                0xb9, 0x8a, 0x38, 0x0c, 0xea, 0x5a, 0x90, 0x06
            ];

        byte[] input = StringExtensions.UTF8Encoding.GetBytes("www.javaCODEgeeks.com!!!");
        byte[] key = [0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef];
        byte[] iv = [0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new DesCipher(key), iv), new PKCS7Padding()), true);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new DesCipher(key), iv), new PKCS7Padding()), true);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("expectedEncrypted");
        output.WriteLine(expectedEncrypted.ToByteArrayLiteral());
        output.WriteLine("decrypted");
        output.WriteLine(decrypted.ToByteArrayLiteral());
        output.WriteLine("input");
        output.WriteLine(input.ToByteArrayLiteral());

        Assert.Equal(expectedEncrypted, encrypted);
        Assert.Equal(input, decrypted);
        }

    [Fact]
    public void DES_CBC_Decrypt()
        {
        byte[] expectedPlain = StringExtensions.UTF8Encoding.GetBytes("www.javaCODEgeeks.com!!!");

        byte[] key = [0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef];
        byte[] iv = [0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00];
        byte[] expectedEncrypted =
            [
                0x15, 0x43, 0x3e, 0x97, 0x65, 0x66, 0xea, 0x81,
                0x22, 0xab, 0xe3, 0x11, 0x0f, 0x7d, 0xcb, 0x78,
                0x56, 0x91, 0x22, 0x3d, 0xd6, 0xca, 0xe3, 0xbd,
                0xb9, 0x8a, 0x38, 0x0c, 0xea, 0x5a, 0x90, 0x06
            ];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new DesCipher(key), iv), new PKCS7Padding()), true);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new DesCipher(key), iv), new PKCS7Padding()), true);

        byte[] plain = decCipher.Decrypt(expectedEncrypted);
        byte[] encrypted = encCipher.Encrypt(plain);

        output.WriteLine("encrypted");
        output.WriteLine(encrypted.ToByteArrayLiteral());
        output.WriteLine("expectedEncrypted");
        output.WriteLine(expectedEncrypted.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("plain");
        output.WriteLine(plain.ToByteArrayLiteral());
        output.WriteLine("expectedPlain");
        output.WriteLine(expectedPlain.ToByteArrayLiteral());

        Assert.Equal(expectedPlain, plain);
        Assert.Equal(expectedEncrypted, encrypted);
        }

    [Theory]
    [InlineData("00000000000000000000000000000000", "00000000000000000000000000000000", "82001573A003FD3B7FD72FFB0EAF63AAC62F12DEB629DCA72785A66268EC758B1EDB36900560898178E0AD009ABF1F491330DC1C246E3D6CB264F6900271D59C")]
    [InlineData("0053A6F94C9FF24598EB3E91E4378ADD", "0D74DB42A91077DE45AC137AE148AF16", "2E1ED12A8551C05AF41FF39D8F9DF933122B5235D48FC2A6F20037E69BDBBCE805782EFC16C455A4B3FF06142317535EF876104C32445138CB26EBC2F88A684C")]
    [InlineData("0558ABFE51A4F74A9DF04396E93C8FE2", "167DE44BB21980E74EB51C83EA51B81F", "4F864BF3C96D0363B1903F0739189138F6ED2BC0AF583FEEA0CEA66BA7E06E63FB28BF8B3CA0031D24ABB511C57DD17BFC2861C32400072CB680DF2E58A5CECC")]
    [InlineData("0A5DB00356A9FC4FA2F5489BEE4194E7", "1F86ED54BB2289F057BE258CF35AC128", "82168AB0023B79AAF1E6B4D823855E14A7084378036A951B1CFEF35173875ED86CB66AB8410491A08582BE40080C3102193BA567F9E95D096C3CC60927DD7901")]
    [InlineData("0F62B5085BAE0154A7FA4DA0F34699EC", "288FF65DC42B92F960C72E95FC63CA31", "1CD8AEDDFE52E217E835D0B7E84E2922D04B1ADBCA53C4522B1AA604C42856A90AF83E2614BCE65C0AECABDD8975B55700D6A26D52FFF0888DA38F1DE20B77B7")]
    public void HC128Cipher(string hexKey, string hexIV, string hexOutput)
        {
        byte[] iv = hexIV.FromHexString();
        CipherVectorTests.ByteProvider(key => new HC128ByteProvider(key, iv), hexKey, hexOutput);
        }

    [Theory]
    [InlineData("00000000000000000000000000000000", "00000000000000000000000000000000", "5B078985D8F6F30D42C5C02FA6B6795153F06534801F89F24E74248B720B4818CD9227ECEBCF4DBF8DBF6977E4AE14FAE8504C7BC8A9F3EA6C0106F5327E6981")]
    [InlineData("09090909090909090909090909090909", "00000000000000000000000000000000", "F5C2926651AEED9AF1A9C2F04C03D0812145B56AEA46EB283A25A4C9E3D8BEB4821B418F06F2B9DCDF1A85AB8C02CD1462E1BBCAEC9AB0E99AA6AFF918BA627C")]
    [InlineData("87878787878787878787878787878787", "00000000000000000000000000000000", "CEC0C3852E3B98233EBCB975C10B11913C69F2275EB97A1402EDF16C6FBE19BE79D65360445BCB63676E6553B609A0650155C3B22DD1975AC0F3F65063A2E16E")]
    [InlineData("0053A6F94C9FF24598EB3E91E4378ADD3083D6297CCF2275C81B6EC11467BA0D", "0D74DB42A91077DE45AC137AE148AF167DE44BB21980E74EB51C83EA51B81F86", "23D9E70A45EB0127884D66D9F6F23C01D1F88AFD629270127247256C1FFF91E91A797BD98ADD23AE15BEE6EEA3CEFDBFA3ED6D22D9C4F459DB10C40CDF4F4DFF")]
    [InlineData("0558ABFE51A4F74A9DF04396E93C8FE23588DB2E81D4277ACD2073C6196CBF12", "167DE44BB21980E74EB51C83EA51B81F86ED54BB2289F057BE258CF35AC1288F", "C44B5262F2EAD9C018213127686DB742A72D3F2D61D18F0F4E7DE5B4F7ADABE07E0C82033B139F02BAACB4E2F2D0BE30110C3A8A2B621523756692877C905DD0")]
    [InlineData("0A5DB00356A9FC4FA2F5489BEE4194E73A8DE03386D92C7FD22578CB1E71C417", "1F86ED54BB2289F057BE258CF35AC1288FF65DC42B92F960C72E95FC63CA3198", "9D13AA06122F4F03AE60D507701F1ED063D7530FF35EE76CAEDCBFB01D8A239EFA4A44B272DE9B4092E2AD56E87C3A6089F5A074D1F6E5B8FC6FABEE0C936F06")]
    [InlineData("0F62B5085BAE0154A7FA4DA0F34699EC3F92E5388BDE3184D72A7DD02376C91C", "288FF65DC42B92F960C72E95FC63CA3198FF66CD349B0269D0379E056CD33AA1", "C8632038DA61679C4685288B37D3E2327BC2D28C266B041FE0CA0D3CFEED8FD5753259BAB757168F85EA96ADABD823CA4684E918423E091565713FEDDE2CCFE0")]
    public void HC256Cipher(string hexKey, string hexIV, string hexOutput)
        {
        byte[] iv = hexIV.FromHexString();
        CipherVectorTests.ByteProvider(key => new HC256ByteProvider(key, iv), hexKey, hexOutput);
        }

    [Theory]
    [InlineData("00112233445566778899AABBCCDDEEFF", "000102030405060708090a0b0c0d0e0f", "ed732271a7b39f475b4b2b6719f194bf")]
    [InlineData("00112233445566778899AABBCCDDEEFF", "f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff", "b8bc6ed5c899265d2bcfad1fc6d4287d")]
    public void Idea(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector((forEncryption, key) => new IdeaEngine(forEncryption, key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00000000", "f650e4c8e448e96d98db2fb4f5fad54f433f1afbedec154ad837048746ca4f9a5de3743e88381097f1d444eb823cedb66a83e1e04a5f6355c744243325890e2e7452e31957161df638a824f3002ed71329f5544951c08d83d78cb99ea0cc74f38f651659cbc8b7c2f5f71c6912ad6419e5792e1b860536b809b3ce98d45d6d81f3b2612917e38f8529cf72ce349947b0c998f9ffb5e13dae32ae2a2bf7cf814c8ebfa303cf22e0640b923200eca4d58aef53cec4d0f7b37d9c411a2affdf8a80b40e27bcb4d2f97644b89b08f37c71d51a70e7e90bdb9c3060dc5207b3c3f24bd7386806229749b54e232cd091dabc65a70e11018b87437e5781414fcdbc62e28107c9ff69d2e4ae3b18e752b143b6886f4e077295138769943c3c74afc17a970fd439636a529b0bd8c58a6aa8bcc22d2db35dfea7a2f4026cb167db538e1f4e7275e2771d3b8e97ecc5dc9115e3a5b90369661430ab93ecac9fe69d7bc7681160eda8da28833522d5295ebc5adb60e7f7e1cdd097166d14b67ec13a210f392564af0fef0d0286843aea3decb058bafbb8b0ccfcf2b5cc05e3a662d9814bc24c2364a1aa37c0ed052b36505c451e7ec85d2a542fe43d0fbb91c8d92560d4d5f812a0594b9e8a51dacd49ebdb1b0dcdc1cd57c7f7e63444517ded386f2f36fa86a6d1210133bc405db388d96cdb6dbe96fe29661c13edc0cbcb0eee4a70cc94aede11ed340606cf9f3a6ce38923d74f4ea37f63ff917bdec2d73f72d40e7e0e673d77d9a213add9228891b3db01a9bd7056a001e3d51f093dcc033ce35ad0d3b034105a8c6a123f57bd2e50247364944be89b1a3b21835c4d9f39e2d9d405ded8294d37e5bccaaeed35a124b56708a2bcb00960ba2a98121a4d8fae820bb3263f12595a196a1075890809e49421c171ec884d682514c8009bb0b84e7b03fb88f428e7cb789388b13bdd2dc1d5848f520a07c28cd168a3935872c9137d127dd430c613f1578c2f0d55f7d3f39f309bfb788406b13746c0a6f53718d59708607f0476904b6d04db4e13cd7411a7b510ce0ebfc7f7ccb83f957afdfef62dc35e45803ff1e5244112d96c02c9b944d5990dfbe7e265810d9c7e7e826dfa8966f1e0ab30bcc764eadebeaced35e5ee0c571a7de4f3a26af7f58f7badf6bc235d023e651ed3ff4eec46b0b6d2a93b51e75b41c97e315aeb61119a5a53245b7933f6d7b1cae8deba50fc8194afa92a6dc87c80064188bfcd8bace62e78ffa5685597ec0fb4415f7d08294766ad56764309c36f903dde9f394a0a283c18080c8e080c79ec79ae4c10cb9e15637cdd662f62d31911a4ca0cf15cf824cd3b708f991e16614cb6b9d7665de87abb7229ea81d5b2d75056e6cd21fe1e42d596da2655c2b9aa36b8f6fd4a6a158d1001913fd3af7d1fb80b5e435f90c107576554abda7a68710f82ac484fd7e1c7be95c85eaa94a302f44d3cfbda786b29081010b27582d53d1221e2a51c3d1e9150b059261dd0638e1a31860f0581f2864dff4cfc350451516dbd086f26bc5654c165dfa427a82427f5582e3014b8d2486dc79a17499a1d77458766bb541e04a7f73d3dff8ad5ec6bf4dbef7d9f36ec0ea31feb2e4f15cfcc5cd8c423fbd0ef3cc9eb244925ba5590c8a5f48ac433c5321c613b67b2479c3a22e21339cc10d210aa931dd7e2ef05ee06b82f2703a385cb2c5d67133c877eb7b41e3437f75afb43ae53c078f394d904811d96458908063a85e13222281956b1e531860f132e7b022f21182ca396f703ac46819e2e0d28fe523724d4dca0eabe6bc66699fdc6112fdd19c1e69c04d3658a4b55dd9931907d62f854b5224d678f2622ae0582eafed133e4a51d2184bd6dd6c1a513753f28ee63fb737b1a70a1660e8a8dfaa31be79937f7476978513c1764531ac6bf12c06908001cdb951a4b6a53d067fce512b2cfb69ddb477f740e006639ddf25acc8bfa2df1b20eaf64f2632c9783cdee63bfd4d80084cfe575f4e9e219b48fd06c48ddd87a36af9371865c4c9ce0199d867027d72cb7b77f84ef01da72f5972f040f7074df9afa29c921f94e75c08a3618c1ef9ad649a428c5b719378a30738ad97cd348858129a6239e3b0abbb8abc480fac4c2ecfcf20bd9d711f9e2a4ef71b5fe87c0be8b06b2aafef5a79c15db3b0aeb81654389a84a253b1d7a19047c797cdc78a2d20adf0356f55a713e730fa8fd8650d8959e234eb7546681dad1b22a142a6e858ef4bce668235b9d85a13f8574096ae7a949bea229322d0dd568385882846526403dae086dd1943ae1279bff9e7e4f041c3a4524484525e481d4cc5fe24124c0037464c0bf1bd69126ceb003275ead3ac5bde90826414ff3a30519add7b43abe2ce5d3d58841276197ca2070e5fbb9c7276df0b4308f751f37a97df6c9cd808cfe4cb3803d469303aee19096c0d5d42a4e823ad3f5f9cc3b4286619c9ca45e1c66c97340891aec4945bae606c798f04752649d6cce86fdfc80c6e402d6ec2f2b27c822821fe26ce092f57ea7de462f4d07497cae5a48755c721502dd6cbe7935836d80039ead7f709ab3a42f4c8652d632e39273e8fa38601da4f25a0cd6ef8102503f7d8854a0a19a30c4e88815715305efe29457c4c9252887d96fc1a71e3ce9f841632d0985ded21e796c6fb5ce5602614abfc3c7be2cb54fed6fa617a083c3142d8f6079e4ceceffc1471d0cb81bdc153e5fe36ef5bbd531161a165b10157aa114ed3f7579b3f7f395f1bc6172c7a86f875e0e6c51b3cdfec2af73c0e762824c2009c5a8774894d401258aba3ffbd32be0608c17eff021e2547e07cffad905340e15f3310c929d8d190886ba527ff943f672ef73fbf046d95ca5c54cd95b9d855e894bb5af29")]
    [InlineData("ffffffff", "de3b3f3c19e0629c1fc8b7836695d523e7804edd86ff7ce9b106f52caebae9d972f845d49ce17d7da44e49bae954aac0d0b1284b98a88eec1524fb6bc91a16b51192ac5334131446ac2442de9ff3d5867b9b9148881ee30a6e87dd88e5d1f7cd98db31ff36f70d9850cfefaef42abb00ecc39ed308bf4b8030cdc2b6b7e42f0e908030dd282f96edacc888b3a986e109c129998f89baa1b5da8970b07a6ab012f10264f23c315c9c8e0c164955c68517b6a4f982b2626db70787f869ac6d551be34931627c7058e965c502e18d2cd370e6db3b70d947d61aa9717cf8394f48c63c796f3a154950846badb28b70d982f29bc670254e3e5e0f8e36b0a5f6da0a046b235ed6a42988c012bde74d879fa8eb5d59f5f40ed5e76601c9847b3edb2690")]
    public void Isaac(string hexKey, string hexOutput) => CipherVectorTests.ByteProvider(key => new IsaacByteProvider(key), hexKey, hexOutput);

    [Theory]
    [InlineData(128, "000102030405060708090A0B0C0D0E0F", "101112131415161718191A1B1C1D1E1F", "81BF1C7D779BAC20E1C9EA39B4D2AD06")]
    [InlineData(128, "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F", "202122232425262728292A2B2C2D2E2F", "58EC3E091000158A1148F7166F334F14")]
    [InlineData(256, "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F", "202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F", "F66E3D570EC92135AEDAE323DCBD2A8CA03963EC206A0D5A88385C24617FD92C")]
    [InlineData(256, "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F", "404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F", "606990E9E6B7B67A4BD6D893D72268B78E02C83C3CD7E102FD2E74A8FDFE5DD9")]
    [InlineData(512, "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F", "404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F", "4A26E31B811C356AA61DD6CA0596231A67BA8354AA47F3A13E1DEEC320EB56B895D0F417175BAB662FD6F134BB15C86CCB906A26856EFEB7C5BC6472940DD9D9")]
    public void KalynaCipher(int blockSizeBits, string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new KalynaCipher(blockSizeBits, key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00000000000000000000000000000000", "00000000000000000000000000000000", "b1656851699e29fa24b70148503d2dfc")]
    [InlineData("ffffffffffffffffffffffffffffffff", "ffffffffffffffffffffffffffffffff", "2a78421b87c7d0924f26113f1d1349b2")]
    [InlineData("b1656851699e29fa24b70148503d2dfc", "2a78421b87c7d0924f26113f1d1349b2", "e2f687e07b75660ffc372233bc47532c")]
    public void NoekeonCipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new NoekeonCipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("0000000000000000", "0000000000000000", "ebb773f993278eff")]
    [InlineData("ffffffffffffffff", "ffffffffffffffff", "278b27e42e2f0d49")]
    [InlineData("3000000000000000", "1000000000000001", "30649edf9be7d2c2")]
    [InlineData("88bca90e90875a7f0f79c384627bafb2", "0000000000000000", "2269552ab0f85ca6")]
    public void RC2Cipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new RC2Cipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    // https://www.cosic.esat.kuleuven.be/nessie/testvectors/bc/rc5/Rc5-128-64.verified.test-vectors
    [InlineData("80000000000000000000000000000000", "0000000000000000", "8F681D7F285CDC2F")]
    [InlineData("40000000000000000000000000000000", "0000000000000000", "DC14832CF4FE61A8")]
    [InlineData("20000000000000000000000000000000", "0000000000000000", "2F2494A0D96958E7")]
    [InlineData("10000000000000000000000000000000", "0000000000000000", "410BCDD35DA0963F")]
    public void RC5Cipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new RC5Cipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00000000000000000000000000000000", "80000000000000000000000000000000", "f71f65e7b80c0c6966fee607984b5cdf")]
    [InlineData("000000000000000000000000000000008000000000000000", "00000000000000000000000000000000", "dd04c176440bbc6686c90aee775bd368")]
    [InlineData("000000000000000000000000000000000000001000000000", "00000000000000000000000000000000", "937fe02d20fcb72f0f57201012b88ba4")]
    [InlineData("1000000000000000000000000000000000000000000000000000000000000000", "00000000000000000000000000000000", "11395d4bfe4c8258979ee2bf2d24dff4")]
    [InlineData("0000000000000000000000000000000000080000000000000000000000000000", "00000000000000000000000000000000", "3d6f7e99f6512553bb983e8f75672b97")]
    public void RC6Cipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new RC6Cipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00000000000000000000000000000000", "000102030405060708090a0b0c0d0e0f", "5EBAC6E0054E166819AFF1CC6D346CDB")]
    [InlineData("000102030405060708090a0b0c0d0e0f", "00000000000000000000000000000000", "c11f22f20140505084483597e4370f43")]
    [InlineData("4706480851E61BE85D74BFB3FD956185", "83A2F8A288641FB9A4E9A5CC2F131C7D", "EE54D13EBCAE706D226BC3142CD40D4A")]
    [InlineData("28DBC3BC49FFD87DCFA509B11D422BE7", "B41E6BE2EBA84A148E2EED84593C5EC7", "9B9B7BFCD1813CB95D0B3618F40F5122")]
    [InlineData("0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E", "0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E", "8296F2F1B007AB9D533FDEE35A9AD850")]
    public void SEEDCipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new SEEDCipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00000000000000000000000000000000", "00000000000000000000000000000000", "3620b17ae6a993d09618b8768266bae9")]
    [InlineData("80000000000000000000000000000000", "00000000000000000000000000000000", "264E5481EFF42A4606ABDA06C0BFDA3D")]
    [InlineData("D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9", "D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9D9", "20EA07F19C8E93FDA30F6B822AD5D486")]
    [InlineData("000000000000000000000000000000000000000000008000", "00000000000000000000000000000000", "40520018C4AC2BBA285AEEB9BCB58755")]
    [InlineData("0000000000000000000000000000000000000000000000000000000000000000", "00000000000000000000000000000001", "AD86DE83231C3203A86AE33B721EAA9F")]
    [InlineData("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F", "3DA46FFA6F4D6F30CD258333E5A61369", "00112233445566778899AABBCCDDEEFF")]
    [InlineData("2BD6459F82C5B300952C49104881FF482BD6459F82C5B300952C49104881FF48", "677C8DFAA08071743FD2B415D1B28AF2", "EA024714AD5C4D84EA024714AD5C4D84")]
    [InlineData("000102030405060708090A0B0C0D0E0F1011121314151617", "4528CACCB954D450655E8CFD71CBFAC7", "00112233445566778899AABBCCDDEEFF")]
    [InlineData("2BD6459F82C5B300952C49104881FF482BD6459F82C5B300", "E0208BE278E21420C4B1B9747788A954", "EA024714AD5C4D84EA024714AD5C4D84")]
    [InlineData("000102030405060708090A0B0C0D0E0F", "33B3DC87EDDD9B0F6A1F407D14919365", "00112233445566778899AABBCCDDEEFF")]
    [InlineData("2BD6459F82C5B300952C49104881FF48", "BEB6C069393822D3BE73FF30525EC43E", "EA024714AD5C4D84EA024714AD5C4D84")]
    [InlineData("DEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDE", "DEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDE", "8ddc2ed210a96a8319980ef9fc179216")]
    [InlineData("DEDEDEDEDEDEDEDEDEDEDEDEDED0DEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDE", "DEDEDEDEDEDEDEDEDEDEDEDEDEDEDEDE", "5fc95467aaaaa090d6297cba1f5250d8")]
    [InlineData("A800000000000000000000000000000000000000000000000000000000000057", "00000000000000000000000000000000", "51c8da7243235b1b56101d795a67894c")]
    public void SerpentCipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new SerpentCipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("0000000000000000000000000000000000000000000000000000000000000000", "00000000000000000000000000000000", "8910494504181950f98dd998a82b6749")]
    [InlineData("00000000000000000000000000000000", "80000000000000000000000000000000", "10b5ffb720b8cb9002a1142b0ba2e94a")]
    [InlineData("00000000000000000000000000000000", "00000000008000000000000000000000", "4f057a42d8d5bd9746e434680ddcd5e5")]
    [InlineData("00000000000000000000000000000000", "00000000000000000000400000000000", "99407bf8582ef12550886ef5b6f169b9")]
    [InlineData("000000000000000000000000000000000000000000000000", "40000000000000000000000000000000", "d522a3b8d6d89d4d2a124fdd88f36896")]
    [InlineData("000000000000000000000000000000000000000000000000", "00000000000200000000000000000000", "189b8ec3470085b3da97e82ca8964e32")]
    [InlineData("000000000000000000000000000000000000000000000000", "00000000000000000000008000000000", "f77d868cf760b9143a89809510ccb099")]
    [InlineData("0000000000000000000000000000000000000000000000000000000000000000", "08000000000000000000000000000000", "d43b7b981b829342fce0e3ec6f5f4c82")]
    [InlineData("0000000000000000000000000000000000000000000000000000000000000000", "00000000000000000100000000000000", "0bf30e1a0c33ccf6d5293177886912a7")]
    [InlineData("0000000000000000000000000000000000000000000000000000000000000000", "00000000000000000000000000000001", "6a7f3b805d2ddcba49b89770ade5e507")]
    [InlineData("80000000000000000000000000000000", "00000000000000000000000000000000", "49afbfad9d5a34052cd8ffa5986bd2dd")]
    [InlineData("000000000000000000000000004000000000000000000000", "00000000000000000000000000000000", "ba8829b1de058c4b48615d851fc74f17")]
    [InlineData("0000000000000000000000000000000000000000000000000000000100000000", "00000000000000000000000000000000", "89f64377bf1e8a46c8247044e8056a98")]
    public void TnepresCipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new TnepresCipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00998877665544332211", "33221100ddccbbaa", "2587cae27a12d300")]
    public void SkipjackCipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new SkipjackCipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00000000000000000000000000000000", "0000000000000000", "41ea3a0a94baa940")]
    [InlineData("00000000000000000000000000000000", "0102030405060708", "6a2f9cf3fccf3c55")]
    [InlineData("0123456712345678234567893456789A", "0000000000000000", "34e943b0900f5dcb")]
    [InlineData("0123456712345678234567893456789A", "0102030405060708", "773dc179878a81c0")]
    public void TeaCipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new TeaCipher(key), hexKey, hexInput, hexOutput);

    [Theory]
    [InlineData("00000000000000000000000000000000", "0000000000000000", "dee9d4d8f7131ed9")]
    [InlineData("00000000000000000000000000000000", "0102030405060708", "065c1b8975c6a816")]
    [InlineData("0123456712345678234567893456789A", "0000000000000000", "1ff9a0261ac64264")]
    [InlineData("0123456712345678234567893456789A", "0102030405060708", "8c67155b2ef91ead")]
    public void XTeaCipher(string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new XTeaCipher(key), hexKey, hexInput, hexOutput);

    [Fact]
    public void Vmpc()
        {
        VmpcByteProvider byteProvider = new VmpcByteProvider("9661410AB797D8A9EB767C21172DF6C7".FromHexString(), "4B5C2F003E67F39557A8D26F3DA2B155".FromHexString());

        Assert.Equal((byte)0xA8, byteProvider.GetNext());
        Assert.Equal((byte)0x24, byteProvider.GetNext());
        Assert.Equal((byte)0x79, byteProvider.GetNext());
        Assert.Equal((byte)0xF5, byteProvider.GetNext());
        }

    [Fact]
    public void VmpcKsa3()
        {
        VmpcKsa3ByteProvider byteProvider = new VmpcKsa3ByteProvider("9661410AB797D8A9EB767C21172DF6C7".FromHexString(), "4B5C2F003E67F39557A8D26F3DA2B155".FromHexString());

        Assert.Equal((byte)0xB6, byteProvider.GetNext());
        Assert.Equal((byte)0xEB, byteProvider.GetNext());
        Assert.Equal((byte)0xAE, byteProvider.GetNext());
        Assert.Equal((byte)0xFE, byteProvider.GetNext());
        }

    [Fact]
    public void TripleDES_CBC()
        {
        byte[] input = [0x00, 0x00, 0x00, 0x1c, 0x0a, 0x05, 0x00, 0x00, 0x00, 0x0c, 0x73, 0x73, 0x68, 0x2d, 0x75, 0x73, 0x65, 0x72, 0x61, 0x75, 0x74, 0x68, 0x72, 0x4e, 0x06, 0x08, 0x28, 0x2d, 0xaa, 0xe2, 0xb3, 0xd9];
        byte[] key = [0x78, 0xf6, 0xc6, 0xbb, 0x57, 0x03, 0x69, 0xca, 0xba, 0x31, 0x18, 0x2f, 0x2f, 0x4c, 0x35, 0x34, 0x64, 0x06, 0x85, 0x30, 0xbe, 0x78, 0x60, 0xb3];
        byte[] iv = [0xc0, 0x75, 0xf2, 0x26, 0x0a, 0x2a, 0x42, 0x96];
        byte[] expected = [0x28, 0x77, 0x2f, 0x07, 0x3e, 0xc2, 0x27, 0xa6, 0xdb, 0x36, 0x4d, 0xc6, 0x7a, 0x26, 0x7a, 0x38, 0xe6, 0x54, 0x0b, 0xab, 0x07, 0x87, 0xf0, 0xa4, 0x73, 0x1f, 0xde, 0xe6, 0x81, 0x1d, 0x4b, 0x4b];

        ICipher encCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new TripleDesCipher(key), iv), null), false);
        ICipher decCipher = new PaddedBlockCipherCipher(new PaddedBlockCipher(new CBCCipherMode(new TripleDesCipher(key), iv), null), false);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(expected, encrypted);
        Assert.Equal(input, decrypted);
        }

    private static void ByteProvider(Func<byte[], ISampleProvider<byte>> providerFactory, string hexKey, string hexOutput)
        {
        byte[] key = hexKey.FromHexString();
        byte[] output = hexOutput.FromHexString();

        ISampleProvider<byte> provider = providerFactory.Invoke(key);

        Assert.Equal(output, EnumerableExtensions.Repeat(provider.GetNext, output.Length));
        }

    /*
    private void Vector(Func<byte[], IStreamCipher> streamCipherFactory, string hexKey, string hexInput, string hexOutput)
        {
        CipherVectorTests.Vector(key => new StreamCipher(streamCipherFactory.Invoke(key)), hexKey, hexInput, hexOutput);
        }
    */
    private static void Vector(Func<bool, byte[], IBlockEngine> engineFactory, string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new BlockEngineCipher(engineFactory.Invoke(true, key), engineFactory.Invoke(false, key)), hexKey, hexInput, hexOutput);

    private static void Vector(Func<byte[], IBlockCipher> blockCipherFactory, string hexKey, string hexInput, string hexOutput) => CipherVectorTests.Vector(key => new PaddedBlockCipherCipher(new PaddedBlockCipher(blockCipherFactory.Invoke(key), null), false), hexKey, hexInput, hexOutput);

    private static void Vector(Func<byte[], ICipher> cipherFactory, string hexKey, string hexInput, string hexOutput)
        {
        byte[] key = hexKey.FromHexString();
        byte[] input = hexInput.FromHexString();
        byte[] output = hexOutput.FromHexString();

        ICipher encCipher = cipherFactory.Invoke(key);
        ICipher decCipher = cipherFactory.Invoke(key);

        byte[] encrypted = encCipher.Encrypt(input);
        byte[] decrypted = decCipher.Decrypt(encrypted);

        Assert.Equal(output, encrypted);
        Assert.Equal(input, decrypted);
        }
    }
