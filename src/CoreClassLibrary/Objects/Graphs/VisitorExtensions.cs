namespace CoreClassLibrary.Objects.Graphs;

/// <summary>
/// Visitor pattern extensions
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Visitor_pattern"/>
/// <see cref="https://refactoring.guru/design-patterns/visitor"/>
/// <see cref="https://ruslanspivak.com/lsbasi-part7/"/>
/// <see cref="https://www.guru99.com/tree-traversals-inorder-preorder-and-postorder.html"/>
public static class VisitorExtensions
    {
    public static IEnumerable<Node<T>> LevelOrderEnumerateTree<T>(this Node<T> root)
        {
        Queue<Node<T>> queue = new Queue<Node<T>>();
        queue.Enqueue(root);

        while (queue.Count > 0) 
            {
            Node<T> current = queue.Dequeue();

            yield return current;

            foreach (Node<T> child in current.NextNodes)
                {
                queue.Enqueue(child);
                }
            }
        }

    public static IEnumerable<Node<T>> PreOrderEnumerateTree<T>(this Node<T> node)
        {
        yield return node;

        foreach (Node<T> child in node.NextNodes)
            {
            foreach (Node<T> descendant in child.PreOrderEnumerateTree())
                {
                yield return descendant;
                }            
            }
        }


    public static IEnumerable<Node<T>> PostOrderEnumerateTree<T>(this Node<T> node)
        {
        foreach (Node<T> child in node.NextNodes)
            {
            foreach (Node<T> descendant in child.PostOrderEnumerateTree())
                {
                yield return descendant;
                }            
            }

        yield return node;
        }

    public static IEnumerable<BinaryNode<T>> InOrderEnumerateTree<T>(this BinaryNode<T> node)
        {
        foreach (BinaryNode<T> descendant in node.Left?.InOrderEnumerateTree() ?? [])
            {
            yield return descendant;
            }            

        yield return node;

        foreach (BinaryNode<T> descendant in node.Right?.InOrderEnumerateTree() ?? [])
            {
            yield return descendant;
            }            
        }

    public static void VisitBinaryTree<T>(this BinaryNode<T> node, Action<BinaryNode<T>>? preOrderAction = null, Action<BinaryNode<T>>? inOrderAction = null, Action<BinaryNode<T>>? postOrderAction = null)
        {
        preOrderAction?.Invoke(node);
        node.Left?.VisitBinaryTree(preOrderAction, inOrderAction, postOrderAction);
        inOrderAction?.Invoke(node);
        node.Right?.VisitBinaryTree(preOrderAction, inOrderAction, postOrderAction);
        postOrderAction?.Invoke(node);
        }

    public static IEnumerable<Node<T>> PreOrderEnumerateGraph<T>(this Node<T> node)
        => node.PreOrderEnumerateNode([]);        

    private static IEnumerable<Node<T>> PreOrderEnumerateNode<T>(this Node<T> node, HashSet<int> visitedNodes)
        {
        yield return node;

        visitedNodes.Add(node.Id);

        foreach (Node<T> next in node.NextNodes)
            {
            if (!visitedNodes.Contains(next.Id))
                {
                foreach (Node<T> other in next.PreOrderEnumerateNode(visitedNodes))
                    {
                    yield return other;
                    }            
                }
            }
        }

    public static IEnumerable<Node<T>> PostOrderEnumerateGraph<T>(this Node<T> node)
        => node.PostOrderEnumerateGraph([]);        

    private static IEnumerable<Node<T>> PostOrderEnumerateGraph<T>(this Node<T> node, HashSet<int> visitedNodes)
        {
        visitedNodes.Add(node.Id);

        foreach (Node<T> next in node.NextNodes)
            {
            if (!visitedNodes.Contains(next.Id))
                {
                foreach (Node<T> other in next.PostOrderEnumerateGraph(visitedNodes))
                    {
                    yield return other;
                    }            
                }
            }

        yield return node;
        }

    public static void VisitGraph<T>(this Node<T> node, Action<Node<T>>? nodeAction = null, Action<Node<T>, Node<T>>? edgeAction = null)
        => node.VisitNode(nodeAction, edgeAction, []);        

    private static void VisitNode<T>(this Node<T> node, Action<Node<T>>? nodeAction, Action<Node<T>, Node<T>>? edgeAction, HashSet<int> visitedNodes)
        {
        visitedNodes.Add(node.Id);

        nodeAction?.Invoke(node);

        foreach (Node<T> nextNode in node.NextNodes)
            {
            edgeAction?.Invoke(node, nextNode);

            if (!visitedNodes.Contains(nextNode.Id))
                {
                nextNode.VisitNode(nodeAction, edgeAction, visitedNodes);
                }
            }
        }
    
    public static void VisitGraph<TContext, TNode>(this Node<TNode> node, TContext context, Action<Node<TNode>, TContext>? nodeAction = null, Action<Node<TNode>, Node<TNode>, TContext>? edgeAction = null)
        => node.VisitNode<TContext, TNode>(context, nodeAction, edgeAction, []);        

    private static void VisitNode<TContext, TNode>(this Node<TNode> node, TContext context, Action<Node<TNode>, TContext>? nodeAction, Action<Node<TNode>, Node<TNode>, TContext>? edgeAction, HashSet<int> visitedNodes)
        {
        visitedNodes.Add(node.Id);

        nodeAction?.Invoke(node, context);

        foreach (Node<TNode> nextNode in node.NextNodes)
            {
            edgeAction?.Invoke(node, nextNode, context);

            if (!visitedNodes.Contains(nextNode.Id))
                {
                nextNode.VisitNode<TContext, TNode>(context, nodeAction, edgeAction, visitedNodes);
                }
            }
        }   
    }
