using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.SampleModel;

namespace Query.Test
{
    [TestClass]
    public class FilterBuilderTest
    {
        #region Text

        [TestMethod]
        public void TextEmptyIsNotValid()
        {
            var builder = new FilterBuilder();

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
            var builder = new FilterBuilder
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
            var builder = new FilterBuilder { Wildcard = "%" };

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
            var builder = new FilterBuilder { Wildcard = "%" };

            var filter = builder.Text("name", "%value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual("%value", filter.OriginalText);
            Assert.AreEqual("value", filter.Value);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.EndsWith, filter.Operator);
        }

        #endregion Text

        #region Boolean

        [TestMethod]
        public void BooleanTrue()
        {
            var builder = new FilterBuilder();

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
            var builder = new FilterBuilder();

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
            var builder = new FilterBuilder();

            var filter = builder.Boolean("name", "not_a_valid_bool_value");

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(null, filter.Value);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual("not_a_valid_bool_value", filter.OriginalText);
        }

        #endregion Boolean

        #region Date

        [TestMethod]
        public void DateEmpty()
        {
            var builder = new FilterBuilder();

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
            var builder = new FilterBuilder();

            const char separator = ';';
            var value = separator.ToString();

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
            var builder = new FilterBuilder();

            var from = DateTime.Today;

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
            var builder = new FilterBuilder();

            var to = DateTime.Today;

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
            var builder = new FilterBuilder();

            var from = DateTime.Today;
            var to = DateTime.Today;

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
            var builder = new FilterBuilder();

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

        #endregion Date

        #region List

        [TestMethod]
        public void ListDefault()
        {
            var builder = new FilterBuilder();

            const string defaultValue = "default";
            const string value = defaultValue;
            var filter = builder.List("name", value, defaultValue);

            Assert.AreEqual("name", filter.Name);
            Assert.IsFalse(filter.Valid);
            Assert.IsNull(filter.Value);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }

        [TestMethod]
        public void ListInteger()
        {
            var builder = new FilterBuilder();

            const string value = "1";
            var filter = builder.List("name", value, "default");

            Assert.AreEqual("name", filter.Name);
            Assert.IsTrue(filter.Valid);
            Assert.AreEqual(1, filter.Value);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }

        #endregion List

        #region Integer

        [TestMethod]
        public void IntegerInvalid()
        {
            var builder = new FilterBuilder();

            const string value = "aaf";
            var filter = builder.Integer("name", value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(null, filter.Value);
        }

        [TestMethod]
        public void IntegerNoOperator()
        {
            var builder = new FilterBuilder();

            const string value = "2";
            var filter = builder.Integer("name", value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(int.Parse(value), filter.Value);
        }

        [TestMethod]
        public void IntegerEqualOperator()
        {
            var builder = new FilterBuilder();
            const string equalOperator = "=";
            const int value = 2;
            
            var originalText = equalOperator + value;
            builder.Symbols[FilterOperator.Equal] = equalOperator;
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerNotEqualOperator()
        {
            var builder = new FilterBuilder();
            const string notEqualOperator = "!=";
            const int value = 2;
            var originalText = notEqualOperator + value;

            builder.Symbols[FilterOperator.NotEqual] = notEqualOperator;
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.NotEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerGreaterThanEqualOperator()
        {
            var builder = new FilterBuilder();
            const string greaterThanEqualOperator = ">=";
            const int value = 2;
            var originalText = greaterThanEqualOperator + value;
            builder.Symbols[FilterOperator.GreaterThanEqual] = greaterThanEqualOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerLessThanEqualOperator()
        {
            var builder = new FilterBuilder();
            const string lessThanEqualOperator = "<=";
            const int value = 2;
            var originalText = lessThanEqualOperator + value;
            builder.Symbols[FilterOperator.LessThanEqual] = lessThanEqualOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerGreaterThanOperator()
        {
            var builder = new FilterBuilder();
            const string greaterThanOperator = ">";
            const int value = 2;
            var originalText = greaterThanOperator + value;
            builder.Symbols[FilterOperator.GreaterThan] = greaterThanOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerLessThanOperator()
        {
            var builder = new FilterBuilder();
            const string lessThanOperator = "<";
            const int value = 2;
            var originalText = lessThanOperator + value;
            builder.Symbols[FilterOperator.LessThan] = lessThanOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerBetweenOperator()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const int leftValue = 1;
            const int rightValue = 2;
            var originalText = leftValue + betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Between, filter.Operator);
            Assert.AreEqual(leftValue, filter.Values[0]);
            Assert.AreEqual(rightValue, filter.Values[1]);
        }

        [TestMethod]
        public void IntegerBetweenOperatorNoNumbers()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorLeftLetters()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "d|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorTwoTimes()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "3||5";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;
            
            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorOnlyLeft()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const int leftValue = 1;
            var originalText = leftValue + betweenOperator;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorOnlyRight()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const int rightValue = 1;
            var originalText = betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        [Ignore]
        public void IntegerBetweenOperatorInvalid()
        {
            // This test shows that we need to improve FilterBuilder.GetOperator
            var builder = new FilterBuilder();
            const string betweenOperator = "|";

            const string originalText = "2|3|";
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Integer("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        #endregion Integer

        #region Decimal

        [TestMethod]
        public void DecimalInvalid()
        {
            var builder = new FilterBuilder();

            const string value = "aaf";
            var filter = builder.Decimal("name", value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(null, filter.Value);
        }

        [TestMethod]
        public void DecimalNoOperator()
        {
            var builder = new FilterBuilder();

            const string value = "2.1";
            var filter = builder.Decimal("name", value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(decimal.Parse(value), filter.Value);
        }

        [TestMethod]
        public void DecimalEqualOperator()
        {
            var builder = new FilterBuilder();
            const string equalOperator = "=";
            const decimal value = 2.1m;

            var originalText = equalOperator + value;
            builder.Symbols[FilterOperator.Equal] = equalOperator;
            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalNotEqualOperator()
        {
            var builder = new FilterBuilder();
            const string notEqualOperator = "!=";
            const decimal value = 2.1m;
            var originalText = notEqualOperator + value;

            builder.Symbols[FilterOperator.NotEqual] = notEqualOperator;
            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.NotEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalGreaterThanEqualOperator()
        {
            var builder = new FilterBuilder();
            const string greaterThanEqualOperator = ">=";
            const decimal value = 2.1m;
            var originalText = greaterThanEqualOperator + value;
            builder.Symbols[FilterOperator.GreaterThanEqual] = greaterThanEqualOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalLessThanEqualOperator()
        {
            var builder = new FilterBuilder();
            const string lessThanEqualOperator = "<=";
            const decimal value = 2.1m;
            var originalText = lessThanEqualOperator + value;
            builder.Symbols[FilterOperator.LessThanEqual] = lessThanEqualOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalGreaterThanOperator()
        {
            var builder = new FilterBuilder();
            const string greaterThanOperator = ">";
            const decimal value = 2.1m;
            var originalText = greaterThanOperator + value;
            builder.Symbols[FilterOperator.GreaterThan] = greaterThanOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalLessThanOperator()
        {
            var builder = new FilterBuilder();
            const string lessThanOperator = "<";
            const decimal value = 2.1m;
            var originalText = lessThanOperator + value;
            builder.Symbols[FilterOperator.LessThan] = lessThanOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalBetweenOperator()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const decimal leftValue = 1.1m;
            const decimal rightValue = 2.1m;
            var originalText = leftValue + betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Between, filter.Operator);
            Assert.AreEqual(leftValue, filter.Values[0]);
            Assert.AreEqual(rightValue, filter.Values[1]);
        }

        [TestMethod]
        public void DecimalBetweenOperatorNoNumbers()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorLeftLetters()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "d|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorTwoTimes()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "3.1||5.7";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorOnlyLeft()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const int leftValue = 1;
            var originalText = leftValue + betweenOperator;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorOnlyRight()
        {
            var builder = new FilterBuilder();
            const string betweenOperator = "|";
            const decimal rightValue = 1.5m;
            var originalText = betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        [Ignore]
        public void DecimalBetweenOperatorInvalid()
        {
            // This test shows that we need to improve FilterBuilder.GetOperator
            var builder = new FilterBuilder();
            const string betweenOperator = "|";

            const string originalText = "2.1|3.8|";
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Decimal("name", originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        #endregion
    }
}
