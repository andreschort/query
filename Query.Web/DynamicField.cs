using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SortDirection = Query.Common.SortDirection;

namespace Query.Web
{
    public class DynamicField : QueryFieldBase
    {
        private QueryFieldBase field;
        
        public FieldType? FieldType { get; set; }

        protected internal override string FilterValue
        {
            get { return this.Field == null? null : this.Field.FilterValue; }
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
        
        protected override DataControlField CreateField()
        {
            return new DynamicField();
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            this.Field.Name = this.Name;
            this.Field.FilterCommand = this.FilterCommand;
            this.Field.SortCommand = this.SortCommand;
            this.Field.HeaderText = this.HeaderText;
            this.Field.Placeholder = this.Placeholder;
            this.Field.AutoFilterDelay = this.AutoFilterDelay;
            this.Field.UrlFormat = this.UrlFormat;
            this.Field.UrlFields = this.UrlFields;

            base.InitializeCell(cell, cellType, rowState, rowIndex);
        }

        protected internal override void InitHeaderCell(DataControlFieldCell cell)
        {
            this.Field.InitHeaderCell(cell);
        }

        protected internal override void InitDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            this.Field.ItemTemplate = this.ItemTemplate;
            this.Field.InitDataCell(cell, rowState);
        }

        protected internal override void HeaderCell_Load(object sender, EventArgs e)
        {
            this.Field.HeaderCell_Load(sender, e);
        }

        protected internal override void HeaderCell_PreRender(object sender, EventArgs e)
        {
            this.Field.HeaderCell_PreRender(sender, e);
        }

        protected internal override void HeaderCell_DataBinding(object sender, EventArgs e)
        {
            this.Field.HeaderCell_DataBinding(sender, e);
        }

        protected internal override void DataCell_DataBinding(object sender, EventArgs eventArgs)
        {
            this.Field.DataCell_DataBinding(sender, eventArgs);
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
    }

    public enum FieldType
    {
        Text = 0,
        List = 1,
        Date = 2
    }
}
