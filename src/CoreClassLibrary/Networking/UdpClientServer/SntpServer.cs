using System.Buffers.Binary;

namespace CoreClassLibrary.Networking.UdpClientServer;

/// <summary>
/// SntpServer
/// </summary>
/// <see cref="https://github.com/schgab/OpenSntpServer/tree/main"/>
/// <see cref="SntpClient"/>
public class SntpServer(TimeProvider timeProvider, int port = 123)
    {
    private readonly RequestResponseUdpServer server = new RequestResponseUdpServer(port);

    public ValueTask ListenAsync(CancellationToken cancellationToken)
        => server.ListenAsync((input, output, cancellationToken) 
            =>  { 
                byte[] bytes = new SntpServerData(timeProvider, input).GetBytes();
                bytes.CopyTo(output);
                return ValueTask.FromResult(bytes.Length);
                },
            cancellationToken);
    }

public class SntpServerData(TimeProvider timeProvider)
    {
    private readonly byte[] bytes = new byte[SntpData.Length];

    public SntpServerData(TimeProvider timeProvider, ReadOnlyMemory<byte> data): this(timeProvider)
        {
        this.bytes[0] = (byte)(data.Span[0] & 0b00111000 | 0b00000100);     // Copy version from client and set mode 4 = server
        this.bytes[1] = 1; // Stratum 
        this.bytes[2] = data.Span[2]; // Copy poll
        this.bytes[3] = unchecked((byte)(-6)); // Precision
            
        // Skip 8 Bytes Root Delay Root Dispersion

        // LOCL as reference
        this.bytes[SntpData.OffsetReferenceIdentifier] = 0x4C;
        this.bytes[SntpData.OffsetReferenceIdentifier + 1] = 0x4F;
        this.bytes[SntpData.OffsetReferenceIdentifier + 2] = 0x43;
        this.bytes[SntpData.OffsetReferenceIdentifier + 3] = 0x4C;
            
        (uint sec, uint frac) arrival = this.GetTimeStamp();

        BinaryPrimitives.WriteUInt32BigEndian(this.bytes.AsSpan(SntpData.OffsetReferenceTimestamp), arrival.sec - 60); // Fake reference

        BinaryPrimitives.WriteUInt32BigEndian(this.bytes.AsSpan(SntpData.OffsetReceiveTimestamp), arrival.sec);
        BinaryPrimitives.WriteUInt32BigEndian(this.bytes.AsSpan(SntpData.OffsetReceiveTimestamp + 4), arrival.frac);

        // Copy client transmit
        data.Span[SntpData.OffsetTransmitTimestamp..].CopyTo(this.bytes.AsSpan(SntpData.OffsetOriginateTimestamp, 8));
        }

    public DateTime GetUTCTime()
        {
        ulong seconds = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(SntpData.OffsetTransmitTimestamp, 4));
        ulong fraction = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(SntpData.OffsetTransmitTimestamp + 4, 4));
        
        return SntpData.BaseDate
            .AddMilliseconds(seconds * 1000)
            .AddMilliseconds((fraction * 1000.0) / (1UL << 32));
        }

    public byte[] GetBytes()        
        {
        (uint sec, uint frac) transmitTime = this.GetTimeStamp();

        BinaryPrimitives.WriteUInt32BigEndian(this.bytes.AsSpan(SntpData.OffsetTransmitTimestamp), transmitTime.sec);
        BinaryPrimitives.WriteUInt32BigEndian(this.bytes.AsSpan(SntpData.OffsetTransmitTimestamp + 4), transmitTime.frac);

        return this.bytes;
        }

    private (uint sec, uint frac) GetTimeStamp()
        {
        double seconds = timeProvider.GetUtcNow().DateTime.Subtract(SntpData.BaseDate).TotalSeconds;
        uint secs = (uint)Math.Floor(seconds);
        uint fractions = (uint)((seconds - secs) * (1UL << 32));
        return (secs, fractions);
        }
    }
