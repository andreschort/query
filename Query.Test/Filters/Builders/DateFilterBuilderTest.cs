﻿using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.Core;
using Query.Core.Filters;
using Query.Core.Filters.Builders;
using Query.Sample.Model;

namespace Query.Test.Filters.Builders
{
    [TestClass]
    public class DateFilterBuilderTest
    {
        [TestMethod]
        public void DateEmpty()
        {
            var builder = new DateFilterBuilder {Separator = ';'};

            var value = string.Empty;
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(0, filter.Values.Count);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }

        [TestMethod]
        public void DateSeparatorOnly()
        {
            var builder = new DateFilterBuilder {Separator = ';'};

            const char separator = ';';
            var value = separator.ToString();

            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(0, filter.Values.Count);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }

        [TestMethod]
        public void DateFrom()
        {
            var builder = new DateFilterBuilder {Separator = ';'};

            var from = DateTime.Today;

            var fromStr = @from.ToString(CultureInfo.InvariantCulture);
            var value = fromStr + builder.Separator;
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(from, filter.Values[0]);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.GreaterThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }

        [TestMethod]
        public void DateTo()
        {
            var builder = new DateFilterBuilder {Separator = ';'};

            var to = DateTime.Today;

            var toStr = to.ToString(CultureInfo.InvariantCulture);
            var value = ";" + toStr;
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(to, filter.Values[0]);
            Assert.AreEqual(true, filter.Valid);
            Assert.AreEqual(FilterOperator.LessThanEqual, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }

        [TestMethod]
        public void DateBetween()
        {
            var builder = new DateFilterBuilder {Separator = ';'};

            var from = DateTime.Today;
            var to = DateTime.Today;

            var fromStr = @from.ToString(CultureInfo.InvariantCulture);
            var toStr = to.ToString(CultureInfo.InvariantCulture);

            var value = fromStr +builder.Separator + toStr;
            var filter = builder.Create(new QueryField<Empleado>{Name = "name"}, value);

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
            var builder = new DateFilterBuilder {Separator = ';'};

            const string value = "wefwef";
            var filter = builder.Create(new QueryField<Empleado> {Name = "name"}, value);

            Assert.AreEqual("name", filter.Name);
            Assert.AreEqual(0, filter.Values.Count);
            Assert.AreEqual(false, filter.Valid);
            Assert.AreEqual(FilterOperator.None, filter.Operator);
            Assert.AreEqual(value, filter.OriginalText);
        }
    }
}