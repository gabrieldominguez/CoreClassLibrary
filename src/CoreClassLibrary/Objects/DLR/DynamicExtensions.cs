using System.Globalization;
using System.Reflection;

namespace CoreClassLibrary.Objects.DLR;

public static class DynamicExtensions
    {
    /// <summary>
    /// Parsea el string al tipo de la propiedad.
    /// </summary>
    /// <param name="property">Propiedad de la cual obtener el tipo al cual castear el string.</param>
    /// <param name="value">String a pasear y castear.</param>
    /// <returns>Valor casteado.</returns>
    public static object CastValue(this PropertyInfo property, string value)
        {
        if (property.PropertyType == typeof(string))
            {
            return value;
            }

        dynamic dynamicType = new StaticMembersDynamicWrapper(property.PropertyType);

        try
            {
            return dynamicType.Parse(value, CultureInfo.InvariantCulture);
            }
        catch
            {
            throw new FormatException($"Property {property.Name} = \"{value}\" cannot be casted to {property.PropertyType.Name}.");
            }
        }
    }
