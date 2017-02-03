using Xunit;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test.Filters.Builders
{
    public class TextFilterBuilderTest
    {
        [Fact]
        public void TextEmptyIsNotValid()
        {
            var builder = new TextFilterBuilder();

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, string.Empty);

            Assert.Equal("name", filter.Name);
            Assert.Equal(string.Empty, filter.OriginalText);
            Assert.Equal(string.Empty, filter.Value);
            Assert.Equal(false, filter.Valid);
            Assert.Equal(FilterOperator.None, filter.Operator);
        }

        [Fact]
        public void TextDefaultStartsWith()
        {
            var builder = new TextFilterBuilder
            {
                MissingWildcardBehavior = FilterOperator.StartsWith
            };

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "value");

            Assert.Equal("name", filter.Name);
            Assert.Equal("value", filter.OriginalText);
            Assert.Equal("value", filter.Value);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.StartsWith, filter.Operator);
        }

        [Fact]
        public void TextStartsWith()
        {
            var builder = new TextFilterBuilder { Wildcard = "%" };

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "value%");

            Assert.Equal("name", filter.Name);
            Assert.Equal("value%", filter.OriginalText);
            Assert.Equal("value", filter.Value);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.StartsWith, filter.Operator);
        }

        [Fact]
        public void TextEndsWith()
        {
            var builder = new TextFilterBuilder { Wildcard = "%" };

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "%value");

            Assert.Equal("name", filter.Name);
            Assert.Equal("%value", filter.OriginalText);
            Assert.Equal("value", filter.Value);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.EndsWith, filter.Operator);
        }
    }
}
