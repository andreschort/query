using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QueryTables.Common.Extension
{
    public static class QueryableExtensions
    {
        public static DataTable ToDataTable(this IQueryable list)
        {
            var dataTable = new DataTable();
            var type = list.ElementType;
            var propertyInfoList = type.GetFields();

            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyType = Nullable.GetUnderlyingType(propertyInfo.FieldType);

                var column = propertyType != null
                        ? new DataColumn(propertyInfo.Name, propertyType)
                        : new DataColumn(propertyInfo.Name, propertyInfo.FieldType);
                
                dataTable.Columns.Add(column);
            }

            foreach (object obj in list)
            {
                var row = new object[propertyInfoList.Length];
                int i = 0;
                foreach (var p in propertyInfoList)
                {
                    row[i++] = p.GetValue(obj);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static List<dynamic> ToDynamic(this IQueryable list)
        {
            return Enumerable.Cast<object>(list).Cast<dynamic>().ToList();
        }
    }
}
