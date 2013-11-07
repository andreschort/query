using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace QueryTables.Web
{
    public class DropDownField : QueryFieldBase
    {
        private DropDownList dropDownList;

        private string externalFilterValue;

        protected internal override string FilterValue
        {
            get { return this.dropDownList == null ? this.externalFilterValue ?? string.Empty : this.dropDownList.SelectedValue; }
            set { this.externalFilterValue = value; }
        }
        
        public List<ListItem> Items { get; set; }

        protected override DataControlField CreateField()
        {
            return new DropDownField();
        }

        protected override void HeaderCell_Init(object sender, EventArgs e)
        {
            base.HeaderCell_Init(sender, e);

            var cell = (DataControlFieldHeaderCell)sender;

            var pnl = new Panel {CssClass = "query-filter"};
            cell.Controls.Add(pnl);

            this.dropDownList = new DropDownList();
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
            
            this.Items.Insert(0, new ListItem(string.Empty, string.Empty)); // add empty option
            
            this.dropDownList.DataSource = this.Items;
            this.dropDownList.DataTextField = "Text";
            this.dropDownList.DataValueField = "Value";
            this.dropDownList.DataBind();

            // set selected value
            this.dropDownList.SelectedValue = string.IsNullOrEmpty(this.externalFilterValue)
                ? HttpContext.Current.Request.Form[this.dropDownList.UniqueID]
                : this.externalFilterValue;

            // postback configuration, must be here to ensure UniqueID is not null
            this.dropDownList.Attributes["data-query-postbackName"] = this.FilterButton.UniqueID;

            // restore focus
            this.dropDownList.Attributes["data-query-focus"] = this.HasFocus(this.dropDownList.UniqueID).ToString();
        }
    }
}