using Xunit;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test.Filters.Builders
{
    public class ListFilterBuilderTest
    {
        [Fact]
        public void ListDefault()
        {
            var builder = new ListFilterBuilder { DefaultValue = "default" };

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, builder.DefaultValue);

            Assert.Equal("name", filter.Name);
            Assert.False(filter.Valid);
            Assert.Null(filter.Value);
            Assert.Equal(FilterOperator.None, filter.Operator);
            Assert.Equal(builder.DefaultValue, filter.OriginalText);
        }

        [Fact]
        public void ListInteger()
        {
            var builder = new ListFilterBuilder { DefaultValue = "default" };

            const string Value = "1";
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, Value);

            Assert.Equal("name", filter.Name);
            Assert.True(filter.Valid);
            Assert.Equal(1, filter.Value);
            Assert.Equal(FilterOperator.Equal, filter.Operator);
            Assert.Equal(Value, filter.OriginalText);
        }
    }
}
