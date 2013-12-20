using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace QueryTables.Web.Filter
{
    public class TextFilter : TextBox, IScriptControl
    {
        private ScriptManager sm;

        private TextBoxWatermarkExtender watermark;

        public string Placeholder { get; set; }

        public int? AutoFilterDelay { get; set; }

        public bool HasFocus { get; set; }

        public string PostbackName { get; set; }

        public string PostbackParameter { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Attributes["class"] = "data-query-textFilter";
            this.AutoCompleteType = AutoCompleteType.Disabled;
            this.Attributes["autocomplete"] = "off";

            this.watermark = new TextBoxWatermarkExtender
            {
                ID = this.ID + "_watermark",
                TargetControlID = this.ID
            };

            this.Parent.Controls.Add(this.watermark);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.watermark.WatermarkText = this.Placeholder;
            this.watermark.Enabled = !string.IsNullOrEmpty(this.Placeholder);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                // Test for ScriptManager and register if it exists
                sm = ScriptManager.GetCurrent(Page);

                if (sm == null)
                    throw new HttpException("A ScriptManager control must exist on the current page.");

                sm.RegisterScriptControl(this);
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                sm.RegisterScriptDescriptors(this);
            }   

            base.Render(writer);
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            var reference = new ScriptReference("QueryTables.Web.Filter.TextFilter.js", "QueryTables.Web");

            return new[] {reference};
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptor = new ScriptControlDescriptor("QueryTables.Web.TextFilter", this.ClientID);

            this.GetScriptDescriptor(descriptor);

            return new [] { descriptor };
        }

        protected void GetScriptDescriptor(ScriptControlDescriptor descriptor)
        {
            descriptor.AddProperty("placeholder", this.Placeholder);
            descriptor.AddProperty("autoFilterDelay", this.AutoFilterDelay);
            descriptor.AddProperty("hasFocus", this.HasFocus);
            descriptor.AddProperty("postbackName", this.PostbackName);
            descriptor.AddProperty("postbackParameter", this.PostbackParameter);
        }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            return GetScriptReferences();
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            return GetScriptDescriptors();
        }
    }
}
