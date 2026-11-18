using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// Establece secuencias de escape para un conjunto mínimo de metacaracteres (\, *, +, ?, |, {, [, (,), ^, $, ., # y espacio en blanco) reemplazándolos por sus códigos de escape.
/// </summary>
public class RegExEscapePipe : IPipe<string> 
    {
    public string Transform(string input) => Regex.Escape(input);
    }
