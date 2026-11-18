using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Filter;

/// <summary>
/// Filtra la entrada utilizando espresiones regulares.
/// </summary>
public class RegExFilterPipe(string filter, RegexOptions options) : FilterPipe(filter)
    {
    /// <summary>
    /// Combinación OR bit a bit de los valores de enumeración RegexOption.
    /// </summary>
    public RegexOptions Options { get; set; } = options;

    public RegExFilterPipe() : this(".*") { }
    public RegExFilterPipe(string filter) : this(filter, RegexOptions.Singleline) { }

    protected override bool ApplyFilter(string input)
        => Regex.IsMatch(input, this.filter, this.Options | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }
