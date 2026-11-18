using System.Net;

namespace CoreClassLibrary.Networking;

public record EndPointSettings(string AddressPort)
    {
    public IPEndPoint EndPoint { get; } = IPEndPoint.Parse(AddressPort);
    }
