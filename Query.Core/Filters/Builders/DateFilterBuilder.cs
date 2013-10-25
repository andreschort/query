using System;
using System.Globalization;
using System.Linq;
using Query.Common.Util;

namespace Query.Core.Filters.Builders
{
    public class DateFilterBuilder : IFilterBuilder
    {
        private const char defaultSeparator = ';';

        public char Separator { get; set; }

        public DateFilterBuilder()
        {
            this.Separator = defaultSeparator;
        }

        public Filter Create<T>(QueryField<T> field, string value)
        {
            var filter = new Filter { Name = field.Name, OriginalText = value };

            var parts = value.Split(new[] { this.Separator }, 2);

            var from = StringUtil.ToDateNullable(
                parts[0],
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.None);

            DateTime? to = null;
            if (parts.Count() > 1)
            {
                to = StringUtil.ToDateNullable(
                    parts[1],
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.None);
            }

            filter.Valid = from.HasValue || to.HasValue;

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
