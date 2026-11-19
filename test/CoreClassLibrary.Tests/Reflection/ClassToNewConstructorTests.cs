
using CoreClassLibrary.Objects;
using CoreClassLibrary.Objects.Pooling;
using System.Reflection;
using Xunit;

namespace CoreClassLibrary.Tests.Reflection;

public class ClassToNewConstructorTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData("CoreClassLibrary.Tests", "CoreClassLibrary.Tests.Reflection.LogEntry")]
    public void ClassToNewConstructor(string assemblyName, string typeName) => ToNewConstructor(ReflectionExtensions.GetRequiredType(assemblyName, typeName));

    [Fact]
    public void CuentaDTOToNewConstructor() => ToNewConstructor(typeof(CuentaDTO));

    [Fact]
    public void PrestamoModelToNewConstructor() => ToNewConstructor(typeof(PrestamoModel));

    [Fact]
    public void LogEntryToNewConstructor() => ToNewConstructor(typeof(LogEntry));

    private void ToNewConstructor(Type type)
    {
        using PooledStringBuilder builder = new PooledStringBuilder();

        builder.StringBuilder.Append("public ").Append(type.Name).AppendLine("()")
        .AppendLine("\t{");

        foreach (PropertyInfo property in type.GetProperties())
        {
            builder.StringBuilder.Append("\tthis.").Append(property.Name).Append(" = ");

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {
                builder.StringBuilder.AppendLine("0;");
                continue;
            }

            if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
            {
                builder.StringBuilder.AppendLine("0L;");
                continue;
            }

            if (property.PropertyType == typeof(float) || property.PropertyType == typeof(float?))
            {
                builder.StringBuilder.AppendLine("0F;");
                continue;
            }

            if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
            {
                builder.StringBuilder.AppendLine("0D;");
                continue;
            }

            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
            {
                builder.StringBuilder.AppendLine("0M;");
                continue;
            }

            if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {
                builder.StringBuilder.AppendLine("false;");
                continue;
            }

            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                builder.StringBuilder.AppendLine("System.DateTime.Now;");
                continue;
            }

            if (property.PropertyType == typeof(DateTimeOffset) || property.PropertyType == typeof(DateTimeOffset?))
            {
                builder.StringBuilder.AppendLine("System.DateTimeOffset.Now;");
                continue;
            }

            if (property.PropertyType == typeof(DateOnly) || property.PropertyType == typeof(DateOnly?))
            {
                builder.StringBuilder.AppendLine("DateExtensions.GetToday();");
                continue;
            }

            if (property.PropertyType == typeof(TimeOnly) || property.PropertyType == typeof(TimeOnly?))
            {
                builder.StringBuilder.AppendLine("TimeExtensions.GetNow();");
                continue;
            }

            if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
            {
                builder.StringBuilder.AppendLine("Guid.NewGuid();");
                continue;
            }

            if (property.PropertyType == typeof(char) || property.PropertyType == typeof(char?))
            {
                builder.StringBuilder.AppendLine("' ';");
                continue;
            }

            if (property.PropertyType == typeof(string))
            {
                builder.StringBuilder.AppendLine("\"\";");
                continue;
            }

            builder.StringBuilder.Append("new ").Append(property.PropertyType.Name).AppendLine("();");
        }

        output.WriteLine(builder.StringBuilder.AppendLine("\t};").ToString());
    }
}
