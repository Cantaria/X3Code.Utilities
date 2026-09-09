using System;
using X3Code.Utils.Converters;
using Xunit;

namespace X3Code.UnitTests.Converter;

/// <summary>
/// The converter used to hand the caller whatever the runtime happened to throw: a
/// NullReferenceException for null, a bare KeyNotFoundException naming neither the character nor
/// its position, and an IndexOutOfRangeException for a negative number.
/// </summary>
public class RomanianNumberValidation_specs
{
    [Fact]
    public void Null_input_is_an_argument_null_exception()
    {
        Assert.Throws<ArgumentNullException>(() => RomanianConverter.RomanToArabic(null!));
    }

    [Theory]
    [InlineData("MCMZIV", 'Z')]
    [InlineData("XI3", '3')]
    [InlineData("A", 'A')]
    public void A_character_that_is_no_roman_numeral_is_named(string input, char offender)
    {
        var exception = Assert.Throws<ArgumentException>(() => RomanianConverter.RomanToArabic(input));

        Assert.Contains($"[{offender}]", exception.Message);
        Assert.Contains("position", exception.Message);
    }

    [Fact]
    public void An_empty_string_is_still_zero()
    {
        Assert.Equal(0, RomanianConverter.RomanToArabic(string.Empty));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-4000)]
    public void A_negative_number_is_out_of_range(int input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RomanianConverter.ArabicToRoman(input));
    }
}
