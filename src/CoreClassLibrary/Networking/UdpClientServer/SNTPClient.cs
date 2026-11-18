
/*
 * The C# SNTP client used by Microsoft in .NET Micro Framework
 * 
 * Copyright (C)2001-2019 Valer BOCAN, PhD <valer@bocan.ro>
 * Last modified: August 3rd, 2019
 * Historically, this has been the very first piece of C# code I've written.
 * 
 */

using System.Net;
using System.Net.Sockets;
using System.Text;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.Objects.Refs;

namespace CoreClassLibrary.Networking.UdpClientServer;

// Leap indicator field values
public enum LeapIndicator
    {
    NoWarning,          // 0 - No warning
    LastMinute61,       // 1 - Last minute has 61 seconds
    LastMinute59,       // 2 - Last minute has 59 seconds
    Alarm               // 3 - Alarm condition (clock not synchronized)
    }

//Mode field values
public enum Mode
    {
    SymmetricActive,    // 1 - Symmetric active
    SymmetricPassive,   // 2 - Symmetric pasive
    Client,             // 3 - Client
    Server,             // 4 - Server
    Broadcast,          // 5 - Broadcast
    Unknown             // 0, 6, 7 - Reserved
    }

// Stratum field values
public enum Stratum
    {
    Unspecified,        // 0 - unspecified or unavailable
    PrimaryReference,   // 1 - primary reference (e.g. radio-clock)
    SecondaryReference, // 2-15 - secondary reference (via NTP or SNTP)
    Reserved            // 16-255 - reserved
    }

/// <summary>
/// A C# SNTP Client
/// </summary>
/// <remarks>
/// <![CDATA[
/// A C# SNTP Client
/// Copyright (C)2001-2003 Valer BOCAN <vbocan@dataman.ro>
/// 
/// SntpClient is a C# class designed to connect to time servers on the Internet and
/// fetch the current date and time. Optionally, it may update the time of the local system.
/// The implementation of the protocol is based on the RFC 2030.
/// 
/// SNTP
/// The Simple Network Time Protocol (SNTP) is a protocol for synchronizing the clocks of computer 
/// systems over packet-switched, variable-latency data networks. SNTP uses UDP port 123 as its 
/// transport layer. It is designed particularly to resist the effects of variable latency. SNTP 
/// uses Marzullo’s algorithm with the UTC time scale, including support for features such as leap 
/// seconds. SNTPv4 can usually maintain time to within 10 milliseconds (1/100 s) over the Internet, 
/// and can achieve accuracies of 200 microseconds (1/5000 s) or better in local area networks under 
/// ideal conditions.
/// 
/// SNTP is one of the oldest internet protocols still in use (since before 1985). 
/// SNTP was originally designed by Dave Mills of the University of Delaware, who still maintains it, 
/// along with a team of volunteers.
/// 
/// SNTP uses a hierarchical system of “clock strata”, where stratum 1 systems are synchronized to 
/// an accurate external clock such as a GPS clock or other radio clock. SNTP stratum 2 systems 
/// derive their time from one or more stratum 1 systems, and so on. 
/// (This is different from the notion of clock strata used in telecom systems)
/// 
/// The 64-bit timestamps used by SNTP consist of a 32-bit seconds part and a 32-bit fractional 
/// second part, giving SNTP a time scale of 2^32 seconds (136 years), with a theoretical resolution 
/// of 2^-32 seconds (0.233 nanoseconds). Although the SNTP timescale wraps round every 2^32 seconds, 
/// implementations should disambiguate SNTP time using a knowledge of the approximate time from 
/// other sources. Since this only requires time accurate to a few decades, this is not a problem 
/// in general use.
/// 
/// 
/// Public class members:
///
/// LeapIndicator - Warns of an impending leap second to be inserted/deleted in the last
/// minute of the current day. (See the _LeapIndicator enum)
/// 
/// VersionNumber - Version number of the protocol (3 or 4).
/// 
/// Mode - Returns mode. (See the _Mode enum)
/// 
/// Stratum - Stratum of the clock. (See the _Stratum enum)
/// 
/// PollInterval - Maximum interval between successive messages
/// 
/// Precision - Precision of the clock
/// 
/// RootDelay - Round trip time to the primary reference source.
/// 
/// RootDispersion - Nominal error relative to the primary reference source.
/// 
/// ReferenceID - Reference identifier (either a 4 character string or an IP address).
/// 
/// ReferenceTimestamp - The time at which the clock was last set or corrected.
/// 
/// OriginateTimestamp - The time at which the request departed the client for the server.
/// 
/// ReceiveTimestamp - The time at which the request arrived at the server.
/// 
/// Transmit Timestamp - The time at which the reply departed the server for client.
/// 
/// RoundTripDelay - The time between the departure of request and arrival of reply.
/// 
/// LocalClockOffset - The offset of the local clock relative to the primary reference source.
/// 
/// Initialize - Sets up data structure and prepares for connection.
/// 
/// Connect - Connects to the time server and populates the data structure.
///	It can also update the system time.
/// 
/// IsResponseValid - Returns true if received data is valid and if comes from
/// a NTP-compliant time server.
/// 
/// ToString - Returns a string representation of the object.
/// 
/// -----------------------------------------------------------------------------
/// Structure of the standard NTP header (as described in RFC 2030)
///                       1                   2                   3
///   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |LI | VN  |Mode |    Stratum    |     Poll      |   Precision   |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                          Root Delay                           |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                       Root Dispersion                         |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                     Reference Identifier                      |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                                                               |
///  |                   Reference Timestamp (64)                    |
///  |                                                               |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                                                               |
///  |                   Originate Timestamp (64)                    |
///  |                                                               |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                                                               |
///  |                    Receive Timestamp (64)                     |
///  |                                                               |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                                                               |
///  |                    Transmit Timestamp (64)                    |
///  |                                                               |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                 Key Identifier (optional) (32)                |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
///  |                                                               |
///  |                                                               |
///  |                 Message Digest (optional) (128)               |
///  |                                                               |
///  |                                                               |
///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
/// 
/// -----------------------------------------------------------------------------
/// 
/// SNTP Timestamp Format (as described in RFC 2030)
///                         1                   2                   3
///     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
/// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
/// |                           Seconds                             |
/// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
/// |                  Seconds Fraction (0-padded)                  |
/// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
/// ]]>
/// </remarks>
/// <see cref="https://tools.ietf.org/html/rfc2030"/>
/// <see cref="http://es.wikipedia.org/wiki/Network_Time_Protocol"/>
/// <see cref="https://github.com/vbocan/sntp-client/blob/master/SNTPClient.cs"/>
/// <see cref="SntpServer"/>
/// <param name="serverEndpoint">The IP of the time server we're connecting to</param>
public class SntpClient(TimeProvider timeProvider, IPEndPoint serverEndpoint)
    {
    private readonly RequestResponseUdpClient client = new RequestResponseUdpClient(serverEndpoint);

    public static async Task ValidateLocalTimeAsync(TimeProvider timeProvider, IEnumerable<IPAddress> addresses, int validOffset, CancellationToken cancellationToken = default)
        {
        using PooledMemory<byte> dataBuffer = new PooledMemory<byte>(SntpData.Length);

        foreach (IPAddress address in addresses)
            {
            try
                {
                SntpClient client = new SntpClient(timeProvider, address);
                (await client.GetDataAsync(dataBuffer.Memory, cancellationToken)).ValidateLocalTime(validOffset);

                return;
                }
            catch (SocketException) { }
            }

        throw new Exception("Time not validated. Any server responded the request.");
        }

    public SntpClient(TimeProvider timeProvider, IPAddress address): this(timeProvider, new IPEndPoint(address, 123)) { }

    /// <summary>
    /// Get SNTP Data Structure (as described in RFC 2030)
    /// </summary>
    public async Task<SntpClientData> GetDataAsync(Memory<byte> dataBuffer, CancellationToken cancellationToken = default)
        {
        using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(SntpData.Length);

        inputBuffer.Memory.Span.Clear();

        #region Initialize the data structure        
        inputBuffer.Memory.Span[0] = 0x1B;      // Set version number to 4 and Mode to 3 (client)

        #region Initialize the transmit timestamp
        ulong milliseconds = (ulong)(timeProvider.GetLocalNow().DateTime - SntpData.BaseDate).TotalMilliseconds;

        ulong temp = milliseconds / 1000; //intpart
        for (int i = 3; i >= 0; i--)
            {
            inputBuffer.Memory.Span[SntpData.OffsetTransmitTimestamp + i] = (byte)(temp % 256);
            temp /= 256;
            }

        temp = milliseconds % 1000 * 0x100000000L / 1000;  //fractpart
        for (int i = 7; i >= 4; i--)
            {
            inputBuffer.Memory.Span[SntpData.OffsetTransmitTimestamp + i] = (byte)(temp % 256);
            temp /= 256;
            }
        #endregion
        #endregion

        await this.client.SendReceiveAsync(inputBuffer.Memory, dataBuffer, cancellationToken);
        
        // Check if the response from server is valid
        return dataBuffer.Length < SntpData.Length || SntpClientData.GetMode(dataBuffer.Span) != Mode.Server
            ? throw new Exception($"Invalid response from {serverEndpoint.Address}")
            : new SntpClientData(dataBuffer);
        }
    }

public class SntpClientData(ReadOnlyMemory<byte> data)
    {
    private readonly DateTimeStamped<ReadOnlyMemory<byte>> stampedData = new DateTimeStamped<ReadOnlyMemory<byte>>(data);

    public LeapIndicator LeapIndicator { get; } = (byte) (data.Span[0] >> 6) switch
        {
        0 => LeapIndicator.NoWarning,
        1 => LeapIndicator.LastMinute61,
        2 => LeapIndicator.LastMinute59,
        _ => LeapIndicator.Alarm,
        };

    public static byte GetVersionNumber(ReadOnlySpan<byte> date)
        => (byte) ((date[0] & 0x38) >> 3);  // Isolate bits 3 - 5

    public byte VersionNumber { get; } = SntpClientData.GetVersionNumber(data.Span);

    public static Mode GetMode(ReadOnlySpan<byte> date)
        => (byte) (date[0] & 0x7) switch   // Isolate bits 0 - 3
        {
        1 => Mode.SymmetricActive,
        2 => Mode.SymmetricPassive,
        3 => Mode.Client,
        4 => Mode.Server,
        5 => Mode.Broadcast,
        _ => Mode.Unknown
        };

    public Mode Mode { get; } = SntpClientData.GetMode(data.Span);

    public static Stratum GeStratum(ReadOnlySpan<byte> date)
        => date[1] == 0
            ? Stratum.Unspecified
            : date[1] == 1
                ? Stratum.PrimaryReference
                : date[1] <= 15
                    ? Stratum.SecondaryReference
                    : Stratum.Reserved;

    public Stratum Stratum { get; } = SntpClientData.GeStratum(data.Span);

    public uint PollInterval { get; } = (uint) Math.Pow(2, (sbyte) data.Span[2]);

    public double Precision { get; } = Math.Pow(2, (sbyte) data.Span[3]);           // Precision (in seconds)

    public double RootDelay { get; } = 1000 * ((double) (256 * (256 * (256 * data.Span[4] + data.Span[5]) + data.Span[6]) + data.Span[7]) / 0x10000);           // Root Delay (in milliseconds)    

    public double RootDispersion { get; } = 1000 * ((double) (256 * (256 * (256 * data.Span[8] + data.Span[9]) + data.Span[10]) + data.Span[11]) / 0x10000);      // Root Dispersion (in milliseconds)    

    public string ReferenceIdentifier { get; } = SntpClientData.GeStratum(data.Span) switch
        {
        Stratum.Unspecified or Stratum.PrimaryReference => String.Concat(
            (char) data.Span[SntpData.OffsetReferenceIdentifier + 0],
            (char) data.Span[SntpData.OffsetReferenceIdentifier + 1],
            (char) data.Span[SntpData.OffsetReferenceIdentifier + 2],
            (char) data.Span[SntpData.OffsetReferenceIdentifier + 3]),
        Stratum.SecondaryReference => SntpClientData.GetVersionNumber(data.Span) switch
            {
            // Version 3, Reference ID is an IPv4 address
            3 => String.Concat(data.Span[SntpData.OffsetReferenceIdentifier + 0], ".",
                                data.Span[SntpData.OffsetReferenceIdentifier + 1], ".",
                                data.Span[SntpData.OffsetReferenceIdentifier + 2], ".",
                                data.Span[SntpData.OffsetReferenceIdentifier + 3]),
            // Version 4, Reference ID is the timestamp of last update
            4 => SntpClientData.ComputeDateWithTimeZone(data.Span, SntpData.OffsetReferenceIdentifier).ToString(),
            _ => "N/A",
            },
        _ => "",
        };

    // Reference Timestamp
    public DateTime ReferenceTimestamp { get; } = SntpClientData.ComputeDateWithTimeZone(data.Span, SntpData.OffsetReferenceTimestamp);

    // Originate Timestamp (T1)
    public DateTime OriginateTimestamp { get; } = SntpClientData.ComputeDate(data.Span, SntpData.OffsetOriginateTimestamp);

    // Receive Timestamp (T2)
    public DateTime ReceiveTimestamp { get; } = SntpClientData.ComputeDateWithTimeZone(data.Span, SntpData.OffsetReceiveTimestamp);

    // Transmit Timestamp (T3)
    public DateTime TransmitTimestamp { get; } = SntpClientData.ComputeDateWithTimeZone(data.Span, SntpData.OffsetTransmitTimestamp);

    // Destination Timestamp (T4)
    public DateTime DestinationTimestamp => this.stampedData.DateTimeStamp.DateTime;

    // Round trip delay (in milliseconds)
    public double RoundTripDelay => (this.DestinationTimestamp - this.OriginateTimestamp - (this.ReceiveTimestamp - this.TransmitTimestamp)).TotalMilliseconds;

    // Local clock offset (in milliseconds)
    public double LocalClockOffset => (this.ReceiveTimestamp - this.OriginateTimestamp + (this.TransmitTimestamp - this.DestinationTimestamp)).TotalMilliseconds / 2;

    // Take care of the time zone
    private static DateTime ComputeDateWithTimeZone(ReadOnlySpan<byte> date, byte timeStamp)
        => SntpClientData.ComputeDate(date, timeStamp) + TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

    private static DateTime ComputeDate(ReadOnlySpan<byte> date, byte timeStamp)
        => SntpClientData.ComputeDate(SntpClientData.GetMilliSeconds(date, timeStamp));

    // Compute date, given the number of milliseconds since January 1, 1900
    private static DateTime ComputeDate(ulong milliseconds)
        => new DateTime(1900, 1, 1) + TimeSpan.FromMilliseconds(milliseconds);

    // Compute the number of milliseconds, given the offset of a 8-byte array
    private static ulong GetMilliSeconds(ReadOnlySpan<byte> date, byte offset)
        {
        ulong intpart = 0;
        for (int i = 0; i <= 3; i++)
            {
            intpart = 256*intpart + date[offset + i];
            }

        ulong fractpart = 0;
        for (int i = 4; i <= 7; i++)
            {
            fractpart = 256*fractpart + date[offset + i];
            }

        return intpart*1000 + fractpart*1000/0x100000000L;
        }


    /// <summary>
    /// Valida que el reloj local se encuentre sincronizado con el del servidor pasado por parametro.
    /// </summary>
    /// <param name="validOffset">
    /// Ventana de tiempo dentro de la cual los relojes se consideran sincronizados, en milisegundos.
    /// </param>
    public void ValidateLocalTime(int validOffset)
        {
        if (!this.IsValidLocalTime(validOffset))
            {
            using PooledStringBuilder builder = new PooledStringBuilder();

            builder.StringBuilder
                .AppendLine("Local and server clocks are not synchronized.")
                .Append("Valid Offset (ms): ").AppendLine(validOffset)
                .AppendLine()
                .AppendData(this);

            throw new Exception(builder.StringBuilder.ToString());
            }
        }

    public bool IsValidLocalTime(int validOffset)
        => Math.Abs(this.LocalClockOffset) <= validOffset;

    public override string ToString() 
        => PooledStringBuilder.ToString(this.AppendTo);

    public void AppendTo(StringBuilder stringBuilder)
        => stringBuilder.AppendData(this);
    }

public static class StringBuilderExtensions
    {
    public static StringBuilder AppendData(this StringBuilder stringBuilder, SntpClientData data) 
        => stringBuilder
                .Append("Local time: ").AppendLine(data.TransmitTimestamp.ToString())
                .Append("Precision (s): ").AppendLine(data.Precision.ToString())
                .Append("Reference Identifier: ").AppendLine(data.ReferenceIdentifier.ToString())
                .Append("Round Trip Delay (ms): ").AppendLine(data.RoundTripDelay.ToString())
                .Append("Local Clock Offset (ms): ").AppendLine(data.LocalClockOffset.ToString())
                .Append("Leap Indicator: ").AppendLine(data.LeapIndicator.ToString())
                .Append("Version number: ").AppendLine(data.VersionNumber.ToString())
                .Append("Mode: ").AppendLine(data.Mode.ToString())
                .Append("Stratum: ").AppendLine(data.Stratum.ToString())
                .Append("Poll Interval (s): ").AppendLine(data.PollInterval.ToString())
                .Append("Root Delay (ms): ").AppendLine(data.RootDelay.ToString())
                .Append("Root Dispersion (ms): ").AppendLine(data.RootDispersion.ToString())
                .Append("Reference Timestamp: ").AppendLine(data.ReferenceTimestamp.ToISO8601String())
                .Append("Originate Timestamp (T1): ").AppendLine(data.OriginateTimestamp.ToISO8601String())
                .Append("Receive Timestamp (T2): ").AppendLine(data.ReceiveTimestamp.ToISO8601String())
                .Append("Transmit Timestamp (T3): ").AppendLine(data.TransmitTimestamp.ToISO8601String())
                .Append("Destination Timestamp (T4): ").AppendLine(data.DestinationTimestamp.ToISO8601String())
                .Append("Round trip delay (ms): ").AppendLine(data.RoundTripDelay.ToString())
                .Append("Local clock offset (ms): ").AppendLine(data.LocalClockOffset.ToString());
    }
