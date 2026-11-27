using CoreClassLibrary.Mathematics.IdGenerators;
using CoreClassLibrary.Objects.Graphs;
using CoreClassLibrary.RegularExpressions.Lexing;

namespace CoreClassLibrary.Mathematics.Expressions.Arithmetic;

/// <summary>
/// Arithmetic Expression AST Builder
/// </summary>
/// <remarks>
/// The matches from matchesEnumerator MUST be SPACE filtered.
/// Grammar:
/// expr   : term ((PLUS | MINUS) term)*
/// term   : factor ((MUL | DIV) factor)*
/// factor : (PLUS | MINUS) factor | INTEGER | LPAREN expr RPAREN
/// </remarks>
/// <see cref="https://ruslanspivak.com/lsbasi-part1/"/>
/// <see cref="https://ruslanspivak.com/lsbasi-part8/"/>
/// <see cref="https://dev.to/j0nimost/series/14710"/>
public class AbstractSintaxTreeBuilder(IEnumerator<LexerMatch<Token>> matchesEnumerator)
    {
    private readonly IntIdGenerator idGenerator = new IntIdGenerator(0);

    private bool IsMatchAvailable = false;

    private BinaryNode<RuleMatch<Token>> BuildFactor()
        {
        if (!(this.IsMatchAvailable = matchesEnumerator.MoveNext()))
            {
            throw new InvalidOperationException($"Sintax error. Token expected.");
            }

        switch (matchesEnumerator.Current.Match.Token)
            {
            case Token.ADDITION:
            case Token.SUBTRACTION:
                return new BinaryNode<RuleMatch<Token>>(this.idGenerator.NextId(), matchesEnumerator.Current.Match)
                    {
                    Left = this.BuildFactor()
                    };

            case Token.NUMBER:
            case Token.PARAMETER:
                BinaryNode<RuleMatch<Token>> leafNode = new BinaryNode<RuleMatch<Token>>(this.idGenerator.NextId(), matchesEnumerator.Current.Match);
                this.IsMatchAvailable = matchesEnumerator.MoveNext();
                return leafNode;
            
            case Token.LPAREN:
                BinaryNode<RuleMatch<Token>> exprNode = this.BuildExpression();
                this.IsMatchAvailable = matchesEnumerator.MoveNext(); // Token.RPAREN
                return exprNode;
            
            default:
                throw new InvalidOperationException($"Sintax error. Invalid token { matchesEnumerator.Current.Match.Value } Line {matchesEnumerator.Current.LineNumber} Position {matchesEnumerator.Current.Position}");
            }
        }

    private BinaryNode<RuleMatch<Token>> BuildTerm()
        {
        BinaryNode<RuleMatch<Token>> node = this.BuildFactor();
        while (this.IsMatchAvailable && (matchesEnumerator.Current.Match.Token == Token.MULTIPLY || matchesEnumerator.Current.Match.Token == Token.DIVISION))
            {
            node = new BinaryNode<RuleMatch<Token>>(this.idGenerator.NextId(), matchesEnumerator.Current.Match) { Left = node, Right = this.BuildFactor() }; 
            }
        return node;
        }

    public BinaryNode<RuleMatch<Token>> BuildExpression()
        {
        BinaryNode<RuleMatch<Token>> node = this.BuildTerm();
        while (this.IsMatchAvailable && (matchesEnumerator.Current.Match.Token == Token.ADDITION || matchesEnumerator.Current.Match.Token == Token.SUBTRACTION))
            {
            node = new BinaryNode<RuleMatch<Token>>(this.idGenerator.NextId(), matchesEnumerator.Current.Match) { Left = node, Right = this.BuildTerm() }; 
            }
        return node;
        }
    }
