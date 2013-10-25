using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.Core;
using Query.Core.Filters;
using Query.Core.Filters.Builders;
using Query.Sample.Model;

namespace Query.Test.Filters.Builders
{
    [TestClass]
    public class ListFilterBuilderTest
    {
        [TestMethod]
        public void ListDefault()
        {
            var builder = new ListFilterBuilder {DefaultValue = "default"};

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, builder.DefaultValue);

            Assert.AreEqual("name", filter.Name);
            Assert.IsFalse(filter.Valid);
            Assert.IsNull(filter.Value);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(builder.DefaultValue, filter.OriginalText);
        }

        [TestMethod]
        public void ListInteger()
        {
            var builder = new ListFilterBuilder { DefaultValue = "default" };

            const string value = "1";
            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.IsTrue(filter.Valid);
            Assert.AreEqual(1, filter.Value);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }
    }
}
