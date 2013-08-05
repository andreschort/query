using System;
using System.Web;
using System.Web.UI.WebControls;

namespace Query.Web
{
    public class DateField : QueryFieldBase
    {
        private TextBox textFrom;

        private TextBox textTo;

        private string externalValue;

        protected override DataControlField CreateField()
        {
            return new DateField();
        }

        public override string Value
        {
            get { return this.GetValue(); }
            set { this.externalValue = value; }
        }

        private string GetValue()
        {
            if (this.textFrom == null)
            {
                return this.externalValue ?? string.Empty;
            }

            return this.textFrom.Text + ";" + this.textTo.Text;
        }

        protected override void InitHeaderCell(DataControlFieldCell cell)
        {
            base.InitHeaderCell(cell);

            this.textFrom = new TextBox();
            this.textTo = new TextBox();
            cell.Controls.Add(this.textFrom);
            cell.Controls.Add(this.textTo);

            this.textFrom.Attributes["class"] = "data-query-datepicker";
            this.textFrom.AutoCompleteType = AutoCompleteType.Disabled;
            this.textFrom.Attributes["autocomplete"] = "off";
            this.textFrom.Attributes["data-query-placeholder"] = this.FilterPlaceholder;

            this.textTo.Attributes["class"] = "data-query-datepicker";
            this.textTo.AutoCompleteType = AutoCompleteType.Disabled;
            this.textTo.Attributes["autocomplete"] = "off";
            this.textTo.Attributes["data-query-placeholder"] = this.FilterPlaceholder;

            if (this.AutoFilterDelay.HasValue)
            {
                this.textFrom.Attributes["data-query-filterDelay"] = this.AutoFilterDelay.ToString();
                this.textTo.Attributes["data-query-filterDelay"] = this.AutoFilterDelay.ToString();
            }

            if (this.TabIndex.HasValue)
            {
                this.textFrom.TabIndex = this.TabIndex.Value;
                this.textTo.TabIndex = (short) (this.TabIndex.Value + 1);
            }
        }

        public override short SetTabIndex(short tabIndex)
        {
            this.TabIndex = tabIndex;
            return (short) (tabIndex + 2); // we have two textboxes
        }

        protected override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            base.HeaderCell_DataBinding(sender, e);

            // Set textbox value
            if (string.IsNullOrEmpty(this.externalValue))
            {
                // restore the value from the form
                var nameValueCollection = HttpContext.Current.Request.Form;
                this.textFrom.Text = nameValueCollection[this.textFrom.UniqueID];
                this.textTo.Text = nameValueCollection[this.textTo.UniqueID];
            }
            else
            {
                var parts = this.externalValue.Split(new[] {';'}, 2);
                this.textFrom.Text = parts[0];
                this.textTo.Text = parts[1];
            }

            // postback configuration, must be here to ensure UniqueID is not null
            this.textFrom.Attributes["data-query-postbackName"] = this.Button.UniqueID;
            this.textTo.Attributes["data-query-postbackName"] = this.Button.UniqueID;
            
            // restore focus
            this.textFrom.Attributes["data-query-focus"] = this.HasFocus(this.textFrom.UniqueID).ToString();
            this.textTo.Attributes["data-query-focus"] = this.HasFocus(this.textTo.UniqueID).ToString();
        }
    }
}
