using System;
using Xunit;

namespace X3Code.UnitTests.Extensions;

public class EntityPropertyExtensionTest
{
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
    public void TheDateShotNotBeLowerThanSqlMin()
    {
        
    }
}