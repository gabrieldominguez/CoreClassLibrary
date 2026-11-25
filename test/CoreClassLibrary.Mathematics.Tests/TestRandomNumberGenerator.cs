
using System.Security.Cryptography;
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Mathematics.Tests;

public class TestRandomNumberGenerator(IEnumerator<byte> circularEnumerator) : RandomNumberGenerator
    {
    public TestRandomNumberGenerator(IReadOnlyList<byte> bytes): this(bytes.ToCircularEnumerable().GetEnumerator()) { }

    public override void GetBytes(byte[] data)
        {
        for (int i = 0; i < data.Length; i++)
            {
            circularEnumerator.MoveNext();
            data[i] = circularEnumerator.Current;
            }
        }
    }
