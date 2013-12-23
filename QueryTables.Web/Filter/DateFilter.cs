using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;
using AjaxControlToolkit;

namespace QueryTables.Web.Filter
{
    public class DateFilter : TextFilter
    {
        private CalendarExtender calendarExtender;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // calendars
            this.calendarExtender = new CalendarExtender
            {
                ID = this.ID + "_calendar",
                TargetControlID = this.ID,
            };

            this.Parent.Controls.Add(this.calendarExtender);
            this.calendarExtender.Format = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern;
        }

        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            var references = new List<ScriptReference>();

            references.AddRange(base.GetScriptReferences());

            references.Add(new ScriptReference("QueryTables.Web.Filter.DateFilter.js", "QueryTables.Web"));

            return references;
        }

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptor = new ScriptControlDescriptor("QueryTables.Web.DateFilter", this.ClientID);
            
            this.GetScriptDescriptor(descriptor);

            descriptor.AddProperty("calendarExtenderId", this.calendarExtender.ClientID);

            return new List<ScriptDescriptor> { descriptor };
        }
    }
}
