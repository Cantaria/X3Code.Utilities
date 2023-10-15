using Xunit;
using System.Collections.Generic;
using X3Code.Utils.Extensions;

namespace X3Code.UnitTests.Extensions;

public class DictionaryExtensionsTest
{
    [Fact]
    public void AddKeyValuePair_ShouldAddKeyValuePair_WhenKeyIsNotPresentInDictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>();
        var keyValuePair = new KeyValuePair<string, string>("key", "value");

        // Act
        var isAdded = dictionary.Add(keyValuePair);

        // Assert
        Assert.True(isAdded);
        Assert.Equal("value", dictionary["key"]);
    }

    [Fact]
    public void AddKeyValuePair_ShouldNotAddKeyValuePair_WhenKeyIsPresentInDictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "key", "value" } };
        var keyValuePair = new KeyValuePair<string, string>("key", "anotherValue");

        // Act
        var isAdded = dictionary.Add(keyValuePair);

        // Assert
        Assert.False(isAdded);
        Assert.Equal("value", dictionary["key"]);
    }

    [Fact]
    public void AddTuple_ShouldAddTuple_WhenKeyIsNotPresentInDictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>();
        var tuple = ("key", "value");

        // Act
        var isAdded = dictionary.Add(tuple);

        // Assert
        Assert.True(isAdded);
        Assert.Equal("value", dictionary["key"]);
    }

    [Fact]
    public void AddTuple_ShouldNotAddTuple_WhenKeyIsPresentInDictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "key", "value" } };
        var tuple = ("key", "anotherValue");

        // Act
        var isAdded = dictionary.Add(tuple);

        // Assert
        Assert.False(isAdded);
        Assert.Equal("value", dictionary["key"]);
    }
}
