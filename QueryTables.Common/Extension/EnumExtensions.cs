using System;
using System.Reflection;

namespace QueryTables.Common.Extension
{
    public static class EnumExtensions
    {
        public static string ToOrdinalString<T>(this T value) where T : struct, IConvertible
        {
            if (!typeof(T).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return Convert.ToInt32(value).ToString();
        }
    }
}
