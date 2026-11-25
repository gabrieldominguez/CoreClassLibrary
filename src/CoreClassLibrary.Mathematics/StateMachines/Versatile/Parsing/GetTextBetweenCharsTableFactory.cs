
namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Parsing;

/// <summary>
/// Obtiene el texto entre dos caracteres distintos, por ej: llaves o corchetes.
/// </summary>
/// <remarks>
/// Si en el texto solo se encuentra el primer caracter, devuelve desde el siguiente hasta el final.
/// Este caso puede identificarse porque el estado de la maquina al finalizar el proceso, no es el estado final.
/// </remarks>
/// <example>
/// [InlineData("aa[b]cc", "[", "]", "b")]
/// [InlineData("aa[bcc", "[", "]", "bcc")]
/// </example>
public static class GetTextBetweenCharsTableFactory
    {
    public static StateTransitionTable<char> CreateStateTransitionTable(TextWriter writer, char openChar = '[', char closeChar = ']')
        => new StateTransitionTable<char>(
            [
            TransitionExtensions.CreateEquatableTransition(States.InText, openChar, States.InTarget, 0, " c = OpenChar"),
            TransitionExtensions.CreateEquatableTransition(States.InTarget, closeChar, States.End, 0, " c = CloseChar"),
            TransitionExtensions.CreateElseTransition<char, TextWriter>(States.InTarget, writer, (c, w) => w.Write(c), States.InTarget, " Write(c)")
            ]);

    //public enum States { InText, InTarget, End}

    public static class States
        {
        public const int InText = 0;
        public const int InTarget = 1;
        public const int End = 2;
        }
    }
