
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.StateMachines.Versatile.Encoding;

public class AsciiEncoder(TextWriter textWriter, IReadOnlyList<bool> encodingMap, char encoderChar = '#')
    {
    public static AsciiEncoder Create(TextWriter textWriter, IEnumerable<char> encondigChars, char encoderChar = '#')
        {
        bool[] encodingMap = new bool[Byte.MaxValue+1];

        foreach (char c in encondigChars.Concat(encoderChar))
            {
            if(c > Byte.MaxValue)    // c > 255 
                {
                throw new InvalidOperationException($"Only ASCII chars are allowed. c = {c}");
                }

            encodingMap[c] = true;
            }

        return new AsciiEncoder(textWriter, encodingMap, encoderChar);
        }

    /// <summary>
    /// Reemplaza los caracters especificados por su ascii en hexa
    /// </summary>
    /// <example>
    /// "AA@CC" => "AA#402AA"
    /// </example>
    public StateTransitionTable<char> CreateEncodingTable()
        => new StateTransitionTable<char>(
            [
            new StateTransition<char, AsciiEncoder>(0, this, (c, encoder) => encoder.IsEncondigChar(c), (c, encoder) => encoder.Encode(c), 0, 0, " IsEncondigChar(c) => Encode(c)"),
            TransitionExtensions.CreateElseTransition<char, AsciiEncoder>(0, this, (c, encoder) => encoder.Write(c), 0, " Write(c)")
            ]);

    public void Write(char c)
        => textWriter.Write(c);

    public bool IsEncondigChar(char c)
        => c <= Byte.MaxValue && encodingMap[c];

    public void Encode(char c)
        {
        textWriter.Write(encoderChar);
        textWriter.Write(((byte)c).ToHEXString());
        }
    }
