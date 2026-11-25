using CoreClassLibrary.Mathematics.IdGenerators;
using CoreClassLibrary.Objects.Graphs;
using CoreClassLibrary.RegularExpressions.Lexing;

namespace CoreClassLibrary.Mathematics.Expressions.Arithmetic;

/// <summary>
/// Arithmetic Expression ParseTree Builder
/// </summary>
/// <remarks>
/// The matches from matchesEnumerator MUST be SPACE filtered.
/// Grammar:
/// expr   : term ((PLUS | MINUS) term)*
/// term   : factor ((MUL | DIV) factor)*
/// factor : INTEGER | LPAREN expr RPAREN
/// </remarks>
/// <see cref="https://ruslanspivak.com/lsbasi-part1/"/>
/// <see cref="https://ruslanspivak.com/lsbasi-part7/"/>
/// <see cref="https://dev.to/j0nimost/series/14710"/>
public class ParseTreeBuilder(IEnumerator<LexerMatch<Token>> matchesEnumerator)
    {
    private readonly IntIdGenerator idGenerator = new IntIdGenerator(0);

    private bool IsMatchAvailable = false;
        
    private void AddCurrentMatch(Node<RuleMatch<Token>> parentNode)
        {
        if (!this.IsMatchAvailable)
            {
            throw new InvalidOperationException($"Sintax error. Token expected.");
            }

        parentNode.NextNodes.Add(new Node<RuleMatch<Token>>(this.idGenerator.NextId(), matchesEnumerator.Current.Match));
        this.IsMatchAvailable = matchesEnumerator.MoveNext();
        }

    private void BuildFactor(Node<RuleMatch<Token>> parentNode)
        {
        Node<RuleMatch<Token>> node = new Node<RuleMatch<Token>>(idGenerator.NextId(), new RuleMatch<Token>("factor", Token.NONE));
        parentNode.NextNodes.Add(node);

        if (!this.IsMatchAvailable)
            {
            throw new InvalidOperationException($"Sintax error. Token expected.");
            }

        switch (matchesEnumerator.Current.Match.Token)
            {
            case Token.NUMBER:
            case Token.PARAMETER:
                this.AddCurrentMatch(node);
                break;
            case Token.LPAREN:
                this.AddCurrentMatch(node); // Token.LPAREN
                this.BuildExpression(node);
                this.AddCurrentMatch(node); // Token.RPAREN
                break;
            default:
                throw new InvalidOperationException($"Sintax error. Invalid token { matchesEnumerator.Current.Match.Value } Line {matchesEnumerator.Current.LineNumber} Position {matchesEnumerator.Current.Position}");
            }
        }

    private void BuildTerm(Node<RuleMatch<Token>> parentNode)
        {
        Node<RuleMatch<Token>> node = new Node<RuleMatch<Token>>(this.idGenerator.NextId(), new RuleMatch<Token>("term", Token.NONE));
        parentNode.NextNodes.Add(node);

        this.BuildFactor(node);
        while (this.IsMatchAvailable && (matchesEnumerator.Current.Match.Token == Token.MULTIPLY || matchesEnumerator.Current.Match.Token == Token.DIVISION))
            {
            this.AddCurrentMatch(node);
            this.BuildFactor(node);
            }
        }

    private void BuildExpression(Node<RuleMatch<Token>> parentNode)
        {
        Node<RuleMatch<Token>> node = new Node<RuleMatch<Token>>(this.idGenerator.NextId(), new RuleMatch<Token>("expr", Token.NONE));
        parentNode.NextNodes.Add(node);

        this.BuildTerm(node);
        while (this.IsMatchAvailable && (matchesEnumerator.Current.Match.Token == Token.ADDITION || matchesEnumerator.Current.Match.Token == Token.SUBTRACTION))
            {
            this.AddCurrentMatch(node);
            this.BuildTerm(node);
            }
        }

    public Node<RuleMatch<Token>> Build()
        {
        Node<RuleMatch<Token>> root = new Node<RuleMatch<Token>>(0, new RuleMatch<Token>("ROOT", Token.NONE));
        this.IsMatchAvailable = matchesEnumerator.MoveNext();
        this.BuildExpression(root);
        return root;
        }
    }
