using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query
{
    public class FilterBuilder
    {
        private const string DefaultWildcard = "%";

        private const FilterOperator DefaultMissingWildcardBehavior = FilterOperator.StartsWith;

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
    }
}
