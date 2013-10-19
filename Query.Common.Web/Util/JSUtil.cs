using System.Web.UI;

namespace Query.Common.Web.Util
{
    public class JSUtil
    {
        public static void AddLoad(Control control, string key, string javascript)
        {
            ScriptManager.RegisterStartupScript(
                control,
                control.GetType(),
                key,
                string.Format("Sys.Application.add_load(function() {{ {0} }});", javascript),
                true);
        }
    }
}
