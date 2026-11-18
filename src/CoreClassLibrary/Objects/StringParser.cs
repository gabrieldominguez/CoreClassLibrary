namespace CoreClassLibrary.Objects;

/// <summary>
/// Permite obtener subcadenas secuencialmente.
/// </summary>
/// <see cref="ArrayReader"/>
/// <see cref="StringSegment"/>
public class StringParser(string str, int startIndex = 0)
    {
    private readonly char[] charArray = str.ToCharArray();

    private int nextIndex = startIndex;

    public char PeekNextChar()
        => this.charArray[this.nextIndex];

    public char GetNextChar()
        => this.GetChar(this.nextIndex);

    public char GetChar(int startIndex)
        {
        this.nextIndex = startIndex + 1;
        return this.charArray[startIndex];
        }

    public string PeekNextString(int length)
        => new string(this.charArray, this.nextIndex, length);

    public string GetNextString(int length)
        => this.GetString(this.nextIndex, length);

    public string GetString(int startIndex, int length)
        {
        this.nextIndex = startIndex + length;
        return new string(this.charArray, startIndex, length);
        }

    public StringParser SetString(int startIndex, int length, out string strRef)
        {
        strRef = this.GetString(startIndex, length);

        return this;
        }

    public StringParser SetNextString(int length, out string strRef)
        {
        strRef = this.GetNextString(length);

        return this;
        }

    public StringParser SkipNextChar()
        {
        this.nextIndex++;

        return this;
        }

    public StringParser SkipNextString(int length)
        {
        this.nextIndex += length;

        return this;
        }
    }
