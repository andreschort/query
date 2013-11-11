using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTables.Common.Util;

namespace QueryTables.Common.Test
{
    [TestClass]
    public class StringUtilTest
    {
        [TestMethod]
        public void ToDecimalNullable()
        {
            const string Value = "3050.91";

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            var result = StringUtil.ToDecimalNullable(Value, NumberStyles.Float);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToStringNull()
        {
            var result = StringUtil.ToString(null, string.Empty);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void ToStringInt()
        {
            const int Value = 2128;
            const string Format = "N";
            var result = StringUtil.ToString(Value, Format);

            Assert.AreEqual(Value.ToString(Format), result);
        }

        [TestMethod]
        public void ToStringNullableInt()
        {
            int? value = 2128;
            const string Format = "N";
            var result = StringUtil.ToString(value, Format);

            Assert.AreEqual(value.Value.ToString(Format), result);
        }

        [TestMethod]
        public void ToStringDecimal()
        {
            const decimal Value = 2128.36m;
            const string Format = "N";
            var result = StringUtil.ToString(Value, Format);

            Assert.AreEqual(Value.ToString(Format), result);
        }

        [TestMethod]
        public void ToStringNullableDecimal()
        {
            decimal? value = 2128.36m;
            const string Format = "N";
            var result = StringUtil.ToString(value, Format);

            Assert.AreEqual(value.Value.ToString(Format), result);
        }

        [TestMethod]
        public void ToStringDouble()
        {
            const double Value = 2128.36;
            const string Format = "N";
            var result = StringUtil.ToString(Value, Format);

            Assert.AreEqual(Value.ToString(Format), result);
        }

        [TestMethod]
        public void ToStringNullableDouble()
        {
            double? value = 2128.36;
            const string Format = "N";
            var result = StringUtil.ToString(value, Format);

            Assert.AreEqual(value.Value.ToString(Format), result);
        }

        [TestMethod]
        public void ToStringDateTime()
        {
            var value = DateTime.Now;
            const string Format = "d";
            var result = StringUtil.ToString(value, Format);

            Assert.AreEqual(value.ToString(Format), result);
        }

        [TestMethod]
        public void ToStringNullableDateTime()
        {
            DateTime? value = DateTime.Now;
            const string Format = "d";
            var result = StringUtil.ToString(value, Format);

            Assert.AreEqual(value.Value.ToString(Format), result);
        }
    }
}
