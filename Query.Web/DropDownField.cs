using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Query.Web
{
    public class DropDownField : QueryFieldBase
    {
        private DropDownList dropDownList;

        private string externalValue;

        private LinkButton button;
        
        public override string Value
        {
            get { return this.dropDownList == null ? this.externalValue ?? this.DefaultValue : this.dropDownList.SelectedValue; }
            set { this.externalValue = value; }
        }

        public string DefaultValue { get; set; }

        /// <summary>
        /// Set the filters options to Yes/No/Null
        /// </summary>
        public bool BooleanList { get; set; }

        public List<ListItem> Items { get; set; } 

        protected override DataControlField CreateField()
        {
            return new DropDownField();
        }

        protected override void InitHeaderCell(DataControlFieldCell cell)
        {
            // title with sorting
            var lnkButtonTitle = new LinkButton
            {
                Text = this.HeaderText,
                CommandName = this.SortCommand,
                CommandArgument = this.Name
            };

            cell.Controls.Add(lnkButtonTitle);

            this.dropDownList = new DropDownList();
            cell.Controls.Add(this.dropDownList);

            this.dropDownList.AutoPostBack = true;
            this.dropDownList.SelectedIndexChanged += this.DropDownList_SelectedIndexChanged;

            if (this.TabIndex.HasValue)
            {
                this.dropDownList.TabIndex = this.TabIndex.Value;
            }

            // Filter button
            this.button = new LinkButton { CommandName = this.FilterCommand, CommandArgument = this.Name };
            cell.Controls.Add(this.button);
        }

        private void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((IPostBackEventHandler) this.button).RaisePostBackEvent(null);
        }

        protected override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            this.dropDownList.DataSource = this.Items;
            this.dropDownList.DataTextField = "Text";
            this.dropDownList.DataValueField = "Value";
            this.dropDownList.DataBind();

            // Set textbox value
            if (string.IsNullOrEmpty(this.externalValue))
            {
                // restore the value from the form
                var nameValueCollection = HttpContext.Current.Request.Form;
                this.dropDownList.SelectedValue = nameValueCollection[this.dropDownList.UniqueID];
            }
            else
            {
                this.dropDownList.SelectedValue = this.externalValue;
            }
        }
    }
}
