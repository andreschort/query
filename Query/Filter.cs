using System.Collections.Generic;

namespace Query
{
    public class Filter
    {
        public Filter()
        {
            this.Values = new List<object>();
        }

        #region Published Properties

        /// <summary>
        /// The name of this Filter.
        /// Used to associate a QueryField with a Filter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines if the filter is applied or not.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// The first value of the filter, in many cases the only value
        /// </summary>
        public object Value
        {
            get { return this.Values == null || this.Values.Count == 0 ? null : this.Values[0]; }
            set { this.Values = new List<object> { value }; }
        }

        /// <summary>
        /// The values of the filter, in general there will be only one value in this list
        /// in which case you can use the single property Value.
        /// For those filters that require multiple values (ie. the Between operator)
        /// this list will hold them.
        /// </summary>
        public List<object> Values { get; set; }

        /// <summary>
        /// THe operator used when comparing the filter value to the target expression
        /// </summary>
        public FilterOperator Operator { get; set; }

        /// <summary>
        /// The original text entered by the user, without any modification.
        /// </summary>
        public string OriginalText { get; set; }

        #endregion Published Properties
    }
}
