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

        private LinkButton button;

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

        protected override void InitDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
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

            // Filter textbox
            this.textBox = new TextBox();
            cell.Controls.Add(textBox);

            this.textBox.Attributes["class"] = "data-query-textFilter";
            this.textBox.AutoCompleteType = AutoCompleteType.Disabled;
            textBox.Attributes["autocomplete"] = "off";
            this.textBox.Attributes["data-query-placeholder"] = this.FilterPlaceholder;

            if (this.AutoFilterDelay.HasValue)
            {
                this.textBox.Attributes["data-query-filterDelay"] = this.AutoFilterDelay.ToString();
            }

            if (this.TabIndex.HasValue)
            {
                this.textBox.TabIndex = this.TabIndex.Value;
            }

            if (this.Focus)
            {
                this.textBox.Attributes["data-query-focus"] = true.ToString();
            }

            // Filter button
            this.button = new LinkButton {CommandName = this.FilterCommand, CommandArgument = this.Name};
            cell.Controls.Add(this.button);
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
            this.textBox.Attributes["data-query-postbackName"] = this.button.UniqueID;
        }
    }
}
