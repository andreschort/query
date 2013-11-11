using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace QueryTables.Web
{
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:TextField runat=server></{0}:TextField>")]
    public class TextField : QueryFieldBase
    {
        #region Fields

        private TextBox textBox;

        private TextBoxWatermarkExtender watermark;

        /// <summary>
        /// Value given from outside of this component
        /// </summary>
        private string externalFilterValue;

        #endregion Fields

        protected internal override string FilterValue
        {
            get
            {
                if (this.externalFilterValue != null)
                {
                    return this.externalFilterValue;
                }

                return this.textBox == null ? string.Empty : this.textBox.Text;
            }

            set
            {
                this.externalFilterValue = value;
            }
        }

        protected override DataControlField CreateField()
        {
            return new TextField();
        }

        protected override void HeaderCell_Init(object sender, EventArgs e)
        {
            base.HeaderCell_Init(sender, e);

            var cell = (DataControlFieldHeaderCell)sender;

            var pnl = new Panel { CssClass = "query-filter" };
            cell.Controls.Add(pnl);

            // Filter textbox
            this.textBox = new TextBox { ID = this.Name + "_textbox" };
            pnl.Controls.Add(this.textBox);

            this.textBox.Attributes["class"] = "data-query-textFilter";
            this.textBox.AutoCompleteType = AutoCompleteType.Disabled;
            this.textBox.Attributes["autocomplete"] = "off";

            this.watermark = new TextBoxWatermarkExtender
                {
                    ID = this.Name + "_watermark",
                    TargetControlID = this.textBox.ID
                };
            pnl.Controls.Add(this.watermark);
        }

        protected override void HeaderCell_Load(object sender, EventArgs e)
        {
            base.HeaderCell_Load(sender, e);

            this.textBox.Attributes["data-query-placeholder"] = this.Placeholder;

            this.watermark.WatermarkText = this.Placeholder;
            this.watermark.Enabled = !string.IsNullOrEmpty(this.Placeholder);

            if (this.AutoFilterDelay.HasValue)
            {
                this.textBox.Attributes["data-query-filterDelay"] = this.AutoFilterDelay.ToString();
            }

            if (this.TabIndex.HasValue)
            {
                this.textBox.TabIndex = this.TabIndex.Value;
            }
        }

        protected override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            base.HeaderCell_DataBinding(sender, e);

            // Set value
            this.textBox.Text = this.externalFilterValue ?? HttpContext.Current.Request.Form[this.textBox.UniqueID];

            // postback configuration, must be here to ensure UniqueID is not null
            this.textBox.Attributes["data-query-postbackName"] = this.FilterButton.UniqueID;

            // restore focus
            this.textBox.Attributes["data-query-focus"] = this.HasFocus(this.textBox.UniqueID).ToString();
        }
    }
}
