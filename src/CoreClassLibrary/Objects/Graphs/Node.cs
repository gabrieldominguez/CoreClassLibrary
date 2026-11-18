namespace CoreClassLibrary.Objects.Graphs;

/// <summary>
/// Graph node
/// </summary>
/// <see cref="https://mathworld.wolfram.com/topics/GraphTheory.html"/>
/// <see cref="https://en.wikipedia.org/wiki/Graph_theory"/>
/// <see cref="LinkedListNode&lt;T&gt;"/>
public record Node<T>(int Id, T Value)
    {
    public ICollection<Node<T>> NextNodes { get; init; } = [];
    }

/*
/// <summary>
/// Graph edge
/// </summary>
/// <see cref="https://mathworld.wolfram.com/GraphEdge.html"/>
public record Edge<TEdge, TNode>(int Id, Node<TNode> FromNode, Node<TNode> ToNode, TEdge Value);
*/