using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X3Code.Utils.Convert;
using Xunit;

// ReSharper disable InconsistentNaming

namespace X3Code.UnitTests.Converter;

public class ConvertList_specs
{
    #region Helpers

    private class A
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
    }

    private class B
    {
        public string NameValue { get; set; }
        public string DescriptionValue { get; set; }
        public int NumberValue { get; set; }
    }

    private IEnumerable<A> SeedData()
    {
        return new List<A>
        {
            new A{Name = "C#", Description = "Programming Language", Number = 1},
            new A{Name = "TCP/IP", Description = "Network protocol", Number = 2},
            new A{Name = "Javascript", Description = "Scripting Language", Number = 3}
        };
    }

    private static B ToB(A source)
    {
        return new B
        {
            DescriptionValue = source.Description,
            NameValue = source.Name,
            NumberValue = source.Number
        };
    }

    #endregion

    [Fact]
    public void ConvertAWholeList()
    {
        var source = SeedData().ToList();
        var result = source.ConvertList(ToB).ToList();

        Assert.Equal(source.Count, result.Count);
        Assert.Contains(result, b => b.NumberValue == 3);
        Assert.Contains(result, b => b.NumberValue == 2);
        Assert.Contains(result, b => b.NumberValue == 1);
        Assert.Contains(result, b => b.NameValue == "C#");
        Assert.Contains(result, b => b.NameValue == "TCP/IP");
        Assert.Contains(result, b => b.NameValue == "Javascript");
        Assert.Contains(result, b => b.DescriptionValue == "Programming Language");
        Assert.Contains(result, b => b.DescriptionValue == "Network protocol");
        Assert.Contains(result, b => b.DescriptionValue == "Scripting Language");
    }
}