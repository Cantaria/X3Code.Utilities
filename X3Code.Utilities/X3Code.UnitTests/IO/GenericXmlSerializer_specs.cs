using System;
using System.Collections.Generic;
using System.IO;
using X3Code.Utils.Serializing;
using Xunit;

namespace X3Code.UnitTests.IO;

[Serializable]
public class TestClass
{
    public string? StringField { get; set; }
    public int IntField { get; set; }
    public List<string> ListField { get; set; } = [];
}

internal static class TestClassHelper
{
    internal static bool AreBothTheSame(TestClass control, TestClass toCheck)
    {
        if (control.IntField != toCheck.IntField) return false;
        if (control.StringField != toCheck.StringField) return false;

        var controlList = control.ListField.ToArray();
        var checkList = toCheck.ListField.ToArray();

        if (controlList.Length != checkList.Length) return false;

        for (var i = 0; i < control.ListField.Count; i++)
        {
            if (controlList[i] != checkList[i]) return false;
        }

        return true;
    }
}

// ReSharper disable once InconsistentNaming
public class XmlSerializer_specs
{
    private TestClass? _testData;
    private readonly string _savePath = Path.Combine(Environment.CurrentDirectory, "Xml_specs.xml");

    [Fact]
    public void CanSaveAndLoadAnXmlFile()
    {
        var referenceData = new TestClass
        {
            IntField = 25,
            ListField = ["Hello", "World"],
            StringField = "Kuckuck"
        };

        //Should not throw an Exception
        GenericXmlSerializer.SaveXml(_savePath, referenceData);
        _testData = GenericXmlSerializer.LoadXml<TestClass>(_savePath);

        Assert.Equal(_testData.IntField, referenceData.IntField);
        Assert.Equal(_testData.ListField, referenceData.ListField);
        Assert.Equal(_testData.StringField, referenceData.StringField);
        Assert.True(TestClassHelper.AreBothTheSame(_testData, referenceData));
    }

    [Fact]
    public void CanDeserialize()
    {
        var referenceData = new TestClass
        {
            IntField = 25,
            ListField = new List<string> { "Hello", "World" },
            StringField = "Kuckuck"
        };

        //Should not throw an Exception
        var testString = GenericXmlSerializer.SerializeObjectToXmlString(referenceData);
        _testData = GenericXmlSerializer.DeserializeXmlStringTo<TestClass>(testString);

        Assert.Equal(_testData.IntField, referenceData.IntField);
        Assert.Equal(_testData.ListField, referenceData.ListField);
        Assert.Equal(_testData.StringField, referenceData.StringField);
        Assert.True(TestClassHelper.AreBothTheSame(_testData, referenceData));
    }

    [Fact]
    public void CanSerialize()
    {
        var referenceData = new TestClass
        {
            IntField = 25,
            ListField = new List<string> { "Hello", "World" },
            StringField = "Kuckuck"
        };

        //Should not throw an Exception
        var testString = GenericXmlSerializer.SerializeToXmlInMemory(referenceData);
        _testData = GenericXmlSerializer.DeserializeXmlMemoryStreamTo<TestClass>(testString);

        Assert.Equal(_testData.IntField, referenceData.IntField);
        Assert.Equal(_testData.ListField, referenceData.ListField);
        Assert.Equal(_testData.StringField, referenceData.StringField);
        Assert.True(TestClassHelper.AreBothTheSame(_testData, referenceData));

    }
}