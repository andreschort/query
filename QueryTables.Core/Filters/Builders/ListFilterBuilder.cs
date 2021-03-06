﻿using QueryTables.Common.Util;

namespace QueryTables.Core.Filters.Builders
{
    public class ListFilterBuilder : IFilterBuilder
    {
        public ListFilterBuilder()
        {
            this.DefaultValue = string.Empty;
        }

        public string DefaultValue { get; set; }

        public Filter Create<T>(QueryField<T> field, string value)
        {
            var filter = new Filter { Name = field.Name, OriginalText = value };

            var text = value ?? this.DefaultValue;

            filter.Valid = !text.Equals(this.DefaultValue);

            if (filter.Valid)
            {
                filter.Operator = FilterOperator.Equal;
                filter.Values.Add(StringUtil.ToInt(text));
            }

            return filter;
        }
    }
}
