namespace CoreClassLibrary.Networking.UdpClientServer;

public static class SntpData
    {
    public const byte Length = 48; // SNTP Data Structure Length
    public static readonly DateTime BaseDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc); // January 1, 1900 12:00 AM
    
    public const byte OffsetReferenceIdentifier    = 12;
    public const byte OffsetReferenceTimestamp     = 16;
    public const byte OffsetOriginateTimestamp     = 24;
    public const byte OffsetReceiveTimestamp       = 32;
    public const byte OffsetTransmitTimestamp      = 40;
    }
