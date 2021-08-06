using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace X3Code.Utils.Extensions
{
    public static class DataRowExtension
    {
        /// <summary>
        /// Tries to convert a date string from the row and returns a nullable DateTime object
        /// </summary>
        /// <param name="source">The whole DataRow</param>
        /// <param name="columnName">The name of the column</param>
        /// <param name="datePattern">The pattern for the date format. Like: dd.MM.yyyy or MM/dd/yyyy ...</param>
        /// <returns>The DateTime on success, otherwise null</returns>
        public static DateTime? ToDateTime(this DataRow source, string columnName, string datePattern)
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
        /// Returns the value as int.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int ToInteger(this DataRow source, string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            var content = source.ToStringOrNull(columnName);

            if (int.TryParse(content, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Tries to convert the string to a decimal.
        /// </summary>
        /// <param name="source">The whole DataRow</param>
        /// <param name="columnName">The name of the column</param>
        /// <param name="culture">The culture information for the value to parse. If none is given, culture EN will be choosen</param>
        /// <returns>The Decimal on success, otherwise '0'</returns>
        public static decimal ToDecimal(this DataRow source, string columnName, IFormatProvider culture = null)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            if (culture == null) culture = new CultureInfo("EN");
            if (source[columnName] == DBNull.Value) return 0;
            
            var content = source.ToStringOrNull(columnName);
            if (string.IsNullOrWhiteSpace(content)) return 0;
            //TODO: not strict enough!
            if (decimal.TryParse(content, NumberStyles.Any, culture, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Tries to convert the string to a float.
        /// </summary>
        /// <param name="source">The whole DataRow</param>
        /// <param name="columnName">The name of the column</param>
        /// <param name="culture">The culture information for the value to parse. If none is given, culture EN will be choosen</param>
        /// <returns>The float on success, otherwise '0'</returns>
        public static float ToFloat(this DataRow source, string columnName, IFormatProvider culture = null)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            if (culture == null) culture = new CultureInfo("EN");
            if (source[columnName] == DBNull.Value) return 0;
            
            var content = source.ToStringOrNull(columnName);
            if (string.IsNullOrWhiteSpace(content)) return 0;
            
            if (float.TryParse(content, NumberStyles.Any, culture, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Tries to convert the string to a double.
        /// </summary>
        /// <param name="source">The whole DataRow</param>
        /// <param name="columnName">The name of the column</param>
        /// <param name="culture">The culture information for the value to parse. If none is given, culture EN will be choosen</param>
        /// <returns>The double on success, otherwise '0'</returns>
        public static double ToDouble(this DataRow source, string columnName, IFormatProvider culture = null)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            if (culture == null) culture = new CultureInfo("EN");
            if (source[columnName] == DBNull.Value) return 0;
            
            var content = source.ToStringOrNull(columnName);
            if (string.IsNullOrWhiteSpace(content)) return 0;
            
            if (double.TryParse(content, NumberStyles.Any, culture, out var result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Tries to convert a string to boolean.
        /// </summary>
        /// <param name="source">The whole DataRow</param>
        /// <param name="columnName">The name of the column</param>
        /// <returns>The boolean on success, otherwise false</returns>
        public static bool ToBoolean(this DataRow source, string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            if (source[columnName] == DBNull.Value) return false;
            
            var content = source.ToStringOrNull(columnName);
            if (string.IsNullOrWhiteSpace(content)) return false;

            var normalizedContent = content.Trim().ToLowerInvariant();
            switch (normalizedContent)
            {
                case "1":
                case "yes":
                case "true": 
                    return true;

                default: return false;
            }
        }
        
        /// <summary>
        /// Tries to convert the column content to a char
        /// </summary>
        /// <param name="row">The whole DataRow</param>
        /// <param name="columnName">The column name within the row</param>
        /// <returns>The character on success, otherwise '\0'</returns>
        public static char ToChar(this DataRow row, string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            
            if (row[columnName] == DBNull.Value) return '\0';
            var result = row.ToStringOrNull(columnName);
            if (result.Length > 1) throw new Exception("String contains more than one character");

            return result.First();
        }
    }
}