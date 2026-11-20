
using System.Linq.Expressions;
using Xunit;

namespace CoreClassLibrary.Tests;

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
    }
