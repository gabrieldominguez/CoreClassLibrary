using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using CoreClassLibrary.Objects.Pooling;
using System.Reflection;
using Xunit;

namespace CoreClassLibrary.Tests.Reflection;

public class ClassToCopyTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData("CoreClassLibrary.Tests", "CoreClassLibrary.Tests.Reflection.LogEntry")]
    public void ClassToCopyString(string assemblyName, string typeName) => ToCopyString(ReflectionExtensions.GetRequiredType(assemblyName, typeName));

    [Fact]
    public void CuentaDTOToCopyString() => ToCopyString(typeof(CuentaDTO));

    [Fact]
    public void PrestamoModelToCopyString() => ToCopyString(typeof(PrestamoModel));

    private void ToCopyString(Type type)
    {
        string typeName = type.Name.Decapitalize();
        char typeLetter = typeName[0];

        using PooledStringBuilder builder = new PooledStringBuilder();

        builder.StringBuilder.Append(type.Name).Append(' ').Append(typeName).Append(" = new ").Append(type.Name).AppendLine("()")
        .AppendLine("\t{");

        bool first = true;
        foreach (PropertyInfo property in type.GetProperties())
        {
            if (first)
            {
                first = false;
            }
            else
            {
                builder.StringBuilder.AppendLine(',');
            }

            builder.StringBuilder.Append('\t').Append(property.Name).Append(" = ").Append(typeLetter).Append('.').Append(property.Name);
        }

        output.WriteLine(builder.StringBuilder.AppendLine().AppendLine("\t};").ToString());
    }
}
