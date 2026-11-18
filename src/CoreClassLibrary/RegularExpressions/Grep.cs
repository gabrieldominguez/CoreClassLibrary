
namespace CoreClassLibrary.RegularExpressions;

/// <summary>
/// Simple Grep implementation for matching regular expressions against text.
/// </summary>
/// <remarks>
/// From Kernighan and Pike - The Practice of Programming - Section 9.2 Regular Expressions
/// And Kernighan & Ritchie - The C Programming Language (1978) - Section 4.1 Basics
/// 
/// Functionality:
///     Literal matching
///     Search anywhere in the text
///     ^beginning anchor
///     $ end anchor
///     .wildcard
///     * zero-or-more quantifier
///     .* combinations
///     Empty pattern and empty string edge cases
/// </remarks>
/// <see cref=""/>
public class Grep
    {
    public static bool Match(string regexp, string text)
        {
        if (string.IsNullOrEmpty(regexp))
            {
            return true;
            }

        // If the pattern starts with '^', only match at the beginning
        if (regexp[0] == '^')
            {
            return Grep.MatchHere(regexp, 1, text, 0);
            }

        // Try matching from every position, including the empty string at the end
        for (int i = 0; i <= text.Length; i++)
            {
            if (Grep.MatchHere(regexp, 0, text, i))
                {  
                return true; 
                }
            }

        return false;
        }

    public static bool MatchHere(string regexp, int r, string text, int t)
        {
        // End of pattern = success
        if (r == regexp.Length)
            {
            return true;
            }
            
        // * operator
        if (r + 1 < regexp.Length && regexp[r + 1] == '*')
            {
            return Grep.MatchStar(regexp[r], regexp, r + 2, text, t);
            }

        // '$' at end of pattern
        if (regexp[r] == '$' && r + 1 == regexp.Length)
            {
            return t == text.Length;
            }

        // Match one character
        if (t < text.Length && (regexp[r] == '.' || regexp[r] == text[t]))
            {
            return MatchHere(regexp, r + 1, text, t + 1);
            }

        return false;
        }

    private static bool MatchStar(char c, string regexp, int r, string text, int t)
        {
        do
            {
            if (Grep.MatchHere(regexp, r, text, t))
                { 
                return true; 
                }
            }
        while (t < text.Length && (text[t++] == c || c == '.'));

        return false;
        }
    }

