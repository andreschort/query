using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QueryTables.Web.Filter
{
    public class DropDownFilter : DropDownList, IScriptControl
    {
        private ScriptManager sm;

        public bool HasFocus { get; set; }

        public string PostbackName { get; set; }

        public string PostbackParameter { get; set; }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            return this.GetScriptReferences();
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            return this.GetScriptDescriptors();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                // Test for ScriptManager and register if it exists
                this.sm = ScriptManager.GetCurrent(this.Page);

                if (this.sm == null)
                {
                    throw new HttpException("A ScriptManager control must exist on the current page.");
                }

                this.sm.RegisterScriptControl(this);
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                this.sm.RegisterScriptDescriptors(this);
            }

            base.Render(writer);
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            var reference = new ScriptReference("QueryTables.Web.Filter.DropDownFilter.js", "QueryTables.Web");

            return new[] { reference };
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptor = new ScriptControlDescriptor("QueryTables.Web.DropDownFilter", this.ClientID);

            descriptor.AddProperty("hasFocus", this.HasFocus);
            descriptor.AddProperty("postbackName", this.PostbackName);
            descriptor.AddProperty("postbackParameter", this.PostbackParameter);

            return new[] { descriptor };
        }
    }
}
