#pragma warning disable IDE0071 // Simplify interpolation
#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.RegularExpressions;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace CoreClassLibrary.Tests;

public class StringTests(ITestOutputHelper outputHelper)
	{
	[Fact]
	// https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/string-interpolation
	public void NullInterpolation()
		{
		Assert.Equal("", $"{null}");
		Assert.Equal(" ", $"{null} ");
		Assert.Equal(" ", $"{null} {null}");
		Assert.Equal("-", $"{null}-{null}");
		}

    [Fact]
    public void InvariantInterpolation()
        {
		decimal value = 1.23M;

        Assert.Equal("1,23", $"{value}");
        Assert.Equal("1,23", $"{value.ToString()}");
        Assert.Equal("1,23", $"{value.ToString(CultureInfo.GetCultureInfo("es-AR"))}");

        Assert.Equal("1.23", $"{value.ToString(CultureInfo.InvariantCulture)}");
        Assert.Equal("1.23", string.Create(CultureInfo.InvariantCulture, $"{value}"));
        Assert.Equal("1.23", FormattableString.Invariant($"{value}"));
        }

    [Fact]
    public void InvariantFormatInterpolation()
        {
        decimal value = 1.235M;

        Assert.Equal("1,235", $"{value}");
        Assert.Equal("1,235", $"{value.ToString()}");
        Assert.Equal("1,24", $"{value.ToString("F2")}");
        Assert.Equal("1,24", $"{value.ToString("0.00")}");
        Assert.Equal("1,24", $"{value.ToString("0.00", CultureInfo.GetCultureInfo("es-AR"))}");
						 
        Assert.Equal("1.24", $"{value.ToString("0.00", CultureInfo.InvariantCulture)}");
        Assert.Equal("1.24", string.Create(CultureInfo.InvariantCulture, $"{value:0.00}"));
        Assert.Equal("1.24", FormattableString.Invariant($"{value:0.00}"));
        }

    [Fact]
	public void NewLine()
		=> Assert.Equal("\r\n", Environment.NewLine);

	[Theory]
	[InlineData("", "")]
	[InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZÁÉÍÓÚÀÈÌÒÙÄËÏÖÜÂÊÎÔÛÑabcdefghijklmnopqrstuvwxyzáéíóúàèìòùäëïöüâêîôûñ", "ABCDEFGHIJKLMNOPQRSTUVWXYZAEIOUAEIOUAEIOUAEIOUNabcdefghijklmnopqrstuvwxyzaeiouaeiouaeiouaeioun")]
	[InlineData("ABCabc", "ABCabc")]
	[InlineData("AEIOU", "AEIOU")]
	[InlineData("ÁÉÍÓÚ", "AEIOU")]
	[InlineData("ÂÊÎÔÛ", "AEIOU")]
	[InlineData("aeiou", "aeiou")]
	[InlineData("áéíóú", "aeiou")]
	[InlineData("123", "123")]
	[InlineData("Una buena prueba vale mil opiniones de expertos. Wernher von Braun (1912-1977), ingeniero mecánico y aeroespacial alemán.", "Una buena prueba vale mil opiniones de expertos. Wernher von Braun (1912-1977), ingeniero mecanico y aeroespacial aleman.")]
	public void RemoveDiacritics(string input, string expected)
		{
		outputHelper.WriteLine(input);
		outputHelper.WriteLine(input.Length.ToString());
		outputHelper.WriteLine("RemoveDiacritics");
		outputHelper.WriteLine(input.RemoveDiacritics());
		outputHelper.WriteLine(input.RemoveDiacritics().Length.ToString());
		outputHelper.WriteLine("");
		outputHelper.WriteLine("FormC");
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormC));
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormC).Length.ToString());
		outputHelper.WriteLine("FormD");
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormD));
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormD).Length.ToString());
		outputHelper.WriteLine("FormKC");
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormKC));
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormKC).Length.ToString());
		outputHelper.WriteLine("FormKD");
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormKD));
		outputHelper.WriteLine(input.Normalize(NormalizationForm.FormKD).Length.ToString());

		Assert.Equal(expected, input.RemoveDiacritics());
		}

	[Fact]
	// https://docs.microsoft.com/en-us/dotnet/api/system.string.normalize
	public void Normalization()
		{
		// Character c; combining characters acute and cedilla; character 3/4
		string s1 = new String(['\u0063', '\u0301', '\u0327', '\u00BE']);
		string divider = new String('-', 80);
		divider = String.Concat(Environment.NewLine, divider, Environment.NewLine);

		this.Show("s1", s1);
		outputHelper.WriteLine("");
		outputHelper.WriteLine("U+0063 = LATIN SMALL LETTER C");
		outputHelper.WriteLine("U+0301 = COMBINING ACUTE ACCENT");
		outputHelper.WriteLine("U+0327 = COMBINING CEDILLA");
		outputHelper.WriteLine("U+00BE = VULGAR FRACTION THREE QUARTERS");
		outputHelper.WriteLine(divider);

		outputHelper.WriteLine("A1) Is s1 normalized to the default form (Form C)?: {0}",
									 s1.IsNormalized());
		outputHelper.WriteLine("A2) Is s1 normalized to Form C?:  {0}",
									 s1.IsNormalized(NormalizationForm.FormC));
		outputHelper.WriteLine("A3) Is s1 normalized to Form D?:  {0}",
									 s1.IsNormalized(NormalizationForm.FormD));
		outputHelper.WriteLine("A4) Is s1 normalized to Form KC?: {0}",
									 s1.IsNormalized(NormalizationForm.FormKC));
		outputHelper.WriteLine("A5) Is s1 normalized to Form KD?: {0}",
									 s1.IsNormalized(NormalizationForm.FormKD));

		outputHelper.WriteLine(divider);

		outputHelper.WriteLine("Set string s2 to each normalized form of string s1.");
		outputHelper.WriteLine("");
		outputHelper.WriteLine("U+1E09 = LATIN SMALL LETTER C WITH CEDILLA AND ACUTE");
		outputHelper.WriteLine("U+0033 = DIGIT THREE");
		outputHelper.WriteLine("U+2044 = FRACTION SLASH");
		outputHelper.WriteLine("U+0034 = DIGIT FOUR");
		outputHelper.WriteLine(divider);

		string s2 = s1.Normalize();
		outputHelper.WriteLine("B1) Is s2 normalized to the default form (Form C)?: ");
		outputHelper.WriteLine(s2.IsNormalized().ToString());
		this.Show("s2", s2);
		outputHelper.WriteLine("");

		s2 = s1.Normalize(NormalizationForm.FormC);
		outputHelper.WriteLine("B2) Is s2 normalized to Form C?: ");
		outputHelper.WriteLine(s2.IsNormalized(NormalizationForm.FormC).ToString());
		this.Show("s2", s2);
		outputHelper.WriteLine("");

		s2 = s1.Normalize(NormalizationForm.FormD);
		outputHelper.WriteLine("B3) Is s2 normalized to Form D?: ");
		outputHelper.WriteLine(s2.IsNormalized(NormalizationForm.FormD).ToString());
		this.Show("s2", s2);
		outputHelper.WriteLine("");

		s2 = s1.Normalize(NormalizationForm.FormKC);
		outputHelper.WriteLine("B4) Is s2 normalized to Form KC?: ");
		outputHelper.WriteLine(s2.IsNormalized(NormalizationForm.FormKC).ToString());
		this.Show("s2", s2);
		outputHelper.WriteLine("");

		s2 = s1.Normalize(NormalizationForm.FormKD);
		outputHelper.WriteLine("B5) Is s2 normalized to Form KD?: ");
		outputHelper.WriteLine(s2.IsNormalized(NormalizationForm.FormKD).ToString());
		this.Show("s2", s2);
		outputHelper.WriteLine("");
		}

	private void Show(string title, string s)
		{
		outputHelper.WriteLine("Characters in string {0} = ", title);
		s.Do(outputHelper, (c, output) => output.WriteLine("{0:X4} ", (short) c));
		outputHelper.WriteLine("");
		}

	[Fact]
	public void Char()
		=> Assert.Equal("12", String.Concat('1', '2'));

	[Fact]
	public void CharStr()
		=> Assert.Equal("12", String.Concat("1", '2'));

	[Fact]
	public void EmptySubstring()
		=> Assert.Throws<ArgumentOutOfRangeException>(() => ""[..3]);

	[Fact]
	public void EmptySubstring2()
		=> Assert.Equal("", "".Substring2(0, 3));

	[Fact]
	public void Range()
		=> Assert.Throws<ArgumentOutOfRangeException>(() => "abcdef"[..20]);

	[Fact]
	public void Range2()
		=> Assert.Equal("abcdef", "abcdef".Substring2(0, 20));

	[Fact]
	public void Range3()
		=> Assert.Equal("BCDEF", "ABCDEF"[1..]);

	[Fact]
	public void MutableArray()
		{
		char[] arr = ['A', 'B'];

		string str = new string(arr);

		arr[0] = 'X';

		Assert.Equal("AB", str);
		}

	[Fact]
	public void MutableGuid()
		{
		Guid guid1 = Guid.NewGuid();

		byte[] arr = guid1.ToByteArray();

		arr[0]++;

		Guid guid2 = new Guid(arr);

		Assert.NotEqual(guid1, guid2);
		}

	[Theory]
	[InlineData("", true)]
	[InlineData(null, true)]
	[InlineData(" ", true)]
	[InlineData("     ", true)]
	[InlineData("A", false)]
	public void IsNullOrWhiteSpace(string? input, bool expected)
		=> Assert.Equal(expected, String.IsNullOrWhiteSpace(input));

	[Theory]
	[InlineData("true")]
	[InlineData("True")]
	[InlineData("TRUE")]
	[InlineData("trUe")]
	[InlineData("   true   ")]
	public void TrueParse(string value)
		=> Assert.True(bool.Parse(value));

	[Theory]
	[InlineData("false")]
	[InlineData("False")]
	[InlineData("FALSE")]
	[InlineData("FaLse")]
	[InlineData("  false ")]
	public void FalseParse(string value)
		=> Assert.False(bool.Parse(value));

	[Theory]
	[InlineData("true", true)]
	[InlineData("false", false)]
	[InlineData("TRUE", true)]
	[InlineData("FALSE", false)]
	[InlineData("True", true)]
	[InlineData("False", false)]
	[InlineData("trUe", true)]
	[InlineData("FaLse", false)]
	[InlineData("   true   ", true)]
	[InlineData("  false ", false)]
	[InlineData("1", true)]
	[InlineData("-1", true)]
	[InlineData("0", false)]
	[InlineData("t", true)]
	[InlineData("f", false)]
	[InlineData("T", true)]
	[InlineData("F", false)]
	public void ParseBoolean(string input, bool expected)
		=> Assert.Equal(expected, input.ParseBoolean());

	[Theory]
	[InlineData("123", 123)]
	[InlineData("  123  ", 123)]
	[InlineData("-123", -123)]
	[InlineData("  -123  ", -123)]
	public void ParseInt(string input, int expected)
		=> Assert.Equal(expected, int.Parse(input));


	[Theory]
	[InlineData("0010", 10)]
	public void IntParse(string input, int expected)
		{
		Assert.Equal(expected, int.Parse(input, CultureInfo.InvariantCulture));
		Assert.Equal(10.20M, decimal.Parse("0010.200", CultureInfo.InvariantCulture));
		}

	[Theory]
	[InlineData("0010", 10)]
	[InlineData("0010.200", 10.2)]
	public void DecimalParse(string input, decimal expected)
		=> Assert.Equal(expected, decimal.Parse(input, CultureInfo.InvariantCulture));

	[Theory]
	[InlineData("abcdef", "abc", "xyz", "xyzdef")]
	[InlineData("ABCdef", "abc", "xyz", "ABCdef")]
	[InlineData("A*BCdef", "a*bc", "xyz", "A*BCdef")]
	[InlineData("abcdef", "abc", "x*yz", "x*yzdef")]
	[InlineData("abcdef", "abc", "$", "$def")]
	[InlineData("abc\\def", "abc", "xyz", "xyz\\def")]
	[InlineData("for more information.", "more", "less", "for less information.")]
	[InlineData("for More information.", "more", "less", "for More information.")]
	[InlineData("for more information.", "More", "less", "for more information.")]
	[InlineData("for more information.", "MMM", "less", "for more information.")]
	public void Replace(string input, string search, string replacement, string expected)
		=> Assert.Equal(expected, input.Replace(search, replacement));

	[Theory]
	[InlineData("INSERT INTO Lote (FechaEstado, IdEstado, XferId, IdTipo, CbuOrigen, CuitOrigen) VALUES (@FechaEstado, @IdEstado, @XferId, @IdTipo, @CbuOrigen, @CuitOrigen); SELECT @@IDENTITY", "@FechaEstado", "'2018-06-27 14:26:14.384'", "INSERT INTO Lote (FechaEstado, IdEstado, XferId, IdTipo, CbuOrigen, CuitOrigen) VALUES ('2018-06-27 14:26:14.384', @IdEstado, @XferId, @IdTipo, @CbuOrigen, @CuitOrigen); SELECT @@IDENTITY")]
	public void ReplaceSqlParams(string input, string search, string replacement, string expected)
		=> Assert.Equal(expected, Regex.Replace(input, String.Concat(search, "\\b"), replacement, RegexOptions.Compiled | RegexOptions.CultureInvariant));

	[Theory]
	[InlineData("", "", 0, "")]
	[InlineData("", "", -10, "")]
	[InlineData("", "", 10, "")]
	[InlineData("", "A", 0, "A")]
	[InlineData("", "A", -10, "A")]
	[InlineData("", "A", 10, "A")]
	[InlineData("---", "A", 0, "A---")]
	[InlineData("---", "A", 1, "-A--")]
	[InlineData("---", "A", -1, "--A-")]
	[InlineData("---", "A", 2, "--A-")]
	[InlineData("---", "A", -2, "-A--")]
	[InlineData("---", "A", 3, "---A")]
	[InlineData("---", "A", -3, "A---")]
	[InlineData("---", "A", 4, "---A")]
	[InlineData("---", "A", -4, "A---")]
	[InlineData("---", "A", -10, "A---")]
	[InlineData("---", "A", 10, "---A")]
	public void Insert2(string input, string value, int startIndex, string output)
		=> outputHelper.Equal(output, input.Insert2(value, startIndex));

	[Theory]
	[InlineData("", 0, 0, "")]
	[InlineData("", -10, 0, "")]
	[InlineData("", 10, 0, "")]
	[InlineData("", 0, -10, "")]
	[InlineData("", 0, 10, "")]
	[InlineData("", 10, 10, "")]
	[InlineData("", 10, -10, "")]
	[InlineData("", -10, 10, "")]
	[InlineData("", -10, -10, "")]
	[InlineData("---", 0, 0, "")]
	[InlineData("---", -10, 0, "")]
	[InlineData("---", 10, 0, "---")]
	[InlineData("---", 0, -10, "---")]
	[InlineData("---", 0, 10, "")]
	[InlineData("---", 10, 10, "---")]
	[InlineData("---", 10, -10, "")]
	[InlineData("---", -10, 5, "---")]
	[InlineData("---", -10, 10, "")]
	[InlineData("---", -10, 20, "")]
	[InlineData("---", -10, -10, "---")]
	[InlineData("ABC", 0, 0, "")]
	[InlineData("ABC", 0, 1, "BC")]
	[InlineData("ABC", 0, 2, "C")]
	[InlineData("ABC", 0, 3, "")]
	[InlineData("ABC", 0, 4, "")]
	[InlineData("ABC", 0, -1, "ABC")]
	[InlineData("ABC", 0, -2, "ABC")]
	[InlineData("ABC", 0, -3, "ABC")]
	[InlineData("ABC", 3, -1, "AB")]
	[InlineData("ABC", 3, -2, "A")]
	[InlineData("ABC", 3, -3, "")]
	[InlineData("ABC", 1, 0, "A")]
	[InlineData("ABC", 1, 1, "AC")]
	[InlineData("ABC", 1, 2, "A")]
	[InlineData("ABC", 1, 3, "A")]
	[InlineData("ABC", 1, 4, "A")]
	[InlineData("ABC", 1, -1, "BC")]
	[InlineData("ABC", 1, -2, "BC")]
	[InlineData("ABC", 1, -3, "BC")]
	[InlineData("ABC", -3, -1, "ABC")]
	[InlineData("ABC", -3, -2, "ABC")]
	[InlineData("ABC", -3, -3, "ABC")]
	[InlineData("ABC", -1, 0, "AB")]
	[InlineData("ABC", -1, 1, "AB")]
	[InlineData("ABC", -1, 2, "AB")]
	[InlineData("ABC", -1, 3, "AB")]
	[InlineData("ABC", -1, 4, "AB")]
	[InlineData("ABC", -1, -1, "AC")]
	[InlineData("ABC", -1, -2, "C")]
	[InlineData("ABC", -1, -3, "C")]
	[InlineData("ABC", 2, -1, "AC")]
	public void Remove2(string input, int startIndex, int count, string output)
		=> outputHelper.Equal(output, input.Remove2(startIndex, count));

	[Theory]
	[InlineData("", 0, 0, "")]
	[InlineData("", -10, 0, "")]
	[InlineData("", 10, 0, "")]
	[InlineData("", 0, -10, "")]
	[InlineData("", 0, 10, "")]
	[InlineData("", 10, 10, "")]
	[InlineData("", 10, -10, "")]
	[InlineData("", -10, 10, "")]
	[InlineData("", -10, -10, "")]
	[InlineData("---", 0, 0, "---")]
	[InlineData("---", -10, 0, "---")]
	[InlineData("---", 10, 0, "")]
	[InlineData("---", 0, -10, "")]
	[InlineData("---", 0, 10, "---")]
	[InlineData("---", 10, 10, "")]
	[InlineData("---", 10, -10, "---")]
	[InlineData("---", -10, 5, "")]
	[InlineData("---", -10, 10, "---")]
	[InlineData("---", -10, 20, "---")]
	[InlineData("---", -10, -10, "")]
	[InlineData("ABC", 0, 0, "ABC")]
	[InlineData("ABC", 0, 1, "A")]
	[InlineData("ABC", 0, 2, "AB")]
	[InlineData("ABC", 0, 3, "ABC")]
	[InlineData("ABC", 0, 4, "ABC")]
	[InlineData("ABC", 0, -1, "")]
	[InlineData("ABC", 0, -2, "")]
	[InlineData("ABC", 0, -3, "")]
	[InlineData("ABC", 3, -1, "C")]
	[InlineData("ABC", 3, -2, "BC")]
	[InlineData("ABC", 3, -3, "ABC")]
	[InlineData("ABC", 1, 0, "BC")]
	[InlineData("ABC", 1, 1, "B")]
	[InlineData("ABC", 1, 2, "BC")]
	[InlineData("ABC", 1, 3, "BC")]
	[InlineData("ABC", 1, 4, "BC")]
	[InlineData("ABC", 1, -1, "A")]
	[InlineData("ABC", 1, -2, "A")]
	[InlineData("ABC", 1, -3, "A")]
	[InlineData("ABC", -3, -1, "")]
	[InlineData("ABC", -3, -2, "")]
	[InlineData("ABC", -3, -3, "")]
	[InlineData("ABC", -1, 0, "C")]
	[InlineData("ABC", -1, 1, "C")]
	[InlineData("ABC", -1, 2, "C")]
	[InlineData("ABC", -1, 3, "C")]
	[InlineData("ABC", -1, 4, "C")]
	[InlineData("ABC", -1, -1, "B")]
	[InlineData("ABC", -1, -2, "AB")]
	[InlineData("ABC", -1, -3, "AB")]
	[InlineData("ABC", 2, -1, "B")]
	public void GetSubstringFromStartLength(string input, int startIndex, int length, string output)
		=> outputHelper.Equal(output, input.Substring2(startIndex, length));

	[Theory]
	[InlineData("  6EQUJ5 11  1 ", "6EQUJ5", true)] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
	[InlineData("abcdef", "abc", true)]
	[InlineData("ABCdef", "abc", false)]
	[InlineData("A*BCdef", "a*bc", false)]
	[InlineData("abcdef", "ABc", false)]
	[InlineData("abcdef", "a*bc", false)]
	[InlineData("abc\\def", "Abc", false)]
	[InlineData("for more information.", "more", true)]
	[InlineData("for More information.", "more", false)]
	[InlineData("for more information.", "More", false)]
	[InlineData("for more information.", "MMM", false)]
	public void Contains(string input, string search, bool expected)
		=> Assert.Equal(expected, input.Contains(search));

	[Theory]
	[InlineData("  6EQUJ5 11  1 ", "6EQUJ5", true)] //Wow! signal: https://en.wikipedia.org/wiki/Wow!_signal
	[InlineData("abcdef", "abc", true)]
	[InlineData("ABCdef", "abc", true)]
	[InlineData("A*BCdef", "a*bc", true)]
	[InlineData("abcdef", "ABc", true)]
	[InlineData("abcdef", "a*bc", false)]
	[InlineData("abc\\def", "Abc", true)]
	[InlineData("for more information.", "more", true)]
	[InlineData("for More information.", "more", true)]
	[InlineData("for more information.", "More", true)]
	[InlineData("for more information.", "MMM", false)]
	public void ContainsIgnoringCase(string input, string search, bool expected)
		=> Assert.Equal(expected, input.ContainsIgnoringCase(search));

	[Theory]
	[InlineData("sql/as/select * from table", "/", 3, 0, "sql")]
	[InlineData("sql/as/select * from table", "/", 3, 1, "as")]
	[InlineData("/sql/as/select * from table", "/", 4, 0, "")]
	[InlineData("/sql/as/select * from table", "/", 4, 1, "sql")]
	[InlineData("/sql/as/select * from table", "/", 4, 2, "as")]
	[InlineData("/sql/as/select * from table", "/", 4, 3, "select * from table")]
	[InlineData("/sql/as/select * from table", "/", 3, 1, "sql")]
	public void Split(string input, string sep, int count, int i, string expected)
		=> Assert.Equal(expected, input.Split(sep, count)[i]);

	[Theory]
	[InlineData("A", "B", "AB")]
	public void AppendNull(string input, string input2, string expected)
		{
		using PooledStringBuilder builder = new PooledStringBuilder();
		Assert.Equal(expected, builder.StringBuilder.Append(input).Append((string?) null).Append(input2).ToString());
		}

	[Theory]
	[InlineData("Bohe Gabriel             -", 25, "Bohe Gabriel             ")]
	[InlineData("Barraza Gonzalo", 25, "Barraza Gonzalo          ")]
	[InlineData("", 25, "                         ")]
	public void AlignLeftOrTruncate(string input, int width, string expected)
		=> Assert.Equal(expected, input.AlignLeftOrTruncate(width));

	[Theory]
	[InlineData(0, "0000000000000", "0000000000000")]
	[InlineData(1, "0000000000000", "0000000000001")]
	[InlineData(-1, "0000000000000", "-0000000000001")]
	public void IntToString(int input, string format, string expected)
		=> Assert.Equal(expected, input.ToString(format));

	[Theory]
	[InlineData("", "", 0)]
	[InlineData("\n", "", 1)]
	[InlineData("\n\n", "", 2)]
	[InlineData("\nA", "", 2)]
	[InlineData("A", "A", 1)]
	[InlineData("A\n", "A", 1)]
	[InlineData("A\nB", "A", 2)]
	[InlineData("A\nB\n", "A", 2)]
	public void GetLines(string input, string firstLine, int lineCount)
		{
		using StringReader reader = new StringReader(input);
		string[] lines = reader.ReadLines().ToArray();

		Assert.Equal(lineCount, lines.Length);

		if (lineCount > 0)
			{
			Assert.Equal(firstLine, lines[0]);
			}
		}

	[Theory]
	[InlineData("aa[b]cc", "[", "]", "[b]")]
	[InlineData("aa[b]cc", "", "]", "aa[b]")]
	[InlineData("aa[b]cc", "[", "", "[b]cc")]
	[InlineData("aa[b]cc", "", "", "aa[b]cc")]
	[InlineData("aa[]cc", "[", "]", "[]")]
	[InlineData("[b]", "[", "]", "[b]")]
	[InlineData("abc", "[", "]", "[]")]
	[InlineData("", "[", "]", "[]")]
	public void GetTextBetweenTokensIncluded(string input, string leftToken, string rightToken, string result)
		=> Assert.Equal(result, input.GetTextBetweenTokensIncluded(leftToken, rightToken));

	[Theory]
	[InlineData("aa[b]cc[dd]eee", "[", "]", "b,dd")]
	[InlineData("aa[]cc[]ee", "[", "]", ",")]
	public void GetAllTextBetweenTokens(string input, string leftToken, string rightToken, string result)
		=> Assert.Equal(result.Split(','), input.GetAllTextBetweenTokens(leftToken, rightToken).ToArray());

	[Theory]
	[InlineData("aa[b]cc", "[", "]", "b")]
	[InlineData("aa[b]cc", "", "]", "aa[b")]
	[InlineData("aa[b]cc", "[", "", "b]cc")]
	[InlineData("aa[b]cc", "", "", "aa[b]cc")]
	[InlineData("aa[]cc", "[", "]", "")]
	[InlineData("[b]", "[", "]", "b")]
	[InlineData("abc", "[", "]", "")]
	[InlineData("", "[", "]", "")]
	[InlineData("aa[b]cc[dd]eee", "[", "]", "b")]
	[InlineData("aa[]cc[]ee", "[", "]", "")]
	[InlineData("aa[bcc", "[", "]", "")]
	[InlineData("aa[bcc", "", "]", "")]
	[InlineData("aa[bcc", "[", "", "bcc")]
	[InlineData("[b", "[", "]", "")]
	[InlineData("ab]c", "[", "]", "")]
	[InlineData("", "", "", "")]
	[InlineData("aa[bcc[dd]eee", "[", "]", "bcc[dd")]
	[InlineData("aa]cc[]ee", "[", "]", "")]
	public void GetTextBetweenTokens(string input, string leftToken, string rightToken, string result)
		=> Assert.Equal(result, input.GetTextBetweenTokens(leftToken, rightToken));

	[Fact]
	public void TestToString()
		{
		Assert.Matches("12", 12.ToString(""));
		Assert.Matches("12", 12.ToString("", CultureInfo.InvariantCulture));
		Assert.Matches("12", 12.ToString("0"));
		Assert.Matches("12", 12.ToString("00"));
		Assert.Matches("012", 12.ToString("000"));
		Assert.Matches("0012", 12.ToString("0000"));
		Assert.Matches("123456", 123456.ToString("0000"));
		}

	[Fact]
	public void New()
		{
		Assert.Equal("", new string('{', 0));
		Assert.Equal("{", new string('{', 1));
		Assert.Equal("{{", new string('{', 2));
		}

	[Fact]
	public void GuidToString()
		{
		Guid guid = Guid.NewGuid();

		outputHelper.WriteLine(guid.ToString());
		outputHelper.WriteLine(guid.ToString("N"));
		outputHelper.WriteLine(guid.ToString("d"));
		outputHelper.WriteLine(guid.ToString("D"));

		Assert.Equal(Guid.Empty, new Guid());
		Assert.NotEqual(Guid.Empty, guid);
		}

	public class Data
		{
		public int Id { get; set; }
		public string? Name { get; set; }

		public Data? Other { get; set; }
		}

	[Fact]
	public void RepeatAppendABC()
		=> Assert.Equal("ABCABCABC", "ABC".RepeatAppend(3));

	[Theory]
	[InlineData("Hola mundo")]
	public void RepeatAppend(string text)
		=> Assert.Equal(text.Length * 2, text.RepeatAppend(2).Length);

	[Theory]
	[InlineData("1234657890")]
	public void Base64(string input)
		{
		byte[] data = StringExtensions.UTF8Encoding.GetBytes(input);
		string base64 = Convert.ToBase64String(data);
		
		Assert.Equal(data, Convert.FromBase64String(base64));

		outputHelper.WriteLine(base64);
		}

	[Theory]
    [InlineData("", 0)]
    [InlineData("for", 1)]
    [InlineData("for\r", 1)]
    [InlineData("for\r\n", 1)]
    [InlineData("for\n ", 2)]
    [InlineData("for\rmore", 2)]
    [InlineData("for\nmore ", 2)]
    [InlineData("for\r\nmore", 2)]
    [InlineData("for\r\nmore\r\ninformation", 3)]
	[InlineData(@"1
		2
		3
		4
		5", 5)]
	public void ReadLines(string input, int count)
		=> Assert.Equal(count, input.ReadLines().Count());


	[Fact]
	public void Uri()
		{
		string strUri = "http://localhost";
		Uri uri = new Uri(strUri);
		
		Assert.Equal(strUri, uri.OriginalString);
		Assert.Equal(String.Concat(strUri, "/"), uri.ToString());
		}

	[Theory]
	[InlineData("", false)]
	[InlineData(" ", false)]
	[InlineData("a a", false)]
	[InlineData("AAA", true)]
	[InlineData("aaa", true)]
	[InlineData("Aaa", true)]
	[InlineData("Aa1", true)]
	[InlineData("aa1", true)]
	[InlineData("_", false)]
	[InlineData("_a", false)]
	[InlineData("_1", false)]
	[InlineData("_a1", false)]
	[InlineData("1aa", true)]
	[InlineData("1a1", true)]
	[InlineData("1_1", false)]
	[InlineData("aaa/", false)]
	[InlineData("aaa\\", false)]
	public void IsAlphaNumericTest2(string input, bool result) 
		=> Assert.Equal(result, input.IsAlphaNumeric());

	[Theory]
	[InlineData("", false)]
	[InlineData(" ", false)]
	[InlineData("a a", false)]
	[InlineData("AAA", false)]
	[InlineData("aaa", false)]
	[InlineData("Aaa", false)]
	[InlineData("Aa1", false)]
	[InlineData("aa1", false)]
	[InlineData("_", true)]
	[InlineData("_a", true)]
	[InlineData("_1", true)]
	[InlineData("_a1", true)]
	[InlineData("1aa", false)]
	[InlineData("1a1", false)]
	[InlineData("1_1", true)]
	[InlineData("aaa/", true)]
	[InlineData("aaa\\", true)]
	[InlineData("\"but", true)]
	public void ContainsSymbolsTest(string input, bool result) 
		=> Assert.Equal(result, input.ContainsSymbols());

	[Theory]
	[InlineData("AAA", true)]
	[InlineData("aaa", true)]
	[InlineData("Aaa", true)]
	[InlineData("Aa1", true)]
	[InlineData("aa1", true)]
	[InlineData("_", false)]
	[InlineData("_a", false)]
	[InlineData("_1", false)]
	[InlineData("_a1", false)]
	[InlineData("1aa", true)]
	[InlineData("1a1", true)]
	[InlineData("1_1", false)]
	[InlineData("aaa/", false)]
	[InlineData("aaa\\", false)]
	public void IsAlphaNumericSymbols(string input, bool result)
		{
		Assert.Equal(result, input.IsAlphaNumeric());
		Assert.Equal(!result, input.ContainsSymbols());
		}

	[Theory]
	[InlineData("", false)]
	[InlineData(" ", false)]
	[InlineData("a a", false)]
	[InlineData("AAA", true)]
	[InlineData("aaa", true)]
	[InlineData("Aaa", true)]
	[InlineData("Aa1", true)]
	[InlineData("aa1", true)]
	[InlineData("_", true)]
	[InlineData("_a", true)]
	[InlineData("_1", true)]
	[InlineData("_a1", true)]
	[InlineData("1aa", false)]
	[InlineData("1a1", false)]
	[InlineData("1_1", false)]
	[InlineData("aaa/", false)]
	[InlineData("aaa\\", false)]
	public void IsIdentifier(string input, bool result) 
		=> Assert.Equal(result, input.IsIdentifier());

	[Theory]
	[InlineData("", false)]
	[InlineData(" ", false)]
	[InlineData("AAA", true)]
	[InlineData("aaa", true)]
	[InlineData("Aaa", true)]
	[InlineData("Aa1", true)]
	[InlineData("aa1", true)]
	[InlineData("_", true)]
	[InlineData("_a", true)]
	[InlineData("_1", true)]
	[InlineData("_a1", true)]
	[InlineData("1aa", false)]
	[InlineData("1a1", false)]
	[InlineData("1_1", false)]
	[InlineData("aaa/", false)]
	[InlineData("aaa\\", false)]
	public void ValidateIdentifier(string input, bool result)
		{
		if(result)
			{
			Assert.Equal(input, input.ValidateIdentifier());
			}
		else
			{
			Assert.Throws<InvalidOperationException>(input.ValidateIdentifier);
			}
		}

	[Theory]
	[InlineData("", "")]
	[InlineData(" ", " ")]
	[InlineData("AAA", "aaa")]
	[InlineData("AAA_BBB", "aaa bbb")]
	[InlineData("CBU_BENEFICIARIO", "cbu beneficiario")]
	[InlineData("CUIT_BENEFICIARIO", "cuit beneficiario")]
	[InlineData("MONTO", "monto")]
	[InlineData("DESCRIPCION" , "descripcion")]
	[InlineData("MOTIVO", "motivo")]
	[InlineData("CORREO_BENEFICIARIO", "correo beneficiario")]
	public void FromConstCase(string input, string result)
		{
		string output = input.FromConstCase();
		outputHelper.WriteLine(output);
		Assert.Equal(result, output);
		}

	[Theory]
	[InlineData("", "")]
	[InlineData("a", "a")]
	[InlineData("aaa", "aaa")]
	[InlineData("aaa bbb", "aaaBbb")]
	public void ToCamelCase(string input, string result)
		{
		string output = input.ToCamelCase();
		outputHelper.WriteLine(output);
		Assert.Equal(result, output);
		}

	[Theory]
	[InlineData("", "")]
	[InlineData("a", "A")]
	[InlineData("aaa", "Aaa")]
	[InlineData("aaa bbb", "AaaBbb")]
	[InlineData("NRO TARJETA", "NroTarjeta")]
	[InlineData("IMPORTE", "Importe")]
	[InlineData("PREFIJO", "Prefijo")]
	[InlineData("MARCA", "Marca")]
	[InlineData("NRO CUENTA", "NroCuenta")]
	[InlineData("DIGITO VERIFICADOR", "DigitoVerificador")]
	public void ToPascalCase(string input, string result)
		{
		string output = input.ToPascalCase();
		outputHelper.WriteLine(output);
		Assert.Equal(result, output);
		}

	[Theory]
	[InlineData("", "")]
	[InlineData("A", "a")]
	[InlineData("AAA", "aaa")]
	[InlineData("AAA_BBB", "aaaBbb")]
	[InlineData("CBU_BENEFICIARIO", "cbuBeneficiario")]
	[InlineData("CUIT_BENEFICIARIO", "cuitBeneficiario")]
	[InlineData("MONTO", "monto")]
	[InlineData("DESCRIPCION" , "descripcion")]
	[InlineData("MOTIVO", "motivo")]
	[InlineData("CORREO_BENEFICIARIO", "correoBeneficiario")]
	public void FromConstToCamelCase(string input, string result)
		{
		string output = input.FromConstCase().ToCamelCase();
		outputHelper.WriteLine(output);
		Assert.Equal(result, output);
		}

	[Theory]
	[InlineData("", "")]
	[InlineData("A", "A")]
	[InlineData("AAA", "Aaa")]
	[InlineData("AAA_BBB", "AaaBbb")]
	[InlineData("CBU_BENEFICIARIO", "CbuBeneficiario")]
	[InlineData("CUIT_BENEFICIARIO", "CuitBeneficiario")]
	[InlineData("MONTO", "Monto")]
	[InlineData("DESCRIPCION" , "Descripcion")]
	[InlineData("MOTIVO", "Motivo")]
	[InlineData("CORREO_BENEFICIARIO", "CorreoBeneficiario")]
	public void FromConstToPascalCase(string input, string result)
		{
		string output = input.FromConstCase().ToPascalCase();
		outputHelper.WriteLine(output);
		Assert.Equal(result, output);
		}    
	}
