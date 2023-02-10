using System.Collections.Generic;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions;

public class ListExtensionTest
{
    [Fact]
    public void CanDetectEmptyList()
    {
        var empty = new List<string>();
        List<object> nothing = null;

        Assert.True(empty.IsNullOrEmpty());
        Assert.True(nothing.IsNullOrEmpty());
        
        empty.Add("Hello");
        Assert.False(empty.IsNullOrEmpty());

        string[] array = null;
        Assert.True(array.IsNullOrEmpty());
    }
}