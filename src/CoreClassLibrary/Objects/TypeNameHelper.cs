// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CoreClassLibrary.Objects.Pooling;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CoreClassLibrary.Objects;

/// <summary>
/// Get Type Display Name
/// </summary>
/// <remarks>
/// Use TypeNameHelper.GetTypeDisplayName() for generic types
/// </remarks>
/// <example>
/// For normal types there is no difference with Type.Name or Type.FullName, but for generic types:
/// 
///     Assert.Equal("Entity<SampleEntity, long>", TypeNameHelper.GetTypeDisplayName(typeof(Entity<SampleEntity, long>), fullName: false));
///     Assert.Equal("Entity`2", typeof(Entity<SampleEntity, long>).Name);
///     Assert.Equal("CoreClassLibrary.Objects.Entity<CoreClassLibrary.Objects.SampleEntity, long>", TypeNameHelper.GetTypeDisplayName(typeof(Entity<SampleEntity, long>)));
///     Assert.StartsWith("CoreClassLibrary.Objects.Entity`2[[CoreClassLibrary.Objects.SampleEntity, CoreClassLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[System.Int64, System.Private.CoreLib, Version=", typeof(Entity<SampleEntity, long>).FullName);
/// </example>
/// <see cref="https://github.com/dotnet/aspnetcore/blob/main/src/Shared/TypeNameHelper/TypeNameHelper.cs"/>
public static class TypeNameHelper
{
    private const char DefaultNestedTypeDelimiter = '+';

    private static readonly Dictionary<Type, string> _builtInTypeNames = new Dictionary<Type, string>
        {
            { typeof(void), "void" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" }
        };

    [return: NotNullIfNotNull(nameof(item))]
    public static string? GetTypeDisplayName(object? item, bool fullName = true) 
        => item == null ? null : GetTypeDisplayName(item.GetType(), fullName);

    /// <summary>
    /// Pretty print a type name.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="fullName"><c>true</c> to print a fully qualified name.</param>
    /// <param name="includeGenericParameterNames"><c>true</c> to include generic parameter names.</param>
    /// <param name="includeGenericParameters"><c>true</c> to include generic parameters.</param>
    /// <param name="nestedTypeDelimiter">Character to use as a delimiter in nested type names</param>
    /// <returns>The pretty printed type name.</returns>
    public static string GetTypeDisplayName(Type type, bool fullName = true, bool includeGenericParameterNames = false, bool includeGenericParameters = true, char nestedTypeDelimiter = DefaultNestedTypeDelimiter)
    {
        using PooledStringBuilder builder = new PooledStringBuilder();
        ProcessType(builder.StringBuilder, type, new DisplayNameOptions(fullName, includeGenericParameterNames, includeGenericParameters, nestedTypeDelimiter));
        return builder.StringBuilder.ToString();
    }

    private static void ProcessType(StringBuilder builder, Type type, in DisplayNameOptions options)
    {
        if (type.IsGenericType)
        {
            Type[] genericArguments = type.GetGenericArguments();
            ProcessGenericType(builder, type, genericArguments, genericArguments.Length, options);
        }
        else if (type.IsArray)
        {
            ProcessArrayType(builder, type, options);
        }
        else if (_builtInTypeNames.TryGetValue(type, out string? builtInName))
        {
            builder.Append(builtInName);
        }
        else if (type.IsGenericParameter)
        {
            if (options.IncludeGenericParameterNames)
            {
                builder.Append(type.Name);
            }
        }
        else
        {
            string name = options.FullName ? type.FullName! : type.Name;
            builder.Append(name);

            if (options.NestedTypeDelimiter != DefaultNestedTypeDelimiter)
            {
                builder.Replace(DefaultNestedTypeDelimiter, options.NestedTypeDelimiter, builder.Length - name.Length, name.Length);
            }
        }
    }

    private static void ProcessArrayType(StringBuilder builder, Type type, in DisplayNameOptions options)
    {
        Type innerType = type;
        while (innerType.IsArray)
        {
            innerType = innerType.GetElementType()!;
        }

        ProcessType(builder, innerType, options);

        while (type.IsArray)
        {
            builder.Append('[');
            builder.Append(',', type.GetArrayRank() - 1);
            builder.Append(']');
            type = type.GetElementType()!;
        }
    }

    private static void ProcessGenericType(StringBuilder builder, Type type, Type[] genericArguments, int length, in DisplayNameOptions options)
    {
        int offset = 0;
        if (type.IsNested)
        {
            offset = type.DeclaringType!.GetGenericArguments().Length;
        }

        if (options.FullName)
        {
            if (type.IsNested)
            {
                ProcessGenericType(builder, type.DeclaringType!, genericArguments, offset, options);
                builder.Append(options.NestedTypeDelimiter);
            }
            else if (!string.IsNullOrEmpty(type.Namespace))
            {
                builder.Append(type.Namespace);
                builder.Append('.');
            }
        }

        int genericPartIndex = type.Name.IndexOf('`');
        if (genericPartIndex <= 0)
        {
            builder.Append(type.Name);
            return;
        }

        builder.Append(type.Name, 0, genericPartIndex);

        if (options.IncludeGenericParameters)
        {
            builder.Append('<');
            for (int i = offset; i < length; i++)
            {
                ProcessType(builder, genericArguments[i], options);
                if (i + 1 == length)
                {
                    continue;
                }

                builder.Append(',');
                if (options.IncludeGenericParameterNames || !genericArguments[i + 1].IsGenericParameter)
                {
                    builder.Append(' ');
                }
            }
            builder.Append('>');
        }
    }

    private readonly record struct DisplayNameOptions
    {
        public DisplayNameOptions(bool fullName, bool includeGenericParameterNames, bool includeGenericParameters, char nestedTypeDelimiter)
        {
            FullName = fullName;
            IncludeGenericParameters = includeGenericParameters;
            IncludeGenericParameterNames = includeGenericParameterNames;
            NestedTypeDelimiter = nestedTypeDelimiter;
        }

        public bool FullName { get; }

        public bool IncludeGenericParameters { get; }

        public bool IncludeGenericParameterNames { get; }

        public char NestedTypeDelimiter { get; }
    }
}