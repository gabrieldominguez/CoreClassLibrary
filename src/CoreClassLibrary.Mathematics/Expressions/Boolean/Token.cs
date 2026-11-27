namespace CoreClassLibrary.Mathematics.Expressions.Boolean;

public enum Token
    {
    NONE,
    SPACE,
    CONSTANT,   // true/false: T/F, 1/0
    PARAMETER,
    OR,         // |
    AND,        // &
    NOT,        // !
    LPAREN,     // (
    RPAREN      // )
    }