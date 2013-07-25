using System;
using System.Globalization;
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

        [TestMethod]
        public void BooleanTrue()
        {
            FilterBuilder builder = new FilterBuilder();

            var filter = builder.Boolean("name", true.ToString());

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(true, filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(true.ToString(), filter.OriginalText);
        }

        [TestMethod]
        public void BooleanFalse()
        {
            FilterBuilder builder = new FilterBuilder();

            var filter = builder.Boolean("name", false.ToString());

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(false, filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(false.ToString(), filter.OriginalText);
        }

        [TestMethod]
        public void BooleanInvalid()
        {
            FilterBuilder builder = new FilterBuilder();

            var filter = builder.Boolean("name", "not_a_valid_bool_value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(null, filter.Value);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual("not_a_valid_bool_value", filter.OriginalText);
        }
        
        [TestMethod]
        public void DateEmpty()
        {
            FilterBuilder builder = new FilterBuilder();

            const char separator = ';';
            var value = string.Empty;
            var filter = builder.Date("name", value, separator);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(null, filter.Values[0]);
            Assert.AreEqual(null, filter.Values[1]);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }

        [TestMethod]
        public void DateSeparatorOnly()
        {
            FilterBuilder builder = new FilterBuilder();

            const char separator = ';';
            string value = separator.ToString();

            var filter = builder.Date("name", value, separator);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(null, filter.Values[0]);
            Assert.AreEqual(null, filter.Values[1]);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }
        
        [TestMethod]
        public void DateFrom()
        {
            FilterBuilder builder = new FilterBuilder();

            DateTime from = DateTime.Today;

            const char separator = ';';
            var fromStr = @from.ToString(CultureInfo.InvariantCulture);
            var value = fromStr + separator;
            var filter = builder.Date("name", value, separator);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(from, filter.Values[0]);
            Assert.AreEqual(null, filter.Values[1]);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }
        
        [TestMethod]
        public void DateTo()
        {
            FilterBuilder builder = new FilterBuilder();

            DateTime to = DateTime.Today;

            const char separator = ';';
            var toStr = to.ToString(CultureInfo.InvariantCulture);
            var value = ";" + toStr;
            var filter = builder.Date("name", value, separator);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(null, filter.Values[0]);
            Assert.AreEqual(to, filter.Values[1]);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }
        
        [TestMethod]
        public void DateBetween()
        {
            FilterBuilder builder = new FilterBuilder();

            DateTime from = DateTime.Today;
            DateTime to = DateTime.Today;

            var fromStr = @from.ToString(CultureInfo.InvariantCulture);
            var toStr = to.ToString(CultureInfo.InvariantCulture);

            var value = fromStr + ";" + toStr;
            var filter = builder.Date("name", value, ';');

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(from, filter.Values[0]);
            Assert.AreEqual(to, filter.Values[1]);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Between, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }
        
        [TestMethod]
        public void DateInvalid()
        {
            FilterBuilder builder = new FilterBuilder();

            const char separator = ';';
            const string value = "wefwef";
            var filter = builder.Date("name", value, separator);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(null, filter.Values[0]);
            Assert.AreEqual(null, filter.Values[1]);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }
    }
}
