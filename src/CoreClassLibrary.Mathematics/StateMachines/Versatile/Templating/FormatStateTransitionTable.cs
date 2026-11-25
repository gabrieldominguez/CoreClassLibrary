
namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Templating;

/// <summary>
/// Reemplaza nombres de propiedades entre tokens distintos, por ej: llaves o corchetes.
/// </summary>
/// <example>
/// "{CurrentTime} - {ProcessName}" => "1975-11-12 00:00:00 - ABC"
/// </example>
/// <see cref="https://stackoverflow.com/questions/159017/named-string-formatting-in-c-sharp"/>
/// <see cref="http://haacked.com/archive/2009/01/04/fun-with-named-formats-string-parsing-and-edge-cases.aspx"/>
/// <see cref="http://haacked.com/archive/2009/01/14/named-formats-redux.aspx/"/>
public class FormatStateTransitionTable(FormatWriter writer) : StateTransitionTable<char>(
        [
        TransitionExtensions.CreateEquatableTransition(States.InText, writer.OpenChar, States.StartKey, 0, " c = OpenChar"),
        TransitionExtensions.CreateEquatableTransition(States.InText, '\0', States.InText, 0, " c = '\0'"),
        TransitionExtensions.CreateElseTransition<char, FormatWriter>(States.InText, writer, (c, w) => w.Write(c), States.InText, " Write(c)"),

        new StateTransition<char, FormatWriter>(States.StartKey, writer, (c, w) => Char.IsLetterOrDigit(c), (c, w) => { w.ClearKey(); w.WriteToKey(c); }, States.InKey, 0, " IsLetterOrDigit(c) => ClearKey(); WriteToKey(c)"),
        new StateTransition<char, FormatWriter>(States.StartKey, writer, (c, w) => c == w.OpenChar || c == '\0', (c, w) => w.WriteOpenChar(), States.StartKey, 0, " c = OpenChar || c = '\0' => WriteOpenChar()"),
        TransitionExtensions.CreateElseTransition<char, FormatWriter>(States.StartKey, writer, (c, w) => { w.WriteOpenChar(); w.Write(c); }, States.InText, " WriteOpenChar(); Write(c)"),

        new StateTransition<char, FormatWriter>(States.InKey, writer, (c, w) => Char.IsLetterOrDigit(c), (c, w) => w.WriteToKey(c), States.InKey, 0, " IsLetterOrDigit(c) => WriteToKey(c)"),
        new StateTransition<char, FormatWriter>(States.InKey, writer, (c, w) => c == w.CloseChar, (c, w) => w.WriteFromDictionary(), States.InText, 0, " c = CloseChar => WriteFromDictionary(c)"),
        new StateTransition<char, FormatWriter>(States.InKey, writer, (c, w) => c == w.OpenChar || c == '\0', (c, w) => w.WriteKeyToOutput(), States.StartKey, 0, " c = OpenChar || c = '\0' => WriteKeyToOutput()"),
        TransitionExtensions.CreateElseTransition<char, FormatWriter>(States.InKey, writer, (c, w) => { w.WriteKeyToOutput(); w.Write(c); }, States.InText, " WriteKeyToOutput(); Write(c)")
        ]);
