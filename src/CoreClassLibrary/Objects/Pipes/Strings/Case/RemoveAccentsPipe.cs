using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Elimina de la entrada los acentos, diéresis y ñ (reemplazándolos por sus caracteres equivalentes).
/// </summary>
public class RemoveAccentsPipe : IPipe<string>
    {
    public string Transform(string input) => input.RemoveAccents();
    }
