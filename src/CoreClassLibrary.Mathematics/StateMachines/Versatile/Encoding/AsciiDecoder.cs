
namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Encoding;

public class AsciiDecoder(TextWriter textWriter, char encoderChar = '#')
    {
    /// <summary>
    /// Reemplaza los caracters encodeados por su valor original
    /// </summary>
    /// <example>
    /// "AA#402AA" => "AA@CC"
    /// </example>
    public StateTransitionTable<char> CreateDecodingTable()
        => new StateTransitionTable<char>(
            [
            new StateTransition<char, AsciiDecoder>(DecodingStates.InText, this, (c, encoder) => encoder.IsEnconderChar(c), (c, decoder) => { }, DecodingStates.StartDecoding, 0, " IsEnconderChar(c)"),
            new StateTransition<char, AsciiDecoder>(DecodingStates.StartDecoding, this, (c, encoder) => true, (c, decoder) => decoder.StartDecoding(c), DecodingStates.EndDecoding, 0, " StartDecoding(c)"),
            new StateTransition<char, AsciiDecoder>(DecodingStates.EndDecoding, this, (c, encoder) => true, (c, decoder) => decoder.EndDecoding(c), DecodingStates.InText, 0, " EndDecoding(c)"),
            TransitionExtensions.CreateElseTransition<char, AsciiDecoder>(DecodingStates.InText, this, (c, decoder) => decoder.Write(c), DecodingStates.InText, " Write(c)")
            ]);

    public void Write(char c)
        => textWriter.Write(c);

    public bool IsEnconderChar(char c)
        => c == encoderChar;

    private char firstHexDigit;

    public void StartDecoding(char c)
        {
        this.firstHexDigit = c; 
        }

    public void EndDecoding(char c)
        {
        textWriter.Write((char)(Binary.FromHexPair(this.firstHexDigit, c)));
        }
    }

public static class DecodingStates
    {
    public const int InText = 0;
    public const int StartDecoding = 1;
    public const int EndDecoding = 2;

    public readonly static string[] Labels = ["InText", "StartDecoding", "EndDecoding"];
    }