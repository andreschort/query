using System;
using System.Collections.Generic;
using System.Globalization;

namespace Common.Util
{
    public class StringUtil
    {
        public static int? ToIntNullable(string value)
        {
            int converted;
            return int.TryParse(value, out converted) ? (int?)converted : null;
        }

        public static int? ToIntNullable(object value)
        {
            return value == null ? null : ToIntNullable(value.ToString());
        }

        public static int ToInt(object value)
        {
            return ToIntNullable(value) ?? 0;
        }

        public static bool? ToBoolNullable(string value)
        {
            bool converted;
            return bool.TryParse(value, out converted) ? (bool?)converted : null;
        }

        public static bool? ToBoolNullable(object value)
        {
            return value == null ? null : ToBoolNullable(value.ToString());
        }

        public static bool ToBool(object value)
        {
            return ToBoolNullable(value) ?? false;
        }

        public static decimal? ToDecimalNullable(string value)
        {
            decimal val;
            return decimal.TryParse(value, out val) ? val : (decimal?)null;
        }

        public static decimal ToDecimal(string value)
        {
            return ToDecimalNullable(value) ?? 0;
        }

        public static DateTime? ToDateNullable(string value)
        {
            DateTime val;
            return DateTime.TryParse(value, out val) ? val : (DateTime?)null;
        }

        public static DateTime? ToDateNullable(
            string value,
            IFormatProvider provider,
            DateTimeStyles styles)
        {
            DateTime val;
            return DateTime.TryParse(value, provider, styles, out val) ? val : (DateTime?)null;
        }

        public static Queue<string> SplitByDotToHeadAndTail(string s)
        {
            return string.IsNullOrEmpty(s) ? new Queue<string>() : new Queue<string>(s.Split('.'));
        }
    }
}
