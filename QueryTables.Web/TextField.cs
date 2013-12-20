using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QueryTables.Web.Filter;

namespace QueryTables.Web
{
    [DefaultProperty("Name")]
    [ToolboxData("<{0}:TextField runat=server></{0}:TextField>")]
    public class TextField : QueryFieldBase
    {
        #region Fields

        private TextFilter textFilter;

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

                return this.textFilter == null ? string.Empty : this.textFilter.Text;
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
            this.textFilter = new TextFilter { ID = this.Name + "_textbox" };
            pnl.Controls.Add(this.textFilter);
        }

        protected override void HeaderCell_Load(object sender, EventArgs e)
        {
            base.HeaderCell_Load(sender, e);

            this.textFilter.Placeholder = this.Placeholder;
            this.textFilter.AutoFilterDelay = this.AutoFilterDelay;
            
            if (this.TabIndex.HasValue)
            {
                this.textFilter.TabIndex = this.TabIndex.Value;
            }
        }

        protected override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            base.HeaderCell_DataBinding(sender, e);

            // Set value
            this.textFilter.Text = this.externalFilterValue ?? HttpContext.Current.Request.Form[this.textFilter.UniqueID];

            this.textFilter.PostbackName = this.PostbackName;
            this.textFilter.PostbackParameter = this.FilterCommand;

            // restore focus
            this.textFilter.HasFocus = this.HasFocus(this.textFilter.UniqueID);
        }
    }
}
