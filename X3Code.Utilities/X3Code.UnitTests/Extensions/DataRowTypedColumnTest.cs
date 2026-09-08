using System;
using System.Data;
using X3Code.Utils.Extensions;
using Xunit;

namespace X3Code.UnitTests.Extensions;

/// <summary>
/// The existing DataRowExtensionTest builds its table exclusively from typeof(string) columns.
/// A DataTable coming back from SQL Server is typed, and against such a table the whole To*
/// family used to fail: ToStringOrNull cast the cell hard to string and threw an
/// InvalidCastException for every non-string column.
/// </summary>
public class DataRowTypedColumnTest
{
    private static readonly DateTime TestDate = new(2020, 5, 6, 13, 12, 30);

    private static DataRow CreateTypedRow()
    {
        var table = new DataTable();
        table.Columns.Add("Date", typeof(DateTime));
        table.Columns.Add("Integer", typeof(int));
        table.Columns.Add("Decimal", typeof(decimal));
        table.Columns.Add("Float", typeof(float));
        table.Columns.Add("Double", typeof(double));
        table.Columns.Add("Boolean", typeof(bool));
        table.Columns.Add("Character", typeof(char));
        table.Columns.Add("Null", typeof(string));

        var row = table.NewRow();
        row["Date"] = TestDate;
        row["Integer"] = 24;
        row["Decimal"] = 24.12M;
        row["Float"] = 5.4F;
        row["Double"] = 7.1D;
        row["Boolean"] = true;
        row["Character"] = 'a';
        row["Null"] = DBNull.Value;
        table.Rows.Add(row);

        return row;
    }

    [Fact]
    public void ToStringOrNullHandlesNonStringColumns()
    {
        var row = CreateTypedRow();

        // Invariant formatting, so this reads the same on any machine.
        Assert.Equal("24", row.ToStringOrNull("Integer"));
        Assert.Equal("24.12", row.ToStringOrNull("Decimal"));
        Assert.Null(row.ToStringOrNull("Null"));
    }

    [Fact]
    public void NumericColumnsAreConvertedWithoutGoingThroughText()
    {
        var row = CreateTypedRow();

        Assert.Equal(24, row.ToInteger("Integer"));
        Assert.Equal(24.12M, row.ToDecimal("Decimal"));
        Assert.Equal(5.4F, row.ToFloat("Float"));
        Assert.Equal(7.1D, row.ToDouble("Double"));
    }

    [Fact]
    public void DateTimeColumnDoesNotNeedAMatchingPattern()
    {
        var row = CreateTypedRow();

        // The pattern deliberately does not match how a DateTime renders. A typed column is
        // read natively, so it is irrelevant here.
        Assert.Equal(TestDate, row.ToDateTime("Date", "dd.MM.yyyy HH:mm:ss"));
    }

    [Fact]
    public void BooleanAndCharColumnsAreReadNatively()
    {
        var row = CreateTypedRow();

        Assert.True(row.ToBoolean("Boolean"));
        Assert.Equal('a', row.ToChar("Character"));
    }

    [Fact]
    public void NullColumnsKeepTheirDocumentedFallbacks()
    {
        var row = CreateTypedRow();

        Assert.Null(row.ToStringOrNull("Null"));
        Assert.Equal(0, row.ToInteger("Null"));
        Assert.Equal(0, row.ToDecimal("Null"));
        Assert.False(row.ToBoolean("Null"));
        Assert.Equal('\0', row.ToChar("Null"));
        Assert.Null(row.ToDateTime("Null", "dd.MM.yyyy"));
    }

    [Fact]
    public void EmptyStringYieldsTheDocumentedNullCharInsteadOfThrowing()
    {
        var table = new DataTable();
        table.Columns.Add("Empty", typeof(string));
        var row = table.NewRow();
        row["Empty"] = string.Empty;
        table.Rows.Add(row);

        // Used to run into an InvalidOperationException from First() on an empty string.
        Assert.Equal('\0', row.ToChar("Empty"));
    }
}
