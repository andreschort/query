using System;
using System.Globalization;
using System.Threading;
using Xunit;
using QueryTables.Common.Util;

namespace QueryTables.Common.Test
{
    public class StringUtilTest
    {
        [Fact]
        public void ToDecimalNullable()
        {
            const string Value = "3050.91";

            CultureInfo.CurrentCulture = new CultureInfo("es-AR");

            var result = StringUtil.ToDecimalNullable(Value, NumberStyles.Float);

            Assert.Null(result);
        }

        [Fact]
        public void ToStringNull()
        {
            var result = StringUtil.ToString(null, string.Empty);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ToStringInt()
        {
            const int Value = 2128;
            const string Format = "N";
            var result = StringUtil.ToString(Value, Format);

            Assert.Equal(Value.ToString(Format), result);
        }

        [Fact]
        public void ToStringNullableInt()
        {
            int? value = 2128;
            const string Format = "N";
            var result = StringUtil.ToString(value, Format);

            Assert.Equal(value.Value.ToString(Format), result);
        }

        [Fact]
        public void ToStringDecimal()
        {
            const decimal Value = 2128.36m;
            const string Format = "N";
            var result = StringUtil.ToString(Value, Format);

            Assert.Equal(Value.ToString(Format), result);
        }

        [Fact]
        public void ToStringNullableDecimal()
        {
            decimal? value = 2128.36m;
            const string Format = "N";
            var result = StringUtil.ToString(value, Format);

            Assert.Equal(value.Value.ToString(Format), result);
        }

        [Fact]
        public void ToStringDouble()
        {
            const double Value = 2128.36;
            const string Format = "N";
            var result = StringUtil.ToString(Value, Format);

            Assert.Equal(Value.ToString(Format), result);
        }

        [Fact]
        public void ToStringNullableDouble()
        {
            double? value = 2128.36;
            const string Format = "N";
            var result = StringUtil.ToString(value, Format);

            Assert.Equal(value.Value.ToString(Format), result);
        }

        [Fact]
        public void ToStringDateTime()
        {
            var value = DateTime.Now;
            const string Format = "d";
            var result = StringUtil.ToString(value, Format);

            Assert.Equal(value.ToString(Format), result);
        }

        [Fact]
        public void ToStringNullableDateTime()
        {
            DateTime? value = DateTime.Now;
            const string Format = "d";
            var result = StringUtil.ToString(value, Format);

            Assert.Equal(value.Value.ToString(Format), result);
        }
    }
}
