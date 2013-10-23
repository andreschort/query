using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Query.Common.Extension;
using Query.Common.Util;
using SortDirection = Query.Common.SortDirection;

namespace Query.Web
{
    public abstract class QueryFieldBase : DataControlField
    {
        #region Fields

        protected LinkButton FilterButton;
        
        private LinkButton sortButton;
        private HtmlInputHidden sortInputHidden;
        private Label sortOrderLabel;

        private bool sortHiddenInputRead;
        
        #endregion Fields

        #region Published Properties

        public string Name { get; set; }

        /// <summary>
        /// The text to show when the filter is empty
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// The number of milliseconds to wait after the user enters some text in the filter UI control.
        /// </summary>
        public int? AutoFilterDelay { get; set; }

        /// <summary>
        /// A comma separeted list of properties to use as parameters to UrlFormat.
        /// </summary>
        [TypeConverterAttribute(typeof(StringArrayConverter))]
        public string[] UrlFields { get; set; }

        /// <summary>
        /// Makes the text in the cell a link to this url
        /// </summary>
        public string UrlFormat { get; set; }

        #endregion Published Properties

        #region Internal Properties

        protected internal abstract string FilterValue { get; set; }

        protected internal string FilterCommand { get; set; }

        protected internal string SortCommand { get; set; }

        protected internal SortDirection? SortDir { get; set; }

        protected internal int SortOrder { get; set; }

        protected short? TabIndex { get; set; }

        #endregion Internal Properties

        #region Published Methods

        public override void InitializeCell(
            DataControlFieldCell cell,
            DataControlCellType cellType,
            DataControlRowState rowState,
            int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);

            if (cellType == DataControlCellType.Header)
            {
                cell.DataBinding += this.HeaderCell_DataBinding;
                cell.Load += this.HeaderCell_Load;
                cell.PreRender += this.HeaderCell_PreRender;
                this.InitHeaderCell(cell);
            }
            else if (cellType == DataControlCellType.DataCell)
            {
                cell.DataBinding += this.DataCell_DataBinding;
                this.InitDataCell(cell, rowState);
            }
        }

        /// <summary>
        /// Moves the Sort Direction to then next value.
        /// In case this field was not sorting until now, it also sets the sort order to newSortOrder.
        /// In case this field is no longer sorting, it also sets the sort order to zero.
        /// </summary>
        /// <param name="newSortOrder"></param>
        public void CycleSort(int newSortOrder)
        {
            if (this.SortDir.Equals(SortDirection.Ascending))
            {
                this.SortDir = SortDirection.Descending;
            }
            else if (this.SortDir.Equals(SortDirection.Descending))
            {
                this.SortDir = null;
                this.SortOrder = 0;
            }
            else
            {
                this.SortDir = SortDirection.Ascending;
                this.SortOrder = newSortOrder;
            }
        }

        /// <summary>
        /// Sets this tab index of the filtering controls
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <returns>The next tab index</returns>
        public virtual short SetTabIndex(short tabIndex)
        {
            this.TabIndex = tabIndex;
            return (short)(tabIndex + 1);
        }

        #endregion Published Methods

        protected void InitDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            // Override if you want to do something before the cell's databinding
        }

        protected virtual void InitHeaderCell(DataControlFieldCell cell)
        {
            // title with sorting
            var pnl = new Panel {CssClass = "query-header"};
            cell.Controls.Add(pnl);
            
            this.sortButton = new LinkButton
            {
                Text = this.HeaderText,
                CommandName = this.SortCommand,
                CommandArgument = this.Name
            };

            this.sortInputHidden = new HtmlInputHidden();
            this.sortOrderLabel = new Label();

            pnl.Controls.Add(this.sortButton);
            pnl.Controls.Add(this.sortInputHidden);
            pnl.Controls.Add(this.sortOrderLabel);

            // Filter button
            this.FilterButton = new LinkButton { CommandName = this.FilterCommand, CommandArgument = this.Name };
            this.FilterButton.Attributes["style"] = "display:none";
            cell.Controls.Add(this.FilterButton);
        }

        private void HeaderCell_Load(object sender, EventArgs e)
        {
            // read the sort order and direction from sortInputHidden
            var hiddenFieldValue = HttpContext.Current.Request.Form[this.sortInputHidden.UniqueID];

            // we should do this only one time in every postback
            if (this.sortHiddenInputRead)
            {
                return;
            }

            if (string.IsNullOrEmpty(hiddenFieldValue))
            {
                return;
            }

            var parts = hiddenFieldValue.Split(new[] { ';' }, 2);

            this.SortOrder = StringUtil.ToInt(parts[0]);
            this.SortDir = parts[1].ToEnum<SortDirection>();

            this.sortHiddenInputRead = true;
        }

        /// <summary>
        /// Save sort order and direction to sortInputHidden.
        /// Set sort order in sortOrderLabel only if it is greater than zero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeaderCell_PreRender(object sender, EventArgs e)
        {
            this.sortInputHidden.Value = this.SortOrder + ";" + this.SortDir;

            this.sortOrderLabel.Text = this.SortOrder > 0 ? this.SortOrder.ToString() : string.Empty;

            if (this.SortDir.HasValue)
            {
                var header = (DataControlFieldHeaderCell) sender;
                header.CssClass = this.SortDir.Equals(SortDirection.Ascending) ? "asc" : "desc";
            }
        }
        
        protected virtual void HeaderCell_DataBinding(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Sets the cell text to the value of the field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected virtual void DataCell_DataBinding(object sender, EventArgs eventArgs)
        {
            var cell = sender as TableCell;
            object dataItem = DataBinder.GetDataItem(cell.NamingContainer);

            var value = this.Eval(dataItem, this.Name);
            var displayValue = this.FormatValue(value);
            
            if (string.IsNullOrEmpty(this.UrlFormat))
            {
                cell.Text = displayValue;
            }
            else
            {
                var linkButton = new HyperLink();
                cell.Controls.Add(linkButton);
                linkButton.Text = displayValue;

                linkButton.NavigateUrl = this.UrlFields == null
                                             ? this.UrlFormat
                                             : string.Format(this.UrlFormat,
                                                             this.UrlFields.Select(x => this.Eval(dataItem, x)).ToArray());
            }
        }

        /// <summary>
        /// Tells if the control with the indicated unique ID was the last control with the focus before the postback
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        protected bool HasFocus(string uniqueID)
        {
            var lastFocus = HttpContext.Current.Request.Form["__LASTFOCUS"];

            return uniqueID.Equals(lastFocus);
        }

        protected virtual string FormatValue(object val)
        {
            return val == null ? string.Empty : val.ToString();
        }

        private object Eval(object dataItem, string dataField)
        {
            var view = dataItem as DataRowView;
            return view == null
                       ? DataBinder.Eval(dataItem, dataField)
                       : view.Row[dataField];
        }
    }
}