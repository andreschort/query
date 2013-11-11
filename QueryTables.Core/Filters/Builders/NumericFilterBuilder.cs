using System;
using System.Collections.Generic;
using System.Linq;
using QueryTables.Common.Util;

namespace QueryTables.Core.Filters.Builders
{
    public class NumericFilterBuilder : IFilterBuilder
    {
        /// <summary>
        /// The default symbols for each operator. The order matters because of the lame implementation of GetOperator
        /// </summary>
        private static readonly Dictionary<FilterOperator, string> DefaultSymbols = new Dictionary<FilterOperator, string>
            {
                { FilterOperator.NotEqual, "!=" },
                { FilterOperator.GreaterThanEqual, ">=" },
                { FilterOperator.LessThanEqual, "<=" },
                { FilterOperator.GreaterThan, ">" },
                { FilterOperator.LessThan, "<" },
                { FilterOperator.Between, "|" },
                { FilterOperator.Equal, "=" },
            };

        public NumericFilterBuilder(Type numericType)
        {
            this.Symbols = DefaultSymbols;
            this.NumericType = numericType;
        }

        /// <summary>
        /// The symbol for each FilterOperator.
        /// </summary>
        public Dictionary<FilterOperator, string> Symbols { get; set; }

        private Type NumericType { get; set; }

        public Filter Create<T>(QueryField<T> field, string value)
        {
            var filter = new Filter { Name = field.Name, OriginalText = value, Operator = this.GetOperator(value) };

            if (filter.Operator.Equals(FilterOperator.None))
            {
                filter.Operator = FilterOperator.Equal;
            }

            var parts = value.Split(new[] { this.Symbols[filter.Operator] }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Any())
            {
                filter.Values.Add(this.CastValue(parts[0]));
            }

            if (parts.Count() > 1)
            {
                filter.Values.Add(this.CastValue(parts[1]));
            }

            filter.Valid = filter.Value != null;

            if (filter.Operator.Equals(FilterOperator.Between))
            {
                filter.Valid = filter.Values.Count == 2 && filter.Values[0] != null && filter.Values[1] != null;
            }

            if (!filter.Valid)
            {
                filter.Operator = FilterOperator.None;
            }

            return filter;
        }

        private object CastValue(string value)
        {
            if (this.NumericType == typeof(int) || this.NumericType == typeof(int?))
            {
                return StringUtil.ToIntNullable(value);
            }

            if (this.NumericType == typeof(decimal) || this.NumericType == typeof(double) || this.NumericType == typeof(decimal?) || this.NumericType == typeof(double?))
            {
                return StringUtil.ToDecimalNullable(value);
            }

            return null;
        }

        private FilterOperator GetOperator(string value)
        {
            return this.Symbols.Keys.FirstOrDefault(x => value.Contains(this.Symbols[x]));
        }
    }
}
