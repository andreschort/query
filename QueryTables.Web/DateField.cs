using System;
using System.Web;
using System.Web.UI.WebControls;
using QueryTables.Web.Filter;

namespace QueryTables.Web
{
    public class DateField : QueryFieldBase
    {
        public const char Separator = ';';

        #region Fields

        private DateFilter textFrom;
        private DateFilter textTo;

        private string externalFilterValue;

        #endregion Fields

        public string PlaceholderFrom { get; set; }

        public string PlaceholderTo { get; set; }

        protected internal override string FilterValue
        {
            get { return this.GetFilterValue(); }
            set { this.externalFilterValue = value; }
        }

        public static string Serialize(DateTime? dateFrom, DateTime? dateTo)
        {
            string value = string.Empty;

            if (dateFrom.HasValue)
            {
                value = dateFrom.Value.ToShortDateString();
            }

            value += Separator;

            if (dateTo.HasValue)
            {
                value += dateTo.Value.ToShortDateString();
            }

            return value;
        }

        protected internal override short SetTabIndex(short tabIndex)
        {
            this.TabIndex = tabIndex;
            return (short)(tabIndex + 2); // we have two textboxes
        }

        protected override DataControlField CreateField()
        {
            return new DateField();
        }

        protected override void HeaderCell_Init(object sender, EventArgs e)
        {
            base.HeaderCell_Init(sender, e);

            var cell = (DataControlFieldHeaderCell)sender;

            var pnl = new Panel { CssClass = "query-date-filter" };
            pnl.Attributes["onkeypress"] = string.Empty; // Remove DefaultButton for good
            cell.Controls.Add(pnl);

            // from and to filter inputs
            this.textFrom = new DateFilter { ID = this.Name + "_from" };
            this.textTo = new DateFilter { ID = this.Name + "_to" };
            pnl.Controls.Add(this.textFrom);
            pnl.Controls.Add(this.textTo);
        }

        protected override void HeaderCell_Load(object sender, EventArgs e)
        {
            base.HeaderCell_Load(sender, e);

            this.textFrom.Placeholder = this.PlaceholderFrom;
            this.textTo.Placeholder = this.PlaceholderTo;
            this.textFrom.AutoFilterDelay = this.AutoFilterDelay;
            this.textTo.AutoFilterDelay = this.AutoFilterDelay;
            
            if (this.TabIndex.HasValue)
            {
                this.textFrom.TabIndex = this.TabIndex.Value;
                this.textTo.TabIndex = (short)(this.TabIndex.Value + 1);
            }
        }

        protected override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            base.HeaderCell_DataBinding(sender, e);

            // set value
            if (this.externalFilterValue == null)
            {
                // restore the value from the form
                this.textFrom.Text = HttpContext.Current.Request.Form[this.textFrom.UniqueID];
                this.textTo.Text = HttpContext.Current.Request.Form[this.textTo.UniqueID];
            }
            else if (!string.IsNullOrEmpty(this.externalFilterValue))
            {
                var parts = this.externalFilterValue.Split(new[] { Separator }, 2);
                this.textFrom.Text = parts[0];

                if (parts.Length > 1)
                {
                    this.textTo.Text = parts[1];
                }
            }

            this.textFrom.PostbackName = this.PostbackName;
            this.textTo.PostbackName = this.PostbackName;

            this.textFrom.PostbackParameter = this.FilterCommand;
            this.textTo.PostbackParameter = this.FilterCommand;
            
            // restore focus
            this.textFrom.HasFocus = this.HasFocus(this.textFrom.UniqueID);
            this.textTo.HasFocus = this.HasFocus(this.textTo.UniqueID);
        }

        private string GetFilterValue()
        {
            if (this.externalFilterValue != null)
            {
                return this.externalFilterValue;
            }

            if (this.textFrom == null)
            {
                return this.externalFilterValue ?? string.Empty;
            }

            return this.textFrom.Text + Separator + this.textTo.Text;
        }
    }
}
