using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using QueryTables.Web.Util;
using SortDirection = QueryTables.Common.SortDirection;

namespace QueryTables.Web
{
    public class GridExtender : WebControl
    {
        public const string FilterCommand = "Filter";
        public const string SortCommand = "SortIt";

        private bool enableFilters = true;

        #region Events

        public event EventHandler Filter;

        public event EventHandler Sort;

        #endregion Events

        #region Published Properties

        public string GridViewId { get; set; }
        
        public Dictionary<string, string> Filters
        {
            get { return this.GetFilters(); }
            set { this.SetFilters(value); }
        }

        public List<KeyValuePair<string, SortDirection>> Sortings
        {
            get { return this.GetSortings(); }
            set { this.SetSortings(value); }
        }

        public bool EnableFilters
        {
            get { return this.enableFilters; }
            set { this.enableFilters = value; }
        }

        /// <summary>
        /// The number of milliseconds to wait after the user enters some text in the filter UI control.
        /// </summary>
        public int? AutoFilterDelay { get; set; }
        
        /// <summary>
        /// The text to show when the filter is empty
        /// </summary>
        public string Placeholder { get; set; }

        public string DatePlaceholderFrom { get; set; }

        public string DatePlaceholderTo { get; set; }

        #endregion Published Properties

        #region Internal Properties

        private GridView Grid
        {
            get { return (GridView)this.NamingContainer.FindControl(this.GridViewId); }
        }

        #endregion Internal Properties

        public void ClearFiltersAndSortings()
        {
            foreach (var field in this.Grid.Columns.OfType<QueryFieldBase>())
            {
                field.FilterValue = string.Empty;
                field.SortDir = null;
                field.SortOrder = 0;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Grid.Load += this.Grid_Load;
            this.Grid.RowCommand += this.Grid_RowCommand;

            var webResourceUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "QueryTables.Web.Query.js");
            this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "Query.js", webResourceUrl);

            var javascript = @"$(document).ready(function () { Query_GridExtender_Init(); });";
            JSUtil.AddLoad(this, "GridExtender", javascript);

            // Force that the hidden input __LASTFOCUS is rendered
            this.Page.ClientScript.GetPostBackEventReference(new System.Web.UI.PostBackOptions(this) { TrackFocus = true });
        }

        private void Grid_Load(object sender, EventArgs e)
        {
            // Set fields parameters
            short tabIndex = 1;
            foreach (var field in this.Grid.Columns.OfType<QueryFieldBase>())
            {
                field.AutoFilterDelay = field.AutoFilterDelay ?? this.AutoFilterDelay;
                field.Placeholder = field.Placeholder ?? this.Placeholder;
                field.FilterCommand = FilterCommand;
                field.SortCommand = SortCommand;
                tabIndex = field.SetTabIndex(tabIndex);
            }

            foreach (var field in this.Grid.Columns.OfType<DateField>())
            {
                field.PlaceholderFrom = this.DatePlaceholderFrom;
                field.PlaceholderTo = this.DatePlaceholderTo;
            }
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals(FilterCommand))
            {
                this.RaiseFilter(sender, e);
            }
            else if (e.CommandName.Equals(SortCommand))
            {
                var fields = this.Grid.Columns.OfType<QueryFieldBase>().ToList();
                var sortingFields = fields.Where(field => field.SortDir.HasValue).ToList();

                var maxSortOrder = sortingFields.Count;
                var sortingField = fields.First(field => field.Name.Equals(e.CommandArgument));

                var sortOrder = sortingField.SortOrder;

                // move the direction to the next value
                sortingField.CycleSort(maxSortOrder + 1);

                // update greater sort orders if the field does not sort anymore
                if (!sortingField.SortDir.HasValue)
                {
                    sortingFields.ForEach(x => x.AdjustSortOrder(sortOrder));
                }

                this.RaiseSort(sender, e);
            }
        }

        private void RaiseFilter(object sender, GridViewCommandEventArgs e)
        {
            if (this.Filter != null)
            {
                this.Filter(this, new EventArgs());
            }
        }

        private void RaiseSort(object sender, GridViewCommandEventArgs e)
        {
            if (this.Sort != null)
            {
                this.Sort(this, new EventArgs());
            }
        }

        private void SetFilters(Dictionary<string, string> value)
        {
            var fields = this.Grid.Columns.OfType<QueryFieldBase>().ToList();

            foreach (var filter in value)
            {
                fields.First(x => x.Name.Equals(filter.Key)).FilterValue = filter.Value;
            }
        }

        private Dictionary<string, string> GetFilters()
        {
            return this.Grid.Columns.OfType<QueryFieldBase>().ToDictionary(field => field.Name, field => field.FilterValue);
        }

        private void SetSortings(IList<KeyValuePair<string, SortDirection>> sortings)
        {
            var fields = this.Grid.Columns.OfType<QueryFieldBase>().ToList();

            for (int index = 0; index < sortings.Count; index++)
            {
                var field = fields.First(x => x.Name.Equals(sortings[index].Key));
                field.SortOrder = index + 1;
                field.SortDir = sortings[index].Value;
            }
        }

        private List<KeyValuePair<string, SortDirection>> GetSortings()
        {
            return this.Grid.Columns.OfType<QueryFieldBase>()
                       .Where(x => x.SortDir.HasValue)
                       .OrderBy(field => field.SortOrder)
                       .Select(field => new KeyValuePair<string, SortDirection>(field.Name, field.SortDir.Value))
                       .ToList();
        }
    }
}
