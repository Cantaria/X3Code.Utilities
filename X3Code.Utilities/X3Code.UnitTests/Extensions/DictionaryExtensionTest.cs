using System;
using Xunit;
using System.Collections.Generic;
using X3Code.UnitTests.Mockups;
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

        // Act
        var result = values.ConvertToDictionary(person => person, out var duplicates);

        // Assert
        Assert.Empty(result);
        Assert.Empty(duplicates);
    }

    [Fact]
    public void ConvertToDictionary_NoDuplicates_ReturnsDictionaryWithAllValues()
    {
        // Arrange
        var values = new List<Person> { PersonMockups.Michael, PersonMockups.Clemens, PersonMockups.Alexander, PersonMockups.John, PersonMockups.Viktoria };

        // Act
        var result = values.ConvertToDictionary(person => person.Name!, out var duplicates);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.True(result.ContainsKey(PersonMockups.Michael.Name!));
        Assert.True(result.ContainsKey(PersonMockups.Alexander.Name!));
        Assert.True(result.ContainsKey(PersonMockups.Viktoria.Name!));
        Assert.Empty(duplicates);
    }

    [Fact]
    public void ConvertToDictionary_DuplicatesWithIgnoreEnabled_TracksDuplicates()
    {
        // Arrange
        var values = new List<Person> { PersonMockups.Michael, PersonMockups.Clemens, PersonMockups.Michael, PersonMockups.John, PersonMockups.Viktoria };

        // Act
        var result = values.ConvertToDictionary(person => person.Name!, out var duplicates);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.True(result.ContainsKey(PersonMockups.Michael.Name!)); 
        Assert.True(result.ContainsKey(PersonMockups.Viktoria.Name!));
        Assert.True(result.ContainsKey(PersonMockups.Clemens.Name!));
        Assert.Single(duplicates);
        Assert.Contains(duplicates, x => x.Name == PersonMockups.Michael.Name! );
    }

    [Fact]
    public void ConvertToDictionary_DuplicatesWithIgnoreDisabled_ThrowsException()
    {
        // Arrange
        var values = new List<Person> { PersonMockups.Michael, PersonMockups.Clemens, PersonMockups.Michael, PersonMockups.John, PersonMockups.Viktoria };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => values.ConvertToDictionary(person => person.Name!, out var duplicates, false));
        Assert.Equal($"Duplicate key [{PersonMockups.Michael.Name!}] found.", exception.Message);
    }

    #endregion

}
