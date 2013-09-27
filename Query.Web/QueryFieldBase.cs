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
using SortDirection = Query.Core.SortDirection;

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
        
        public abstract string FilterValue { get; set; }
        
        public short? TabIndex { get; protected set; }

        [Bindable(true)]
        public string DataField { get; set; }

        public string FilterPlaceholder { get; set; }

        public int? AutoFilterDelay { get; set; }

        public string FilterCommand { get; set; }

        public string SortCommand { get; set; }

        public SortDirection? SortDir { get; set; }

        public int SortOrder { get; set; }

        [TypeConverterAttribute(typeof(StringArrayConverter))]
        public string[] UrlFields { get; set; }

        public string UrlFormat { get; set; }

        #endregion Published Properties

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
            this.sortButton = new LinkButton
            {
                Text = this.HeaderText,
                CommandName = this.SortCommand,
                CommandArgument = this.Name
            };

            this.sortInputHidden = new HtmlInputHidden();
            this.sortOrderLabel = new Label();

            cell.Controls.Add(this.sortButton);
            cell.Controls.Add(this.sortInputHidden);
            cell.Controls.Add(this.sortOrderLabel);

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

            var value = this.Eval(dataItem, this.DataField);
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