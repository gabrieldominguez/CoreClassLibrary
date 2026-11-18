namespace CoreClassLibrary.RegularExpressions.Lexing.Rules;

public class CompositeCharRule<T>(IReadOnlyList<RuleMatch<T>> ruleMatchs): IRule<T>
    {
    private static RuleMatch<T>[] CreateRuleMatchs(IEnumerable<CharRule<T>> rules)
        {
        RuleMatch<T>[] array = new RuleMatch<T>[Byte.MaxValue+1];

        foreach (CharRule<T> rule in rules)
            {
            if(rule.C > Byte.MaxValue)    // c > 255 
                {
                throw new InvalidOperationException($"Only ASCII chars are allowed. c = {rule.C}");
                }

            array[rule.C] = new RuleMatch<T>(rule.C.ToString(), rule.Token);
            }

        return array;
        }

    public CompositeCharRule(IEnumerable<CharRule<T>> rules): this(CompositeCharRule<T>.CreateRuleMatchs(rules)) { }

    public RuleMatch<T>? Match(string input)
        => input.Length > 0? ruleMatchs[input[0]]: default;
    }
