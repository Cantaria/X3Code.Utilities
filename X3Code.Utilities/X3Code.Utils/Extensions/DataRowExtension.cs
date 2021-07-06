using System.Data;

namespace X3Code.Utils.Extensions
{
    public static class DataRowExtension
    {
        public static string ToStringOrNull(this DataRow row, string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return null;

            return (string) row?[columnName];
        }
    }
}