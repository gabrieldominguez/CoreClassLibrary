using CoreClassLibrary.Objects.Graphs;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public static class OutputGraphExtensions
    {
    public static Task WriteGraphAsync<T>(this ITestOutputHelper output, Node<T> node, bool isDigraph = true)
        => output.WriteDotAsync(node.WriteGraph(isDigraph));
    }

public class GraphTests(ITestOutputHelper output)
    {     
    [Fact]
    public async Task Node()
        {
        Node<string> node1 = new Node<string>(1, "Node 1");
        await output.WriteGraphAsync(node1);
        }    
    
    [Fact]
    public async Task MinimalGraph()
        {
        //Node<string> node1 = new Node<string>(1, "Node 1", [ new Node<string>(2, "Node 2", []) ]);
        Node<string> node1 = new Node<string>(1, "Node 1");
        node1.NextNodes.Add(new Node<string>(2, "Node 2"));
        await output.WriteGraphAsync(node1, false);
        }
    
    [Fact]
    public async Task MinimalDigraph()
        {
        Node<string> node1 = new Node<string>(1, "Node 1");
        node1.NextNodes.Add(new Node<string>(2, "Node 2"));
        await output.WriteGraphAsync(node1);
        }
    
    [Fact]
    public async Task MinimalCyclicDigraph()
        {
        Node<string> node1 = new Node<string>(1, "Node 1");
        Node<string> node2 = new Node<string>(2, "Node 2");
        node1.NextNodes.Add(node2);
        node2.NextNodes.Add(node1);
        await output.WriteGraphAsync(node1);
        }  
    
    [Fact]
    public async Task CyclicDigraph()
        {
        Node<string> node1 = new Node<string>(1, "Node 1");
        Node<string> node2 = new Node<string>(2, "Node 2");
        Node<string> node3 = new Node<string>(3, "Node 3");
        node1.NextNodes.Add(node2);
        node2.NextNodes.Add(node3);
        node3.NextNodes.Add(node1);
        await output.WriteGraphAsync(node1);
        } 

    [Fact]
    public async Task AcyclicDigraph()
        {
        Node<string> node1 = new Node<string>(1, "Node 1");
        Node<string> node2 = new Node<string>(2, "Node 2");
        Node<string> node3 = new Node<string>(3, "Node 3");
        Node<string> node4 = new Node<string>(4, "Node 4");
        node1.NextNodes.Add(node2);
        node1.NextNodes.Add(node3);
        node2.NextNodes.Add(node4);
        node3.NextNodes.Add(node4);
        await output.WriteGraphAsync(node1);
        } 
    
    [Fact]
    public async Task MinimalTree()
        {
        Node<string> root = new Node<string>(0, "Root") 
            { 
            NextNodes = 
                [ 
                new Node<string>(1, "Node 1"),
                new Node<string>(2, "Node 2"),
                new Node<string>(3, "Node 3")
                ]};
        await output.WriteGraphAsync(root);
        }
    }
