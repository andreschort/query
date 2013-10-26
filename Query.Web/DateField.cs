﻿using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Query.Web
{
    public class DateField : QueryFieldBase
    {
        #region Fields

        private TextBox textFrom;

        private TextBox textTo;

        private string externalFilterValue;

        #endregion Fields

        protected override DataControlField CreateField()
        {
            return new DateField();
        }

        protected internal override string FilterValue
        {
            get { return this.GetFilterValue(); }
            set { this.externalFilterValue = value; }
        }

        protected internal override void InitHeaderCell(DataControlFieldCell cell)
        {
            base.InitHeaderCell(cell);

            var pnl = new Panel {CssClass = "query-date-filter"};
            cell.Controls.Add(pnl);

            this.textFrom = new TextBox();
            this.textTo = new TextBox();
            pnl.Controls.Add(this.textFrom);
            pnl.Controls.Add(this.textTo);

            this.textFrom.Attributes["class"] = "data-query-datepicker";
            this.textFrom.AutoCompleteType = AutoCompleteType.Disabled;
            this.textFrom.Attributes["autocomplete"] = "off";
            this.textFrom.Attributes["placeholder"] = this.Placeholder;

            this.textTo.Attributes["class"] = "data-query-datepicker";
            this.textTo.AutoCompleteType = AutoCompleteType.Disabled;
            this.textTo.Attributes["autocomplete"] = "off";
            this.textTo.Attributes["placeholder"] = this.Placeholder;

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

        protected internal override short SetTabIndex(short tabIndex)
        {
            this.TabIndex = tabIndex;
            return (short) (tabIndex + 2); // we have two textboxes
        }

        protected internal override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            base.HeaderCell_DataBinding(sender, e);

            // set value
            if (string.IsNullOrEmpty(this.externalFilterValue))
            {
                // restore the value from the form
                this.textFrom.Text = HttpContext.Current.Request.Form[this.textFrom.UniqueID];
                this.textTo.Text = HttpContext.Current.Request.Form[this.textTo.UniqueID];
            }
            else
            {
                var parts = this.externalFilterValue.Split(new[] {';'}, 2);
                this.textFrom.Text = parts[0];

                if (parts.Length > 1)
                {
                    this.textTo.Text = parts[1];
                }
            }

            // postback configuration, must be here to ensure UniqueID is not null
            this.textFrom.Attributes["data-query-postbackName"] = this.FilterButton.UniqueID;
            this.textTo.Attributes["data-query-postbackName"] = this.FilterButton.UniqueID;
            
            // restore focus
            this.textFrom.Attributes["data-query-focus"] = this.HasFocus(this.textFrom.UniqueID).ToString();
            this.textTo.Attributes["data-query-focus"] = this.HasFocus(this.textTo.UniqueID).ToString();
        }

        protected internal override string FormatValue(object val)
        {
            var date = val as DateTime?;

            return date.HasValue ? date.Value.ToString(this.Format) : base.FormatValue(val);
        }

        private string GetFilterValue()
        {
            if (this.textFrom == null)
            {
                return this.externalFilterValue ?? string.Empty;
            }

            return this.textFrom.Text + ";" + this.textTo.Text;
        }
    }
}
