using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.Core;
using Query.Core.Filters;
using Query.Core.Filters.Builders;
using Query.Test.Model;

namespace Query.Test.Filters.Builders
{
    [TestClass]
    public class TextFilterBuilderTest
    {
        [TestMethod]
        public void TextEmptyIsNotValid()
        {
            var builder = new TextFilterBuilder();

            var queryField = new QueryField<Empleado> {Name = "name"};
            var filter = builder.Create(queryField, string.Empty);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(string.Empty, filter.OriginalText);
            Assert.AreEqual(string.Empty, filter.Value);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void TextDefaultStartsWith()
        {
            var builder = new TextFilterBuilder
            {
                MissingWildcardBehavior = FilterOperator.StartsWith
            };

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual("value", filter.OriginalText);
            Assert.AreEqual("value", filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.StartsWith, filter.Operator);
        }

        [TestMethod]
        public void TextStartsWith()
        {
            var builder = new TextFilterBuilder { Wildcard = "%" };

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "value%");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual("value%", filter.OriginalText);
            Assert.AreEqual("value", filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.StartsWith, filter.Operator);
        }

        [TestMethod]
        public void TextEndsWith()
        {
            var builder = new TextFilterBuilder { Wildcard = "%" };

            var queryField = new QueryField<Empleado> { Name = "name" };
            var filter = builder.Create(queryField, "%value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual("%value", filter.OriginalText);
            Assert.AreEqual("value", filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.EndsWith, filter.Operator);
        }

    }
}
