using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace X3Code.Utils.Extensions;

public static class DataRowExtension
{
    /// <summary>
    /// Converts a DataRow to a specified data type.
    /// </summary>
    /// <typeparam name="T">The type of object to which the DataRow should be converted. The type should be a class and have a parameterless constructor.</typeparam>
    /// <param name="source">The DataRow that contains the data.</param>
    /// <returns>An object of type T populated with the DataRow's values.</returns>
    /// <exception cref="InvalidCastException">Thrown when unable to cast the data row's value to the property's type.</exception>
    public static T ToDataType<T>(this DataRow source) where T : class, new()
    {
        var result = new T();
        var genericType = typeof(T);

        foreach (var property in genericType.GetProperties())
        {
            var propertyType = property;
            var valueFromRow = System.Convert.ChangeType(source[property.Name], propertyType.PropertyType);
            genericType.GetProperty(property.Name)?.SetValue(result, valueFromRow);
        }
            
        return result;
    }
        
    /// <summary>
    /// Converts a specific column of a DataRow to a DateTime type, using the provided date pattern for parsing.
    /// </summary>
    /// <param name="source">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the date string.</param>
    /// <param name="datePattern">The pattern used for date parsing.</param>
    /// <returns>A Nullable DateTime populated with the date value from the DataRow's specific column. If the conversion fails or the value is null or empty, returns null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName or datePattern is null, empty or consists only of white-space characters.</exception>
    public static DateTime? ToDateTime(this DataRow source, string columnName, string datePattern)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
        if (string.IsNullOrWhiteSpace(datePattern)) throw new ArgumentNullException(nameof(datePattern));
            
        var dateAsString = source.ToStringOrNull(columnName);
        if (string.IsNullOrEmpty(dateAsString)) return null;

        if (DateTime.TryParseExact(dateAsString, datePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Converts a specific column of a DataRow to a trimmed string type.
    /// </summary>
    /// <param name="row">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the string.</param>
    /// <returns>A string populated with the value from the DataRow's specific column. If the conversion fails or the value is null or empty, returns null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName is null, empty or consists only of white-space characters.</exception>
    public static string ToStringOrNull(this DataRow row, string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            
        if (row[columnName] == DBNull.Value) return null;
        var result = (string)row[columnName];

        return result.Trim();
    }

    /// <summary>
    /// Converts a specific column of a DataRow to an integer.
    /// </summary>
    /// <param name="source">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the integer value.</param>
    /// <returns>An integer representation of the data in the specified DataRow column. If the conversion fails, returns 0.</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName is null, empty or consists only of white-space characters.</exception>
    public static int ToInteger(this DataRow source, string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
        var content = source.ToStringOrNull(columnName);

        if (int.TryParse(content, out var result))
        {
            return result;
        }

        return 0;
    }

    /// <summary>
    /// Converts a specific column of a DataRow to a decimal.
    /// </summary>
    /// <param name="source">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the decimal value.</param>
    /// <param name="culture">An optional IFormatProvider that supplies culture-specific formatting information. Defaults to English culture if not provided.</param>
    /// <returns>A decimal representation of the DataRow's specific column's value. If the conversion fails or the value is null or DBNull, returns 0.</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName is null, empty or consists only of white-space characters.</exception>
    public static decimal ToDecimal(this DataRow source, string columnName, IFormatProvider culture = null)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
        if (culture == null) culture = new CultureInfo("EN");
        if (source[columnName] == DBNull.Value) return 0;

        var content = source.ToStringOrNull(columnName);
        if (string.IsNullOrWhiteSpace(content)) return 0;

        if (decimal.TryParse(content, NumberStyles.Any, culture, out var result))
        {
            return result;
        }

        return 0;
    }

    /// <summary>
    /// Converts a specific column of a DataRow to a float.
    /// </summary>
    /// <param name="source">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the float value.</param>
    /// <param name="culture">An optional IFormatProvider that supplies culture-specific formatting information. Defaults to English culture if not provided.</param>
    /// <returns>A float representation of the DataRow's specific column's value. If the conversion fails or the value is null or DBNull, returns 0.</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName is null, empty or consists only of white-space characters.</exception>
    public static float ToFloat(this DataRow source, string columnName, IFormatProvider culture = null)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
        if (culture == null) culture = new CultureInfo("EN");
        if (source[columnName] == DBNull.Value) return 0;
            
        var content = source.ToStringOrNull(columnName);
        if (string.IsNullOrWhiteSpace(content)) return 0;
            
        if (float.TryParse(content, NumberStyles.Any, culture, out var result))
        {
            return result;
        }
        return 0;
    }

    /// <summary>
    /// Converts a specific column of a DataRow to a double.
    /// </summary>
    /// <param name="source">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the double value.</param>
    /// <param name="culture">An optional IFormatProvider that supplies culture-specific formatting information. Defaults to English culture if not provided.</param>
    /// <returns>A double representation of the DataRow's specific column's value. If the conversion fails or the value is null or DBNull, returns 0.</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName is null, empty or consists only of white-space characters.</exception>
    public static double ToDouble(this DataRow source, string columnName, IFormatProvider culture = null)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
        if (culture == null) culture = new CultureInfo("EN");
        if (source[columnName] == DBNull.Value) return 0;
            
        var content = source.ToStringOrNull(columnName);
        if (string.IsNullOrWhiteSpace(content)) return 0;
            
        if (double.TryParse(content, NumberStyles.Any, culture, out var result))
        {
            return result;
        }
        return 0;
    }

    /// <summary>
    /// Converts a specific column of a DataRow to a boolean.
    /// </summary>
    /// <param name="source">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the boolean value.</param>
    /// <returns>A boolean representation of the DataRow's specific column's value. If the value is "1", "yes", or "true" (case-insensitive), returns true. Otherwise, returns false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName is null, empty or consists only of white-space characters.</exception>
    public static bool ToBoolean(this DataRow source, string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
        if (source[columnName] == DBNull.Value) return false;
            
        var content = source.ToStringOrNull(columnName);
        if (string.IsNullOrWhiteSpace(content)) return false;

        var normalizedContent = content.Trim().ToLowerInvariant();
        switch (normalizedContent)
        {
            case "1":
            case "yes":
            case "true": 
                return true;

            default: return false;
        }
    }
        
    /// <summary>
    /// Converts a specific column of a DataRow to a char.
    /// </summary>
    /// <param name="row">The DataRow that contains the data.</param>
    /// <param name="columnName">The name of the column that contains the character value.</param>
    /// <returns>A char representation of the DataRow's specific column's value. If the conversion fails or the value is null or DBNull, returns '\0'</returns>
    /// <exception cref="ArgumentNullException">Thrown when columnName is null, empty or consists only of white-space characters.</exception>
    /// <exception cref="Exception">Thrown when the string contains more than one character.</exception>
    public static char ToChar(this DataRow row, string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            
        if (row[columnName] == DBNull.Value) return '\0';
        var result = row.ToStringOrNull(columnName);
        if (result.Length > 1) throw new Exception("String contains more than one character");

        return result.First();
    }
}