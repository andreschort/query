using System;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace QueryTables.Web
{
    public class DateField : QueryFieldBase
    {
        #region Fields

        private TextBox textFrom;
        private TextBox textTo;

        private TextBoxWatermarkExtender watermarkFrom;
        private TextBoxWatermarkExtender watermarkTo;

        private string externalFilterValue;

        #endregion Fields

        public string PlaceholderFrom { get; set; }

        public string PlaceholderTo { get; set; }

        protected internal override string FilterValue
        {
            get { return this.GetFilterValue(); }
            set { this.externalFilterValue = value; }
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
            cell.Controls.Add(pnl);

            // from and to filter inputs
            this.textFrom = new TextBox { ID = this.Name + "_textbox_from" };
            this.textTo = new TextBox { ID = this.Name + "_textbox_to" };
            pnl.Controls.Add(this.textFrom);
            pnl.Controls.Add(this.textTo);

            this.textFrom.Attributes["class"] = "data-query-datepicker";
            this.textFrom.AutoCompleteType = AutoCompleteType.Disabled;
            this.textFrom.Attributes["autocomplete"] = "off";

            this.textTo.Attributes["class"] = "data-query-datepicker";
            this.textTo.AutoCompleteType = AutoCompleteType.Disabled;
            this.textTo.Attributes["autocomplete"] = "off";

            // watermarks (placeholders)
            this.watermarkFrom = new TextBoxWatermarkExtender
            {
                ID = this.Name + "_watermark_from",
                TargetControlID = this.textFrom.ID
            };
            this.watermarkTo = new TextBoxWatermarkExtender
            {
                ID = this.Name + "_watermark_to",
                TargetControlID = this.textTo.ID
            };
            pnl.Controls.Add(this.watermarkFrom);
            pnl.Controls.Add(this.watermarkTo);

            // calendars
            var calendarFrom = new CalendarExtender
                {
                    ID = this.Name + "_calendar_from",
                    TargetControlID = this.textFrom.ID,
                };
            var calendarTo = new CalendarExtender
                {
                    ID = this.Name + "_calendar_to",
                    TargetControlID = this.textTo.ID,
                };
            pnl.Controls.Add(calendarFrom);
            pnl.Controls.Add(calendarTo);
            
            calendarFrom.Format = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            calendarTo.Format = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern;

            calendarFrom.OnClientShowing = "Query_DateField_CancelShowWhenFocus";
            calendarTo.OnClientShowing = "Query_DateField_CancelShowWhenFocus";
        }

        protected override void HeaderCell_Load(object sender, EventArgs e)
        {
            base.HeaderCell_Load(sender, e);

            this.textFrom.Attributes["data-query-placeholder"] = this.PlaceholderFrom;
            this.textTo.Attributes["data-query-placeholder"] = this.PlaceholderTo;

            this.watermarkFrom.WatermarkText = this.PlaceholderFrom;
            this.watermarkFrom.Enabled = !string.IsNullOrEmpty(this.PlaceholderFrom);

            this.watermarkTo.WatermarkText = this.PlaceholderTo;
            this.watermarkTo.Enabled = !string.IsNullOrEmpty(this.PlaceholderTo);

            if (this.AutoFilterDelay.HasValue)
            {
                this.textFrom.Attributes["data-query-filterDelay"] = this.AutoFilterDelay.ToString();
                this.textTo.Attributes["data-query-filterDelay"] = this.AutoFilterDelay.ToString();
            }

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
            else if (string.IsNullOrEmpty(this.externalFilterValue))
            {
                var parts = this.externalFilterValue.Split(new[] { ';' }, 2);
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

            return this.textFrom.Text + ";" + this.textTo.Text;
        }
    }
}
