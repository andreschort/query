using Query.Common.Util;

namespace Query.Core.Filters.Builders
{
    public class BooleanFilterBuilder : IFilterBuilder
    {
        public Filter Create<T>(QueryField<T> field, string value)
        {
            var filter = new Filter
            {
                Name = field.Name,
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
    }
}
