using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Query.Web
{
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:TextField runat=server></{0}:TextField>")]
    public class TextField : QueryFieldBase
    {
        #region Fields

        private TextBox textBox;

        /// <summary>
        /// Value given from outside of this component
        /// </summary>
        private string externalFilterValue;

        #endregion Fields

        protected internal override string FilterValue
        {
            get { return this.textBox == null ? this.externalFilterValue ?? string.Empty : this.textBox.Text; }
            set { this.externalFilterValue = value; }
        }

        protected override DataControlField CreateField()
        {
            return new TextField();
        }

        protected internal override void InitHeaderCell(DataControlFieldCell cell)
        {
            base.InitHeaderCell(cell);

            var pnl = new Panel {CssClass = "query-filter"};
            cell.Controls.Add(pnl);

            // Filter textbox
            this.textBox = new TextBox();
            pnl.Controls.Add(textBox);

            this.textBox.Attributes["class"] = "data-query-textFilter";
            this.textBox.AutoCompleteType = AutoCompleteType.Disabled;
            this.textBox.Attributes["autocomplete"] = "off";
            this.textBox.Attributes["placeholder"] = this.Placeholder;

            if (this.AutoFilterDelay.HasValue)
            {
                this.textBox.Attributes["data-query-filterDelay"] = this.AutoFilterDelay.ToString();
            }

            if (this.TabIndex.HasValue)
            {
                this.textBox.TabIndex = this.TabIndex.Value;
            }
        }

        protected internal override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            base.HeaderCell_DataBinding(sender, e);

            // Set value
            this.textBox.Text = string.IsNullOrEmpty(this.externalFilterValue)
                ? HttpContext.Current.Request.Form[this.textBox.UniqueID]
                : this.externalFilterValue;

            // postback configuration, must be here to ensure UniqueID is not null
            this.textBox.Attributes["data-query-postbackName"] = this.FilterButton.UniqueID;

            // restore focus
            this.textBox.Attributes["data-query-focus"] = this.HasFocus(this.textBox.UniqueID).ToString();
        }
    }
}
