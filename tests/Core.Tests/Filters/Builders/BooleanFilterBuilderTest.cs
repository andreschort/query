using Xunit;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test.Filters.Builders
{
    public class BooleanFilterBuilderTest
    {
        [Fact]
        public void BooleanTrue()
        {
            var builder = new BooleanFilterBuilder();

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, true.ToString());

            Assert.Equal("name", filter.Name);
            Assert.Equal(true, filter.Value);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.Equal, filter.Operator);
            Assert.Equal(true.ToString(), filter.OriginalText);
        }

        [Fact]
        public void BooleanFalse()
        {
            var builder = new BooleanFilterBuilder();

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, false.ToString());

            Assert.Equal("name", filter.Name);
            Assert.Equal(false, filter.Value);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.Equal, filter.Operator);
            Assert.Equal(false.ToString(), filter.OriginalText);
        }

        [Fact]
        public void BooleanInvalid()
        {
            var builder = new BooleanFilterBuilder();

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "not_a_valid_bool_value");

            Assert.Equal("name", filter.Name);
            Assert.Equal(null, filter.Value);
            Assert.Equal(false, filter.Valid);
            Assert.Equal(FilterOperator.None, filter.Operator);
            Assert.Equal("not_a_valid_bool_value", filter.OriginalText);
        }
    }
}
