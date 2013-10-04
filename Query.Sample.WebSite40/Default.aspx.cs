using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Query.Common.Extension;
using Query.Sample.Model;
using Query.Web;
using SortDirection = Query.Core.SortDirection;

namespace Query.Sample.WebSite40
{
    public partial class _Default : Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // Databind of a drop down filter. The value of the ListItem will be the value of the filter.
            var field = (DropDownField)this.GridView.Columns[4];
            field.Items = new List<ListItem>
                {
                    new ListItem("", "-1"),
                    new ListItem("Soltero", EstadoCivil.Soltero.ToOrdinalString()),
                    new ListItem("Casado", EstadoCivil.Casado.ToOrdinalString()),
                    new ListItem("Separado", EstadoCivil.Separado.ToOrdinalString()),
                    new ListItem("Casado/Separado", "10"),
                    new ListItem("Divorciado", EstadoCivil.Divorciado.ToOrdinalString()),
                    new ListItem("Viudo", EstadoCivil.Viudo.ToOrdinalString()),
                };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            // initial filters
            this.GridExtender.Filters = new Dictionary<string, string>
                {
                    {"EstadoCivil", EstadoCivil.Casado.ToOrdinalString()}
                };

            // initial sortings
            this.GridExtender.Sortings = new List<KeyValuePair<string, SortDirection>>
                {
                    new KeyValuePair<string, SortDirection>("Salario", SortDirection.Descending),
                    new KeyValuePair<string, SortDirection>("Edad", SortDirection.Ascending)
                };

            this.GridView.DataSourceID = this.OdsEmpleado.ID;
            this.GridView.DataBind();
        }

        protected void GridExtender_Filter(object sender, EventArgs e)
        {
            this.GridView.DataSourceID = this.OdsEmpleado.ID;
            this.GridView.PageIndex = 0;
            this.GridView.DataBind();
        }

        protected void GridExtender_Sort(object sender, EventArgs e)
        {
            this.GridView.DataSourceID = this.OdsEmpleado.ID;
            this.GridView.DataBind();
        }

        protected void OdsEmpleado_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = new EmpleadoService();
        }

        protected void OdsEmpleado_ObjectSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["filters"] = this.GridExtender.Filters;
            e.InputParameters["sortings"] = this.GridExtender.Sortings;
        }

        protected void OdsEmpleado_ObjectSelected(object sender, ObjectDataSourceStatusEventArgs e)
        {

        }
    }
}