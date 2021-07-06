using System;
using System.ComponentModel;
using System.Data;

namespace X3Code.UnitTests.Extensions
{
    // ReSharper disable once InconsistentNaming
    public class DataRowExtension_specs
    {
        private const string DateColumName = "Date";
        private const string StringColumName = "String";
        private const string IntegerColumName = "Integer";
        private const string DecimalColumName = "Decimal";
        private const string FloatColumName = "Float";
        private const string DoubleColumName = "Double";
        private const string BooleanColumName = "Boolean";
        private const string CharColumName = "Character";

        private static DateTime _testDate = new DateTime(2020, 05, 06, 13, 12, 30);
        private static string _testString = "Hello World.";
        private static int _testInt = 24;
        private static decimal _testDecimal = 24.12M;
        private static float _testFloat = 5.4F;
        private static double _testDouble = 7.1;
        private static bool _testBoolean = true;
        private static char _testChar = 'a';
        
        private static DataTable GetTestTable()
        {
            var result = new DataTable();
            result.Columns.Add(DateColumName, typeof(DateTime));
            result.Columns.Add(StringColumName, typeof(string));
            result.Columns.Add(IntegerColumName, typeof(int));
            result.Columns.Add(DecimalColumName, typeof(decimal));
            result.Columns.Add(FloatColumName, typeof(float));
            result.Columns.Add(DoubleColumName, typeof(double));
            result.Columns.Add(BooleanColumName, typeof(bool));
            result.Columns.Add(CharColumName, typeof(char));

            var row = result.NewRow();
            row[DateColumName] = _testDate;
            row[StringColumName] = _testString;

            return result;
        }
    }
}