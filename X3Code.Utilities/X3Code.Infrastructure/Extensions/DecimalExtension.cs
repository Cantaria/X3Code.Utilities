using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace X3Code.Infrastructure.Extensions;

/// <summary>
/// Provides some decimal configurations for decimals
/// </summary>
public static class DecimalExtension
{
    /// <summary>
    /// Configures a decimal with 19 digits and up to 4 digits right of the decimal point
    /// </summary>
    /// <param name="property">The EF-property which should be set</param>
    public static void ConfigureDecimalColum(this PropertyBuilder<decimal> property)
    {
        property.HasColumnType("decimal(19, 4)");
    }

    /// <summary>
    /// Configures a decimal with 19 digits and up to 4 digits right of the decimal point
    /// </summary>
    /// <param name="property">The EF-property which should be set</param>
    /// <param name="precision">The maximum total number of decimal digits to be stored. This number includes both the left and the right sides of the decimal point. The precision must be a value from 1 through the maximum precision of 38.</param>
    /// <param name="scale">The number of decimal digits that are stored to the right of the decimal point. This number is subtracted from p to determine the maximum number of digits to the left of the decimal point. The default scale is 0 and so 0 <= s <= p. Maximum storage sizes vary, based on the precision.</param>
    public static void ConfigureDecimalColumn(this PropertyBuilder<decimal> property, int precision, int scale)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (precision is <= 0 or > 38)
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be between 1 and 38.");
        if (scale < 0 || scale >= precision)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be non-negative and less than precision.");
        
        property.HasColumnType($"decimal({precision}, {scale})");
    }
}