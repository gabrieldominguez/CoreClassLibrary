using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// Decodifica la entrada en formato de las URL
/// </summary>
public class UrlDecodePipe : IPipe<string> 
    {
    /// <summary>
    /// Si es true, decodea los '+' como espacio 
    /// </summary>
    public bool SpaceAsPlus { get; set; } = false;

    #region Constructors
    public UrlDecodePipe() { }

    public UrlDecodePipe(bool plusAsSpaceUnescape)
        {
        this.SpaceAsPlus = plusAsSpaceUnescape;
        }
    #endregion 

    public string Transform(string input)
        => input.DecodeUrl(this.SpaceAsPlus);
    }
