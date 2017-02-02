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

            const string Value = "aaf";
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, Value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(Value, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(null, filter.Value);
        }

        [TestMethod]
        public void IntegerNoOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));

            const string Value = "2";
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, Value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(Value, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(int.Parse(Value), filter.Value);
        }

        [TestMethod]
        public void IntegerEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string EqualOperator = "=";
            const int Value = 2;

            var originalText = EqualOperator + Value;
            builder.Symbols[FilterOperator.Equal] = EqualOperator;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void IntegerNotEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string NotEqualOperator = "!=";
            const int Value = 2;
            var originalText = NotEqualOperator + Value;

            builder.Symbols[FilterOperator.NotEqual] = NotEqualOperator;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.NotEqual, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void IntegerGreaterThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string GreaterThanEqualOperator = ">=";
            const int Value = 2;
            var originalText = GreaterThanEqualOperator + Value;
            builder.Symbols[FilterOperator.GreaterThanEqual] = GreaterThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void IntegerLessThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string LessThanEqualOperator = "<=";
            const int Value = 2;
            var originalText = LessThanEqualOperator + Value;
            builder.Symbols[FilterOperator.LessThanEqual] = LessThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void IntegerGreaterThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string GreaterThanOperator = ">";
            const int Value = 2;
            var originalText = GreaterThanOperator + Value;
            builder.Symbols[FilterOperator.GreaterThan] = GreaterThanOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThan, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void IntegerLessThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string LessThanOperator = "<";
            const int Value = 2;
            var originalText = LessThanOperator + Value;
            builder.Symbols[FilterOperator.LessThan] = LessThanOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThan, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void IntegerBetweenOperator()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string BetweenOperator = "|";
            const int LeftValue = 1;
            const int RightValue = 2;
            var originalText = LeftValue + BetweenOperator + RightValue;
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Between, filter.Operator);
            Assert.AreEqual(LeftValue, filter.Values[0]);
            Assert.AreEqual(RightValue, filter.Values[1]);
        }

        [TestMethod]
        public void IntegerBetweenOperatorNoNumbers()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string BetweenOperator = "|";
            const string OriginalText = BetweenOperator;

            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorLeftLetters()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string BetweenOperator = "d|";
            const string OriginalText = BetweenOperator;

            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorTwoTimes()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string BetweenOperator = "3||5";
            const string OriginalText = BetweenOperator;

            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorOnlyLeft()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string BetweenOperator = "|";
            const int LeftValue = 1;
            var originalText = LeftValue + BetweenOperator;
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void IntegerBetweenOperatorOnlyRight()
        {
            var builder = new NumericFilterBuilder(typeof(int));
            const string BetweenOperator = "|";
            const int RightValue = 1;
            var originalText = BetweenOperator + RightValue;
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

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
            const string BetweenOperator = "|";

            const string OriginalText = "2|3|";
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        #endregion Integer

        #region Decimal

        [TestMethod]
        public void DecimalInvalid()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));

            const string Value = "aaf";
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, Value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(Value, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(null, filter.Value);
        }

        [TestMethod]
        public void DecimalNoOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));

            var value = 2.1m.ToString();
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, value);

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
            const string EqualOperator = "=";
            const decimal Value = 2.1m;

            var originalText = EqualOperator + Value;
            builder.Symbols[FilterOperator.Equal] = EqualOperator;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void DecimalNotEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string NotEqualOperator = "!=";
            const decimal Value = 2.1m;
            var originalText = NotEqualOperator + Value;

            builder.Symbols[FilterOperator.NotEqual] = NotEqualOperator;
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.NotEqual, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void DecimalGreaterThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string GreaterThanEqualOperator = ">=";
            const decimal Value = 2.1m;
            var originalText = GreaterThanEqualOperator + Value;
            builder.Symbols[FilterOperator.GreaterThanEqual] = GreaterThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void DecimalLessThanEqualOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string LessThanEqualOperator = "<=";
            const decimal Value = 2.1m;
            var originalText = LessThanEqualOperator + Value;
            builder.Symbols[FilterOperator.LessThanEqual] = LessThanEqualOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void DecimalGreaterThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string GreaterThanOperator = ">";
            const decimal Value = 2.1m;
            var originalText = GreaterThanOperator + Value;
            builder.Symbols[FilterOperator.GreaterThan] = GreaterThanOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThan, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void DecimalLessThanOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string LessThanOperator = "<";
            const decimal Value = 2.1m;
            var originalText = LessThanOperator + Value;
            builder.Symbols[FilterOperator.LessThan] = LessThanOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThan, filter.Operator);
            Assert.AreEqual(Value, filter.Value);
        }

        [TestMethod]
        public void DecimalBetweenOperator()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string BetweenOperator = "|";
            const decimal LeftValue = 1.1m;
            const decimal RightValue = 2.1m;
            var originalText = LeftValue + BetweenOperator + RightValue;
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.Between, filter.Operator);
            Assert.AreEqual(LeftValue, filter.Values[0]);
            Assert.AreEqual(RightValue, filter.Values[1]);
        }

        [TestMethod]
        public void DecimalBetweenOperatorNoNumbers()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string BetweenOperator = "|";
            const string OriginalText = BetweenOperator;

            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorLeftLetters()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string BetweenOperator = "d|";
            const string OriginalText = BetweenOperator;

            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorTwoTimes()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string BetweenOperator = "3.1||5.7";
            const string OriginalText = BetweenOperator;

            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorOnlyLeft()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string BetweenOperator = "|";
            const int LeftValue = 1;
            var originalText = LeftValue + BetweenOperator;
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(originalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        [TestMethod]
        public void DecimalBetweenOperatorOnlyRight()
        {
            var builder = new NumericFilterBuilder(typeof(decimal));
            const string BetweenOperator = "|";
            const decimal RightValue = 1.5m;
            var originalText = BetweenOperator + RightValue;
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, originalText);

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
            const string BetweenOperator = "|";

            const string OriginalText = "2.1|3.8|";
            builder.Symbols[FilterOperator.Between] = BetweenOperator;

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, OriginalText);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(OriginalText, filter.OriginalText);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
        }

        #endregion Decimal
    }
}
