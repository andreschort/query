using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Common.Extension;
using Query.SampleModel;

namespace Query.SampleWebSite
{
    public class EmpleadoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters">Key=field name, Value=filter value</param>
        /// <param name="sortings">Key=field name, Value=(SortOrder, SortDirection)</param>
        /// <param name="maximumRows"></param>
        /// <param name="startRowIndex"></param>
        /// <returns></returns>
        public DataTable GetAll(
            Dictionary<string, string> filters,
            Dictionary<string, Tuple<int, SortDirection>> sortings,
            int maximumRows,
            int startRowIndex)
        {
            var fieldBuilder = new QueryFieldBuilder<Empleado>();
            
            var query = new Query<Empleado>();

            // Simple text and integer fields. The field names come from the select expression.
            query.Fields.Add(fieldBuilder.Create(x => x.Nombre).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Apellido).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Dni).Instance);

            // Date filter with truncated time. We need to specify the name of the field because of the select expression.
            query.Fields.Add(fieldBuilder.Create("FechaNacimiento").Select(x => x.FechaNacimiento.Date).Instance);

            // List filter targeting an enum
            query.Fields.Add(fieldBuilder.Create("EstadoCivil")
                .Select(x => x.EstadoCivil_Id)  // enums in EF5 for .NET 4.0
                .FilterAs(FilterType.List)      // since the select targets an int we need this to force a list filter instead of an integer filter.
                // Return constants for specific values of the target, not necesary but allows us to translates the enum values.
                .SelectWhen((int)EstadoCivil.Soltero, "Soltero")
                .SelectWhen((int)EstadoCivil.Casado, "Casado")
                .SelectWhen((int)EstadoCivil.Separado, "Separado")
                .SelectWhen((int)EstadoCivil.Divorciado, "Divorciado")
                .SelectWhen((int)EstadoCivil.Viudo, "Viudo")
                // (mandatory when using SelectWhen) if the target value is non of the above return this
                .SelectElse(string.Empty)
                .Instance);

            query.Fields.Add(fieldBuilder.Create(x => x.Edad).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Salario).Instance);

            var empleados = this.GetEmpleados();

            return query.Apply(empleados.AsQueryable(), filters).ToDataTable();
        }

        public int GetCount(Dictionary<string, string> filters, Dictionary<string, Tuple<int, SortDirection>> sortings, int maximumRows, int startRowIndex)
        {
            return this.GetAll(filters, null, maximumRows, startRowIndex).Rows.Count;
        }

        private IEnumerable<Empleado> GetEmpleados()
        {
            return new List<Empleado>
                {
                    new Empleado
                        {
                            Nombre = "Andres",
                            Apellido = "Chort",
                            Dni = 31333555,
                            EstadoCivil = EstadoCivil.Soltero,
                            Edad = 29,
                            Salario = 150.33m,
                            FechaNacimiento = DateTime.Today
                        },
                    new Empleado
                        {
                            Nombre = "Matias",
                            Apellido = "Gieco",
                            Dni = 28444555,
                            EstadoCivil = EstadoCivil.Casado,
                            Edad = 35,
                            Salario = 200.94m,
                            FechaNacimiento = DateTime.Today.AddDays(1)
                        },
                    new Empleado
                        {
                            Nombre = "Neri",
                            Apellido = "Diaz",
                            Dni = 34123321,
                            EstadoCivil = EstadoCivil.Soltero,
                            Edad = 24,
                            Salario = 300.44m,
                            FechaNacimiento = DateTime.Today.AddDays(2)
                        },
                };
        }

    }
}