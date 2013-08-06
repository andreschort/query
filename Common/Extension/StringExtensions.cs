using Common.Util;

namespace Common.Extension
{
    public static class StringExtensions
    {
        public static string Quote(this string aString)
        {
            return "'" + aString + "'";
        }

        public static T? ToEnum<T>(this string aString) where T : struct
        {
            return StringUtil.ParseEnum<T>(aString);
        }
    }
}
