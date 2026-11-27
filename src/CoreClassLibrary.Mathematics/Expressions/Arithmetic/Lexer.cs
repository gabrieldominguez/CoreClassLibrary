using CoreClassLibrary.RegularExpressions.Lexing;
using CoreClassLibrary.RegularExpressions.Lexing.Rules;

namespace CoreClassLibrary.Mathematics.Expressions.Arithmetic;

/// <summary>
/// Arithmetic Expression Lexer
/// </summary>
/// <see cref="https://ruslanspivak.com/lsbasi-part1/"/>
/// <see cref="https://dev.to/j0nimost/series/14710"/>
public class Lexer(string input) : 
    StringLexer<Token>(
        input, 
        new CompositeRule<Token>(
            [
            //new RegexRule<Token>(@"\s+", Token.SPACE),    // See CharRule below.
            //new RegexRule<Token>(@"-?\d+(\.\d+)?", Token.NUMBER), // Allow negative numbers in the lexer, but not in the parser. The parser will handle unary minus.
            new RegexRule<Token>(@"\d+(\.\d+)?", Token.NUMBER),
            new RegexRule<Token>(@"[A-Za-z]+", Token.PARAMETER),
            new CompositeCharRule<Token>(
                [
                new CharRule<Token>(' ', Token.SPACE),
                new CharRule<Token>('+', Token.ADDITION),
                new CharRule<Token>('-', Token.SUBTRACTION),
                new CharRule<Token>('*', Token.MULTIPLY),
                new CharRule<Token>('/', Token.DIVISION),
                new CharRule<Token>('(', Token.LPAREN),
                new CharRule<Token>(')', Token.RPAREN)
                ])
            ])
        );
