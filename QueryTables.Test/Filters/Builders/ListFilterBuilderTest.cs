﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test.Filters.Builders
{
    [TestClass]
    public class ListFilterBuilderTest
    {
        [TestMethod]
        public void ListDefault()
        {
            var builder = new ListFilterBuilder { DefaultValue = "default" };

            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, builder.DefaultValue);

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

            const string Value = "1";
            var filter = builder.Create(new QueryField<Empleado> { Name = "name" }, Value);

            Assert.AreEqual("name", filter.Name);
            Assert.IsTrue(filter.Valid);
            Assert.AreEqual(1, filter.Value);
            Assert.AreEqual(FilterOperator.Equal, filter.Operator);
            Assert.AreEqual(Value, filter.OriginalText);
        }
    }
}
