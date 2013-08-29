using System.Globalization;
using System.Threading;
using Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Test
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
    }
}
