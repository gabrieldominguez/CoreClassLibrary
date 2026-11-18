using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CoreClassLibrary.Objects;

public static class ReflectionExtensions
    {
    public static MethodInfo GetRequiredGenericMethod(this Type type, string name, params Type[] typeArguments)
        => type.GetRequiredMethod(name).MakeGenericMethod(typeArguments);

    public static MethodInfo GetRequiredMethod(this Type type, string name) 
        => type.GetMethod(name) ?? throw new InvalidOperationException($"Method not found: {name}");

    /// <summary>
    /// Enumera las interfaces en un assembly que terminan en un sufijo específico.
    /// </summary>
    /// <param name="assembly">Assembly con los tipos que se va a registrar.</param>
    /// <param name="suffix">Subfijo del nombre.</param>
    public static IEnumerable<TypeInfo> GetInterfaces(this Assembly assembly, string suffix)
        => assembly.GetTypes(type => type.IsInterface, suffix);

    /// <summary>
    /// Enumera las clases concretas en un assembly que terminan en un sufijo específico.
    /// </summary>
    /// <param name="assembly">Assembly con los tipos que se va a registrar.</param>
    /// <param name="suffix">Subfijo del nombre.</param>
    public static IEnumerable<TypeInfo> GetConcreteClasses(this Assembly assembly, string suffix)
        => assembly.GetTypes(type => type.IsClass && !type.IsAbstract, suffix);
    
    public static IEnumerable<TypeInfo> GetTypes(this Assembly assembly, Predicate<TypeInfo> predicate, string suffix)
        => assembly.DefinedTypes.Where(type => predicate(type) && type.Name.EndsWith(suffix));

    public static string GetFullName(this PropertyInfo property)
        => String.Concat(property.ReflectedType?.FullName, '.', property.Name);

    /// <summary>
    /// Obtiene recursivamente los assemblies referenciados por el assembly especificado
    /// (y por las referencias de las referencias)
    /// que se encuentran en el mismo directorio.
    /// </summary>
    /// <remarks>
    /// Los assemblies referenciados son previamente cargados.
    /// Se incluye rootAssembly como primer assembly retornado.
    /// </remarks>
    /// <returns>Los assemblies locales referenciados.</returns>
    public static IEnumerable<Assembly> GetAllLocalReferencedAssemblies(this Assembly rootAssembly)
        => rootAssembly
            .Traverse(ReflectionExtensions.GetLocalReferencedAssemblies)
            .Distinct();

    /// <summary>
    /// Obtiene los assemblies referenciados que se encuentran en el mismo directorio.
    /// </summary>
    /// <remarks>Los assemblies referenciados son previamente cargados.</remarks>
    /// <returns>Los assemblies locales referenciados.</returns>
    public static IEnumerable<Assembly> GetLocalReferencedAssemblies(this Assembly assembly)
        => assembly.GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Where(assembly.GetDirectoryName(), (referencedAssembly, ass) => referencedAssembly.GetDirectoryName() == ass);

    /// <summary>
    /// Retorna el path completo del archivo de un assembly.
    /// </summary>
    /// <param name="assembly">Assembly a partir del cual obtener el path</param>
    /// <returns>Nombre completo del archivo.</returns>
    public static string GetDirectoryName(this Assembly assembly)
        => Path.GetDirectoryName(assembly.Location) ?? "";

    /// <summary>
    /// Retorna el nombre completo del archivo config de un assembly.
    /// </summary>
    /// <param name="assembly">Assembly a partir del cual obtener el nombre</param>
    /// <returns>Nombre completo del archivo.</returns>
    public static string GetConfigFileName(this Assembly assembly)
        => String.Concat(assembly.Location, ".config");

    /// <summary>
    /// Retorna el nombre completo del archivo log de un assembly.
    /// </summary>
    /// <param name="assembly">Assembly a partir del cual obtener el nombre</param>
    /// <returns>Nombre completo del archivo.</returns>
    public static string GetLogFileName(this Assembly assembly)
        => String.Concat(assembly.Location, ".log");

    public static string GetFullCopyright(this Assembly assembly)
        => String.Concat(assembly.GetNameAndVersion(), " - ", assembly.GetCopyright());

    /// <summary>
    /// Retorna la version de un assembly de un forma leible.
    /// </summary>
    /// <param name="assembly">Assembly a partir del cual obtener la version.</param>
    /// <returns>Version del assembly .</returns>
    public static string GetNameAndVersion(this Assembly assembly)
        => assembly.GetName().GetNameAndVersion();

    public static string GetNameAndVersion(this AssemblyName assemblyName)
        => String.Concat(assemblyName.Name, " v", assemblyName.Version?.ToString(3) ?? "0");

    public static string GetCopyright(this Assembly assembly)
        => assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "";

    /// <summary>
    /// Retorna el nombre del assembly.
    /// </summary>
    /// <param name="assembly">Assembly a partir del cual obtener el nombre</param>
    /// <returns>Nombre completo del archivo.</returns>
    public static string GetDisplayName(this Assembly assembly)
        => assembly.GetName().Name ?? "";

    /// <summary>
    /// Obtiene el nombre del assembly al que pertenece el tipo.
    /// </summary>
    public static string GetNamespace(string typeFullname)
        => typeFullname[0..typeFullname.LastIndexOf('.')];

    /// <summary>
    /// Obtiene el nombre del assembly al que pertenece el tipo.
    /// </summary>
    public static string? GetAssemblyName(this Type type)
        => type.Assembly.GetName().Name;

    public static string GetFriendlyGenericName(this Type type)
        => type.IsGenericType?
                String.Concat(type.GetGenericName(), "<", String.Join(", ", type.GetGenericArguments().Select(ReflectionExtensions.GetFriendlyGenericName)), ">"):
                type.Name;

    public static string GetGenericName(this Type type)
        => type.IsGenericType ?
                type.Name[..type.Name.IndexOf('`')] :
                type.Name;

    public static bool IsStaticProperty(this MemberInfo member)
        => member is PropertyInfo prop && prop.IsStatic();

    // A property is considered static if *any* accessor (get/set) is static.
    public static bool IsStatic(this PropertyInfo prop)
        {
        MethodInfo? accessor = prop.GetGetMethod(true) ?? prop.GetSetMethod(true);
        return accessor != null && accessor.IsStatic;
        }

    public static bool IsNullable(this Type type)
        => !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

    //public static bool HasCustomAttribute<T>(this MemberInfo member) where T : Attribute  => member.GetCustomAttribute<T>() != null;
    public static bool HasCustomAttribute<T>(this MemberInfo element)
        => element.IsDefined(typeof(T));

    public static object? GetPropertyValue(this object obj, string propertyName)
        => obj.GetType().GetProperty(propertyName)?.GetValue(obj);

    public static IEnumerable<object?> GetPropertyValues(this object obj)
        => obj.GetType()
            .GetProperties()
            .Select(prop => prop.GetValue(obj));

    // No siempre es posible castear un objecto a ExpandoObject, por ej, AnonymousType
    //public static ExpandoObject GetPropertiesDictionary(this object obj) => (ExpandoObject)obj;

    public static Dictionary<string, object?> GetPropertiesDictionary(this object obj)
        => obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(obj));

    public static Dictionary<string, string?> GetPropertiesStringDictionary(this object obj)
        => obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(obj)?.ToString());

    public static Dictionary<string, string?> GetRecursivePropertiesStringDictionary(this object obj, string? parentPath = null, string separator = ":")
        => obj.GetRecursiveKeyValuePairProperties(parentPath, separator).ToDictionary();

    public static IEnumerable<KeyValuePair<string, string?>> GetRecursiveKeyValuePairProperties(this object obj, string? parentPath = null, string separator = ":")
        {
        Type type = obj.GetType();

        parentPath ??= type.Name;

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
            string key = String.Concat(parentPath, separator, prop.Name);
            object? value = prop.GetValue(obj);

            if (value == null || value.GetType().IsSimple())
                {
                yield return KeyValuePair.Create(key, value?.ToString());
                }
            else
                {
                foreach(KeyValuePair<string, string?> keyValuePair in value.GetRecursiveKeyValuePairProperties(key, separator))
                    {
                    yield return keyValuePair;
                    }
                }
            }
        }

    // Simple types → direct conversion with .ToString(), no recursion needed.
    public static bool IsSimple(this Type type)
        => type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(Guid)
            || type == typeof(TimeSpan);     

    /// <summary>
    /// Creates a <see cref="PropertyChangedEventArgs" /> instance for a given property.
    /// </summary>
    /// <param name="expression">The property.</param>
    /// <returns></returns>
    public static PropertyChangedEventArgs CreateChangeEventArgs<TDelegate>(this Expression<TDelegate> expression)
        => new PropertyChangedEventArgs(expression.GetProperty().Name);

    /// <summary>
    /// Extract properties name from expression return type
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// Expression<Func<T>> expressionT = () => new { this.Id, this.Name };
    /// Assert.Equal(new string[] {"Id" "Name"}, expressionT.GetProperties().Select(p => p.Name));
    ///
    /// Expression<Func<T, TPROP>> expressionTT = rec => new { rec.IdLote, rec.NroLote };
    /// Assert.Equal(new string[] {"IdLote" "NroLote"}, expressionTT.GetProperties().Select(p => p.Name));
    /// ]]>
    /// </example>
    public static IEnumerable<PropertyInfo> GetProperties<TDelegate>(this Expression<TDelegate> expression)
        => expression.Body.GetProperties();

    public static IEnumerable<PropertyInfo> GetProperties(this Expression expression)
        => expression is NewExpression newExpression ?
                newExpression.Arguments.Select(ReflectionExtensions.GetProperty) :
                [ expression.GetProperty() ];

    /// <summary>
    /// Extract property name from expression return
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// Expression<Func<T>> expressionT = () => this.Name;
    /// Assert.Equal("Name", expressionT.GetProperty().Name);
    ///
    /// Expression<Func<T, TPROP>> expressionTT = rec => rec.IdLote;
    /// Assert.Equal("IdLote", expressionTT.GetProperty().Name);
    /// ]]>
    /// </example>
    public static PropertyInfo GetProperty<TDelegate>(this Expression<TDelegate> expression)
        => expression.Body.GetProperty();

    public static PropertyInfo GetProperty(this Expression expression)
        {
        if (expression is not MemberExpression memberExpression)
            {
            if (expression is not UnaryExpression unaryExpression)
                {
                throw new ArgumentException("The expression is not a member access expression.");
                }
 
            memberExpression = (MemberExpression) unaryExpression.Operand;
            }
        
        PropertyInfo? property = memberExpression.Member as PropertyInfo ?? throw new ArgumentException("The member access expression does not access a property.");

        /*
        MethodInfo getMethod = property.GetMethod;
        if (getMethod.IsStatic)
            {
            throw new ArgumentException("The referenced property is a static property.");
            }
        */

        return property;
        }

    public static string ToCSharpString<TDelegate>(this Expression<TDelegate> expression)
        => expression
            .ToString()
            .Replace("AndAlso", "&&")
            .Replace("OrElse", "||");

    public static bool IsFatal(this Exception? exception)
        {
        while (exception != null)
            {
            if (exception is OutOfMemoryException && exception is not SEHException)
                {
                return true;
                }

            if (exception is not TypeInitializationException && exception is not TargetInvocationException)
                {
                break;
                }

            exception = exception.InnerException;
            }

        return false;
        }

    public static IEnumerable<PropertyInfo> GetReadableProperties(this Type type)
        => type.GetProperties(p => p.CanRead);

    public static IEnumerable<PropertyInfo> GetWriteableProperties(this Type type)
        => type.GetProperties(p => p.CanWrite);

    public static IEnumerable<PropertyInfo> GetProperties(this Type type, Func<PropertyInfo, bool> predicate)
        => type.GetProperties().Where(predicate);

    public static Type GetReflectedType(this MemberInfo memberInfo)
        => memberInfo.ReflectedType ?? throw new InvalidOperationException($"Cannot get ReflectedType from {memberInfo.Name}");

    public static Type GetRequiredType(string assemblyName, string typeName)
        => Assembly.Load(assemblyName).GetRequiredType(typeName);

    public static Type GetRequiredType(this Assembly assembly, string typeName)
        => assembly.GetType(typeName, true) ?? throw new InvalidOperationException($"Cannot get required type {typeName} from {assembly.FullName}");    

    public static PropertyInfo GetRequiredProperty(this Type type, string name)
        => type.GetProperty(name) ?? throw new InvalidOperationException($"Cannot get required property {name} from {type.FullName}");
    }
