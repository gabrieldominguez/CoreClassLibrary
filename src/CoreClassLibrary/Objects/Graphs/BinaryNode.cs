namespace CoreClassLibrary.Objects.Graphs;

public record BinaryNode<T>(int Id, T Value)
    {
    public BinaryNode<T>? Left { get; set; }
    public BinaryNode<T>? Right { get; set; }

    public Node<T> ToNode()
        {
        Node<T> node = new Node<T>(this.Id, this.Value);

        if (this.Left != null)
            {
            node.NextNodes.Add(this.Left.ToNode());
            }

        if (this.Right != null) 
            {
            node.NextNodes.Add(this.Right.ToNode());
            }

        return node;
        }
    }