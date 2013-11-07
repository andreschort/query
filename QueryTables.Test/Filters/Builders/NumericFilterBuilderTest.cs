using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test.Filters.Builders
{
    [TestClass]
    public class NumericFilterBuilderTest
    {
        #region Integer

        [TestMethod]
        public void IntegerInvalid()
        {
            var builder = new NumericFilterBuilder(typeof(int));

            const string value = "aaf";
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(null, filter.Value);
        }

        [TestMethod]
        public void IntegerNoOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));

            const string value = "2";
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(int.Parse(value), filter.Value);
        }

        [TestMethod]
        public void IntegerEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string equalOperator = "=";
            const int value = 2;

            var originalText = equalOperator + value;
            builder.Symbols[FilterOperator.Equal] = equalOperator;
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerNotEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string notEqualOperator = "!=";
            const int value = 2;
            var originalText = notEqualOperator + value;

            builder.Symbols[FilterOperator.NotEqual] = notEqualOperator;
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.NotEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerGreaterThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string greaterThanEqualOperator = ">=";
            const int value = 2;
            var originalText = greaterThanEqualOperator + value;
            builder.Symbols[FilterOperator.GreaterThanEqual] = greaterThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerLessThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string lessThanEqualOperator = "<=";
            const int value = 2;
            var originalText = lessThanEqualOperator + value;
            builder.Symbols[FilterOperator.LessThanEqual] = lessThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerGreaterThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string greaterThanOperator = ">";
            const int value = 2;
            var originalText = greaterThanOperator + value;
            builder.Symbols[FilterOperator.GreaterThan] = greaterThanOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerLessThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string lessThanOperator = "<";
            const int value = 2;
            var originalText = lessThanOperator + value;
            builder.Symbols[FilterOperator.LessThan] = lessThanOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void IntegerBetweenOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string betweenOperator = "|";
            const int leftValue = 1;
            const int rightValue = 2;
            var originalText = leftValue + betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

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
            var builder = new NumericFilterBuilder(typeof(int));
            const string betweenOperator = "|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorLeftLetters()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string betweenOperator = "d|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorTwoTimes()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string betweenOperator = "3||5";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorOnlyLeft()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string betweenOperator = "|";
            const int leftValue = 1;
            var originalText = leftValue + betweenOperator;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorOnlyRight()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string betweenOperator = "|";
            const int rightValue = 1;
            var originalText = betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

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
            var builder = new NumericFilterBuilder(typeof(int));
            const string betweenOperator = "|";

            const string originalText = "2|3|";
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, originalText);

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
            var builder = new NumericFilterBuilder(typeof(decimal));

            const string value = "aaf";
            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(null, filter.Value);
        }

        [TestMethod]
        public void DecimalNoOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));

            var value = 2.1m.ToString();
            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(value, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(decimal.Parse(value), filter.Value);
        }

        [TestMethod]
        public void DecimalEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string equalOperator = "=";
            const decimal value = 2.1m;

            var originalText = equalOperator + value;
            builder.Symbols[FilterOperator.Equal] = equalOperator;
            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalNotEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string notEqualOperator = "!=";
            const decimal value = 2.1m;
            var originalText = notEqualOperator + value;

            builder.Symbols[FilterOperator.NotEqual] = notEqualOperator;
            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.NotEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalGreaterThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string greaterThanEqualOperator = ">=";
            const decimal value = 2.1m;
            var originalText = greaterThanEqualOperator + value;
            builder.Symbols[FilterOperator.GreaterThanEqual] = greaterThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalLessThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string lessThanEqualOperator = "<=";
            const decimal value = 2.1m;
            var originalText = lessThanEqualOperator + value;
            builder.Symbols[FilterOperator.LessThanEqual] = lessThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalGreaterThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string greaterThanOperator = ">";
            const decimal value = 2.1m;
            var originalText = greaterThanOperator + value;
            builder.Symbols[FilterOperator.GreaterThan] = greaterThanOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalLessThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string lessThanOperator = "<";
            const decimal value = 2.1m;
            var originalText = lessThanOperator + value;
            builder.Symbols[FilterOperator.LessThan] = lessThanOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThan, filter.Operator);
            Assert.AreEqual(value, filter.Value);
        }

        [TestMethod]
        public void DecimalBetweenOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string betweenOperator = "|";
            const decimal leftValue = 1.1m;
            const decimal rightValue = 2.1m;
            var originalText = leftValue + betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

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
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string betweenOperator = "|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorLeftLetters()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string betweenOperator = "d|";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorTwoTimes()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string betweenOperator = "3.1||5.7";
            const string originalText = betweenOperator;

            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorOnlyLeft()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string betweenOperator = "|";
            const int leftValue = 1;
            var originalText = leftValue + betweenOperator;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorOnlyRight()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string betweenOperator = "|";
            const decimal rightValue = 1.5m;
            var originalText = betweenOperator + rightValue;
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

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
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string betweenOperator = "|";

            const string originalText = "2.1|3.8|";
            builder.Symbols[FilterOperator.Between] = betweenOperator;

            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }


        #endregion Decimal
    }
}
