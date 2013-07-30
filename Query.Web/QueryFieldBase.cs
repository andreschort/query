﻿using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Query.Web
{
    public abstract class QueryFieldBase : DataControlField
    {
        public string Name { get; set; }
        public abstract string Value { get; set; }
        public bool Focus { get; set; }
        public short? TabIndex { get; set; }

        [Bindable(true)]
        public string DataField { get; set; }

        public string FilterPlaceholder { get; set; }
        public int? AutoFilterDelay { get; set; }
        public string FilterCommand { get; set; }
        public string SortCommand { get; set; }

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

        protected abstract void InitDataCell(DataControlFieldCell cell, DataControlRowState rowState);
        protected abstract void InitHeaderCell(DataControlFieldCell cell);
        protected abstract void HeaderCell_DataBinding(object sender, EventArgs e);

        protected virtual void DataCell_DataBinding(object sender, EventArgs eventArgs)
        {
            TableCell cell = sender as TableCell;
            object dataItem = DataBinder.GetDataItem(cell.NamingContainer);

            var view = dataItem as DataRowView;
            cell.Text = view == null
                            ? DataBinder.GetPropertyValue(dataItem, this.DataField, null)
                            : view.Row[this.DataField].ToString();
        }
    }
}