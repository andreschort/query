using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Query.Test
{
    [TestClass]
    public class FilterBuilderTest
    {
        [TestMethod]
        public void TextEmptyIsNotValid()
        {
            FilterBuilder builder = new FilterBuilder();

            var filter = builder.Text("name", string.Empty);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(string.Empty, filter.OriginalText);
            Assert.AreEqual(string.Empty, filter.Value);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void TextDefaultStartsWith()
        {
            FilterBuilder builder = new FilterBuilder
                {
                    MissingWildcardBehavior = FilterOperator.StartsWith
                };

            var filter = builder.Text("name", "value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual("value", filter.OriginalText);
            Assert.AreEqual("value", filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.StartsWith, filter.Operator);
        }

        [TestMethod]
        public void TextStartsWith()
        {
            FilterBuilder builder = new FilterBuilder {Wildcard = "%"};

            var filter = builder.Text("name", "value%");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual("value%", filter.OriginalText);
            Assert.AreEqual("value", filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.StartsWith, filter.Operator);
        }

        [TestMethod]
        public void TextEndsWith()
        {
            FilterBuilder builder = new FilterBuilder { Wildcard = "%" };

            var filter = builder.Text("name", "%value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual("%value", filter.OriginalText);
            Assert.AreEqual("value", filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.EndsWith, filter.Operator);
        }
    }
}
