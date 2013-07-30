using System;

namespace Common.Extension
{
    public static class EnumExtensions
    {
        public static string ToOrdinalString<T>(this T value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return Convert.ToInt32(value).ToString();
        }

    }
}
