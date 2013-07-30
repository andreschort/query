using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Common.Util;

namespace Query
{
    public class FilterBuilder
    {
        private const string DefaultWildcard = "%";

        private const FilterOperator DefaultMissingWildcardBehavior = FilterOperator.StartsWith;

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

        public FilterBuilder()
        {
            this.Symbols = DefaultSymbols;
            this.Wildcard = DefaultWildcard;
            this.MissingWildcardBehavior = DefaultMissingWildcardBehavior;
        }

        /// <summary>
        /// The simbol for each FilterOperator.
        /// </summary>
        public Dictionary<FilterOperator, string> Symbols { get; set; }

        /// <summary>
        /// Only for Text filters
        /// The wildcard used for StartsWith, Contains and EndsWith operators.
        /// </summary>
        public string Wildcard { get; set; }

        /// <summary>
        /// Only for Text filters
        /// Determines the behavior when the filter text does not contain any wildcard.
        /// Options: StartsWith, Contains or EndsWith
        /// </summary>
        public FilterOperator MissingWildcardBehavior { get; set; }

        public Filter Text(string name, string value)
        {
            Filter filter = new Filter { Name = name, OriginalText = value };

            var text = value ?? string.Empty;

            filter.Valid = !string.IsNullOrEmpty(text);
            filter.Values.Add(text.Replace(this.Wildcard, string.Empty));

            if (!filter.Valid)
            {
                return filter;
            }

            if (!text.Contains(this.Wildcard))
            {
                switch (this.MissingWildcardBehavior)
                {
                    case FilterOperator.StartsWith:
                        text = text + this.Wildcard;
                        break;
                    case FilterOperator.Contains:
                        text = this.Wildcard + text + this.Wildcard;
                        break;
                    case FilterOperator.EndsWith:
                        text = this.Wildcard + text;
                        break;
                }
            }
            
            var startsWith = text.EndsWith(this.Wildcard);
            var endsWith = text.StartsWith(this.Wildcard);

            if (startsWith && endsWith)
            {
                filter.Operator = FilterOperator.Contains;
            }
            else if (startsWith)
            {
                filter.Operator = FilterOperator.StartsWith;
            }
            else if (endsWith)
            {
                filter.Operator = FilterOperator.EndsWith;
            }
            else
            {
                filter.Operator = FilterOperator.Equal;
            }

            return filter;
        }

        public Filter Boolean(string name, string value)
        {
            var filter = new Filter
                {
                    Name = name,
                    OriginalText = value
                };

            var val = StringUtil.ToBoolNullable(value);

            if (val.HasValue)
            {
                filter.Valid = true;
                filter.Value = val.Value;
                filter.Operator = FilterOperator.Equal;
            }
            
            return filter;
        }

        public Filter Date(string name, string datesStr, char separator)
        {
            var filter = new Filter {Name = name, OriginalText = datesStr};

            var parts = datesStr.Split(new[] {separator}, 2);

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

            filter.Values.Add(from);
            filter.Values.Add(to);

            if (from.HasValue && to.HasValue)
            {
                filter.Operator = FilterOperator.Between;
            }
            else if (from.HasValue)
            {
                filter.Operator = FilterOperator.GreaterThanEqual;
            }
            else if (to.HasValue)
            {
                filter.Operator = FilterOperator.LessThanEqual;
            }

            return filter;
        }

        public Filter List(
            string name,
            string value,
            string defaultValue,
            Type enumType = null)
        {
            var filter = new Filter {Name = name, OriginalText = value};

            var text = value ?? defaultValue;

            filter.Valid = !text.Equals(defaultValue);

            if (filter.Valid)
            {
                filter.Operator = FilterOperator.Equal;
            }

            return filter;
        }

        public Filter Integer(string name, string value)
        {
            var filter = new Filter {Name = name, OriginalText = value, Operator = GetOperator(value)};

            if (filter.Operator.Equals(FilterOperator.None))
            {
                filter.Operator = FilterOperator.Equal;
            }

            var parts = value.Split(new[] { this.Symbols[filter.Operator] }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Any())
            {
                filter.Values.Add(StringUtil.ToIntNullable(parts[0]));
            }

            if (parts.Count() > 1)
            {
                filter.Values.Add(StringUtil.ToIntNullable(parts[1]));
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

        public Filter Decimal(string name, string value)
        {
            Filter filter = new Filter {Name = name, OriginalText = value, Operator = GetOperator(value)};

            if (filter.Operator.Equals(FilterOperator.None))
            {
                filter.Operator = FilterOperator.Equal;
            }

            var parts = value.Split(new[] { this.Symbols[filter.Operator] }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Any())
            {
                filter.Values.Add(StringUtil.ToDecimalNullable(parts[0]));
            }

            if (parts.Count() > 1)
            {
                filter.Values.Add(StringUtil.ToDecimalNullable(parts[1]));
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

        private FilterOperator GetOperator(string value)
        {
            return this.Symbols.Keys.FirstOrDefault(x => value.Contains(Symbols[x]));
        }
    }
}
