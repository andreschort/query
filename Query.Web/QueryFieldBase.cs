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
        private HtmlInputHidden sortInputHidden; // used to save the sort direction and order across postbacks
        private Label sortOrderLabel;

        /// <summary>
        /// Tells if the value in sortInputHidden was already read in this postback.
        /// Used to avoid overriding a value set manually during postback.
        /// </summary>
        private bool sortHiddenInputRead;

        private bool itemEnabled = true;

        private LinkButton linkButton; // used when the data cell must show a link
        private HtmlInputHidden valueHidden; // used to save the text of the link across postbacks
        private HtmlInputHidden urlHidden; // used to save the url of the link across postbacks

        private string displayValue; // the text of the link
        private string navigateUrl; // the url of the link
        
        #endregion Fields

        public event EventHandler Click; // the click in a data cell link

        #region Published Properties

        /// <summary>
        /// The field's name
        /// </summary>
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

        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(IDataItemContainer), BindingDirection.TwoWay)]
        public virtual ITemplate ItemTemplate { get; set; }

        /// <summary>
        /// Controls wheather the elements inside the data cell are enabled or not
        /// </summary>
        public bool ItemEnabled
        {
            get { return this.itemEnabled; }
            set { this.itemEnabled = value; }
        }

        /// <summary>
        /// The format to use when converting the data cell value to string.
        /// Can be any standard format
        /// </summary>
        public string Format { get; set; }

        public Func<object, string> FormatDelegate { get; set; }

        #endregion Published Properties

        #region Internal Properties

        protected internal abstract string FilterValue { get; set; }

        protected internal string FilterCommand { get; set; }

        protected internal string SortCommand { get; set; }

        protected internal virtual SortDirection? SortDir { get; set; }

        protected internal virtual int SortOrder { get; set; }

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

            switch (cellType)
            {
                case DataControlCellType.Header:
                    cell.Init += this.HeaderCell_Init;
                    cell.DataBinding += this.HeaderCell_DataBinding;
                    cell.Load += this.HeaderCell_Load;
                    cell.PreRender += this.HeaderCell_PreRender;
                    break;
                case DataControlCellType.DataCell:
                    cell.Init += this.DataCell_Init;
                    cell.DataBinding += this.DataCell_DataBinding;
                    cell.Load += this.DataCell_Load;
                    cell.PreRender += this.DataCell_PreRender;
                    break;
            }
        }

        /// <summary>
        /// Moves the Sort Direction to then next value.
        /// In case this field was not sorting until now, it also sets the sort order to newSortOrder.
        /// In case this field is no longer sorting, it also sets the sort order to zero.
        /// </summary>
        /// <param name="newSortOrder"></param>
        protected internal virtual void CycleSort(int newSortOrder)
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
        protected internal virtual short SetTabIndex(short tabIndex)
        {
            this.TabIndex = tabIndex;
            return (short)(tabIndex + 1);
        }

        protected internal virtual void AdjustSortOrder(int removedSortOrder)
        {
            if (this.SortOrder > removedSortOrder)
            {
                this.SortOrder--;
            }
        }

        #endregion Published Methods

        #region HeaderCell Events Handlers

        protected virtual void HeaderCell_Init(object sender, EventArgs e)
        {
            var cell = (DataControlFieldHeaderCell) sender;

            // title with sorting
            var pnl = new Panel {CssClass = "query-header", DefaultButton = null};
            cell.Controls.Add(pnl);

            this.sortButton = new LinkButton();
            this.sortInputHidden = new HtmlInputHidden();
            this.sortOrderLabel = new Label();

            pnl.Controls.Add(this.sortButton);
            pnl.Controls.Add(this.sortInputHidden);
            pnl.Controls.Add(this.sortOrderLabel);

            // Filter button
            this.FilterButton = new LinkButton();
            this.FilterButton.Attributes["style"] = "display:none";
            cell.Controls.Add(this.FilterButton);
        }

        protected virtual void HeaderCell_Load(object sender, EventArgs e)
        {
            this.sortButton.Text = this.HeaderText;
            this.sortButton.CommandName = this.SortCommand;
            this.sortButton.CommandArgument = this.Name;

            this.FilterButton.CommandName = this.FilterCommand;
            this.FilterButton.CommandArgument = this.Name;

            // read the sort order and direction from sortInputHidden
            // we should do this only one time in every postback
            if (this.sortHiddenInputRead)
            {
                return;
            }
            
            var hiddenFieldValue = HttpContext.Current.Request.Form[this.sortInputHidden.UniqueID];

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
        protected virtual void HeaderCell_PreRender(object sender, EventArgs e)
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

        #endregion HeaderCell Events Handlers

        #region DataCell Event Handlers

        /// <summary>
        /// Creates linkButton, valueHidden and urlHidden for data cells with a link
        /// </summary>
        protected virtual void DataCell_Init(object sender, EventArgs e)
        {
            var cell = (TableCell)sender;

            cell.Enabled = this.ItemEnabled;
            if (this.ItemTemplate != null)
            {
                this.ItemTemplate.InstantiateIn(cell);
                return;
            }

            if (string.IsNullOrEmpty(this.UrlFormat) && this.Click == null)
            {
                return; // plain text cell
            }

            this.linkButton = new LinkButton();
            cell.Controls.Add(this.linkButton);

            if (this.Click != null)
            {
                this.linkButton.Click += this.DataCell_Click;
            }

            this.valueHidden = new HtmlInputHidden();
            cell.Controls.Add(this.valueHidden);

            this.urlHidden = new HtmlInputHidden();
            cell.Controls.Add(this.urlHidden);
        }

        protected virtual void DataCell_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.displayValue) && this.valueHidden != null)
            {
                this.displayValue = HttpContext.Current.Request.Form[this.valueHidden.UniqueID];
                this.navigateUrl = HttpContext.Current.Request.Form[this.urlHidden.UniqueID];
            }

            if (this.linkButton != null)
            {
                this.linkButton.Text = this.displayValue;
                this.linkButton.PostBackUrl = this.navigateUrl;
            }
        }

        protected virtual void DataCell_PreRender(object sender, EventArgs e)
        {
            if (this.valueHidden != null)
            {
                this.valueHidden.Value = this.displayValue;
                this.urlHidden.Value = this.navigateUrl;
            }
        }

        /// <summary>
        /// Sets the cell text to the value of the field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected virtual void DataCell_DataBinding(object sender, EventArgs eventArgs)
        {
            var cell = (TableCell)sender;

            if (this.ItemTemplate != null)
            {
                return;
            }

            var dataItem = DataBinder.GetDataItem(cell.NamingContainer);

            if (string.IsNullOrEmpty(this.UrlFormat) && this.Click == null)
            {
                cell.Text = this.FormatValue(this.Eval(dataItem, this.Name));
            }
            else
            {
                this.displayValue = this.FormatValue(this.Eval(dataItem, this.Name));
                this.linkButton.Text = this.displayValue;

                if (this.Click == null)
                {
                    this.navigateUrl = this.UrlFields == null
                        ? this.UrlFormat
                        : string.Format(this.UrlFormat,
                            this.UrlFields.Select(x => this.Eval(dataItem, x)).ToArray());

                    this.linkButton.PostBackUrl = this.navigateUrl;
                }
            }
        }

        protected virtual void DataCell_Click(object sender, EventArgs e)
        {
            var handler = this.Click;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion DataCell Event Handlers

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

        protected string FormatValue(object val)
        {
            return this.FormatDelegate == null ? StringUtil.ToString(val, this.Format) : this.FormatDelegate(val);
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