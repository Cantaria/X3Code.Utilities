using System;
using System.ComponentModel;
using System.Data;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions
{
    // ReSharper disable once InconsistentNaming
    public class DataRowExtension_specs
    {
        #region Const Values

        private const string DateColumnName = "Date";
        private const string StringColumnName = "String";
        private const string IntegerColumnName = "Integer";
        private const string DecimalColumnName = "Decimal";
        private const string FloatColumnName = "Float";
        private const string DoubleColumnName = "Double";
        private const string BooleanColumnName = "Boolean";
        private const string Boolean2ColumnName = "Boolean2";
        private const string Boolean3ColumnName = "Boolean3";
        private const string CharColumnName = "Character";

        private static readonly DateTime TestDate = new DateTime(2020, 05, 06, 13, 12, 30);
        private const string TestString = "Hello World.";
        private const int TestInt = 24;
        private const decimal TestDecimal = 24.12M;
        private const float TestFloat = 5.4F;
        private const double TestDouble = 7.1;
        private const bool TestBoolean = true;
        private const int TestBoolean2 = 1;
        private const string TestBoolean3 = "true";
        private const char TestChar = 'a';

        #endregion

        #region Test Helpers

        private static DataTable CreateTestTable()
        {
            var result = new DataTable();
            result.Columns.Add(DateColumnName, typeof(string));
            result.Columns.Add(StringColumnName, typeof(string));
            result.Columns.Add(IntegerColumnName, typeof(string));
            result.Columns.Add(DecimalColumnName, typeof(string));
            result.Columns.Add(FloatColumnName, typeof(string));
            result.Columns.Add(DoubleColumnName, typeof(string));
            result.Columns.Add(BooleanColumnName, typeof(string));
            result.Columns.Add(Boolean2ColumnName, typeof(string));
            result.Columns.Add(Boolean3ColumnName, typeof(string));
            result.Columns.Add(CharColumnName, typeof(string));

            var row = result.NewRow();
            row[DateColumnName] = TestDate.ToString("dd.MM.yyyy HH:mm:ss");
            row[StringColumnName] = TestString;
            row[IntegerColumnName] = TestInt;
            row[DecimalColumnName] = TestDecimal;
            row[FloatColumnName] = TestFloat;
            row[DoubleColumnName] = TestDouble;
            row[BooleanColumnName] = TestBoolean;
            row[Boolean2ColumnName] = TestBoolean2;
            row[Boolean3ColumnName] = TestBoolean3;
            row[CharColumnName] = TestChar;
            result.Rows.Add(row);

            return result;
        }

        #endregion

        [Fact]
        public void CanConvertString()
        {
            var testTable = CreateTestTable();
            var row = testTable.Rows[0];
            Assert.NotNull(row);

            var stringResult = row.ToStringOrNull(StringColumnName);
            Assert.Equal(TestString, stringResult);
        }
        
        [Fact]
        public void CanConvertDate()
        {
            var testTable = CreateTestTable();
            var row = testTable.Rows[0];
            Assert.NotNull(row);

            var dateResult = row.ToDateTime(DateColumnName, "dd.MM.yyyy HH:mm:ss");
            Assert.True(dateResult.HasValue);
            Assert.Equal(TestDate, dateResult.Value);
        }
        
        [Fact]
        public void CanConvertBool()
        {
            var testTable = CreateTestTable();
            var row = testTable.Rows[0];
            Assert.NotNull(row);

            var boolResult = row.ToBoolean(BooleanColumnName);
            var bool2Result = row.ToBoolean(Boolean2ColumnName);
            var bool3Result = row.ToBoolean(Boolean3ColumnName);
            Assert.True(boolResult);
            Assert.True(bool2Result);
            Assert.True(bool3Result);
        }
    }
}