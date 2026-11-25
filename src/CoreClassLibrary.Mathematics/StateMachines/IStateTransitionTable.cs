
using CoreClassLibrary.Objects.Pooling;
using System.Globalization;

namespace CoreClassLibrary.Mathematics.StateMachines;

/// <summary>
/// State-transition table
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/State-transition_table"/>
public interface IStateTransitionTable
    {
    bool IsFinalState(int state);

    /// <summary>
    /// Emite una cadena en lenguaje DOT
    /// </summary>
    /// <see cref="https://www.graphviz.org/"/>
    /// <see cref="http://www.webgraphviz.com"/>
    /// <see cref="https://www.devtoolsdaily.com/graphviz/"/>
    /// <see cref="https://en.wikipedia.org/wiki/Graphviz"/>
    /// <see cref="https://en.wikipedia.org/wiki/DOT_%28graph_description_language%29"/>
    /// <see cref="https://www.graphviz.org/pdf/dotguide.pdf"/>
    /// <see cref="https://github.com/ggerganov/dot-to-ascii"/>
    /// <see cref="https://dot-to-ascii.ggerganov.com/"/>
    /// <see cref="https://tiarkrompf.github.io/notes/?/drawing-graphs/"/>
    void Write(TextWriter writer, IReadOnlyList<string>? stateLabels = null);
    }

public static class StateTransitionTableExtensions
    {
    public static string Write(this IStateTransitionTable table, IReadOnlyList<string>? stateLabels = null)
        { 
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);
        table.Write(outputWriter, stateLabels);
        return outputBuilder.ToString();
        }    
    }
