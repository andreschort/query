﻿using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Query.Web
{
    public abstract class QueryFieldBase : DataControlField
    {
        #region Fields

        protected LinkButton FilterButton;
        
        private LinkButton sortButton;
        private HtmlInputHidden sortInputHidden;
        
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

        public SortDirection? SortDirection { get; set; }

        public int SortOrder { get; set; }

        [TypeConverterAttribute(typeof(StringArrayConverter))]
        public string[] UrlFields { get; set; }

        public string UrlFormat { get; set; }

        #endregion Published Properties

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
            this.sortButton = new LinkButton
            {
                Text = this.HeaderText,
                CommandName = this.SortCommand,
                CommandArgument = this.Name
            };

            this.sortInputHidden = new HtmlInputHidden();

            cell.Controls.Add(this.sortButton);
            cell.Controls.Add(this.sortInputHidden);

            // Filter button
            this.FilterButton = new LinkButton { CommandName = this.FilterCommand, CommandArgument = this.Name };
            this.FilterButton.Attributes["style"] = "display:none";
            cell.Controls.Add(this.FilterButton);
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

        private SortDirection? GetSorting()
        {
            var sortInputUniqueId = this.sortInputHidden == null ? string.Empty : this.sortInputHidden.UniqueID;
            var form = HttpContext.Current.Request.Form;

            if (string.IsNullOrEmpty(sortInputUniqueId) || !form.AllKeys.Contains(sortInputUniqueId))
            {
                return null;
            }

            var s = form[sortInputUniqueId];

            SortDirection direction;
            SortDirection? newDirection = null;
            if (Enum.TryParse(s, out direction))
            {
                newDirection = direction;
            }

            var eventTarget = HttpContext.Current.Request.Form["__EVENTTARGET"];
            if (this.sortButton.UniqueID.Equals(eventTarget))
            {
                if (newDirection.Equals(System.Web.UI.WebControls.SortDirection.Ascending))
                {
                    newDirection = System.Web.UI.WebControls.SortDirection.Descending;
                }
                else if (newDirection.Equals(System.Web.UI.WebControls.SortDirection.Descending))
                {
                    newDirection = null;
                }
                else
                {
                    newDirection = System.Web.UI.WebControls.SortDirection.Ascending;
                }
            }

            return newDirection;
        }
    }
}