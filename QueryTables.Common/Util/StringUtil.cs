using System;
using System.Collections.Generic;
using System.Globalization;

namespace QueryTables.Common.Util
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

        public static decimal? ToDecimalNullable(string value, NumberStyles style)
        {
            decimal val;
            return decimal.TryParse(value, style, CultureInfo.CurrentCulture, out val) ? val : (decimal?)null;
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

        public static T? ParseEnum<T>(string value) where T : struct
        {
            T result;
            if (Enum.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static Queue<string> SplitByDotToHeadAndTail(string s)
        {
            return string.IsNullOrEmpty(s) ? new Queue<string>() : new Queue<string>(s.Split('.'));
        }

        public static string ToString(object val, string format)
        {
            if (val == null)
            {
                return string.Empty;
            }

            if (val is int)
            {
                return ((int)val).ToString(format);
            }

            if (val is decimal)
            {
                return ((decimal)val).ToString(format);
            }

            if (val is double)
            {
                return ((double)val).ToString(format);
            }

            if (val is DateTime)
            {
                return ((DateTime)val).ToString(format);
            }

            return val.ToString();
        }
    }
}
