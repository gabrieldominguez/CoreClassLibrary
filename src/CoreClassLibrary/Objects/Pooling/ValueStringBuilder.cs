using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// Value type StringBuilder
/// </summary>
/// <see cref="https://github.com/dotnet/coreclr/blob/efebb38f3c18425c57f94ff910a50e038d13c848/src/mscorlib/shared/System/Text/ValueStringBuilder.cs"/>
/// <see cref="https://habr.com/en/company/clrium/blog/443976/"/>
/// <see cref="https://andrewlock.net/a-deep-dive-on-stringbuilder-part-6-vaulestringbuilder-a-stack-based-string-builder"/>
public ref struct ValueStringBuilder: IDisposable
    {
    private char[]? _arrayToReturnToPool = null;
    private Span<char> _chars;
    private int _pos = 0;

    public ValueStringBuilder(Span<char> chars)
        {
        _chars = chars;
        }

    public int Length
        {
        readonly get => _pos;
        set
            {
            int delta = value - _pos;
            if (delta > 0)
                {
                Append('\0', delta);
                }
            else
                {
                _pos = value;
                }
            }
        }

    public override readonly string ToString()
        => new string(_chars[.._pos]);

    public readonly bool TryCopyTo(Span<char> destination, out int charsWritten)
        {
        if (_chars[.._pos].TryCopyTo(destination))
            {
            charsWritten = _pos;
            return true;
            }
        else
            {
            charsWritten = 0;
            return false;
            }
        }

    public void Insert(int index, char value, int count)
        {
        if (_pos > _chars.Length - count)
            {
            Grow(count);
            }

        int remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);
        _chars.Slice(index, count).Fill(value);
        _pos += count;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
        {
        int pos = _pos;
        if (pos < _chars.Length)
            {
            _chars[pos] = c;
            _pos = pos + 1;
            }
        else
            {
            GrowAndAppend(c);
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string s)
        {
        int pos = _pos;
        if (s.Length == 1 && pos < _chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            {
            _chars[pos] = s[0];
            _pos = pos + 1;
            }
        else
            {
            AppendSlow(s);
            }
        }

    private void AppendSlow(string s)
        {
        int pos = _pos;
        if (pos > _chars.Length - s.Length)
            {
            Grow(s.Length);
            }

        ((ReadOnlySpan<char>)s).CopyTo(_chars[pos..]);
        _pos += s.Length;
        }

    public void Append(char c, int count)
        {
        if (_pos > _chars.Length - count)
            {
            Grow(count);
            }

        Span<char> dst = _chars.Slice(_pos, count);
        for (int i = 0; i < dst.Length; i++)
            {
            dst[i] = c;
            }
        _pos += count;
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
        {
        int origPos = _pos;
        if (origPos > _chars.Length - length)
            {
            Grow(length);
            }

        _pos = origPos + length;
        return _chars.Slice(origPos, length);
        }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
        {
        Grow(1);
        Append(c);
        }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int requiredAdditionalCapacity)
        {
        Debug.Assert(requiredAdditionalCapacity > _chars.Length - _pos);

        char[] poolArray = ArrayPool<char>.Shared.Rent(Math.Max(_pos + requiredAdditionalCapacity, _chars.Length * 2));

        _chars.CopyTo(poolArray);

        char[]? toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
            {
            ArrayPool<char>.Shared.Return(toReturn);
            }
        }

    public void Dispose()
        {
        char[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
            {
            ArrayPool<char>.Shared.Return(toReturn);
            }
        }
    }
