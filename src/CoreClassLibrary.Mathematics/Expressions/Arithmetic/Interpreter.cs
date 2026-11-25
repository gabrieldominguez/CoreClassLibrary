
using CoreClassLibrary.Collections;
using CoreClassLibrary.Objects.Graphs;
using CoreClassLibrary.RegularExpressions.Lexing;
using System.Globalization;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Expressions.Arithmetic;

/// <summary>
/// Arithmetic Expression Evaluator
/// </summary>
public static class Interpreter
    {
    public static T Compute<T>(this BinaryNode<RuleMatch<Token>> node) where T: INumber<T>
        => node.Compute<T>(EmptyReadOnlyDictionary<string, T>.Value);

    public static T Compute<T>(this BinaryNode<RuleMatch<Token>> node, IReadOnlyDictionary<string, T> parameters) where T: INumber<T>
        {
        if (node.Left != null)
            {
            T left = node.Left.Compute<T>(parameters);

            if (node.Right != null)
                {
                T right = node.Right.Compute<T>(parameters);

                return node.Value.Token switch
                    {
                    Token.ADDITION => left + right,
                    Token.SUBTRACTION => left - right,
                    Token.MULTIPLY => left * right,
                    Token.DIVISION => left / right,
                    _ => throw new NotImplementedException($"Unknown binary operation: {node.Value.Token}")
                    };
                }
            else
                {                
                return node.Value.Token == Token.SUBTRACTION? -left: left;  // Unary op
                }
            }
        else
            {
            if (node.Right != null)
                {
                T right = node.Right.Compute<T>(parameters);
                return node.Value.Token == Token.SUBTRACTION? -right: right;  // Unary op
                }
            else
                {
                return node.Value.Token switch
                    {
                    Token.NUMBER => T.Parse(node.Value.Value, CultureInfo.InvariantCulture),
                    Token.PARAMETER => parameters.TryGetValue(node.Value.Value, out T? value)? value: T.AdditiveIdentity,
                    _ => throw new NotImplementedException($"Unknown value type: {node.Value.Token}")
                    };
                }
            }
        }
    }
