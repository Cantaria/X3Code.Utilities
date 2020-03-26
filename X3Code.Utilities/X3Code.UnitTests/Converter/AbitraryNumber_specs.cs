using X3Code.Utils.Convert;
using Xunit;

namespace X3Code.UnitTests
{
    public class AbitraryNumber_specs
    {
        [Theory]
        [InlineData("11011010", 2, 218)]
        [InlineData("DA", 16, 218)]
        [InlineData("332", 8, 218)]
        [InlineData("A", 36, 10)]
        public void ConvertToDecimal(string number, int numberSystemBase, long expected)
        {
            var result = AbitraryNumberConverter.ArbitraryToDecimalSystem(number, numberSystemBase);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("11011010", 2, 218)]
        [InlineData("DA", 16, 218)]
        [InlineData("332", 8, 218)]
        [InlineData("A", 36, 10)]
        public void ConvertFromDecimal(string expected, int numberSystemBase, long number)
        {
            var result = AbitraryNumberConverter.DecimalToArbitrarySystem(number, numberSystemBase);
            Assert.Equal(expected, result);
        }
    }
}
