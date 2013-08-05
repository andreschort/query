﻿using System;
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
        private string externalValue;

        #endregion Fields

        #region Published Properties

        public override string Value
        {
            get { return this.textBox == null ? this.externalValue ?? string.Empty : this.textBox.Text; }
            set { this.externalValue = value; }
        }

        #endregion Published Properties

        protected override DataControlField CreateField()
        {
            return new TextField();
        }

        protected override void InitHeaderCell(DataControlFieldCell cell)
        {
            base.InitHeaderCell(cell);

            // Filter textbox
            this.textBox = new TextBox();
            cell.Controls.Add(textBox);

            this.textBox.Attributes["class"] = "data-query-textFilter";
            this.textBox.AutoCompleteType = AutoCompleteType.Disabled;
            this.textBox.Attributes["autocomplete"] = "off";
            this.textBox.Attributes["data-query-placeholder"] = this.FilterPlaceholder;

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
            // Set textbox value
            if (string.IsNullOrEmpty(this.externalValue))
            {
                // restore the value from the form
                var nameValueCollection = HttpContext.Current.Request.Form;
                this.textBox.Text = nameValueCollection[this.textBox.UniqueID];
            }
            else
            {
                this.textBox.Text = this.externalValue;
            }

            // postback configuration, must be here to ensure UniqueID is not null
            this.textBox.Attributes["data-query-postbackName"] = this.Button.UniqueID;

            // restore focus
            this.textBox.Attributes["data-query-focus"] = this.HasFocus(this.textBox.UniqueID).ToString();
        }
    }
}
