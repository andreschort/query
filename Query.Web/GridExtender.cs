using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Common.Web.Util;

namespace Query.Web
{
    public class GridExtender : WebControl
    {
        public const string FilterCommand = "Filter";
        public const string SortCommand = "Sort";

        #region Published Properties

        private bool enableFilters = true;

        public string GridViewId { get; set; }
        
        public Dictionary<string, string> Filters
        {
            get { return this.GetFilters(); }
            set { this.SetFilters(value); }
        }

        public Dictionary<string, SortDirection> Sortings
        {
            get { return this.GetSortings(); }
            set { this.SetSortings(value); }
        }

        public bool EnableFilters
        {
            get { return this.enableFilters; }
            set { this.enableFilters = value; }
        }

        public int? AutoFilterDelay { get; set; }

        #endregion Published Properties

        #region Events

        public event EventHandler Filter;
        public event EventHandler Sort;
        
        #endregion Events

        #region Internal Properties

        private GridView Grid
        {
            get { return (GridView)this.NamingContainer.FindControl(this.GridViewId); }
        }

        #endregion Internal Properties

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Grid.Init += this.Grid_Init;
            this.Grid.RowCommand += this.Grid_RowCommand;

            var webResourceUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Query.Web.Query.js");
            this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "Query.js", webResourceUrl);

            var javascript = @"$(document).ready(function () {
                                $('.data-query-textFilter').each(function (index, element) {
                                    initTextFilter($(this));
                                });
                                $('.data-query-datepicker').each(function (index, element) {
                                    initDateFilter($(this));
                                });
                                $('.data-query-dropdown').each(function (index, element) {
                                    initDropDownFilter($(this));
                                });
                            });";
            JSUtil.AddLoad(this, "GridExtender", javascript);

            // Force that the hidden input __LASTFOCUS is rendered
            this.Page.ClientScript.GetPostBackEventReference(new System.Web.UI.PostBackOptions(this) { TrackFocus = true });
        }

        private void Grid_Init(object sender, EventArgs e)
        {
            // Set fields parameters
            short tabIndex = 1;
            foreach (var field in this.Grid.Columns.OfType<QueryFieldBase>())
            {
                field.AutoFilterDelay = this.AutoFilterDelay;
                field.FilterCommand = FilterCommand;
                field.SortCommand = SortCommand;
                tabIndex = field.SetTabIndex(tabIndex);
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
                fields.First(x => x.Name.Equals(filter.Key)).Value = filter.Value;
            }
        }

        private Dictionary<string, string> GetFilters()
        {
            return this.Grid.Columns.OfType<QueryFieldBase>().ToDictionary(field => field.Name, field => field.Value);
        }

        private void SetSortings(Dictionary<string, SortDirection> sortings)
        {
            var fields = this.Grid.Columns.OfType<QueryFieldBase>().ToList();

            foreach (var sorting in sortings)
            {
                fields.First(x => x.Name.Equals(sorting.Key)).Sorting = sorting.Value;
            }
        }

        private Dictionary<string, SortDirection> GetSortings()
        {
            return this.Grid.Columns.OfType<QueryFieldBase>()
                       .Where(x => x.Sorting.HasValue)
                       .ToDictionary(field => field.Name, field => field.Sorting.Value);
        }
    }
}
