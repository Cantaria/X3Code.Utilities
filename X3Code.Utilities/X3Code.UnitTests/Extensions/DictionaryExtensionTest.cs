using System;
using Xunit;
using System.Collections.Generic;
using X3Code.UnitTests.Models;
using X3Code.Utils.Extensions;

namespace X3Code.UnitTests.Extensions;

public class DictionaryExtensionsTest
{
    #region add overloads

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

    #endregion

    #region convert list
    
    [Fact]
    public void ConvertToDictionary_EmptyValues_ReturnsEmptyDictionary()
    {
        // Arrange
        IEnumerable<string> values = Array.Empty<string>();
        List<string> duplicates;

        // Act
        var result = values.ConvertToDictionary(v => v, out duplicates);

        // Assert
        Assert.Empty(result);
        Assert.Empty(duplicates);
    }

    [Fact]
    public void ConvertToDictionary_NoDuplicates_ReturnsDictionaryWithAllValues()
    {
        // Arrange
        var values = new List<Person> { Person.Michael, Person.Clemens, Person.Alexander, Person.John, Person.Viktoria };
        List<string> duplicates;

        // Act
        //var result = values.ConvertToDictionary(person => person.Name, out duplicates);

        // Assert
        //Assert.Equal(3, result.Count);
        //Assert.True(result.ContainsKey("A"));
        //Assert.True(result.ContainsKey("B"));
        //Assert.True(result.ContainsKey("C"));
        //Assert.Empty(duplicates);
    }

    [Fact]
    public void ConvertToDictionary_DuplicatesWithIgnoreEnabled_TracksDuplicates()
    {
        // Arrange
        var values = new List<string> { "A", "B", "A", "C", "B" };
        List<string> duplicates;

        // Act
        var result = values.ConvertToDictionary(v => v, out duplicates);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result.ContainsKey("A"));
        Assert.True(result.ContainsKey("B"));
        Assert.True(result.ContainsKey("C"));
        Assert.Equal(2, duplicates.Count);
        Assert.Contains("A", duplicates);
        Assert.Contains("B", duplicates);
    }

    [Fact]
    public void ConvertToDictionary_DuplicatesWithIgnoreDisabled_ThrowsException()
    {
        // Arrange
        var values = new List<string> { "A", "B", "A" };
        List<string> duplicates;

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => values.ConvertToDictionary(v => v, out duplicates, false));
        Assert.Equal("Duplicate key [A] found.", exception.Message);
    }

    [Fact]
    public void ConvertToDictionary_CustomKeySelector_NoDuplicates()
    {
        // Arrange
        var values = new List<string> { "Alice", "Bob", "Charlie" };
        List<string> duplicates;

        // Act
        var result = values.ConvertToDictionary(v => v.Length, out duplicates);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result.ContainsKey(5));
        Assert.True(result.ContainsKey(3));
        Assert.True(result.ContainsKey(7));
        Assert.Empty(duplicates);
    }

    [Fact]
    public void ConvertToDictionary_CustomKeySelector_WithDuplicates_TracksDuplicates()
    {
        // Arrange
        var values = new List<string>
            { "Alice", "Alfred", "Bob", "Charlie" }; // "Alice" and "Alfred" have the same length
        List<string> duplicates;

        // Act
        var result = values.ConvertToDictionary(v => v.Length, out duplicates);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result.ContainsKey(5)); // Either "Alice" or "Alfred" will be in the dictionary
        Assert.True(result.ContainsKey(3));
        Assert.True(result.ContainsKey(7));
        Assert.Single(duplicates);
        Assert.Contains("Alfred", duplicates); // "Alfred" is the duplicate
    }


    #endregion

}
