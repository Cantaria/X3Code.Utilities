using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO.Pipes;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions
{
    // ReSharper disable once InconsistentNaming
    public class DataRowExtension_specs
    {
        #region Const Values

        private const string DateTimePattern = "dd.MM.yyyy HH:mm:ss";
        
        private const string DateColumnName = "Date";
        private const string StringColumnName = "String";
        private const string EmptyColumnName = "Empty";
        private const string IntegerColumnName = "Integer";
        private const string DecimalColumnName = "Decimal";
        private const string DecimalDeColumnName = "DecimalDe";
        private const string FloatColumnName = "Float";
        private const string DoubleColumnName = "Double";
        private const string BooleanColumnName = "Boolean";
        private const string Boolean2ColumnName = "Boolean2";
        private const string Boolean3ColumnName = "Boolean3";
        private const string Boolean4ColumnName = "Boolean4";
        private const string CharColumnName = "Character";
        private const string NullColumnName = "NullColumn";

        private static readonly DateTime TestDate = new DateTime(2020, 05, 06, 13, 12, 30);
        private const string TestString = "Hello World.";
        private const int TestInt = 24;
        private const decimal TestDecimal = 24.12M;
        private const float TestFloat = 5.4F;
        private const double TestDouble = 7.1;
        private const bool TestBoolean = true;
        private const int TestBoolean2 = 1;
        private const string TestBoolean3 = "true";
        private const string TestBoolean4 = "false";
        private const char TestChar = 'a';

        #endregion

        #region Test Helpers

        private static DataTable CreateTestTable()
        {
            var result = new DataTable();
            result.Columns.Add(DateColumnName, typeof(string));
            result.Columns.Add(StringColumnName, typeof(string));
            result.Columns.Add(EmptyColumnName, typeof(string));
            result.Columns.Add(IntegerColumnName, typeof(string));
            result.Columns.Add(DecimalColumnName, typeof(string));
            result.Columns.Add(DecimalDeColumnName, typeof(string));
            result.Columns.Add(FloatColumnName, typeof(string));
            result.Columns.Add(DoubleColumnName, typeof(string));
            result.Columns.Add(BooleanColumnName, typeof(string));
            result.Columns.Add(Boolean2ColumnName, typeof(string));
            result.Columns.Add(Boolean3ColumnName, typeof(string));
            result.Columns.Add(Boolean4ColumnName, typeof(string));
            result.Columns.Add(CharColumnName, typeof(string));
            result.Columns.Add(NullColumnName, typeof(string));

            var row = result.NewRow();
            row[DateColumnName] = TestDate.ToString(DateTimePattern);
            row[StringColumnName] = TestString;
            row[IntegerColumnName] = TestInt;
            row[DecimalColumnName] = TestDecimal.ToString(new CultureInfo("en"));
            row[DecimalDeColumnName] = TestDecimal.ToString(new CultureInfo("de"));
            row[FloatColumnName] = TestFloat.ToString(new CultureInfo("en"));
            row[DoubleColumnName] = TestDouble.ToString(new CultureInfo("en"));
            row[BooleanColumnName] = TestBoolean;
            row[Boolean2ColumnName] = TestBoolean2;
            row[Boolean3ColumnName] = TestBoolean3;
            row[Boolean4ColumnName] = TestBoolean4;
            row[CharColumnName] = TestChar;
            row[NullColumnName] = DBNull.Value;
            row[EmptyColumnName] = string.Empty;
            result.Rows.Add(row);

            return result;
        }

        private static DataRow GetTestRow()
        {
            var testTable = CreateTestTable();
            return testTable.Rows[0];
        }

        #endregion

        [Fact]
        public void CanConvertString()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var stringResult = row.ToStringOrNull(StringColumnName);
            Assert.Equal(TestString, stringResult);

            var nullValue = row.ToStringOrNull(NullColumnName);
            Assert.Null(nullValue);
        }
        
        [Fact]
        public void CanConvertDate()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var dateResult = row.ToDateTime(DateColumnName, DateTimePattern);
            Assert.True(dateResult.HasValue);
            Assert.Equal(TestDate, dateResult.Value);

            var nullResult = row.ToDateTime(NullColumnName, DateTimePattern);
            Assert.Null(nullResult);
            
            var parseFailed = row.ToDateTime(DateColumnName, "hh-mm-dd");
            Assert.Null(parseFailed);
        }
        
        [Fact]
        public void CanConvertBool()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var boolResult = row.ToBoolean(BooleanColumnName);
            var bool2Result = row.ToBoolean(Boolean2ColumnName);
            var bool3Result = row.ToBoolean(Boolean3ColumnName);
            var bool4Result = row.ToBoolean(Boolean4ColumnName);
            Assert.True(boolResult);
            Assert.True(bool2Result);
            Assert.True(bool3Result);
            Assert.False(bool4Result);
            
            var nullValue = row.ToBoolean(NullColumnName);
            Assert.False(nullValue);
            
            var emptyValue = row.ToBoolean(EmptyColumnName);
            Assert.False(emptyValue);
        }
        
        [Fact]
        public void CanConvertInt()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var intResult = row.ToInteger(IntegerColumnName);
            Assert.Equal(TestInt, intResult);
            
            var nullValue = row.ToInteger(NullColumnName);
            Assert.Equal(0, nullValue);
        }
        
        [Fact]
        public void CanConvertDecimal()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var decimalResult = row.ToDecimal(DecimalColumnName);
            Assert.Equal(TestDecimal, decimalResult);
            
            var nullValue = row.ToDecimal(NullColumnName);
            Assert.Equal(0, nullValue);
            
            var emptyValue = row.ToDecimal(EmptyColumnName);
            Assert.Equal(0, emptyValue);
            
            var wrongFormat = row.ToDecimal(DecimalDeColumnName, new CultureInfo("de"));
            Assert.Equal(TestDecimal, wrongFormat);
            
            var nodecimal = row.ToDecimal(StringColumnName);
            Assert.Equal(0, nodecimal);
        }
        
        [Fact]
        public void CanConvertFloat()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var floatResult = row.ToFloat(FloatColumnName);
            Assert.Equal(TestFloat, floatResult);
            
            var nullValue = row.ToFloat(NullColumnName);
            Assert.Equal(0, nullValue);
            
            var emptyValue = row.ToFloat(EmptyColumnName);
            Assert.Equal(0, emptyValue);
            
            var nodecimal = row.ToFloat(StringColumnName);
            Assert.Equal(0, nodecimal);
        }
        
        [Fact]
        public void CanConvertDouble()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var doubleResult = row.ToDouble(DoubleColumnName);
            Assert.Equal(TestDouble, doubleResult);
            
            var nullValue = row.ToDouble(NullColumnName);
            Assert.Equal(0, nullValue);
            
            var emptyValue = row.ToDouble(EmptyColumnName);
            Assert.Equal(0, emptyValue);
            
            var nodecimal = row.ToDouble(StringColumnName);
            Assert.Equal(0, nodecimal);
        }
                
        [Fact]
        public void CanConvertChar()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            var charResult = row.ToChar(CharColumnName);
            Assert.Equal(TestChar, charResult);
            
            var nullValue = row.ToChar(NullColumnName);
            Assert.Equal(0, nullValue);
            
            Assert.Throws<Exception>(() => row.ToChar(StringColumnName));
        }

        [Fact]
        public void CanThrowArgumentNull()
        {
            var row = GetTestRow();
            Assert.NotNull(row);

            Assert.Throws<ArgumentNullException>(() => row.ToStringOrNull(string.Empty));
            Assert.Throws<ArgumentNullException>(() => row.ToDateTime(string.Empty, DateTimePattern));
            Assert.Throws<ArgumentNullException>(() => row.ToDateTime(DateColumnName, string.Empty));
            Assert.Throws<ArgumentNullException>(() => row.ToInteger(string.Empty));
            Assert.Throws<ArgumentNullException>(() => row.ToDecimal(string.Empty));
            Assert.Throws<ArgumentNullException>(() => row.ToFloat(string.Empty));
            Assert.Throws<ArgumentNullException>(() => row.ToDouble(string.Empty));
            Assert.Throws<ArgumentNullException>(() => row.ToBoolean(string.Empty));
            Assert.Throws<ArgumentNullException>(() => row.ToChar(string.Empty));
        }

        [Fact]
        public void CanConvertRowToEntity()
        {
            var reference = new Person
            {
                Name = "Darth Vader",
                Age = 99,
                Birthday = new DateTime(1990, 5, 4),
                CheckMark = 'D',
                IsEvil = true,
                SomeDecimal = 25.6M,
                SomeDouble = 17.3,
                SomeFloat = 19.2F
            };
            var asList = new List<Person> {reference};
            var dataTable = asList.ToDataTable();
            var row = dataTable.Rows[0];

            var test = row.ToDataType<Person>();
            Assert.NotNull(test);
            Assert.Equal(reference.Name, test.Name);
            Assert.Equal(reference.Age, test.Age);
            Assert.Equal(reference.Birthday, test.Birthday);
            Assert.Equal(reference.IsEvil, test.IsEvil);
            Assert.Equal(reference.CheckMark, test.CheckMark);
            Assert.Equal(reference.SomeDecimal, test.SomeDecimal);
            Assert.Equal(reference.SomeFloat, test.SomeFloat);
            Assert.Equal(reference.SomeDouble, test.SomeDouble);
        }
        
    }

    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsEvil { get; set; }
        public char CheckMark { get; set; }
        public decimal SomeDecimal { get; set; }
        public float SomeFloat { get; set; }
        public double SomeDouble { get; set; }
    }
}