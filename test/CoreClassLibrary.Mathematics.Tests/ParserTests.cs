#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.Expressions.Arithmetic;
using CoreClassLibrary.Objects.Graphs;
using CoreClassLibrary.RegularExpressions.Lexing;
using CoreClassLibrary.Tests;
using System.Linq.Expressions;
using System.Text.Json;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

/// <summary>
/// Math Expression Parser tests
/// </summary>
/// <see cref="https://dev.to/j0nimost/implementing-a-math-interpreter-using-c-part2-lexer-4i81"/>
/// <see cref="https://ruslanspivak.com/lsbasi-part7"/>
public class ParserTests(ITestOutputHelper output)
    {
    public class OuputExpressionVisitor(ITestOutputHelper output): ExpressionVisitor
        {
        public override Expression? Visit(Expression? node)
            {
            output.WriteLine(node?.ToString() ?? "<null>");
            return base.Visit(node);
            }
        }

    [Fact]
    // f(x) = ((3 * (x - 1)) / 2) + 4
    public void TestExpression()
        {
        ParameterExpression x = Expression.Parameter(typeof(double), "x");

        LambdaExpression expression = Expression.Lambda(
            Expression.Add(
                Expression.Divide(
                Expression.Multiply(
                    Expression.Constant(3D), 
                        Expression.Subtract(
                            x, 
                            Expression.Constant(1D))),
                Expression.Constant(2D)),
            Expression.Constant(4D)),
        x);

        OuputExpressionVisitor visitor = new OuputExpressionVisitor(output);
        visitor.Visit(expression);

        Func<double, double> f = (Func<double, double>) expression.Compile();

        Assert.Equal(5.5D, f(2D));
        Assert.Equal(7D, f(3D));
        }

    [Theory]
    [InlineData("1")]
    [InlineData("1)")]
    [InlineData("-1")]
    [InlineData("x")]
    [InlineData("1 + 2")]
    [InlineData("1+2")]
    [InlineData("2 - 1")]
    [InlineData("2 * 1")]
    [InlineData("2 / 1")]
    [InlineData("1 / 2")]
    [InlineData("-1 + 2")]
    [InlineData("1 + 2 + 3")]
    [InlineData("1 * 2 * 3")]
    [InlineData("1 + 2 - 3")]
    [InlineData("1 * 2 / 3")]
    [InlineData("1 + 2 * 3")]
    [InlineData("3 + 2 * 7")]
    [InlineData("2 * 7 + 3")]
    [InlineData("14 + 2 * 3 - 6 / 2")]
    [InlineData("7 + ((2 + 3))")]
    [InlineData("1.1 + 2.1 * 3.1")]
    [InlineData("14.5 + 2 * x - y / 2")]
    [InlineData("(1)")]
    [InlineData("(1 + 2) * 3")]
    [InlineData("6 / (2 + 3)")]
    [InlineData("(2 + 3) / 6")]
    [InlineData("((1))")]
    public async Task ParseTree(string input)
        {
        LexerMatch<Token>[] lexerMatches = new Lexer(input).GetMatches().Where(match => match.Match.Token != Token.SPACE).ToArray();

        output.Write(lexerMatches);

        await output.WriteGraphAsync(new ParseTreeBuilder(lexerMatches.GetGenericEnumerator()).Build());
        }    

    [Theory]
    [InlineData("")]
    [InlineData("#")]
    [InlineData("--1")]
    [InlineData("+1")]
    [InlineData("++2")]
    [InlineData("C+")]
    [InlineData("C++")]
    [InlineData("2 ++ 1")]
    [InlineData("2 -- 1")]
    [InlineData("2 ** 1")]
    [InlineData("(")]
    [InlineData("()")]
    [InlineData(")")]
    [InlineData("(1")]
    public async Task ParseTreeError(string input)
        {
        try
            {
            LexerMatch<Token>[] lexerMatches = new Lexer(input).GetMatches().Where(match => match.Match.Token != Token.SPACE).ToArray();
            output.Write(lexerMatches);
            Node<RuleMatch<Token>> root = new ParseTreeBuilder(lexerMatches.GetGenericEnumerator()).Build();
            await output.WriteGraphAsync(root);
            Assert.Fail();
            }
        catch (InvalidOperationException e) when (e.Message.StartsWith("Sintax error"))
            {
            output.WriteLine(e.ToString());
            }
        }    

    private BinaryNode<RuleMatch<Token>> CreateManualAbstractSintaxTree()
        {
        LexerMatch<Token>[] lexerMatches = new Lexer("2 * 7 + 3").GetMatches().Where(match => match.Match.Token != Token.SPACE).ToArray();

        output.Write(lexerMatches);
        /*
        0001:0000 NUMBER '2'
        0001:0002 MULTIPLY '*'
        0001:0004 NUMBER '7'
        0001:0006 ADDITION '+'
        0001:0008 NUMBER '3'
        */
        BinaryNode<RuleMatch<Token>> num2Node = new BinaryNode<RuleMatch<Token>>(1, lexerMatches[0].Match);
        BinaryNode<RuleMatch<Token>> num7Node = new BinaryNode<RuleMatch<Token>>(2, lexerMatches[2].Match);
        BinaryNode<RuleMatch<Token>> mulNode = new BinaryNode<RuleMatch<Token>>(3, lexerMatches[1].Match) { Left = num2Node, Right = num7Node };
        BinaryNode<RuleMatch<Token>> num3Node = new BinaryNode<RuleMatch<Token>>(4, lexerMatches[4].Match);
        BinaryNode<RuleMatch<Token>> addNode = new BinaryNode<RuleMatch<Token>>(5, lexerMatches[3].Match) { Left = mulNode, Right = num3Node };
        
        return addNode;
        }    

    [Fact]
    public async Task ManualAbstractSintaxTree()
        {
        await output.WriteGraphAsync(this.CreateManualAbstractSintaxTree().ToNode());
        }    

    [Fact]
    public void LevelOrderEnumerateTree()
        {
        Assert.Equal(["+", "*", "3", "2", "7"], this.CreateManualAbstractSintaxTree().ToNode().LevelOrderEnumerateTree().Select(node => node.Value.ToString()));
        }    

    [Fact]
    public void PreOrderEnumerateTree()
        {
        Assert.Equal(["+", "*", "2", "7", "3"], this.CreateManualAbstractSintaxTree().ToNode().PreOrderEnumerateTree().Select(node => node.Value.ToString()));
        }    

    [Fact]
    public void PostOrderEnumerateTree()
        {
        Assert.Equal(["2", "7", "*", "3", "+"], this.CreateManualAbstractSintaxTree().ToNode().PostOrderEnumerateTree().Select(node => node.Value.ToString()));
        }    

    [Fact]
    public void InOrderEnumerateTree()
        {
        Assert.Equal(["2", "*", "7", "+", "3"], this.CreateManualAbstractSintaxTree().InOrderEnumerateTree().Select(node => node.Value.ToString()));
        }    
    
    [Fact]
    public void VisitBinaryTree()
        {
        List<BinaryNode<RuleMatch<Token>>> preOrderList = [];
        List<BinaryNode<RuleMatch<Token>>> inOrderList = [];
        List<BinaryNode<RuleMatch<Token>>> postOrderList = [];

        this.CreateManualAbstractSintaxTree().VisitBinaryTree(
            preOrderList.Add,
            inOrderList.Add,
            postOrderList.Add
            );

        Assert.Equal(["+", "*", "2", "7", "3"], preOrderList.Select(node => node.Value.ToString()));
        Assert.Equal(["2", "*", "7", "+", "3"], inOrderList.Select(node => node.Value.ToString()));
        Assert.Equal(["2", "7", "*", "3", "+"], postOrderList.Select(node => node.Value.ToString()));
        }    

    [Fact]
    public void PreOrderEnumerateGraph()
        {
        Assert.Equal(["+", "*", "2", "7", "3"], this.CreateManualAbstractSintaxTree().ToNode().PreOrderEnumerateGraph().Select(node => node.Value.ToString()));
        }    

    [Fact]
    public void PostOrderEnumerateGraph()
        {
        Assert.Equal(["2", "7", "*", "3", "+"], this.CreateManualAbstractSintaxTree().ToNode().PostOrderEnumerateGraph().Select(node => node.Value.ToString()));
        }    

    [Theory]
    [InlineData("1")]
    [InlineData("1)")]
    [InlineData("-1")]
    [InlineData("x")]
    [InlineData("1 + 2")]
    [InlineData("1+2")]
    [InlineData("2 - 1")]
    [InlineData("2 * 1")]
    [InlineData("2 / 1")]
    [InlineData("1 / 2")]
    [InlineData("-1 + 2")]
    [InlineData("1 + 2 + 3")]
    [InlineData("1 + 2 + 3 + 4 + 5")]
    [InlineData("7 + 3 * (10 / (12 / (3 + 1) - 1))")]    
    [InlineData("1 * 2 * 3")]
    [InlineData("1 + 2 - 3")]
    [InlineData("1 * 2 / 3")]
    [InlineData("1 + 2 * 3")]
    [InlineData("3 + 2 * 7")]
    [InlineData("2 * 7 + 3")]
    [InlineData("14 + 2 * 3 - 6 / 2")]
    [InlineData("7 + ((2 + 3))")]
    [InlineData("1.1 + 2.1 * 3.1")]
    [InlineData("1.1 + 2.1 + 3.1")]
    [InlineData("14.5 + 2 * x - y / 2")]
    [InlineData("(1)")]
    [InlineData("(1 + 2) * 3")]
    [InlineData("6 / (2 + 3)")]
    [InlineData("(2 + 3) / 6")]
    [InlineData("((1))")]
    public async Task AbstractSintaxTree(string input)
        {
        LexerMatch<Token>[] lexerMatches = new Lexer(input).GetMatches().Where(match => match.Match.Token != Token.SPACE).ToArray();

        output.Write(lexerMatches);

        await output.WriteGraphAsync(new AbstractSintaxTreeBuilder(lexerMatches.GetGenericEnumerator()).BuildExpression().ToNode());
        }    
    
    [Fact]
    public void ManualCompute()
        {
        Assert.Equal(17, this.CreateManualAbstractSintaxTree().Compute<int>());
        }    

    [Theory]
    [InlineData("1", 1)]
    [InlineData("1)", 1)]
    [InlineData("-1", -1)]
    [InlineData("-(1)", -(1))]
    [InlineData("-(-1)", -(-1))]
    [InlineData("-(+1)", -(+1))]
    [InlineData("x", 0)]
    [InlineData("1 + 2", 3)]
    [InlineData("1 + -2", 1 + -2)]
    [InlineData("1 + (-2)", 1 + (-2))]
    [InlineData("1 + + + (+2)", 1 + + + (+2))]
    [InlineData("1 - - - 1", 1 - - - 1)]
    [InlineData("1+2", 3)]
    [InlineData("2 - 1", 1)]
    [InlineData("2 * 1", 2)]
    [InlineData("2 / 1", 2)]
    [InlineData("1 / 2", 0.5)]
    [InlineData("-1 + 2", 1)]
    [InlineData("1 + 2 + 3", 6)]
    [InlineData("1 + 2 + 3 + 4 + 5", 15)]
    [InlineData("7 + 3 * (10 / (12 / (3 + 1) - 1))", 22)]    
    [InlineData("1 * 2 * 3", 6)]
    [InlineData("1 + 2 - 3", 0)]
    [InlineData("1 * 2 / 3", 2.0 / 3)]
    [InlineData("1 + 2 * 3", 7)]
    [InlineData("3 + 2 * 7", 17)]
    [InlineData("2 * 7 + 3", 17)]
    [InlineData("14 + 2 * 3 - 6 / 2", 17)]
    [InlineData("7 + ((2 + 3))", 12)]
    [InlineData("1.1 + 2.1 * 3.1", 7.61)]
    [InlineData("1.1 + 2.1 + 3.1", 6.3)]
    [InlineData("14.5 + 2 * x - y / 2", 14.5)]
    [InlineData("(1)", 1)]
    [InlineData("(1 + 2) * 3", 9)]
    [InlineData("6 / (2 + 3)", 6.0 / 5)]
    [InlineData("(2 + 3) / 6", (2.0 + 3) / 6)]
    [InlineData("((1))", 1)]
    public void Compute(string input, decimal result)
        {
        Assert.Equal(result, new AbstractSintaxTreeBuilder(
                new Lexer(input)
                    .GetMatches()
                    .Where(match => match.Match.Token != Token.SPACE)
                    .GetEnumerator())
            .BuildExpression()
            .Compute<decimal>(), 4);
        } 

    [Theory]
    [InlineData("1", 1, "{}")]
    [InlineData("x", 0, "{}")]
    [InlineData("x", 0, "{ \"x\": 0 }")]
    [InlineData("x", 1, "{ \"x\": 1 }")]
    [InlineData("14.5 + 2 * x - y / 2", 14.5, "{}")]
    [InlineData("14.5 + 2 * x - y / 2", 14.5, "{ \"x\": 0 }")]
    [InlineData("14.5 + 2 * x - y / 2", 14.5, "{ \"x\": 0, \"y\": 0 }")]
    [InlineData("14.5 + 2 * x - y / 2", 16.5, "{ \"x\": 1, \"y\": 0 }")]
    [InlineData("14.5 + 2 * x - y / 2", 16, "{ \"x\": 1, \"y\": 1 }")]
    [InlineData("14.5 + 2 * x - y / 2", 14.5 + 2 * 1.5 - 1.5 / 2, "{ \"x\": 1.5, \"y\": 1.5 }")]
    public void ComputeWithParameters(string input, decimal result, string jsonParameters)
        {
        Assert.Equal(result, new AbstractSintaxTreeBuilder(
                new Lexer(input)
                    .GetMatches()
                    .Where(match => match.Match.Token != Token.SPACE)
                    .GetEnumerator())
            .BuildExpression()
            .Compute<decimal>(JsonSerializer.Deserialize<IReadOnlyDictionary<string, decimal>>(jsonParameters) ?? throw new ArgumentException("Invalid parameters format")), 4);
        } 
    }
