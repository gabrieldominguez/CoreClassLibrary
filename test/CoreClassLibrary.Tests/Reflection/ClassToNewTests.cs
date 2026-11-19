
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using CoreClassLibrary.Objects.Pooling;
using System.Reflection;
using Xunit;

namespace CoreClassLibrary.Tests.Reflection;

public class ClassToNewTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData("CoreClassLibrary.Tests", "CoreClassLibrary.Tests.Reflection.LogEntry")]
    public void ClassToNewString(string assemblyName, string typeName) => this.ToNewString(ReflectionExtensions.GetRequiredType(assemblyName, typeName));

    [Fact]
    public void CuentaDTOToNewString()
        => this.ToNewString(typeof(CuentaDTO));

    [Fact]
    public void PrestamoModelToNewString()
        => this.ToNewString(typeof(PrestamoModel));

    [Fact]
    public void PRHOTSALBFToNewString()
        => this.ToNewString(typeof(PRHOTSALBF));

    private void ToNewString(Type type)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();

        builder.StringBuilder.Append(type.Name).Append(' ').Append(type.Name.Decapitalize()).Append(" = new ").Append(type.Name).AppendLine("()")
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

            builder.StringBuilder.Append('\t').Append(property.Name).Append(" = ");

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                {
                builder.StringBuilder.Append('0');
                }

            if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
                {
                builder.StringBuilder.Append("0L");
                }

            if (property.PropertyType == typeof(float) || property.PropertyType == typeof(float?))
                {
                builder.StringBuilder.Append("0F");
                }

            if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                {
                builder.StringBuilder.Append("0D");
                }

            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                {
                builder.StringBuilder.Append("0M");
                }

            if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                {
                builder.StringBuilder.Append("false");
                }

            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                builder.StringBuilder.Append("System.DateTime.Now");
                }

            if (property.PropertyType == typeof(DateTimeOffset) || property.PropertyType == typeof(DateTimeOffset?))
                {
                builder.StringBuilder.Append("System.DateTimeOffset.Now");
                }

            if (property.PropertyType == typeof(DateOnly) || property.PropertyType == typeof(DateOnly?))
                {
                builder.StringBuilder.Append("DateExtensions.GetToday()");
                continue;
                }

            if (property.PropertyType == typeof(TimeOnly) || property.PropertyType == typeof(TimeOnly?))
                {
                builder.StringBuilder.Append("TimeExtensions.GetNow()");
                continue;
                }

            if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                {
                builder.StringBuilder.Append("Guid.NewGuid()");
                }

            if (property.PropertyType == typeof(char) || property.PropertyType == typeof(char?))
                {
                builder.StringBuilder.Append("' '");
                }

            if (property.PropertyType == typeof(string))
                {
                builder.StringBuilder.Append("\"\"");
                }
            }

        output.WriteLine(builder.StringBuilder.AppendLine().AppendLine("\t};").ToString());
        }
    }
