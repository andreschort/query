using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test.Filters.Builders
{
    [TestClass]
    public class BooleanFilterBuilderTest
    {
        [TestMethod]
        public void BooleanTrue()
        {
            var builder = new BooleanFilterBuilder();

            var queryField = new QueryField<Empleado> {Name = "name"};
            var filter = builder.Create(queryField, true.ToString());

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(true, filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(true.ToString(), filter.OriginalText);
        }

        [TestMethod]
        public void BooleanFalse()
        {
            var builder = new BooleanFilterBuilder();

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, false.ToString());

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(false, filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(false.ToString(), filter.OriginalText);
        }

        [TestMethod]
        public void BooleanInvalid()
        {
            var builder = new BooleanFilterBuilder();

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "not_a_valid_bool_value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(null, filter.Value);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual("not_a_valid_bool_value", filter.OriginalText);
        }

    }
}
