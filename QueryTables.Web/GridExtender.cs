using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SortDirection = QueryTables.Common.SortDirection;

namespace QueryTables.Web
{
    public class GridExtender : WebControl, IPostBackEventHandler
    {
        public const string FilterCommand = "Filter";
        public const string SortCommand = "SortIt";

        private bool enableFilters = true;

        /// <summary>
        /// Filters that do not have a field, commonnly set by the client.
        /// These filters will remain until the Filter event is triggered.
        /// </summary>
        private Dictionary<string, string> additionalFilters;

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

            this.additionalFilters = null;
        }

        public void ClearFilters()
        {
            foreach (var field in this.Grid.Columns.OfType<QueryFieldBase>())
            {
                field.FilterValue = string.Empty;
            }

            this.additionalFilters = null;
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith(FilterCommand))
            {
                this.additionalFilters = null;
                this.RaiseFilter();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Grid.Load += this.Grid_Load;
            this.Grid.RowCommand += this.Grid_RowCommand;
            
            // Force that the hidden input __LASTFOCUS is rendered
            this.Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this) { TrackFocus = true });

            this.Page.RegisterRequiresControlState(this);
        }

        protected override void LoadControlState(object savedState)
        {
            var result = (Pair)savedState;

            this.additionalFilters = (Dictionary<string, string>)result.Second;

            base.LoadControlState(result.First);
        }

        protected override object SaveControlState()
        {
            var saveControlState = base.SaveControlState();

            var result = new Pair(saveControlState, this.additionalFilters);

            return result;
        }

        private void Grid_Load(object sender, EventArgs e)
        {
            this.Grid.CssClass += " QueryTables";

            // Set fields parameters
            short tabIndex = 1;
            foreach (var field in this.Grid.Columns.OfType<QueryFieldBase>())
            {
                field.EnableFilter = field.EnableFilter ?? this.EnableFilters;
                field.AutoFilterDelay = field.AutoFilterDelay ?? this.AutoFilterDelay;
                field.Placeholder = field.Placeholder ?? this.Placeholder;
                field.PostbackName = this.UniqueID;
                field.FilterCommand = FilterCommand + "$" + field.Name;
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
            if (e.CommandName.Equals(SortCommand))
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

        private void RaiseFilter()
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

        private void SetFilters(Dictionary<string, string> values)
        {
            var fields = this.Grid.Columns.OfType<QueryFieldBase>().ToList();

            var additionalFilters = new Dictionary<string, string>();

            foreach (var filter in values)
            {
                var field = fields.FirstOrDefault(x => x.Name.Equals(filter.Key));

                if (field == null)
                {
                    additionalFilters.Add(filter.Key, filter.Value);
                }
                else
                {
                    field.FilterValue = filter.Value;
                }
            }

            if (additionalFilters.Any())
            {
                this.additionalFilters = additionalFilters;
            }
        }

        private Dictionary<string, string> GetFilters()
        {
            var dictionary = this.Grid.Columns.OfType<QueryFieldBase>()
                                 .ToDictionary(field => field.Name, field => field.FilterValue);

            if (this.additionalFilters != null)
            {
                foreach (var additionalFilter in this.additionalFilters)
                {
                    dictionary.Add(additionalFilter.Key, additionalFilter.Value);
                }
            }

            return dictionary;
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
