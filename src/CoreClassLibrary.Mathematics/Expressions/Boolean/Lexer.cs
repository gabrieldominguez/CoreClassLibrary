using CoreClassLibrary.RegularExpressions.Lexing;
using CoreClassLibrary.RegularExpressions.Lexing.Rules;

namespace CoreClassLibrary.Mathematics.Expressions.Boolean;

/// <summary>
/// Boolean Expression Lexer
/// </summary>
/// <see cref="https://ruslanspivak.com/lsbasi-part1/"/>
/// <see cref="https://en.wikipedia.org/wiki/Boolean_expression"/>
public class Lexer(string input) : 
    StringLexer<Token>(
        input, 
        new CompositeRule<Token>(
            [
            //new RegexRule<Token>(@"\s+", Token.SPACE),    // See CharRule below.
            new RegexRule<Token>(@"[A-Za-z]+", Token.PARAMETER),
            new CompositeCharRule<Token>(
                [
                new CharRule<Token>(' ', Token.SPACE),
                new CharRule<Token>('1', Token.CONSTANT),
                new CharRule<Token>('0', Token.CONSTANT),
                new CharRule<Token>('T', Token.CONSTANT),
                new CharRule<Token>('F', Token.CONSTANT),
                new CharRule<Token>('|', Token.OR),
                new CharRule<Token>('&', Token.AND),
                new CharRule<Token>('!', Token.NOT),
                new CharRule<Token>('(', Token.LPAREN),
                new CharRule<Token>(')', Token.RPAREN)
                ])
            ])
        );
