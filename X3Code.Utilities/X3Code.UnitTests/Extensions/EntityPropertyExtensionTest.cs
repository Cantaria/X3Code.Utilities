using System;
using System.Globalization;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions;

public class EntityPropertyExtensionTest
{
    private const string StringPropertyName = "StringProperty";
    private const string IntegerPropertyName = "IntegerProperty";
    private const string DateTimePropertyName = "DateTimeProperty";
    private const string DecimalPropertyName = "DecimalProperty";
    private const string FloatPropertyName = "FloatProperty";
    private const string DoublePropertyName = "DoubleProperty";
    private const string BoolPropertyName = "BoolProperty";

    private class PropertyUnitTest
    {
        public string StringProperty { get; set; } = "Unit-Test";
        public int IntegerProperty { get; set; } = 42;
        public DateTime DateTimeProperty { get; set; } = new(2015, 5, 21); //Birthday X3-Code as hobby-project
        public decimal DecimalProperty { get; set; } = 42.42M;
        public float FloatProperty { get; set; } = 24F;
        public double DoubleProperty { get; set; } = 12F;
        public bool BoolProperty { get; set; } = true;
    }

    [Fact]
    public void CanReadDataAsStringNonGeneric()
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
        Assert.Equal("42.42", decimalResult);
        Assert.Equal("24", floatResult);
        Assert.Equal("12", doubleResult);
        Assert.Equal("True", boolResult);

        var formatDateResult = tester.TryReadPropertyAsString(DateTimePropertyName, dateTimeFormat: "dd.MM.yyyy");
        var formatDecimalResult = tester.TryReadPropertyAsString(DecimalPropertyName, "000.00");
        var formatFloatResult = tester.TryReadPropertyAsString(FloatPropertyName, "000.00");
        var formatDoubleResult = tester.TryReadPropertyAsString(DoublePropertyName, "000.00");

        Assert.Equal("21.05.2015", formatDateResult);
        Assert.Equal("042.42", formatDecimalResult);
        Assert.Equal("024.00", formatFloatResult);
        Assert.Equal("012.00", formatDoubleResult);
        
        //Special case: if a number format is given, but no number is wanted
        var numberFormatForString = tester.TryReadPropertyAsString(BoolPropertyName, numberFormat: "000.00");
        Assert.Equal("True", numberFormatForString);
    }
    
    [Fact]
    public void CanHandleNullsNonGeneric()
    {
        PropertyUnitTest? tester = null;
        var inputNullResult = tester.TryReadPropertyAsString(DecimalPropertyName);
        Assert.Equal("", inputNullResult);

        var propertyNullResult = tester.TryReadPropertyAsString(string.Empty);
        Assert.Equal("", propertyNullResult);

        var secondTester = new PropertyUnitTest();
        var nonExistProperty = secondTester.TryReadPropertyAsString("DontExist");
        Assert.Equal("", nonExistProperty);
    }

    [Fact]
    public void CanReadDataGeneric()
    {
        var tester = new PropertyUnitTest();
        var stringResult = tester.TryReadProperty<string, PropertyUnitTest>(StringPropertyName);
        var intResult = tester.TryReadProperty<int, PropertyUnitTest>(IntegerPropertyName);
        var dateTimeResult = tester.TryReadProperty<DateTime, PropertyUnitTest>(DateTimePropertyName);
        var decimalResult = tester.TryReadProperty<decimal, PropertyUnitTest>(DecimalPropertyName);
        var floatResult = tester.TryReadProperty<float, PropertyUnitTest>(FloatPropertyName);
        var doubleResult = tester.TryReadProperty<double, PropertyUnitTest>(DoublePropertyName);
        var boolResult = tester.TryReadProperty<bool, PropertyUnitTest>(BoolPropertyName);

        Assert.Equal("Unit-Test", stringResult);
        Assert.Equal(42, intResult);
        Assert.Equal(new DateTime(2015, 5, 21, 0, 0, 0), dateTimeResult);
        Assert.Equal(42.42M, decimalResult);
        Assert.Equal(24F, floatResult);
        Assert.Equal(12F, doubleResult);
        Assert.True(boolResult);
    }

    [Fact]
    public void CanHandleNullsGeneric()
    {
        PropertyUnitTest? tester = null;
        var inputNullResult = tester.TryReadProperty<decimal, PropertyUnitTest>(DecimalPropertyName);
        Assert.Equal(new decimal(), inputNullResult);

        var propertyNullResult = tester.TryReadProperty<decimal, PropertyUnitTest>(string.Empty);
        Assert.Equal(new decimal(), propertyNullResult);

        var secondTester = new PropertyUnitTest();
        var nonExistProperty = secondTester.TryReadProperty<decimal, PropertyUnitTest>("DontExist");
        Assert.Equal(new decimal(), nonExistProperty);
    }
    
    
    [Fact]
    public void CanHandleTypeMissMatchGenric()
    {
        var tester = new PropertyUnitTest();

        Assert.Throws<InvalidOperationException>(() => tester.TryReadProperty<int, PropertyUnitTest>(DecimalPropertyName));
    }
}