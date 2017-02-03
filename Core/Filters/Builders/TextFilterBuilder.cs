namespace QueryTables.Core.Filters.Builders
{
    public class TextFilterBuilder : IFilterBuilder
    {
        private const string DefaultWildcard = "%";

        private const FilterOperator DefaultMissingWildcardBehavior = FilterOperator.StartsWith;

        public TextFilterBuilder()
        {
            this.Wildcard = DefaultWildcard;
            this.MissingWildcardBehavior = DefaultMissingWildcardBehavior;
        }

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

        public Filter Create<T>(QueryField<T> field, string value)
        {
            var filter = new Filter { Name = field.Name, OriginalText = value };

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
