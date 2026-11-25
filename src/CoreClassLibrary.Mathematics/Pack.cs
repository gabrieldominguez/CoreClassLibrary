namespace CoreClassLibrary.Mathematics;

/// <summary>
/// Provides convenience methods for conversion to and from both Big Endian and Little Endian.
/// </summary>
public static class Pack
    {
    #region https://github.com/sshnet/SSH.NET/blob/master/src/Renci.SshNet/Common/Pack.cs
    /// <summary>
    /// Converts little endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Converted <see cref="ushort" />.</returns>
    public static ushort LittleEndianToUInt16(byte[] buffer)
        {
        ushort n = buffer[0];
        n |= (ushort)(buffer[1] << 8);
        return n;
        }

    /// <summary>
    /// Converts little endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The buffer offset.</param>
    /// <returns>Converted <see cref="uint" />.</returns>
    public static uint LittleEndianToUInt32(byte[] buffer, int offset)
        {
        uint n = buffer[offset];
        n |= (uint)buffer[offset + 1] << 8;
        n |= (uint)buffer[offset + 2] << 16;
        n |= (uint)buffer[offset + 3] << 24;
        return n;
        }

    /// <summary>
    /// Converts little endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Converted <see cref="uint" />.</returns>
    public static uint LittleEndianToUInt32(byte[] buffer)
        {
        uint n = buffer[0];
        n |= (uint)buffer[1] << 8;
        n |= (uint)buffer[2] << 16;
        n |= (uint)buffer[3] << 24;
        return n;
        }
    /*
    /// <summary>
    /// Converts little endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Converted <see cref="ulong" />.</returns>
    public static ulong LittleEndianToUInt64(byte[] buffer)
        {
        ulong n = buffer[0];
        n |= (ulong)buffer[1] << 8;
        n |= (ulong)buffer[2] << 16;
        n |= (ulong)buffer[3] << 24;
        n |= (ulong)buffer[4] << 32;
        n |= (ulong)buffer[5] << 40;
        n |= (ulong)buffer[6] << 48;
        n |= (ulong)buffer[7] << 56;
        return n;
        }
    */
    /// <summary>
    /// Populates buffer with little endian number representation.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    public static byte[] UInt16ToLittleEndian(ushort value)
        {
        byte[] buffer = new byte[2];
        UInt16ToLittleEndian(value, buffer);
        return buffer;
        }

    /// <summary>
    /// Populates buffer with little endian number representation.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The buffer.</param>
    public static void UInt16ToLittleEndian(ushort value, byte[] buffer)
        {
        buffer[0] = (byte)(value & 0x00FF);
        buffer[1] = (byte)((value & 0xFF00) >> 8);
        }

    /// <summary>
    /// Populates buffer with little endian number representation.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    public static byte[] UInt32ToLittleEndian(uint value)
        {
        byte[] buffer = new byte[4];
        UInt32ToLittleEndian(value, buffer);
        return buffer;
        }

    /// <summary>
    /// Populates buffer with little endian number representation.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The buffer.</param>
    public static void UInt32ToLittleEndian(uint value, byte[] buffer)
        {
        buffer[0] = (byte)(value & 0x000000FF);
        buffer[1] = (byte)((value & 0x0000FF00) >> 8);
        buffer[2] = (byte)((value & 0x00FF0000) >> 16);
        buffer[3] = (byte)((value & 0xFF000000) >> 24);
        }

    /// <summary>
    /// Populates buffer with little endian number representation.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The buffer offset.</param>
    public static void UInt32ToLittleEndian(uint value, byte[] buffer, int offset)
        {
        buffer[offset] = (byte)(value & 0x000000FF);
        buffer[offset + 1] = (byte)((value & 0x0000FF00) >> 8);
        buffer[offset + 2] = (byte)((value & 0x00FF0000) >> 16);
        buffer[offset + 3] = (byte)((value & 0xFF000000) >> 24);
        }

    /// <summary>
    /// Populates buffer with little endian number representation.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    public static byte[] UInt64ToLittleEndian(ulong value)
        {
        byte[] buffer = new byte[8];
        UInt64ToLittleEndian(value, buffer);
        return buffer;
        }

    /// <summary>
    /// Populates buffer with little endian number representation.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <param name="buffer">The buffer.</param>
    public static void UInt64ToLittleEndian(ulong value, byte[] buffer)
        {
        buffer[0] = (byte)(value & 0x00000000000000FF);
        buffer[1] = (byte)((value & 0x000000000000FF00) >> 8);
        buffer[2] = (byte)((value & 0x0000000000FF0000) >> 16);
        buffer[3] = (byte)((value & 0x00000000FF000000) >> 24);
        buffer[4] = (byte)((value & 0x000000FF00000000) >> 32);
        buffer[5] = (byte)((value & 0x0000FF0000000000) >> 40);
        buffer[6] = (byte)((value & 0x00FF000000000000) >> 48);
        buffer[7] = (byte)((value & 0xFF00000000000000) >> 56);
        }

    public static byte[] UInt16ToBigEndian(ushort value)
        {
        byte[] buffer = new byte[2];
        UInt16ToBigEndian(value, buffer);
        return buffer;
        }

    public static void UInt16ToBigEndian(ushort value, byte[] buffer)
        {
        buffer[0] = (byte)(value >> 8);
        buffer[1] = (byte)(value & 0x00FF);
        }

    public static void UInt16ToBigEndian(ushort value, byte[] buffer, int offset)
        {
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)(value & 0x00FF);
        }

    public static void UInt32ToBigEndian(uint value, byte[] buffer)
        {
        buffer[0] = (byte)((value & 0xFF000000) >> 24);
        buffer[1] = (byte)((value & 0x00FF0000) >> 16);
        buffer[2] = (byte)((value & 0x0000FF00) >> 8);
        buffer[3] = (byte)(value & 0x000000FF);
        }

    public static void UInt32ToBigEndian(uint value, byte[] buffer, int offset)
        {
        buffer[offset++] = (byte)((value & 0xFF000000) >> 24);
        buffer[offset++] = (byte)((value & 0x00FF0000) >> 16);
        buffer[offset++] = (byte)((value & 0x0000FF00) >> 8);
        buffer[offset] = (byte)(value & 0x000000FF);
        }

    public static byte[] UInt32ToBigEndian(uint value)
        {
        byte[] buffer = new byte[4];
        UInt32ToBigEndian(value, buffer);
        return buffer;
        }

    /// <summary>
    /// Returns the specified 64-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    public static byte[] UInt64ToBigEndian(ulong value) => new[]
            {
                (byte) ((value & 0xFF00000000000000) >> 56),
                (byte) ((value & 0x00FF000000000000) >> 48),
                (byte) ((value & 0x0000FF0000000000) >> 40),
                (byte) ((value & 0x000000FF00000000) >> 32),
                (byte) ((value & 0x00000000FF000000) >> 24),
                (byte) ((value & 0x0000000000FF0000) >> 16),
                (byte) ((value & 0x000000000000FF00) >> 8),
                (byte) (value & 0x00000000000000FF)
            };

    /// <summary>
    /// Converts big endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Converted <see cref="ushort" />.</returns>
    public static ushort BigEndianToUInt16(byte[] buffer) => (ushort) (buffer[0] << 8 | buffer[1]);

    /// <summary>
    /// Converts big endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="offset">The buffer offset.</param>
    /// <returns>Converted <see cref="uint" />.</returns>
    public static uint BigEndianToUInt32(byte[] buffer, int offset) => (uint) buffer[offset + 0] << 24 |
               (uint) buffer[offset + 1] << 16 |
               (uint) buffer[offset + 2] << 8 |
               buffer[offset + 3];

    /// <summary>
    /// Converts big endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Converted <see cref="uint" />.</returns>
    public static uint BigEndianToUInt32(byte[] buffer) => (uint) buffer[0] << 24 |
               (uint) buffer[1] << 16 |
               (uint) buffer[2] << 8 |
               buffer[3];

    /// <summary>
    /// Converts big endian bytes into number.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Converted <see cref="ulong" />.</returns>
    public static ulong BigEndianToUInt64(byte[] buffer) => (ulong) buffer[0] << 56 |
               (ulong) buffer[1] << 48 |
               (ulong) buffer[2] << 40 |
               (ulong) buffer[3] << 32 |
               (ulong) buffer[4] << 24 |
               (ulong) buffer[5] << 16 |
               (ulong) buffer[6] << 8 |
               buffer[7];
    #endregion

    #region https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/util/Pack.cs
    public static void UInt16_To_BE(ushort n, byte[] bs)
        {
        bs[0] = (byte)(n >> 8);
        bs[1] = (byte)n;
        }

    public static void UInt16_To_BE(ushort n, byte[] bs, int off)
        {
        bs[off] = (byte)(n >> 8);
        bs[off + 1] = (byte)n;
        }

    public static ushort BE_To_UInt16(byte[] bs)
        {
        uint n = (uint)bs[0] << 8
            | (uint)bs[1];
        return (ushort)n;
        }

    public static ushort BE_To_UInt16(byte[] bs, int off)
        {
        uint n = (uint)bs[off] << 8
            | (uint)bs[off + 1];
        return (ushort)n;
        }

    public static byte[] UInt32_To_BE(uint n)
        {
        byte[] bs = new byte[4];
        UInt32_To_BE(n, bs, 0);
        return bs;
        }

    public static void UInt32_To_BE(uint n, byte[] bs)
        {
        bs[0] = (byte)(n >> 24);
        bs[1] = (byte)(n >> 16);
        bs[2] = (byte)(n >> 8);
        bs[3] = (byte)n;
        }

    public static void UInt32_To_BE(uint n, byte[] bs, int off)
        {
        bs[off] = (byte)(n >> 24);
        bs[off + 1] = (byte)(n >> 16);
        bs[off + 2] = (byte)(n >> 8);
        bs[off + 3] = (byte)n;
        }

    public static byte[] UInt32_To_BE(uint[] ns)
        {
        byte[] bs = new byte[4 * ns.Length];
        UInt32_To_BE(ns, bs, 0);
        return bs;
        }

    public static void UInt32_To_BE(uint[] ns, byte[] bs, int off)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            UInt32_To_BE(ns[i], bs, off);
            off += 4;
            }
        }

    public static uint BE_To_UInt32(byte[] bs) => (uint) bs[0] << 24
            | (uint) bs[1] << 16
            | (uint) bs[2] << 8
            | (uint) bs[3];

    public static uint BE_To_UInt32(byte[] bs, int off) => (uint) bs[off] << 24
            | (uint) bs[off + 1] << 16
            | (uint) bs[off + 2] << 8
            | (uint) bs[off + 3];

    public static void BE_To_UInt32(byte[] bs, int off, uint[] ns)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            ns[i] = BE_To_UInt32(bs, off);
            off += 4;
            }
        }

    public static byte[] UInt64_To_BE(ulong n)
        {
        byte[] bs = new byte[8];
        UInt64_To_BE(n, bs, 0);
        return bs;
        }

    public static void UInt64_To_BE(ulong n, byte[] bs)
        {
        UInt32_To_BE((uint)(n >> 32), bs);
        UInt32_To_BE((uint)n, bs, 4);
        }

    public static void UInt64_To_BE(ulong n, byte[] bs, int off)
        {
        UInt32_To_BE((uint)(n >> 32), bs, off);
        UInt32_To_BE((uint)n, bs, off + 4);
        }

    public static byte[] UInt64_To_BE(ulong[] ns)
        {
        byte[] bs = new byte[8 * ns.Length];
        UInt64_To_BE(ns, bs, 0);
        return bs;
        }

    public static void UInt64_To_BE(ulong[] ns, byte[] bs, int off)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            UInt64_To_BE(ns[i], bs, off);
            off += 8;
            }
        }

    public static ulong BE_To_UInt64(byte[] bs)
        {
        uint hi = BE_To_UInt32(bs);
        uint lo = BE_To_UInt32(bs, 4);
        return ((ulong)hi << 32) | (ulong)lo;
        }

    public static ulong BE_To_UInt64(byte[] bs, int off)
        {
        uint hi = BE_To_UInt32(bs, off);
        uint lo = BE_To_UInt32(bs, off + 4);
        return ((ulong)hi << 32) | (ulong)lo;
        }

    public static void BE_To_UInt64(byte[] bs, int off, ulong[] ns)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            ns[i] = BE_To_UInt64(bs, off);
            off += 8;
            }
        }

    public static void UInt16_To_LE(ushort n, byte[] bs)
        {
        bs[0] = (byte)n;
        bs[1] = (byte)(n >> 8);
        }

    public static void UInt16_To_LE(ushort n, byte[] bs, int off)
        {
        bs[off] = (byte)n;
        bs[off + 1] = (byte)(n >> 8);
        }

    public static ushort LE_To_UInt16(byte[] bs)
        {
        uint n = (uint)bs[0]
            | (uint)bs[1] << 8;
        return (ushort)n;
        }

    public static ushort LE_To_UInt16(byte[] bs, int off)
        {
        uint n = (uint)bs[off]
            | (uint)bs[off + 1] << 8;
        return (ushort)n;
        }

    public static byte[] UInt32_To_LE(uint n)
        {
        byte[] bs = new byte[4];
        UInt32_To_LE(n, bs, 0);
        return bs;
        }

    public static void UInt32_To_LE(uint n, byte[] bs)
        {
        bs[0] = (byte)n;
        bs[1] = (byte)(n >> 8);
        bs[2] = (byte)(n >> 16);
        bs[3] = (byte)(n >> 24);
        }

    public static void UInt32_To_LE(uint n, byte[] bs, int off)
        {
        bs[off] = (byte)n;
        bs[off + 1] = (byte)(n >> 8);
        bs[off + 2] = (byte)(n >> 16);
        bs[off + 3] = (byte)(n >> 24);
        }

    public static byte[] UInt32_To_LE(uint[] ns)
        {
        byte[] bs = new byte[4 * ns.Length];
        UInt32_To_LE(ns, bs, 0);
        return bs;
        }

    public static void UInt32_To_LE(uint[] ns, byte[] bs, int off)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            UInt32_To_LE(ns[i], bs, off);
            off += 4;
            }
        }

    public static uint LE_To_UInt32(byte[] bs) => (uint) bs[0]
            | (uint) bs[1] << 8
            | (uint) bs[2] << 16
            | (uint) bs[3] << 24;

    public static uint LE_To_UInt32(byte[] bs, int off) => (uint) bs[off]
            | (uint) bs[off + 1] << 8
            | (uint) bs[off + 2] << 16
            | (uint) bs[off + 3] << 24;

    public static void LE_To_UInt32(byte[] bs, int off, uint[] ns)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            ns[i] = LE_To_UInt32(bs, off);
            off += 4;
            }
        }

    public static void LE_To_UInt32(byte[] bs, int bOff, uint[] ns, int nOff, int count)
        {
        for (int i = 0; i < count; ++i)
            {
            ns[nOff + i] = LE_To_UInt32(bs, bOff);
            bOff += 4;
            }
        }

    public static uint[] LE_To_UInt32(byte[] bs, int off, int count)
        {
        uint[] ns = new uint[count];
        for (int i = 0; i < ns.Length; ++i)
            {
            ns[i] = LE_To_UInt32(bs, off);
            off += 4;
            }
        return ns;
        }

    public static byte[] UInt64_To_LE(ulong n)
        {
        byte[] bs = new byte[8];
        UInt64_To_LE(n, bs, 0);
        return bs;
        }

    public static void UInt64_To_LE(ulong n, byte[] bs)
        {
        UInt32_To_LE((uint)n, bs);
        UInt32_To_LE((uint)(n >> 32), bs, 4);
        }

    public static void UInt64_To_LE(ulong n, byte[] bs, int off)
        {
        UInt32_To_LE((uint)n, bs, off);
        UInt32_To_LE((uint)(n >> 32), bs, off + 4);
        }

    public static byte[] UInt64_To_LE(ulong[] ns)
        {
        byte[] bs = new byte[8 * ns.Length];
        UInt64_To_LE(ns, bs, 0);
        return bs;
        }

    public static void UInt64_To_LE(ulong[] ns, byte[] bs, int off)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            UInt64_To_LE(ns[i], bs, off);
            off += 8;
            }
        }

    public static void UInt64_To_LE(ulong[] ns, int nsOff, int nsLen, byte[] bs, int bsOff)
        {
        for (int i = 0; i < nsLen; ++i)
            {
            UInt64_To_LE(ns[nsOff + i], bs, bsOff);
            bsOff += 8;
            }
        }

    public static ulong LE_To_UInt64(byte[] bs)
        {
        uint lo = LE_To_UInt32(bs);
        uint hi = LE_To_UInt32(bs, 4);
        return ((ulong)hi << 32) | (ulong)lo;
        }

    public static ulong LE_To_UInt64(byte[] bs, int off)
        {
        uint lo = LE_To_UInt32(bs, off);
        uint hi = LE_To_UInt32(bs, off + 4);
        return ((ulong)hi << 32) | (ulong)lo;
        }

    public static void LE_To_UInt64(byte[] bs, int off, ulong[] ns)
        {
        for (int i = 0; i < ns.Length; ++i)
            {
            ns[i] = LE_To_UInt64(bs, off);
            off += 8;
            }
        }

    public static void LE_To_UInt64(byte[] bs, int bsOff, ulong[] ns, int nsOff, int nsLen)
        {
        for (int i = 0; i < nsLen; ++i)
            {
            ns[nsOff + i] = LE_To_UInt64(bs, bsOff);
            bsOff += 8;
            }
        }
    #endregion
    }
