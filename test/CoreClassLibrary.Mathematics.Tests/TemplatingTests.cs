
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.StateMachines;
using CoreClassLibrary.Mathematics.StateMachines.Versatile;
using CoreClassLibrary.Mathematics.StateMachines.Versatile.Templating;
using CoreClassLibrary.Objects.Pooling;
using CoreClassLibrary.RegularExpressions.Templating;
using CoreClassLibrary.Tests;
using System.Globalization;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class TemplatingTests(ITestOutputHelper output)
    {
    public record MessageEntry(DateTime DateTime, string Message);
    public record LogEntry(DateTimeOffset DateTime, int Level, int EventId, string Name, string State, string Message, string Exception);

    [Fact]
    public void FormatStateTableWrite()
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        FormatStateTransitionTable stateTable = new FormatStateTransitionTable(new FormatWriter(new { Data = "A", CurrentTime = DateTime.Now }, outputWriter, keyBuilder.StringBuilder));

        output.WriteLine(stateTable.Write(States.Labels));
        }

    [Fact]
    public void ParamStateTableWrite()
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        ParamStateTransitionTable stateTable = new ParamStateTransitionTable(new ParamWriter(new { Data = "A", CurrentTime = DateTime.Now }, outputWriter, keyBuilder.StringBuilder));

        output.WriteLine(stateTable.Write(States.Labels));
        }

    [Theory]
    [InlineData("A{}C", "A{}C", "X", "1975-11-12")]
    [InlineData("A{{}C", "A{{}C", "X", "1975-11-12")]
    [InlineData("A{{}}C", "A{{}}C", "X", "1975-11-12")]
    [InlineData("A", "A", "X", "1975-11-12")]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("ABC", "A{Data}C", "B", "1975-11-12")]
    [InlineData("B", "{Data}", "B", "1975-11-12")]
    [InlineData("{B}", "{{Data}}", "B", "1975-11-12")]
    [InlineData("A{B}C", "A{{Data}}C", "B", "1975-11-12")]
    [InlineData("A{{B}}C", "A{{{Data}}}C", "B", "1975-11-12")]
    [InlineData("BC", "{Data}C", "B", "1975-11-12")]
    [InlineData("AB", "A{Data}", "B", "1975-11-12")]
    [InlineData("{ABC}", "{A{Data}C}", "B", "1975-11-12")]
    [InlineData("{A{B}C}", "{A{{Data}}C}", "B", "1975-11-12")]
    [InlineData("{A{{Otro}}C}", "{A{{Otro}}C}", "B", "1975-11-12")]
    [InlineData("A{", "A{", "B", "1975-11-12")]
    [InlineData("{A{Data", "{A{Data", "B", "1975-11-12")]
    [InlineData("A{Data", "A{Data", "B", "1975-11-12")]
    [InlineData("A{Data{", "A{Data{", "B", "1975-11-12")]
    [InlineData("A{Data{Data", "A{Data{Data", "B", "1975-11-12")]
    [InlineData("A{DataB", "A{Data{Data}", "B", "1975-11-12")]
    [InlineData("AB{Data", "A{Data}{Data", "B", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "{CurrentTime} - {Data}", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "{CurrentTime} - [{Data}]", null, "1975-11-12")]
    [InlineData("{CurrentTime:yyyy-MM-dd} - ABC", "{CurrentTime:yyyy-MM-dd} - {Data}", "ABC", "1975-11-12")]
    [InlineData("{{CurrentTime:yyyy-MM-dd}} - {{ABC}}", "{{CurrentTime:yyyy-MM-dd}} - {{{Data}}}", "ABC", "1975-11-12")]
    public void FormatExtensions(string expected, string format, string? data, string datetime)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        FormatWriter writer = new FormatWriter(new { Data = data, CurrentTime = DateTime.Parse(datetime) }, outputWriter, keyBuilder.StringBuilder);

        writer.Process(format);

        output.Equal(expected, outputBuilder.ToString());
        }

    [Theory]
    [InlineData("A@C", "A@C", "X", "1975-11-12")]
    [InlineData("A@@C", "A@@C", "X", "1975-11-12")]
    [InlineData("A@.@C", "A@.@C", "X", "1975-11-12")]
    [InlineData("@A@C@", "@A@C@", "X", "1975-11-12")]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("A@.B C", "A@.@Data C", "B", "1975-11-12")]
    [InlineData("AB C", "A@Data C", "B", "1975-11-12")]
    [InlineData("AB)C", "A@Data)C", "B", "1975-11-12")]
    [InlineData("AB,C", "A@Data,C", "B", "1975-11-12")]
    [InlineData("A@Otro,C", "A@Otro,C", "B", "1975-11-12")]
    [InlineData("A@DataC", "A@DataC", "B", "1975-11-12")]
    [InlineData("B", "@Data", "B", "1975-11-12")]
    [InlineData("@B", "@@Data", "B", "1975-11-12")]
    [InlineData("A@B.C", "A@@Data.C", "B", "1975-11-12")]
    [InlineData("A@@B.C", "A@@@Data.C", "B", "1975-11-12")]
    [InlineData("@DataC", "@DataC", "B", "1975-11-12")]
    [InlineData("B.C", "@Data.C", "B", "1975-11-12")]
    [InlineData("AB", "A@Data", "B", "1975-11-12")]
    [InlineData("@A@DataC", "@A@DataC", "B", "1975-11-12")]
    [InlineData("@AB.C", "@A@Data.C", "B", "1975-11-12")]
    [InlineData("@A@B,C", "@A@@Data,C", "B", "1975-11-12")]
    [InlineData("@A@@Otro.C", "@A@@Otro.C", "B", "1975-11-12")]
    [InlineData("A@", "A@", "B", "1975-11-12")]
    [InlineData("@A@Dat", "@A@Dat", "B", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "@CurrentTime - @Data", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "@CurrentTime - [@Data]", null, "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - @otraProp - ABC", "@CurrentTime - @otraProp - @Data", "ABC", "1975-11-12")]
    [InlineData("@1975-11-12 00:00:00.000 - @ABC", "@@CurrentTime - @@Data", "ABC", "1975-11-12")]
    [InlineData("@@ 1975-11-12 00:00:00.000 @-@ ABC @@", "@@ @CurrentTime @-@ @Data @@", "ABC", "1975-11-12")]
    public void ParamExtensions(string expected, string format, string? data, string datetime)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        ParamWriter writer = new ParamWriter(new { Data = data, CurrentTime = DateTime.Parse(datetime) }, outputWriter, keyBuilder.StringBuilder);

        writer.Process(format);

        output.Equal(expected, outputBuilder.ToString());
        }

    [Theory]
    [InlineData("A{}C", "A{}C", "X", "1975-11-12")]
    [InlineData("A{{}C", "A{{}C", "X", "1975-11-12")]
    [InlineData("A{{}}C", "A{{}}C", "X", "1975-11-12")]
    [InlineData("A", "A", "X", "1975-11-12")]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("ABC", "A{Data}C", "B", "1975-11-12")]
    [InlineData("B", "{Data}", "B", "1975-11-12")]
    [InlineData("{B}", "{{Data}}", "B", "1975-11-12")]
    [InlineData("A{B}C", "A{{Data}}C", "B", "1975-11-12")]
    [InlineData("A{{B}}C", "A{{{Data}}}C", "B", "1975-11-12")]
    [InlineData("BC", "{Data}C", "B", "1975-11-12")]
    [InlineData("AB", "A{Data}", "B", "1975-11-12")]
    [InlineData("{ABC}", "{A{Data}C}", "B", "1975-11-12")]
    [InlineData("{A{B}C}", "{A{{Data}}C}", "B", "1975-11-12")]
    [InlineData("{A{{Otro}}C}", "{A{{Otro}}C}", "B", "1975-11-12")]
    [InlineData("A{", "A{", "B", "1975-11-12")]
    [InlineData("{A{Data", "{A{Data", "B", "1975-11-12")]
    [InlineData("A{Data", "A{Data", "B", "1975-11-12")]
    [InlineData("A{Data{", "A{Data{", "B", "1975-11-12")]
    [InlineData("A{Data{Data", "A{Data{Data", "B", "1975-11-12")]
    [InlineData("A{DataB", "A{Data{Data}", "B", "1975-11-12")]
    [InlineData("AB{Data", "A{Data}{Data", "B", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "{CurrentTime} - {Data}", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "{CurrentTime} - [{Data}]", null, "1975-11-12")]
    [InlineData("{CurrentTime:yyyy-MM-dd} - ABC", "{CurrentTime:yyyy-MM-dd} - {Data}", "ABC", "1975-11-12")]
    [InlineData("{{CurrentTime:yyyy-MM-dd}} - {{ABC}}", "{{CurrentTime:yyyy-MM-dd}} - {{{Data}}}", "ABC", "1975-11-12")]
    public void FormatSwitchStateMachine(string expected, string format, string? data, string datetime)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        FormatSwitchStateMachine stateMachine = new FormatSwitchStateMachine(new FormatWriter(new { Data = data, CurrentTime = DateTime.Parse(datetime) }, outputWriter, keyBuilder.StringBuilder));

        format.Where(c => c != '\0').Append('\0').ForEach<char, FormatSwitchStateMachine>(stateMachine, (b, sm) => sm.Process(b), cancellationToken: TestContext.Current.CancellationToken);

        output.Equal(expected, outputBuilder.ToString());
        }

    [Theory]
    [InlineData("A{}C", "A{}C", "X", "1975-11-12")]
    [InlineData("A{{}C", "A{{}C", "X", "1975-11-12")]
    [InlineData("A{{}}C", "A{{}}C", "X", "1975-11-12")]
    [InlineData("A", "A", "X", "1975-11-12")]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("ABC", "A{Data}C", "B", "1975-11-12")]
    [InlineData("B", "{Data}", "B", "1975-11-12")]
    [InlineData("{B}", "{{Data}}", "B", "1975-11-12")]
    [InlineData("A{B}C", "A{{Data}}C", "B", "1975-11-12")]
    [InlineData("A{{B}}C", "A{{{Data}}}C", "B", "1975-11-12")]
    [InlineData("BC", "{Data}C", "B", "1975-11-12")]
    [InlineData("AB", "A{Data}", "B", "1975-11-12")]
    [InlineData("{ABC}", "{A{Data}C}", "B", "1975-11-12")]
    [InlineData("{A{B}C}", "{A{{Data}}C}", "B", "1975-11-12")]
    [InlineData("{A{{Otro}}C}", "{A{{Otro}}C}", "B", "1975-11-12")]
    [InlineData("A{", "A{", "B", "1975-11-12")]
    [InlineData("{A{Data", "{A{Data", "B", "1975-11-12")]
    [InlineData("A{Data", "A{Data", "B", "1975-11-12")]
    [InlineData("A{Data{", "A{Data{", "B", "1975-11-12")]
    [InlineData("A{Data{Data", "A{Data{Data", "B", "1975-11-12")]
    [InlineData("A{DataB", "A{Data{Data}", "B", "1975-11-12")]
    [InlineData("AB{Data", "A{Data}{Data", "B", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "{CurrentTime} - {Data}", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "{CurrentTime} - [{Data}]", null, "1975-11-12")]
    [InlineData("{CurrentTime:yyyy-MM-dd} - ABC", "{CurrentTime:yyyy-MM-dd} - {Data}", "ABC", "1975-11-12")]
    [InlineData("{{CurrentTime:yyyy-MM-dd}} - {{ABC}}", "{{CurrentTime:yyyy-MM-dd}} - {{{Data}}}", "ABC", "1975-11-12")]
    public void FormatStateMachine(string expected, string format, string? data, string datetime)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        StateMachine<char> stateMachine = new StateMachine<char>(new FormatStateTransitionTable(new FormatWriter(new { Data = data, CurrentTime = DateTime.Parse(datetime) }, outputWriter, keyBuilder.StringBuilder)));

        stateMachine.Process(format.Where(c => c != '\0').Append('\0'));

        output.Equal(expected, outputBuilder.ToString());
        }

    [Theory]
    [InlineData("A@C", "A@C", "X", "1975-11-12")]
    [InlineData("A@@C", "A@@C", "X", "1975-11-12")]
    [InlineData("A@.@C", "A@.@C", "X", "1975-11-12")]
    [InlineData("@A@C@", "@A@C@", "X", "1975-11-12")]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("A@.B C", "A@.@Data C", "B", "1975-11-12")]
    [InlineData("AB C", "A@Data C", "B", "1975-11-12")]
    [InlineData("AB)C", "A@Data)C", "B", "1975-11-12")]
    [InlineData("AB,C", "A@Data,C", "B", "1975-11-12")]
    [InlineData("A@Otro,C", "A@Otro,C", "B", "1975-11-12")]
    [InlineData("A@DataC", "A@DataC", "B", "1975-11-12")]
    [InlineData("B", "@Data", "B", "1975-11-12")]
    [InlineData("@B", "@@Data", "B", "1975-11-12")]
    [InlineData("A@B.C", "A@@Data.C", "B", "1975-11-12")]
    [InlineData("A@@B.C", "A@@@Data.C", "B", "1975-11-12")]
    [InlineData("@DataC", "@DataC", "B", "1975-11-12")]
    [InlineData("B.C", "@Data.C", "B", "1975-11-12")]
    [InlineData("AB", "A@Data", "B", "1975-11-12")]
    [InlineData("@A@DataC", "@A@DataC", "B", "1975-11-12")]
    [InlineData("@AB.C", "@A@Data.C", "B", "1975-11-12")]
    [InlineData("@A@B,C", "@A@@Data,C", "B", "1975-11-12")]
    [InlineData("@A@@Otro.C", "@A@@Otro.C", "B", "1975-11-12")]
    [InlineData("A@", "A@", "B", "1975-11-12")]
    [InlineData("@A@Dat", "@A@Dat", "B", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "@CurrentTime - @Data", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "@CurrentTime - [@Data]", null, "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - @otraProp - ABC", "@CurrentTime - @otraProp - @Data", "ABC", "1975-11-12")]
    [InlineData("@1975-11-12 00:00:00.000 - @ABC", "@@CurrentTime - @@Data", "ABC", "1975-11-12")]
    [InlineData("@@ 1975-11-12 00:00:00.000 @-@ ABC @@", "@@ @CurrentTime @-@ @Data @@", "ABC", "1975-11-12")]
    public void ParamSwitchStateMachine(string expected, string format, string? data, string datetime)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);

        ParamSwitchStateMachine stateMachine = new ParamSwitchStateMachine(new ParamWriter(new { Data = data, CurrentTime = DateTime.Parse(datetime) }, outputWriter, keyBuilder.StringBuilder));

        format.Where(c => c != '\0').Append('\0').ForEach<char, ParamSwitchStateMachine>(stateMachine, (b, sm) => sm.Process(b), cancellationToken: TestContext.Current.CancellationToken);

        output.Equal(expected, outputBuilder.ToString());
        }

    [Theory]
    [InlineData("A@C", "A@C", "X", "1975-11-12")]
    [InlineData("A@@C", "A@@C", "X", "1975-11-12")]
    [InlineData("A@.@C", "A@.@C", "X", "1975-11-12")]
    [InlineData("@A@C@", "@A@C@", "X", "1975-11-12")]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("A@.B C", "A@.@Data C", "B", "1975-11-12")]
    [InlineData("AB C", "A@Data C", "B", "1975-11-12")]
    [InlineData("AB)C", "A@Data)C", "B", "1975-11-12")]
    [InlineData("AB,C", "A@Data,C", "B", "1975-11-12")]
    [InlineData("A@Otro,C", "A@Otro,C", "B", "1975-11-12")]
    [InlineData("A@DataC", "A@DataC", "B", "1975-11-12")]
    [InlineData("B", "@Data", "B", "1975-11-12")]
    [InlineData("@B", "@@Data", "B", "1975-11-12")]
    [InlineData("A@B.C", "A@@Data.C", "B", "1975-11-12")]
    [InlineData("A@@B.C", "A@@@Data.C", "B", "1975-11-12")]
    [InlineData("@DataC", "@DataC", "B", "1975-11-12")]
    [InlineData("B.C", "@Data.C", "B", "1975-11-12")]
    [InlineData("AB", "A@Data", "B", "1975-11-12")]
    [InlineData("@A@DataC", "@A@DataC", "B", "1975-11-12")]
    [InlineData("@AB.C", "@A@Data.C", "B", "1975-11-12")]
    [InlineData("@A@B,C", "@A@@Data,C", "B", "1975-11-12")]
    [InlineData("@A@@Otro.C", "@A@@Otro.C", "B", "1975-11-12")]
    [InlineData("A@", "A@", "B", "1975-11-12")]
    [InlineData("@A@Dat", "@A@Dat", "B", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "@CurrentTime - @Data", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "@CurrentTime - [@Data]", null, "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - @otraProp - ABC", "@CurrentTime - @otraProp - @Data", "ABC", "1975-11-12")]
    [InlineData("@1975-11-12 00:00:00.000 - @ABC", "@@CurrentTime - @@Data", "ABC", "1975-11-12")]
    [InlineData("@@ 1975-11-12 00:00:00.000 @-@ ABC @@", "@@ @CurrentTime @-@ @Data @@", "ABC", "1975-11-12")]
    public void ParamStateMachine(string expected, string format, string? data, string datetime)
        {
        using PooledStringBuilder outputBuilder = new PooledStringBuilder();
        using PooledStringBuilder keyBuilder = new PooledStringBuilder();

        using StringWriter outputWriter = new StringWriter(outputBuilder.StringBuilder, CultureInfo.InvariantCulture);
        StateMachine<char> stateMachine = new StateMachine<char>(new ParamStateTransitionTable(new ParamWriter(new { Data = data, CurrentTime = DateTime.Parse(datetime) }, outputWriter, keyBuilder.StringBuilder)));

        stateMachine.Process(format.Where(c => c != '\0').Append('\0'));

        output.Equal(expected, outputBuilder.ToString());
        }

    /*
    En los tipos anonimos el orden en que se enumeran las propiedades no está garantizado.
    ya sea con Type.GetProperties() o con TypeInfo.DeclaredProperties.

    [Fact]
    public void PropertiesOrderAnon()
        {
        Assert.Equal("Data,CurrentTime".Split(','),
            new { Data = "Data", CurrentTime = DateTime.Parse("1975-11-12") }.GetType().GetProperties().Select(prop => prop.Name).ToArray()
            );
        }
    */

    private class C { public int B { get; } public int A { get; } }
    [Fact]
    public void PropertiesOrder()
        => Assert.Equal("B,A".Split(','), typeof(C).GetProperties().Select(prop => prop.Name).ToArray());

    [Theory]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("ABC", "A{0:Data}C", "B", "1975-11-12")]
    [InlineData("A{0:Data}C", "A{{0:Data}}C", "B", "1975-11-12")]
    [InlineData("A{B}C", "A{{{0:Data}}}C", "B", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "{0:CurrentTime} - {0:Data}", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "{0:CurrentTime} - [{0:Data}]", null, "1975-11-12")]
    [InlineData("{0:CurrentTime} - {ABC}", "{{0:CurrentTime}} - {{{0:Data}}}", "ABC", "1975-11-12")]
    public void TestPropertyFormatProvider(string expected, string format, string? data, string datetime) => output.Equal(
            expected,
            String.Format(PropertyFormatProvider.Instance, format, new { Data = data, CurrentTime = DateTime.Parse(datetime) })
            );

    [Fact]
    public void StringFormat() => output.Equal(
            "{CurrentTime:yyyy-MM-dd} - {ABC",
            String.Format("{{CurrentTime:yyyy-MM-dd}} - {{{0}", "ABC")
            );

    [Theory]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "{CurrentTime} - {Data}", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "{CurrentTime} - [{Data}]", null, "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - {otraProp} - ABC", "{CurrentTime} - {otraProp} - {Data}", "ABC", "1975-11-12")]
    [InlineData("{CurrentTime:yyyy-MM-dd} - ABC", "{CurrentTime:yyyy-MM-dd} - {Data}", "ABC", "1975-11-12")]
    [InlineData("{1975-11-12 00:00:00.000} - {ABC}", "{{CurrentTime}} - {{Data}}", "ABC", "1975-11-12")]
    [InlineData("}}1975-11-12 00:00:00.000 }-{ ABC {{", "}}{CurrentTime} }-{ {Data} {{", "ABC", "1975-11-12")]
    public void NamedReplace(string expected, string format, string? data, string datetime) => output.Equal(
            expected,
            format.NamedReplace(new { CurrentTime = DateTime.Parse(datetime), Data = data })
            );

    [Theory]
    [InlineData("1975-11-12 00:00:00.000 - ABC", "@CurrentTime - @Data", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - []", "@CurrentTime - [@Data]", null, "1975-11-12")]
    [InlineData("1975-11-12 00:00:00.000 - @otraProp - ABC", "@CurrentTime - @otraProp - @Data", "ABC", "1975-11-12")]
    [InlineData("@1975-11-12 00:00:00.000 - @ABC", "@@CurrentTime - @@Data", "ABC", "1975-11-12")]
    [InlineData("@@ 1975-11-12 00:00:00.000 @-@ ABC @@", "@@ @CurrentTime @-@ @Data @@", "ABC", "1975-11-12")]
    public void ParamReplace(string expected, string format, string? data, string datetime) => output.Equal(
            expected,
            format.ParamReplace(new { CurrentTime = DateTime.Parse(datetime), Data = data })
            );

    [Theory]
    [InlineData("}}A{{0}}B{{{1}}}{otro}{{", "}}A{{DateTime}}B{{{Message}}}{otro}{{", "DateTime,Message")]
    [InlineData("A{0}B{1}C", "A{DateTime}B{Message}C", "DateTime,Message")]
    [InlineData("[{0}] {1}", "[{DateTime}] {Message}", "DateTime,Message")]
    [InlineData("[{0}] [{1}] {2} [{3}] {4} {5}{6}", "[{DateTime}] [{Level}] {Name} [{EventId}] {Message} {Exception}{State}", "DateTime,Level,Name,EventId,Message,Exception,State")]
    [InlineData("[{0:format}] {1}", "[{DateTime:format}] {Message}", "DateTime,Message")]
    [InlineData("A{{d{0}B{1}C", "A{{d{DateTime}B{Message}C", "DateTime,Message")]
    [InlineData("A{{0}}C", "A{{Data}}C", "Data")]
    public void ToNamedFormat(string numberedFormat, string namedFormat, string names)
        {
        string[] formatKeys = names.Split(',');

        Assert.Equal(namedFormat, numberedFormat.ToNamedFormat(formatKeys));
        Assert.Equal(numberedFormat, namedFormat.ToNumberedFormat(formatKeys));
        }

    [Theory]
    [InlineData("}}A{{0}}B{{{1}}}{otro}{{", "}}A{{DateTime}}B{{{Message}}}{otro}{{")]
    [InlineData("A{0}B{1}C", "A{DateTime}B{Message}C")]
    [InlineData("[{0}] {1}", "[{DateTime}] {Message}")]
    [InlineData("[{0:format}] {1}", "[{DateTime:format}] {Message}")]
    [InlineData("A{{d{0}B{1}C", "A{{d{DateTime}B{Message}C")]
    public void MessageEntryNamedFormat(string numberedFormat, string namedFormat)
        {
        Assert.Equal(namedFormat, numberedFormat.ToNamedFormat<MessageEntry>());
        Assert.Equal(numberedFormat, namedFormat.ToNumberedFormat<MessageEntry>());
        }

    [Theory]
    [InlineData("}}A{{0}}B{{{5}}}{otro}{{", "}}A{{DateTime}}B{{{Message}}}{otro}{{")]
    [InlineData("A{0}B{5}C", "A{DateTime}B{Message}C")]
    [InlineData("[{0}] {5}", "[{DateTime}] {Message}")]
    [InlineData("[{0}] [{1}] {3} [{2}] {5} {6}{4}", "[{DateTime}] [{Level}] {Name} [{EventId}] {Message} {Exception}{State}")]
    [InlineData("[{0:format}] {5}", "[{DateTime:format}] {Message}")]
    [InlineData("A{{d{0}B{5}C", "A{{d{DateTime}B{Message}C")]
    public void LogEntryNamedFormat(string numberedFormat, string namedFormat)
        {
        Assert.Equal(namedFormat, numberedFormat.ToNamedFormat<LogEntry>());
        Assert.Equal(numberedFormat, namedFormat.ToNumberedFormat<LogEntry>());
        }

    //private class ConcreteClass { public string Data { get; set; } public DateTime CurrentTime { get; set; } }

    [Theory]
    [InlineData("ABC", "ABC", "X", "1975-11-12")]
    [InlineData("ABC", "A{Data}C", "B", "1975-11-12")]
    [InlineData("A{B}C", "A{{{Data}}}C", "B", "1975-11-12")]
    [InlineData("12/11/1975 00:00:00 - ABC", "{CurrentTime} - {Data}", "ABC", "1975-11-12")]
    [InlineData("12/11/1975 00:00:00 - []", "{CurrentTime} - [{Data}]", null, "1975-11-12")]
    //[InlineData("12/11/1975 00:00:00 - {otraProp} - ABC", "{CurrentTime} - {otraProp} - {Data}", "ABC", "1975-11-12")]
    [InlineData("1975-11-12 - ABC", "{CurrentTime:yyyy-MM-dd} - {Data}", "ABC", "1975-11-12")]
    //[InlineData("{12/11/1975 00:00:00} - {ABC}", "{{CurrentTime}} - {{Data}}", "ABC", "1975-11-12")]
    //[InlineData("}}12/11/1975 00:00:00 }-{ ABC {{", "}}{CurrentTime} }-{ {Data} {{", "ABC", "1975-11-12")]
    public void NamedFormat(string expected, string format, string? data, string datetime) => output.Equal(
            expected,
            format.NamedFormat(new { Data = data, CurrentTime = DateTime.Parse(datetime) })
            );

    [Theory]
    [InlineData("A{0}C", "AbC", "b")]
    //[InlineData("A{{0}C", "A{{0}C", "b")]
    [InlineData("A{{{0}C", "A{bC", "b")]
    public void EscapeFormat(string format, string formatted, string arg) => output.Equal(formatted, String.Format(format, arg));

    [Theory]
    [InlineData("A\tC", "A\\tC")]
    [InlineData("A\'C", "A\\'C")]
    [InlineData("A\"C", "A\\\"C")]
    [InlineData(@"A\\tC", @"A\\\\tC")]
    [InlineData("A\\tC", "A\\\\tC")]
    [InlineData("A\\\tC", "A\\\\\\tC")]
    public void EscapeCString(string str, string scaped)
        {
        output.Equal(scaped, str.EscapeCString());
        output.Equal(str, scaped.UnescapeCString());
        }

    [Fact]
    public void NotPropertyFormatTheory()
        {
        try
            {
            output.Equal("A{Prop}C", TemplatingExtensions.NamedFormat("A{Prop}C", new { Data = "B" }));
            }
        catch (FormatException) { }
        }
    }
