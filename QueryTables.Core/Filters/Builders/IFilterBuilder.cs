namespace Query.Core.Filters.Builders
{
    public interface IFilterBuilder
    {
        Filter Create<T>(QueryField<T> field, string value);
    }
}
