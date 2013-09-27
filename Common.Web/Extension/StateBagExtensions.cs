using System.Collections.Generic;
using System.Web.UI;

namespace Query.Common.Web.Extension
{
    public static class StateBagExtensions
    {
        #region Published Methods

        public static T GetValue<T>(this StateBag stateBag, string key, T defaultValue)
        {
            if (stateBag[key] == null)
            {
                stateBag[key] = defaultValue;
            }

            return (T)stateBag[key];
        }

        public static T GetValue<T>(this StateBag stateBag, string key)
        {
            return stateBag.GetValue(key, default(T));
        }

        public static List<T> GetListValue<T>(this StateBag stateBag, string key)
        {
            if (stateBag[key] == null)
            {
                stateBag[key] = new List<T>();
            }

            return (List<T>)stateBag[key];
        }

        public static HashSet<T> GetHashSetValue<T>(this StateBag stateBag, string key)
        {
            if (stateBag[key] == null)
            {
                stateBag[key] = new HashSet<T>();
            }

            return (HashSet<T>)stateBag[key];
        }

        public static Dictionary<T, K> GetDictionaryValue<T, K>(this StateBag stateBag, string key)
        {
            if (stateBag[key] == null)
            {
                stateBag[key] = new Dictionary<T, K>();
            }

            return (Dictionary<T, K>)stateBag[key];
        }

        public static void SetValue<T>(this StateBag stateBag, string key, List<T> list)
        {
            stateBag[key] = list ?? new List<T>();
        }

        public static void SetValue<T>(this StateBag stateBag, string key, T value)
        {
            stateBag[key] = value;
        }

        public static int NextTransientId(this StateBag stateBag, int seed)
        {
            var value = stateBag.GetValue("NextTransientId", -seed - 1);
            stateBag.SetValue("NextTransientId", value - 1);
            return value;
        }

        #endregion
    }
}