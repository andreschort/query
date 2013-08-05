using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Query.Web
{
    public abstract class QueryFieldBase : DataControlField
    {
        public string Name { get; set; }
        
        public abstract string Value { get; set; }
        
        public short? TabIndex { get; protected set; }

        [Bindable(true)]
        public string DataField { get; set; }

        public string FilterPlaceholder { get; set; }
        public int? AutoFilterDelay { get; set; }
        public string FilterCommand { get; set; }
        public string SortCommand { get; set; }
        public SortDirection Sorting { get; set; }

        [TypeConverterAttribute(typeof(StringArrayConverter))]
        public string[] UrlFields { get; set; }

        public string UrlFormat { get; set; }

        protected LinkButton Button;

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
                this.InitHeaderCell(cell);
            }
            else if (cellType == DataControlCellType.DataCell)
            {
                cell.DataBinding += this.DataCell_DataBinding;
                this.InitDataCell(cell, rowState);
            }
        }

        public virtual short SetTabIndex(short tabIndex)
        {
            this.TabIndex = tabIndex;
            return (short)(tabIndex + 1);
        }

        protected void InitDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            // Override if you want to do something before the cell's databinding
        }

        protected virtual void InitHeaderCell(DataControlFieldCell cell)
        {
            // title with sorting
            var lnkButtonTitle = new LinkButton
            {
                Text = this.HeaderText,
                CommandName = this.SortCommand,
                CommandArgument = this.Name
            };

            cell.Controls.Add(lnkButtonTitle);

            // Filter button
            this.Button = new LinkButton { CommandName = this.FilterCommand, CommandArgument = this.Name };
            cell.Controls.Add(this.Button);
        }
        
        protected abstract void HeaderCell_DataBinding(object sender, EventArgs e);

        /// <summary>
        /// Sets the cell text to the value of the field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected virtual void DataCell_DataBinding(object sender, EventArgs eventArgs)
        {
            TableCell cell = sender as TableCell;
            object dataItem = DataBinder.GetDataItem(cell.NamingContainer);

            var displayValue = this.Eval(dataItem, this.DataField).ToString();

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
                                                             this.UrlFields.Select(x => this.Eval(dataItem, x))
                                                                 .ToArray());
            }
        }

        protected bool HasFocus(string uniqueID)
        {
            var lastFocus = HttpContext.Current.Request.Form["__LASTFOCUS"];

            return uniqueID.Equals(lastFocus);
        }

        private object Eval(object dataItem, string dataField)
        {
            var view = dataItem as DataRowView;
            return view == null
                       ? DataBinder.GetPropertyValue(dataItem, dataField, null)
                       : view.Row[dataField].ToString();
        }
    }
}