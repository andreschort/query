using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SortDirection = QueryTables.Common.SortDirection;

namespace QueryTables.Web
{
    public enum FieldType
    {
        Text = 0,
        List = 1,
        Date = 2
    }

    public class DynamicField : QueryFieldBase
    {
        private QueryFieldBase field;
        
        public FieldType? FieldType { get; set; }

        public List<ListItem> Items { get; set; }

        protected internal override string FilterValue
        {
            get { return this.Field == null ? null : this.Field.FilterValue; }
            set { this.Field.FilterValue = value; }
        }

        protected internal override SortDirection? SortDir
        {
            get { return this.Field.SortDir; }
            set { this.Field.SortDir = value; }
        }

        protected internal override int SortOrder
        {
            get { return this.Field.SortOrder; }
            set { this.Field.SortOrder = value; }
        }

        private QueryFieldBase Field
        {
            get
            {
                if (this.field != null)
                {
                    return this.field;
                }

                if (!this.FieldType.HasValue)
                {
                    return null;
                }

                switch (this.FieldType.Value)
                {
                    case Web.FieldType.Text:
                        this.field = new TextField();
                        break;
                    case Web.FieldType.List:
                        this.field = new DropDownField();
                        break;
                    case Web.FieldType.Date:
                        this.field = new DateField();
                        break;
                }

                return this.field;
            }
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            this.CopyPropertiesToInnerField(this, null);

            cell.Load += this.CopyPropertiesToInnerField;

            this.Field.InitializeCell(cell, cellType, rowState, rowIndex);
        }

        protected internal override void CycleSort(int newSortOrder)
        {
            this.Field.CycleSort(newSortOrder);
        }

        protected internal override short SetTabIndex(short tabIndex)
        {
            return this.Field.SetTabIndex(tabIndex);
        }

        protected internal override void AdjustSortOrder(int removedSortOrder)
        {
            this.Field.AdjustSortOrder(removedSortOrder);
        }

        protected override DataControlField CreateField()
        {
            return new DynamicField();
        }

        private void CopyPropertiesToInnerField(object sender, EventArgs e)
        {
            this.Field.Name = this.Name;
            this.Field.PostbackName = this.PostbackName;
            this.Field.FilterCommand = this.FilterCommand;
            this.Field.SortCommand = this.SortCommand;
            this.Field.HeaderText = this.HeaderText;
            this.Field.Placeholder = this.Placeholder;
            this.Field.AutoFilterDelay = this.AutoFilterDelay;
            this.Field.UrlFormat = this.UrlFormat;
            this.Field.UrlFields = this.UrlFields;
            this.Field.ItemEnabled = this.ItemEnabled;
            this.Field.Format = this.Format;
            this.Field.FormatDelegate = this.FormatDelegate;
            this.Field.ItemTemplate = this.ItemTemplate;

            if (this.HasDataCellClick())
            {
                this.Field.Click += this.DataCell_Click;
            }

            var dropDownField = this.Field as DropDownField;

            if (dropDownField != null)
            {
                dropDownField.Items = this.Items;
            }
        }
    }
}
