using System;
using System.Collections.Generic;
using System.IO;
using X3Code.Utils.Serializing;
using Xunit;

namespace X3Code.UnitTests.IO
{
    [Serializable]
    public class TestClass
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
        public List<string> ListField { get; set; }
    }

    internal class TestclassHelper
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
        private TestClass _testDaten;
        private readonly string _savePath = Path.Combine(Environment.CurrentDirectory, "Xml_specs.xml");

        [Fact]
        public void Falls_der_Xml_Worker_eine_Datei_speichert_und_Lädt()
        {
            var referenzDaten = new TestClass
            {
                IntField = 25,
                ListField = new List<string> { "Hello", "World" },
                StringField = "Kuckuck"
            };

            //Should not throw an Exception
            GenericXmlSerializer.SaveXml(_savePath, referenzDaten);
            _testDaten = GenericXmlSerializer.LoadXml<TestClass>(_savePath);

            Assert.Equal(_testDaten.IntField, referenzDaten.IntField);
            Assert.Equal(_testDaten.ListField, referenzDaten.ListField);
            Assert.Equal(_testDaten.StringField, referenzDaten.StringField);
            Assert.True(TestclassHelper.AreBothTheSame(_testDaten, referenzDaten));
        }

        [Fact]
        public void Falls_ein_Objekt_in_einen_Xml_String_serialisiert_wird()
        {
            var referenzDaten = new TestClass
            {
                IntField = 25,
                ListField = new List<string> { "Hello", "World" },
                StringField = "Kuckuck"
            };

            //Should not throw an Exception
            var testString = GenericXmlSerializer.SerializeObjectToXmlString(referenzDaten);
            _testDaten = GenericXmlSerializer.DeserializeXmlStringTo<TestClass>(testString);

            Assert.Equal(_testDaten.IntField, referenzDaten.IntField);
            Assert.Equal(_testDaten.ListField, referenzDaten.ListField);
            Assert.Equal(_testDaten.StringField, referenzDaten.StringField);
            Assert.True(TestclassHelper.AreBothTheSame(_testDaten, referenzDaten));
        }

        [Fact]
        public void Falls_ein_Objekt_in_Xml_InMemory_serialisiert_wird()
        {
            var referenzDaten = new TestClass
            {
                IntField = 25,
                ListField = new List<string> { "Hello", "World" },
                StringField = "Kuckuck"
            };

            //Should not throw an Exception
            var testString = GenericXmlSerializer.SerializeToXmlInMemory(referenzDaten);
            _testDaten = GenericXmlSerializer.DeserializeXmlMemoryStreamTo<TestClass>(testString);

            Assert.Equal(_testDaten.IntField, referenzDaten.IntField);
            Assert.Equal(_testDaten.ListField, referenzDaten.ListField);
            Assert.Equal(_testDaten.StringField, referenzDaten.StringField);
            Assert.True(TestclassHelper.AreBothTheSame(_testDaten, referenzDaten));

        }
    }
}
