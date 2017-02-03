using System;
using System.Globalization;
using Xunit;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test.Filters.Builders
{
    public class DateFilterBuilderTest
    {
        [Fact]
        public void DateEmpty()
        {
            var builder = new DateFilterBuilder { Separator = ';' };

            var value = string.Empty;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, value);

            Assert.Equal("name", filter.Name);
            Assert.Equal(0, filter.Values.Count);
            Assert.Equal(false, filter.Valid);
            Assert.Equal(FilterOperator.None, filter.Operator);
            Assert.Equal(value, filter.OriginalText);
        }

        [Fact]
        public void DateSeparatorOnly()
        {
            var builder = new DateFilterBuilder { Separator = ';' };

            const char Separator = ';';
            var value = Separator.ToString();

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, value);

            Assert.Equal("name", filter.Name);
            Assert.Equal(0, filter.Values.Count);
            Assert.Equal(false, filter.Valid);
            Assert.Equal(FilterOperator.None, filter.Operator);
            Assert.Equal(value, filter.OriginalText);
        }

        [Fact]
        public void DateFrom()
        {
            var builder = new DateFilterBuilder { Separator = ';' };

            var from = DateTime.Today;

            var fromStr = @from.ToString();
            var value = fromStr + builder.Separator;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, value);

            Assert.Equal("name", filter.Name);
            Assert.Equal(from, filter.Values[0]);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.Equal(value, filter.OriginalText);
        }

        [Fact]
        public void DateTo()
        {
            var builder = new DateFilterBuilder { Separator = ';' };

            var to = DateTime.Today;

            var toStr = to.ToString();
            var value = ";" + toStr;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, value);

            Assert.Equal("name", filter.Name);
            Assert.Equal(to, filter.Values[0]);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.LessThanEqual, filter.Operator);
            Assert.Equal(value, filter.OriginalText);
        }

        [Fact]
        public void DateBetween()
        {
            var builder = new DateFilterBuilder { Separator = ';' };

            var from = DateTime.Today;
            var to = DateTime.Today;

            var fromStr = @from.ToString();
            var toStr = to.ToString();

            var value = fromStr + builder.Separator + toStr;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, value);

            Assert.Equal("name", filter.Name);
            Assert.Equal(from, filter.Values[0]);
            Assert.Equal(to, filter.Values[1]);
            Assert.Equal(true, filter.Valid);
            Assert.Equal(FilterOperator.Between, filter.Operator);
            Assert.Equal(value, filter.OriginalText);
        }

        [Fact]
        public void DateInvalid()
        {
            var builder = new DateFilterBuilder { Separator = ';' };

            const string Value = "wefwef";
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, Value);

            Assert.Equal("name", filter.Name);
            Assert.Equal(0, filter.Values.Count);
            Assert.Equal(false, filter.Valid);
            Assert.Equal(FilterOperator.None, filter.Operator);
            Assert.Equal(Value, filter.OriginalText);
        }
    }
}
