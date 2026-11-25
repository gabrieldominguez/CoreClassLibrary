using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers;
using CoreClassLibrary.Mathematics.Cryptography.Ciphers.BlockCiphers.Paddings;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests.Cryptography;

/// <summary>
/// https://github.com/sshnet/SSH.NET/tree/master/src/Renci.SshNet/Security/Cryptography tests
/// </summary>
public class CipherPaddingTests(ITestOutputHelper output)
    {
    private void Padding(IPadding padding, int blockSize, byte[] input, byte[] expected)
        {
        byte[] padded = padding.Pad(blockSize, input);
        byte[] unpadded = padding.Unpad(padded);

        output.WriteLine("input");
        output.WriteLine(input.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("padded");
        output.WriteLine(padded.ToByteArrayLiteral());
        output.WriteLine("");
        output.WriteLine("unpadded");
        output.WriteLine(unpadded.ToByteArrayLiteral());
        output.WriteLine("");

        Assert.Equal(expected, padded);
        Assert.Equal(input, unpadded);
        }

    [Theory]
    [InlineData(
        8,
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 },
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x03, 0x03, 0x03 })]
    [InlineData(
        8,
        new byte[0],
        new byte[] { 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08 })]
    public void PKCS7Padding(int blockSize, byte[] input, byte[] expected)
        => this.Padding(new PKCS7Padding(), blockSize, input, expected);

    [Theory]
    [InlineData(
        8,
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 },
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00, 0x00, 0x00 })]
    [InlineData(
        8,
        new byte[0],
        new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
    public void ZerosPadding(int blockSize, byte[] input, byte[] expected)
        => this.Padding(new ZerosPadding(), blockSize, input, expected);

    [Theory]
    [InlineData(
        8,
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 },
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x00, 0x00, 0x03 })]
    [InlineData(
        8,
        new byte[0],
        new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08 })]
    public void ANSIX923Padding(int blockSize, byte[] input, byte[] expected)
        => this.Padding(new ANSIX923Padding(), blockSize, input, expected);

    [Theory]
    [InlineData(
        8,
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 },
        new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x0A, 0x0B, 0x03 })]
    [InlineData(
        8,
        new byte[0],
        new byte[] { 0x0A, 0x0B, 0x0C, 0x0A, 0x0B, 0x0C, 0x0A, 0x08 })]
    public void ISO10126Padding(int blockSize, byte[] input, byte[] expected)
        => this.Padding(new ISO10126Padding(new TestRandomNumberGenerator([0x0A, 0x0B, 0x0C])), blockSize, input, expected);

    [Fact]
    public void PKCS7Padding_BlockSizeAndInput_LessThanBlockSize()
        {
        byte[] input = [0x01, 0x02, 0x03, 0x04, 0x05];
        byte[] expected = [0x01, 0x02, 0x03, 0x04, 0x05, 0x03, 0x03, 0x03];

        PKCS7Padding padding = new PKCS7Padding();

        byte[] actual = padding.Pad(8, input);
        byte[] unpadded = padding.Unpad(actual);

        Assert.Equal(expected, actual);
        Assert.Equal(input, unpadded);
        }

    [Fact]
    public void PKCS7Padding_BlockSizeAndInput_MoreThanBlockSizeButNoMultipleOfBlockSize()
        {
        byte[] input = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09];
        byte[] expected = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07];

        PKCS7Padding padding = new PKCS7Padding();

        byte[] actual = padding.Pad(8, input);
        byte[] unpadded = padding.Unpad(actual);

        Assert.Equal(expected, actual);
        Assert.Equal(input, unpadded);
        }

    [Fact]
    public void PKCS7Padding_BlockSizeAndInputAndOffsetAndLength_LessThanBlockSize()
        {
        byte[] input = [0x0f, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06];
        byte[] expectedPadded = [0x01, 0x02, 0x03, 0x04, 0x05, 0x03, 0x03, 0x03];
        byte[] expectedUnpadded = [0x01, 0x02, 0x03, 0x04, 0x05];

        PKCS7Padding padding = new PKCS7Padding();

        byte[] padded = padding.Pad(8, input, 1, input.Length - 2);
        byte[] unpadded = padding.Unpad(padded);

        Assert.Equal(expectedPadded, padded);
        Assert.Equal(expectedUnpadded, unpadded);
        }

    [Fact]
    public void PKCS7Padding_BlockSizeAndInputAndOffsetAndLength_MoreThanBlockSizeButNoMultipleOfBlockSize()
        {
        byte[] input = [0x0f, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10];
        byte[] expectedPadded = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07];
        byte[] expectedUnpadded = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09];

        PKCS7Padding padding = new PKCS7Padding();

        byte[] padded = padding.Pad(8, input, 1, input.Length - 2);
        byte[] unpadded = padding.Unpad(padded);

        Assert.Equal(expectedPadded, padded);
        Assert.Equal(expectedUnpadded, unpadded);
        }
    }
