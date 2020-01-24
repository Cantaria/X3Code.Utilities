using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace X3Code.Utils.Extensions
{
    //Source of Code:
    //https://blogs.msdn.microsoft.com/dal/2009/04/08/how-to-convert-an-ienumerable-to-a-datatable-in-the-same-way-as-we-use-tolist-or-toarray/
    /// <summary>
    /// Convert Ienumerable to a Datatable
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Converts an IEnumerable into a DataTable. It creates a column for every writeable Property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The colletion that needs to be converted</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string tableName)
        {
            var result = ToDataTable(collection);
            result.TableName = tableName;
            return result;
        }

        /// <summary>
        /// Converts an IEnumerable into a DataTable. It creates a column for every writeable Property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The colletion that needs to be converted</param>>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            var table = new DataTable();
            var objectType = typeof(T);
            var properties = objectType.GetProperties();
            //Create the columns in the DataTable

            foreach (var property in properties)
            {
                if (!property.CanWrite) continue;
                table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            //Populate the table
            foreach (var item in collection)
            {
                var row = table.NewRow();
                row.BeginEdit();

                foreach (var property in properties)
                {
                    if (!property.CanWrite) continue;
                    var value = property.GetValue(item, null);
                    if (value == null)
                    {
                        row[property.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[property.Name] = value;
                    }
                }
                row.EndEdit();
                table.Rows.Add(row);
            }
            return table;
        }

    }
}
