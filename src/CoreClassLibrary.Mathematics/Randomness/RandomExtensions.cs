using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Ranges;
using System.Numerics;

namespace CoreClassLibrary.Mathematics.Randomness;

public static class RandomExtensions
    {
    /// <summary>
    /// Genera un número aleatorio.
    /// </summary>
    /// <remarks>
    /// El número generado es mayor o igual a 0 y menor que int.MaxValue: [0, int.MaxValue).
    /// </remarks>
    public static T Next<T>(this IRandom random) where T : INumber<T>, IMinMaxValue<T>
        => random.Next(T.MaxValue);

    /// <summary>
    /// Genera un número aleatorio menor que maxValue.
    /// </summary>
    /// <remarks>
    /// El número generado es mayor o igual a 0 y menor que maxValue: [0, maxValue).
    /// </remarks>
    public static T Next<T>(this IRandom random, T maxValue) where T : INumber<T>
        => T.CreateChecked(random.Sample() * double.CreateChecked(maxValue));

    /// <summary>
    /// Genera un número aleatorio dentro del rango, incluyendo el Range<T>.End.
    /// </summary>
    /// <remarks>
    /// El número generado pertenece a [range.Start, range.End].
    /// </remarks>
    public static T NextInclusive<T>(this IRandom random, Range<T> range) where T : INumber<T>, IBinaryInteger<T>
        => random.Next(range.Start, range.End + T.One);

    /// <summary>
    /// Genera un número aleatorio dentro del rango, excluyendo el Range<T>.End.
    /// </summary>
    /// <remarks>
    /// El número generado pertenece a [range.Start, range.End).
    /// </remarks>
    public static T Next<T>(this IRandom random, Range<T> range) where T : INumber<T>
        => random.Next(range.Start, range.End);

    /// <summary>
    /// Genera un número aleatorio mayor o igual a minValue y menor que maxValue.
    /// </summary>
    /// <remarks>
    /// El número generado pertenece a [minValue, maxValue).
    /// </remarks>
    public static T Next<T>(this IRandom random, T minValue, T maxValue) where T : INumber<T>
        => T.CreateChecked(random.Sample() * double.CreateChecked(maxValue - minValue)) + minValue;

   
    /// <summary>
    /// Retorna un double en el rango (0, 1]
    /// </summary>
    /// <remarks>Idem Sample() pero excluye al 0 e incluye al 1.</remarks>
    /// <returns>
    /// Un double en el rango (0, 1]
    /// </returns>
    public static double PositiveSample(this IRandom random)
        => 1.0 - random.Sample();

    /// <summary>
    /// Genera un número entero dentro del rango.
    /// </summary>
    /// <remarks>
    /// El número generado pertenece a [range.Start, range.End].
    /// </remarks>
    public static int Next(this IRandom random, Range range)
        => random.Next(range.Start.Value, range.End.Value);

    public static BigInteger NextBigInteger(this IRandom random, int length)
        {
        Span<byte> buffer = stackalloc byte[length];
        random.SetBytes(buffer);
        return new BigInteger(buffer);
        }

    public static BigInteger NextPositiveBigInteger(this IRandom random, int length)
        {
        Span<byte> buffer = stackalloc byte[length];
        random.SetBytes(buffer);
        buffer[^1] = (byte)(buffer[^1] & 0x7F);   //  Ensure not a negative value
        return new BigInteger(buffer);
        }

    /// <summary>
    /// Generates a random boolean.
    /// </summary>
    /// <param name="random">Generador</param>
    /// <param name="trueRate">Probabilidad de retornar True.</param>
    /// <remarks>
    /// trueRate debe ser un valor entre 0 y 1.
    /// </remarks>
    /// <example>
    /// trueRate == 0.30 => 30% de valores True
    /// </example>
    /// <returns>A random boolean.</returns>
    public static bool NextBool(this IRandom random, double trueRate = 0.5)
        => random.Sample() < trueRate;

    public static string NextNumericString(this IRandom random, int length)
        => random.NextString(length, StringExtensions.NumericChars);

    public static string NextAlphaNumericString(this IRandom random, int length)
        => random.NextString(length, StringExtensions.AlphaNumericChars);

    public static string NextSymbolsString(this IRandom random, int length)
        => random.NextString(length, StringExtensions.SymbolsChars);

    /// <summary>
    /// Retorna una cadena aleatoria del largo especificado.
    /// </summary>
    public static string NextString(this IRandom random, int length)
        => random.NextString(length, StringExtensions.Chars);

    /// <summary>
    /// Retorna una cadena aleatoria del largo especificado.
    /// </summary>
    public static string NextString(this IRandom random, int length, string chars)
        => random.EnumerateChars(chars).Take(length).CreateString(length);

    /// <summary>
    /// Retorna una secuencia de caracteres aleatorios.
    /// </summary>
    public static IEnumerable<char> EnumerateChars(this IRandom random, string chars, CancellationToken cancellationToken = default)
        {
        while (!cancellationToken.IsCancellationRequested)
            {
            yield return random.NextChar(chars);
            }
        }

    /// <summary>
    /// Retorna un caracter aleatorio.
    /// </summary>
    public static char NextAlphaNumericChar(this IRandom random)
        => random.NextChar(StringExtensions.AlphaNumericChars);

    /// <summary>
    /// Retorna un caracter aleatorio de la cadena.
    /// </summary>
    public static char NextChar(this IRandom random, string chars)
        => chars[random.Next(chars.Length)];

    /// <summary>
    /// Retorna un caracter aleatorio.
    /// </summary>
    public static char NextChar(this IRandom random, IReadOnlyList<char> chars)
        => random.NextItem(chars);

    /// <summary>
    /// Genera una fecha aleatoria.
    /// </summary>
    /// <remarks>
    /// La fecha generada es mayor o igual a 01/01/0001 00.00.00 y menor que DateTime.MaxValue: [01/01/0001 00.00.00, DateTime.MaxValue).
    /// </remarks>
    public static DateTime NextDateTime(this IRandom random)
        => random.NextDateTime(DateTime.MaxValue);

    /// <summary>
    /// Genera una fecha menor que maxValue.
    /// </summary>
    /// <remarks>
    /// La fecha generada es mayor o igual a 01/01/0001 00.00.00 y menor que maxValue: [01/01/0001 00.00.00, maxValue).
    /// </remarks>
    public static DateTime NextDateTime(this IRandom random, DateTime maxValue)
        => new DateTime(random.Next(maxValue.Ticks));

    /// <summary>
    /// Genera una fecha aleatoria.
    /// </summary>
    /// <remarks>
    /// La fecha generada es mayor o igual a minValue y menor que maxValue: [minValue, maxValue).
    /// </remarks>
    public static DateTime NextDateTime(this IRandom random, DateTime minValue, DateTime maxValue)
        => new DateTime(random.Next(minValue.Ticks, maxValue.Ticks));

    /// <summary>
    /// Genera un TimeSpan aleatorio.
    /// </summary>
    /// <remarks>
    /// El TimeSpan generado es mayor o igual a 01/01/0001 00.00.00 y menor que TimeSpan.MaxValue: [01/01/0001 00.00.00, TimeSpan.MaxValue).
    /// </remarks>
    public static TimeSpan NextTimeSpan(this IRandom random)
        => random.NextTimeSpan(TimeSpan.MaxValue);

    /// <summary>
    /// Genera un TimeSpan menor que maxValue.
    /// </summary>
    /// <remarks>
    /// El TimeSpan generado es mayor o igual a 1/1/1 00.00.00 y menor que maxValue: [1/1/1 00.00.00, maxValue).
    /// </remarks>
    public static TimeSpan NextTimeSpan(this IRandom random, TimeSpan maxValue)
        => new TimeSpan(random.Next(maxValue.Ticks));

    /// <summary>
    /// Genera un TimeSpan aleatorio.
    /// </summary>
    /// <remarks>
    /// El TimeSpan generado es mayor o igual a minValue y menor que maxValue: [minValue, maxValue).
    /// </remarks>
    public static TimeSpan NextTimeSpan(this IRandom random, TimeSpan minValue, TimeSpan maxValue)
        => new TimeSpan(random.Next(minValue.Ticks, maxValue.Ticks));

    public const int MaxByteValueExclusive = byte.MaxValue + 1;

    /// <summary>
    /// Fills the array with random bytes.
    /// </summary>
    public static void SetBytes(this IRandom random, Span<byte> buffer)
        {
        for (int i = 0; i < buffer.Length; i++)
            {
            buffer[i] = random.Next<byte>();
            }
        }

    public static byte[] GetBytes(this IRandom random, int length)
        {
        byte[] data = new byte[length];
        random.SetBytes(data);
        return data;
        }

    /// <summary>
    /// Retorna una secuencia de samples.
    /// </summary>
    public static IEnumerable<double> EnumerateSamples(this IRandom random, CancellationToken cancellationToken = default)
        {
        while (!cancellationToken.IsCancellationRequested)
            {
            yield return random.Sample();
            }
        }

    /// <summary>
    /// Retorna una secuencia de samples discretos.
    /// </summary>
    public static IEnumerable<uint> EnumerateDiscreteSamples(this IDiscreteRandom random, CancellationToken cancellationToken = default)
        {
        while (!cancellationToken.IsCancellationRequested)
            {
            yield return random.DiscreteSample();
            }
        }

    /// <summary>
    /// Implementation of Fisher-Yates-Durstenfeld shuffle.
    /// </summary>
    /// <remarks>
    /// Fuentes:
    /// http://s347.codeinspot.com/q/1079369
    /// http://en.wikipedia.org/wiki/Fisher-Yates_shuffle#The_modern_algorithm
    /// http://www.4guysfromrolla.com/articles/070710-1.aspx
    /// https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/RandomSubset.cs
    ///
    /// Sobre porque no usar enumerable.OrderBy(x => random.Next()) o sequence.OrderBy(elem => Guid.NewGuid()):
    /// http://stackoverflow.com/questions/3958011/in-linq-does-orderby-execute-the-comparing-function-only-once-or-execute-it-w
    /// http://stackoverflow.com/questions/1287567/is-using-random-and-orderby-a-good-shuffle-algorithm
    /// </remarks>
    public static void Shuffle<T>(this IRandom random, Span<T> span)
        {
        for (int i = span.Length-1; i > 0; i--)
            {
            ObjectExtensions.Swap(ref span[i], ref span[random.Next(0, i+1)]);
            }
        /*
        for (int i = 0; i < span.Length-1; i++)
            {
            ObjectExtensions.Swap(ref span[i], ref span[random.Next(i, span.Length)]);
            }
        */
        }

    /// <summary>
    /// Retorna aleatoriamente una tupla de items de la lista.
    /// </summary>
    /// <remarks>La tupla retornada puede estar formada por el mismo item duplicado.</remarks>
    public static (T, T) NextTuple<T>(this IRandom random, IReadOnlyList<T> list)
        => (random.NextItem(list), random.NextItem(list));

    /// <summary>
    /// Retorna aleatoriamente un item de la lista.
    /// </summary>
    public static T NextItem<T>(this IRandom random, IReadOnlyList<T> list)
        => list[random.NextIndex(list)];

    /// <summary>
    /// Retorna aleatoriamente un indice de la lista.
    /// </summary>
    public static int NextIndex<T>(this IRandom random, IReadOnlyCollection<T> list)
        => random.Next(list.Count);

    public static int NextMinOfTwoIndex<T>(this IRandom random, IReadOnlyList<T> list) where T : IComparable<T>
        => random.NextBestOfTwoIndex(list, (a, b) => a.CompareTo(b) < 0);

    public static int NextMaxOfTwoIndex<T>(this IRandom random, IReadOnlyList<T> list) where T : IComparable<T>
        => random.NextBestOfTwoIndex(list, (a, b) => a.CompareTo(b) > 0);

    /// <summary>
    /// Selects the better of two randomly chosen list indexes.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="random">The random number generator.</param>
    /// <param name="list">The list from which two elements are selected.</param>
    /// <param name="compare">The comparison function, must return true if the first element is better than the second.</param>
    /// <returns>The index of the selected element.</returns>
    /// <see cref="https://medium.com/the-intuition-project/load-balancing-the-intuition-behind-the-power-of-two-random-choices-6de2e139ac2f"/>
    public static int NextBestOfTwoIndex<T>(this IRandom random, IReadOnlyList<T> list, Func<T, T, bool> compare)
        {
        int first = random.Next(list.Count);
        int second = random.Next(list.Count);
        return compare(list[first], list[second]) ? first : second;
        }

    public static Range<int> NextRange(this IRandom random, Range<int> outerRange, int innerCount)
        {
        int start = random.Next(outerRange.Start, outerRange.End - innerCount + 2);
        return new Range<int>(start, start + innerCount - 1);
        }

    public static Range NextRange(this IRandom random, Range outerRange, int innerCount)
        {
        int start = random.Next(outerRange.Start.Value, outerRange.End.Value - innerCount + 2);
        return new Range(start, start + innerCount - 1);
        }

    /// <summary>
    /// Ejecuta el test chi-cuadrado sobre el generador.
    /// </summary>
    /// <param name="random">Generador</param>
    /// <param name="N">Cantidad de samples a realizar.</param>
    /// <param name="r">Intervalo de nros a generar: [0..r)</param>
    /// <remarks>Extraído del libro Algoritmos en C++ (Robert Sedgewick) - Cap. 35.</remarks>
    /// <returns>Resultado del test: se considera bueno si es r * 0.8 < result < r * 1.2 (r-+20%).</returns>
    public static float ChiSquareTest(this IRandom random, int N = 1000, int r = 100)
        {
        Span<int> f = r < 1024? stackalloc int[r]: new int[r];

        for (int i = 0; i < N; i++)
            {
            f[random.Next(r)]++;
            }

        int t = 0;
        for (int i = 0; i < r; i++)
            {
            t += f[i].Pow2();
            }

        return (float)r * t / N - N;
        }

    public static IDiscreteRandom ToDiscrete(this IRandom random)
        => new DiscreteRandomAdapter(random);

    public static IRandom ToRandom(this IDiscreteRandom discreteRandom)
        => new RandomDiscreteAdapter(discreteRandom);

    /// <summary>
    /// Lorem Ipsum Generator
    /// </summary>
    /// <remarks>
    /// "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit..."
    /// "There is no one who loves pain itself, who seeks after it and wants to have it, simply because it is pain..."
    ///
    /// The standard Lorem Ipsum passage, used since the 1500s
    /// "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
    /// "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam hendrerit nisi sed sollicitudin pellentesque. Nunc posuere purus rhoncus pulvinar aliquam. Ut aliquet tristique nisl vitae volutpat. Nulla aliquet porttitor venenatis. Donec a dui et dui fringilla consectetur id nec massa. Aliquam erat volutpat. Sed ut dui ut lacus dictum fermentum vel tincidunt neque. Sed sed lacinia lectus. Duis sit amet sodales felis. Duis nunc eros, mattis at dui ac, convallis semper risus. In adipiscing ultrices tellus, in suscipit massa vehicula eu."
    /// </remarks>
    /// <see cref="https://stackoverflow.com/questions/4286487/is-there-any-lorem-ipsum-generator-in-c"/>
    /// <see cref="https://www.lipsum.com/"/>
    /// <see cref="https://loremipsum.io/"/>
    /// <see cref="http://littlerunningbear.com/465/lorem-ipsum-for-ever/"/>
    public static readonly string[] LatinWords = ["lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua", "enim", "ad", "minim", "veniam", "quis", "nostrud", "exercitation", "ullamco", "laboris", "nisi", "aliquip", "ex", "ea", "commodo", "consequat", "duis", "aute", "irure", "in", "reprehenderit", "voluptate", "velit", "esse", "cillum", "eu", "fugiat", "nulla", "pariatur", "excepteur", "sint", "occaecat", "cupidatat", "non", "proident", "sunt", "culpa", "qui", "officia", "deserunt", "mollit", "anim", "id", "est", "laborum", "nam", "hendrerit", "sollicitudin", "pellentesque", "nunc", "posuere", "purus", "rhoncus", "pulvinar", "aliquam", "aliquet", "tristique", "nisl", "vitae", "volutpat", "porttitor", "venenatis", "donec", "a", "dui", "fringilla", "nec", "massa", "erat", "lacus", "dictum", "fermentum", "vel", "tincidunt", "neque", "lacinia", "lectus", "sodales", "felis", "eros", "mattis", "at", "ac", "convallis", "semper", "risus", "ultrices", "tellus", "suscipit", "vehicula", "ullamcorper", "dapibus", "sem", "cras", "pretium", "nibh", "eget", "efficitur", "auctor", "dignissim", "lobortis", "leo", "mollis", "varius", "arcu", "quam", "interdum", "justo", "luctus", "ultricies", "vivamus", "odio", "curabitur", "faucibus", "quisque", "orci", "bibendum", "sagittis", "praesent", "rutrum", "etiam", "urna", "molestie", "vestibulum", "libero", "morbi", "metus", "sapien", "ante", "primis", "cubilia", "curae", "mauris", "integer", "proin", "finibus", "turpis", "ligula", "porta", "laoreet", "mi", "augue", "imperdiet", "gravida", "viverra", "accumsan", "ornare", "placerat", "tortor", "iaculis", "suspendisse", "potenti", "tempus", "vulputate", "hac", "habitasse", "platea", "dictumst", "scelerisque", "blandit", "feugiat", "nullam", "elementum", "cursus", "fusce", "malesuada", "diam", "condimentum", "aenean", "maximus", "facilisis", "egestas", "congue", "facilisi", "fames", "phasellus", "eleifend", "nascetur", "ridiculus", "mus", "pharetra", "natoque", "penatibus", "magnis", "dis", "parturient", "montes", "vero", "eos", "accusamus", "iusto", "dignissimos", "ducimus", "blanditiis", "praesentium", "voluptatum", "deleniti", "atque", "corrupti", "quos", "dolores", "quas", "molestias", "excepturi", "obcaecati", "cupiditate", "provident", "similique", "mollitia", "animi", "dolorum", "fuga", "harum", "quidem", "rerum", "facilis", "expedita", "distinctio", "tempore", "cum", "soluta", "nobis", "eligendi", "optio", "cumque", "nihil", "impedit", "quo", "minus", "quod", "maxime", "placeat", "facere", "possimus", "omnis", "voluptas", "assumenda", "repellendus", "temporibus", "autem", "quibusdam", "aut", "officiis", "debitis", "necessitatibus", "saepe", "eveniet", "voluptates", "repudiandae", "molestiae", "recusandae", "itaque", "earum", "hic", "tenetur", "sapiente", "delectus", "reiciendis", "voluptatibus", "maiores", "alias", "consequatur", "perferendis", "doloribus", "asperiores", "repellat", "porro", "quisquam", "dolorem", "quia", "adipisci"];

    public static string LoremIpsum(this IRandom random, int wordsCount = 1000, double commaProbability = 0.1, double dotProbability = 0.05, double paragraphProbability = 0.01)
        => "Lorem ipsum dolor sit amet,".AsEnumerable()
            .Concat(
                EnumerableExtensions
                    .Repeat(() => random.NextItem(RandomExtensions.LatinWords), wordsCount)
                    .SelectMany(word => random.NextBool(commaProbability) && word.Length > 1 ? EnumerableExtensions.AsEnumerable(",", word) : word.AsEnumerable())
                    .SelectMany(word => random.NextBool(dotProbability) && word.Length > 1? EnumerableExtensions.AsEnumerable(".", word.Capitalize()) : word.AsEnumerable())
                    .SelectMany(word => EnumerableExtensions.AsEnumerable(random.NextBool(paragraphProbability) && word.Length > 1 ? String.Concat(".", Environment.NewLine, word.Capitalize()) : word))
                )
            .Interleave(" ")
            .Append(".")
            .Join()
            .Replace(" ,", ",")
            .Replace(" .", ".")
            .Replace(",.", ".");
    }
