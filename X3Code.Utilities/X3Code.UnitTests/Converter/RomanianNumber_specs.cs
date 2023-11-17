using X3Code.Utils.Convert;
using Xunit;
// ReSharper disable InconsistentNaming

namespace X3Code.UnitTests.Converter;

public class RomanianNumber_specs
{
    [Theory]
    [InlineData("(V)DLXII", 5562)]
    [InlineData("MDLXII", 1562)]
    [InlineData("V", 5)]
    [InlineData("I", 1)]
    [InlineData("II", 2)]
    [InlineData("III", 3)]
    [InlineData("IV", 4)]
    [InlineData("XIV", 14)]
    public void ConvertToArabic(string number, int expected)
    {
        var result = RomanianConverter.RomanToArabic(number);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("(V)DLXII", 5562)]
    [InlineData("MDLXII", 1562)]
    [InlineData("V", 5)]
    [InlineData("I", 1)]
    [InlineData("II", 2)]
    [InlineData("III", 3)]
    [InlineData("IV", 4)]
    [InlineData("XIV", 14)]
    public void ConvertRoRomanian(string expected, int number)
    {
        var result = RomanianConverter.ArabicToRoman(number);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HandleEmptyString()
    {
        var result = RomanianConverter.RomanToArabic(string.Empty);
        Assert.Equal(0, result);
    }
}