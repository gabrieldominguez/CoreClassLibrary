using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using CoreClassLibrary.Objects.Pooling;
using System.Reflection;
using Xunit;

namespace CoreClassLibrary.Tests.Reflection;

public class ClassToCopyConstructorTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData("CoreClassLibrary.Tests", "CoreClassLibrary.Tests.Reflection.LogEntry")]
    public void ClassToCopyConstructor(string assemblyName, string typeName) => ToCopyConstructor(ReflectionExtensions.GetRequiredType(assemblyName, typeName));

    [Fact]
    public void CuentaDTOToCopyConstructor() => ToCopyConstructor(typeof(CuentaDTO));

    [Fact]
    public void PrestamoModelToCopyConstructor() => ToCopyConstructor(typeof(PrestamoModel));

    [Fact]
    public void LogEntryToCopyConstructor() => ToCopyConstructor(typeof(LogEntry));

    private void ToCopyConstructor(Type type)
        {
        string typeName = type.Name.Decapitalize();
        char typeLetter = typeName[0];

        using PooledStringBuilder builder = new PooledStringBuilder();

        builder.StringBuilder.Append("public ").Append(type.Name).Append('(').Append(type.Name).Append(' ').Append(typeLetter).AppendLine(")")
        .AppendLine("\t{");

        foreach (PropertyInfo property in type.GetProperties())
            {
            builder.StringBuilder.Append("\tthis.").Append(property.Name).Append(" = ").Append(typeLetter).Append('.').Append(property.Name).AppendLine(';');
            }

        output.WriteLine(builder.StringBuilder.AppendLine("\t}").ToString());
        }
    }
