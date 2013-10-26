using System;
using System.Globalization;
using System.Threading;
using Query.Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Query.Common.Test
{
    [TestClass]
    public class StringUtilTest
    {
        [TestMethod]
        public void ToDecimalNullable()
        {
            const string value = "3050.91";

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");

            var result = StringUtil.ToDecimalNullable(value, NumberStyles.Float);

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
            const int value = 2128;
            const string format = "N";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.ToString(format), result);
        }

        [TestMethod]
        public void ToStringNullableInt()
        {
            int? value = 2128;
            const string format = "N";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.Value.ToString(format), result);
        }

        [TestMethod]
        public void ToStringDecimal()
        {
            const decimal value = 2128.36m;
            const string format = "N";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.ToString(format), result);
        }

        [TestMethod]
        public void ToStringNullableDecimal()
        {
            decimal? value = 2128.36m;
            const string format = "N";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.Value.ToString(format), result);
        }

        [TestMethod]
        public void ToStringDouble()
        {
            const double value = 2128.36;
            const string format = "N";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.ToString(format), result);
        }

        [TestMethod]
        public void ToStringNullableDouble()
        {
            double? value = 2128.36;
            const string format = "N";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.Value.ToString(format), result);
        }

        [TestMethod]
        public void ToStringDateTime()
        {
            var value = DateTime.Now;
            const string format = "d";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.ToString(format), result);
        }

        [TestMethod]
        public void ToStringNullableDateTime()
        {
            DateTime? value = DateTime.Now;
            const string format = "d";
            var result = StringUtil.ToString(value, format);

            Assert.AreEqual(value.Value.ToString(format), result);
        }
    }
}
