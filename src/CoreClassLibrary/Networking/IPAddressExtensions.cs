using System.Net;

namespace CoreClassLibrary.Networking;

public static class IPAddressExtensions
    {
    /// <summary>
    /// An extension method to determine if an IP address is internal, as specified in RFC1918
    /// </summary>
    /// <param name="address">The IP address that will be tested</param>
    /// <returns>Returns true if the IP is internal, false if it is external</returns>
    public static bool IsInternal(this IPAddress address)
        => IPAddressExtensions.IsInternalAddress(address.GetAddressBytes());

    public static bool IsInternalAddress(ReadOnlySpan<byte> bytes) 
        => bytes[0] switch
            {
            10 => true,
            172 => bytes[1] < 32 && bytes[1] >= 16,
            192 => bytes[1] == 168,
            _ => false,
            };
    }
