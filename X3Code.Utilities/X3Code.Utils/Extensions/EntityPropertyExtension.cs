using System;

namespace X3Code.Utils.Extensions;

public static class EntityPropertyExtension
{
    /// <summary>
    /// Get the value for a specific property from an entity as string. If the property is not found or can't read, this method will return string.Empty.
    /// </summary>
    /// <param name="source">The source entity, the property is needed from</param>
    /// <param name="propertyName">The name for the property which needs to be read</param>
    /// <param name="numberFormat">If the property is a number (float, double, decimal), it's possible to provide an own format.</param>
    /// <param name="dateTimeFormat">Optional datetime format for output. If missing 'yyyy.MM.dd HH:mm' will be used.</param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static string? TryReadPropertyAsString<TEntity>(this TEntity source, string propertyName, string? numberFormat = null, string? dateTimeFormat = null)
    {
        if (source == null || string.IsNullOrWhiteSpace(propertyName)) return string.Empty;

        var property = typeof(TEntity).GetProperty(propertyName);
        if (property == null) return string.Empty;

        if (!string.IsNullOrWhiteSpace(numberFormat))
        {
            if (property.PropertyType == typeof(decimal))
                return source.TryReadProperty<decimal, TEntity>(propertyName).ToString(numberFormat);

            if (property.PropertyType == typeof(float))
                return source.TryReadProperty<float, TEntity>(propertyName).ToString(numberFormat);
        
            if (property.PropertyType == typeof(double))
                return source.TryReadProperty<double, TEntity>(propertyName).ToString(numberFormat);
        }

        if (property.PropertyType == typeof(DateTime))
        {
            var datetime = (DateTime)property.GetValue(source)!;
            if (string.IsNullOrWhiteSpace(dateTimeFormat))
            {
                return datetime.ToString("yyyy.MM.dd HH:mm");
            }

            return datetime.ToString(dateTimeFormat);
        }

        return property.GetValue(source)?.ToString();
    }

    /// <summary>
    /// Tries to read and return the specific property from the given entity
    /// </summary>
    /// <param name="source">Source entity, the property should be read</param>
    /// <param name="propertyName">The name for the property</param>
    /// <typeparam name="TOutput">Output type for the property</typeparam>
    /// <typeparam name="TEntity">Type of the input entity</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TOutput? TryReadProperty<TOutput, TEntity>(this TEntity source, string propertyName)
    {
        if (source == null || string.IsNullOrWhiteSpace(propertyName)) return default;
        
        var property = typeof(TEntity).GetProperty(propertyName);
        if (property == null) return default;

        if (property.PropertyType != typeof(TOutput)) 
            throw new InvalidOperationException($"Property type miss match. The source property is from type [{property.PropertyType}] and can't be converted into destination type [{typeof(TOutput)}].");

        return (TOutput) property.GetValue(source)!;
    }
}