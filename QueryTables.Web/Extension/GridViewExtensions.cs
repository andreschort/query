using System.Linq;
using System.Web.UI.WebControls;

namespace QueryTables.Web.Extension
{
    public static class GridViewExtensions
    {
        public static QueryFieldBase Field(this GridView instance, string name)
        {
            return instance.Columns.OfType<QueryFieldBase>().FirstOrDefault(x => x.Name.Equals(name));
        }

        public static T Field<T>(this GridView instance, string name) where T : QueryFieldBase
        {
            return (T)instance.Columns.OfType<QueryFieldBase>().FirstOrDefault(x => x.Name.Equals(name));
        }
    }
}
