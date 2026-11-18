using System.Security.Claims;

namespace CoreClassLibrary.Objects;

public static class Security
    {
    public static ClaimsPrincipal CreatePrincipal(string username)
        => new ClaimsPrincipal(new ClaimsIdentity([ new Claim(ClaimTypes.Name, username) ]));
    }
