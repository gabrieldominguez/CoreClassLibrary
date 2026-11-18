using CoreClassLibrary.Objects.Pooling;
using System.Globalization;

namespace CoreClassLibrary.Objects.Graphs;

/// <summary>
/// Escribe el grafo en lenguaje DOT
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
public static class DotExtensions
    {
    public static string WriteGraph<T>(this Node<T> node, bool isDigraph = true)
        {
        using PooledStringBuilder pooledStringBuilder = new PooledStringBuilder();
        using StringWriter stringWriter = new StringWriter(pooledStringBuilder.StringBuilder, CultureInfo.InvariantCulture);
        node.WriteGraph(stringWriter, isDigraph);
        return pooledStringBuilder.StringBuilder.ToString();
        }

    public static void WriteGraph<T>(this Node<T> node, TextWriter writer, bool isDigraph = true)
        {
        writer.Write(isDigraph ? "digraph {" : "graph {");
        
        node.VisitGraph<(TextWriter Writer, bool IsDigraph), T>((writer, isDigraph),
            (node, context) =>
                {
                context.Writer.Write(node.Id);

                String? label = node.Value?.ToString();
                if (!String.IsNullOrWhiteSpace(label))
                    {
                    context.Writer.Write(" [label=\"");
                    context.Writer.Write(label);
                    context.Writer.Write("\"]; ");
                    }
                },
            (node, nextNode, context) =>
                {
                context.Writer.Write(node.Id);
                context.Writer.Write(context.IsDigraph ? "->" : "--");
                context.Writer.Write(nextNode.Id);
                context.Writer.Write("; ");
                }                        
            );

        writer.Write("}");
        }
    }
