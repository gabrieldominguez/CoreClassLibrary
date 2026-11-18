using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// Codifica la entrada al formato de las URL
/// </summary>
public class UrlEncodePipe : IPipe<string> 
    {
    /// <summary>
    /// Si es true, encodea los espacios con '+'
    /// </summary>
    public bool SpaceAsPlus { get; set; } = false;

    #region Constructors
    public UrlEncodePipe() { }

    public UrlEncodePipe(bool spaceAsPlusEscape)
        {
        this.SpaceAsPlus = spaceAsPlusEscape;
        }
    #endregion 

    public string Transform(string input) 
        => input.EncodeUrl(this.SpaceAsPlus);
    }
