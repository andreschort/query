namespace Common.Extension
{
    public static class StringExtensions
    {
        public static string Quote(this string aString)
        {
            return "'" + aString + "'";
        }
    }
}
