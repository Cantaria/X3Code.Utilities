using System;
using System.Data;
using System.Globalization;

namespace X3Code.Utils.Extensions
{
    public static class DataRowExtension
    {
        /// <summary>
        /// Tries to convert the column content to a string
        /// </summary>
        /// <param name="row">The whole DataRow</param>
        /// <param name="columnName">The column name within the row</param>
        /// <returns></returns>
        public static string ToStringOrNull(this DataRow row, string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            
            if (row[columnName] == DBNull.Value) return null;
            var result = (string)row[columnName];

            return result.Trim();
        }

        /// <summary>
        /// Tries to convert a date string from the row and returns a nullable DateTime object
        /// </summary>
        /// <param name="source">Thw whole DataRow</param>
        /// <param name="columnName">The name of the column</param>
        /// <param name="datePattern">The pattern for the date format. Like: dd.MM.yyyy or MM/dd/yyyy ...</param>
        /// <returns>The DateTime on success, otherwise null</returns>
        public static DateTime? ToDate(this DataRow source, string columnName, string datePattern)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            if (string.IsNullOrWhiteSpace(datePattern)) throw new ArgumentNullException(nameof(datePattern));
            
            var dateAsString = source.ToStringOrNull(columnName);
            if (string.IsNullOrEmpty(dateAsString)) return null;

            if (DateTime.TryParseExact(dateAsString, datePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return null;
        }
    }
}