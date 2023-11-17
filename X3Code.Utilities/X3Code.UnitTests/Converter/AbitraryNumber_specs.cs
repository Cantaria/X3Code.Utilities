using System;
using X3Code.Utils.Convert;
using Xunit;
// ReSharper disable InconsistentNaming

namespace X3Code.UnitTests.Converter;

public class AbitraryNumber_specs
{
    [Theory]
    [InlineData("11011010", 2, 218)]
    [InlineData("DA", 16, 218)]
    [InlineData("332", 8, 218)]
    [InlineData("A", 36, 10)]
    public void ConvertToDecimal(string number, int numberSystemBase, long expected)
    {
        var result = ArbitraryNumberConverter.ArbitraryToDecimalSystem(number, numberSystemBase);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("11011010", 2, 218)]
    [InlineData("DA", 16, 218)]
    [InlineData("332", 8, 218)]
    [InlineData("A", 36, 10)]
    public void ConvertFromDecimal(string expected, int numberSystemBase, long number)
    {
        var result = ArbitraryNumberConverter.DecimalToArbitrarySystem(number, numberSystemBase);
        Assert.Equal(expected, result);
    }
        
    [Fact]
    public void CanHandleInvalids()
    {
        var number = 332;
        Assert.Throws<ArgumentException>(() => ArbitraryNumberConverter.DecimalToArbitrarySystem(number, 37));
            
        var zeroResult = ArbitraryNumberConverter.DecimalToArbitrarySystem(0, 8);
        Assert.Equal("0", zeroResult);
    }
}