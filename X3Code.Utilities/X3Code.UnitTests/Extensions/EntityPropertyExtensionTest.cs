using System;
using System.Globalization;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions;

public class EntityPropertyExtensionTest
{
    public const string StringPropertyName = "StringProperty";
    public const string IntegerPropertyName = "IntegerProperty";
    public const string DateTimePropertyName = "DateTimeProperty";
    public const string DecimalPropertyName = "DecimalProperty";
    public const string FloatPropertyName = "FloatProperty";
    public const string DoublePropertyName = "DoubleProperty";
    public const string BoolPropertyName = "BoolProperty";
    
    private class PropertyUnitTest
    {
        public string StringProperty { get; set; } = "Unit-Test";
        public int IntegerProperty { get; set; } = 42;
        public DateTime DateTimeProperty { get; set; } = new (2015, 5, 21); //Birthday X3-Code as hobby-project
        public decimal DecimalProperty { get; set; } = 42.42M;
        public float FloatProperty { get; set; } = 24F;
        public double DoubleProperty { get; set; } = 12F;
        public bool BoolProperty { get; set; } = true;
    }
    
    [Fact]
    public void CanReadDataAsString()
    {
        var tester = new PropertyUnitTest();
        var stringResult = tester.TryReadPropertyAsString(StringPropertyName);
        var intResult = tester.TryReadPropertyAsString(IntegerPropertyName);
        var dateTimeResult = tester.TryReadPropertyAsString(DateTimePropertyName);
        var decimalResult = tester.TryReadPropertyAsString(DecimalPropertyName);
        var floatResult = tester.TryReadPropertyAsString(FloatPropertyName);
        var doubleResult = tester.TryReadPropertyAsString(DoublePropertyName);
        var boolResult = tester.TryReadPropertyAsString(BoolPropertyName);
        
        Assert.Equal("Unit-Test", stringResult);
        Assert.Equal("42", intResult);
        Assert.Equal("2015.05.21 00:00", dateTimeResult);
        Assert.Equal("42,42", decimalResult);
        Assert.Equal("24", floatResult);
        Assert.Equal("12", doubleResult);
        Assert.Equal("True", boolResult);

        var formatDateResult = tester.TryReadPropertyAsString(DateTimePropertyName, dateTimeFormat: "dd.MM.yyyy");
        var formatDecimalResult = tester.TryReadPropertyAsString(DecimalPropertyName, "000.00");
        var formatFloatResult = tester.TryReadPropertyAsString(FloatPropertyName, "000.00");
        var formatDoubleResult = tester.TryReadPropertyAsString(DoublePropertyName, "000.00");
        
        Assert.Equal("21.05.2015", formatDateResult);
        Assert.Equal("042,42", formatDecimalResult);
        Assert.Equal("024,00", formatFloatResult);
        Assert.Equal("012,00", formatDoubleResult);
    }
}