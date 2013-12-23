using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using QueryTables.Web.Filter;

namespace QueryTables.Web
{
    public class DropDownField : QueryFieldBase
    {
        private DropDownFilter dropDownList;

        private string externalFilterValue;

        public List<ListItem> Items { get; set; }

        protected internal override string FilterValue
        {
            get
            {
                if (this.externalFilterValue != null)
                {
                    return this.externalFilterValue;
                }

                return this.dropDownList == null ? string.Empty : this.dropDownList.SelectedValue;
            }

            set
            {
                this.externalFilterValue = value;
            }
        }

        protected override DataControlField CreateField()
        {
            return new DropDownField();
        }

        protected override void HeaderCell_Init(object sender, EventArgs e)
        {
            base.HeaderCell_Init(sender, e);

            var cell = (DataControlFieldHeaderCell)sender;

            var pnl = new Panel { CssClass = "query-filter" };
            cell.Controls.Add(pnl);

            this.dropDownList = new DropDownFilter { ID = this.Name + "_dropdown" };
            pnl.Controls.Add(this.dropDownList);
            
            this.dropDownList.Attributes["class"] = "data-query-dropdown";
        }

        protected override void HeaderCell_Load(object sender, EventArgs e)
        {
            base.HeaderCell_Load(sender, e);

            if (this.TabIndex.HasValue)
            {
                this.dropDownList.TabIndex = this.TabIndex.Value;
            }
        }

        protected override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            base.HeaderCell_DataBinding(sender, e);
            
            this.Items.Insert(0, new ListItem(this.Placeholder ?? string.Empty, string.Empty)); // add empty option
            
            this.dropDownList.DataSource = this.Items;
            this.dropDownList.DataTextField = "Text";
            this.dropDownList.DataValueField = "Value";
            this.dropDownList.DataBind();

            this.dropDownList.PostbackName = this.PostbackName;
            this.dropDownList.PostbackParameter = this.FilterCommand;
            this.dropDownList.HasFocus = this.HasFocus(this.dropDownList.UniqueID);

            // set selected value
            if (this.externalFilterValue == null)
            {
                this.dropDownList.SelectedValue = HttpContext.Current.Request.Form[this.dropDownList.UniqueID];
            }
            else if (!string.IsNullOrEmpty(this.externalFilterValue))
            {
                this.dropDownList.SelectedValue = this.externalFilterValue;
            }
        }
    }
}