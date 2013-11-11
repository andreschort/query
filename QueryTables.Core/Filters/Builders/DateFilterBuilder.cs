using System;
using System.Linq;
using QueryTables.Common.Util;

namespace QueryTables.Core.Filters.Builders
{
    public class DateFilterBuilder : IFilterBuilder
    {
        private const char DefaultSeparator = ';';

        public DateFilterBuilder()
        {
            this.Separator = DefaultSeparator;
        }

        public char Separator { get; set; }

        public static DateTime?[] Parse(string value, char separator = DefaultSeparator)
        {
            var parts = value.Split(new[] { separator }, 2);

            var from = StringUtil.ToDateNullable(parts[0]);

            DateTime? to = null;
            if (parts.Count() > 1)
            {
                to = StringUtil.ToDateNullable(parts[1]);
            }

            return new[] { from, to };
        }

        public Filter Create<T>(QueryField<T> field, string value)
        {
            var filter = new Filter { Name = field.Name, OriginalText = value };

            var parsed = Parse(value, this.Separator);
            var from = parsed[0];
            var to = parsed[1];

            filter.Valid = parsed[0].HasValue || to.HasValue;

            if (from.HasValue && to.HasValue)
            {
                filter.Operator = FilterOperator.Between;
                filter.Values.Add(from.Value.Date);
                filter.Values.Add(to.Value.Date);
            }
            else if (from.HasValue)
            {
                filter.Operator = FilterOperator.GreaterThanEqual;
                filter.Values.Add(from.Value.Date);
            }
            else if (to.HasValue)
            {
                filter.Operator = FilterOperator.LessThanEqual;
                filter.Values.Add(to.Value.Date);
            }

            return filter;
        }
    }
}
