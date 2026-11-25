#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Randomness;
using System.Collections;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class BinaryTests
    {  
    [Fact]
    public void NativeSizedIntegers()
        {
        // when run in a 64-bit process
        Assert.Equal(4, sizeof(int));
        Assert.Equal(4, sizeof(uint));
        Assert.Equal(8, IntPtr.Size);
        Assert.Equal(8, UIntPtr.Size);
        Assert.Equal(8, sizeof(long));
        Assert.Equal(8, sizeof(ulong));
        }

    [Theory]
    [InlineData(0u, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
    [InlineData(1u, new byte[] { 0x00, 0x00, 0x00, 0x01 })]
    [InlineData(0x12345678u, new byte[] { 0x12, 0x34, 0x56, 0x78 })]
    [InlineData(0xFFFFFFFFu, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
    public void WriteInt_WritesBigEndianCorrectly(uint value, byte[] expected)
    {
        byte[] buffer = new byte[4];
        Binary.WriteBigEndianInt(value, buffer, 0);

        Assert.Equal(expected, buffer);
    }

    [Fact]
    public void WriteInt_WithOffset_WritesAtCorrectPosition()
    {
        byte[] buffer = new byte[10];
        Binary.WriteBigEndianInt(0xAABBCCDDu, buffer, 3);

        Assert.Equal(0xAA, buffer[3]);
        Assert.Equal(0xBB, buffer[4]);
        Assert.Equal(0xCC, buffer[5]);
        Assert.Equal(0xDD, buffer[6]);
    }

    [Theory]
    [InlineData( 8, "00001000")]
    [InlineData( 9, "00001001")]
    [InlineData(15, "00001111")]
    [InlineData(20, "00010100")]
    [InlineData(22, "00010110")]
    [InlineData(32, "00100000")]
    [InlineData(119,"01110111")]
    public void ByteToBitsHighFirst(byte n, string binString)
        {
        bool[] bin = binString.Select(b => b == '1').ToArray();

        Assert.Equal(binString, new String(n.ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));
        Assert.Equal(n, bin.FromBitsHighFirst().Single());

        BitArray bits = new BitArray(bin);
        for (int i = 0; i < binString.Length; i++)
            {
            Assert.Equal(binString[i] == '1', bits[i]);
            }
        }

    [Theory]
    [InlineData( 8, "00010000")]
    [InlineData( 9, "10010000")]
    [InlineData(15, "11110000")]
    [InlineData(20, "00101000")]
    [InlineData(22, "01101000")]
    [InlineData(32, "00000100")]
    [InlineData(119,"11101110")]
    public void ByteToBitsLowFirst(byte n, string binString)
        {
        Assert.Equal(binString, new String(n.ToBitsLowFirst().Select(b => b? '1': '0').ToArray()));
        Assert.Equal(n, binString.Select(b => b == '1').FromBitsLowFirst().Single());

        BitArray bits = new BitArray((byte[]) [ n ]);
        for (int i = 0; i < binString.Length; i++)
            {
            Assert.Equal(binString[i] == '1', bits[i]);
            }
        }

    [Theory]
    [InlineData(   8, "0000000000001000")]
    [InlineData(   9, "0000000000001001")]
    [InlineData(  15, "0000000000001111")]
    [InlineData(  20, "0000000000010100")]
    [InlineData(  22, "0000000000010110")]
    [InlineData(  32, "0000000000100000")]
    [InlineData( 119, "0000000001110111")]
    [InlineData( 529, "0000001000010001")]
    [InlineData(1041, "0000010000010001")]
    public void Int16ToBitsHighFirst(UInt16 n, string binString)
        {
        Assert.Equal(binString, new String(n.GetBytesHighFirst().ToBitsHighFirst().Select(b => b? '1': '0').ToArray()));
       
        Assert.Equal(BitConverter.GetBytes(n), binString.Select(b => b == '1').FromBitsHighFirst().Reverse());
        }

    [Theory]
    [InlineData(   8, "0001000000000000")]
    [InlineData(   9, "1001000000000000")]
    [InlineData(  15, "1111000000000000")]
    [InlineData(  20, "0010100000000000")]
    [InlineData(  22, "0110100000000000")]
    [InlineData(  32, "0000010000000000")]
    [InlineData( 119, "1110111000000000")]
    [InlineData( 529, "1000100001000000")]
    [InlineData(1041, "1000100000100000")]
    public void Int16ToBitsLowFirst(UInt16 n, string binString)
        {
        Assert.Equal(binString, new String(n.GetBytesLowFirst().ToBitsLowFirst().Select(b => b? '1': '0').ToArray()));
 
        byte[] bytes = binString.Select(b => b == '1').FromBitsLowFirst().ToArray();
        Assert.Equal(n, BitConverter.ToUInt16(bytes));
        Assert.Equal(BitConverter.GetBytes(n), bytes);
        }

    [Fact]
    public void ByteToBits()
        {
        IRandom random = new RandomWrapper();
        byte n = random.Next<byte>();

        Assert.Equal(n, n.ToBitsLowFirst().FromBitsLowFirst().Single());
        Assert.Equal(n, n.ToBitsHighFirst().FromBitsHighFirst().Single());
        }

    [Fact]
    public void UInt16ToBits()
        {
        IRandom random = new RandomWrapper();
        UInt16 n = BitConverter.ToUInt16([random.Next<byte>(), random.Next<byte>()]);

        Assert.Equal(n, BitConverter.ToUInt16(n.GetBytesLowFirst().ToBitsLowFirst().FromBitsLowFirst().ToArray()));
        Assert.Equal(n, BitConverter.ToUInt16(n.GetBytesHighFirst().ToBitsHighFirst().FromBitsHighFirst().Reverse().ToArray()));
        }

    [Fact]
    public void UInt32ToBits()
        {
        IRandom random = new RandomWrapper();
        uint n = random.Next<uint>();

        Assert.Equal(n, BitConverter.ToUInt32(n.GetBytesLowFirst().ToBitsLowFirst().FromBitsLowFirst().ToArray()));
        Assert.Equal(n, BitConverter.ToUInt32(n.GetBytesHighFirst().ToBitsHighFirst().FromBitsHighFirst().Reverse().ToArray()));
        }

    [Fact]
    public void UInt64ToBits()
        {
        IRandom random = new RandomWrapper();
        ulong n = random.Next<ulong>();

        Assert.Equal(n, BitConverter.ToUInt64(n.GetBytesLowFirst().ToBitsLowFirst().FromBitsLowFirst().ToArray()));
        Assert.Equal(n, BitConverter.ToUInt64(n.GetBytesHighFirst().ToBitsHighFirst().FromBitsHighFirst().Reverse().ToArray()));
        }

    [Fact]
    public void RotateLeft()
        {
        long n = 7;
        long n2 = n.RotateLeft(1);

        Assert.Equal(14L, n2);
        Assert.Equal(7L, n);
        }

    [Fact]
    public void RotateRight()
        {
        long n2 = 14;
        long n = n2.RotateRight(1);

        Assert.Equal(14L, n2);
        Assert.Equal(7L, n);
        }
    }
