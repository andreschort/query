using QueryTables.Common.Util;

namespace QueryTables.Common.Extension
{
    public static class StringExtensions
    {
        public static T? ToEnum<T>(this string aString) where T : struct
        {
            return StringUtil.ParseEnum<T>(aString);
        }
    }
}
