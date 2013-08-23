using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            List<KeyValuePair<string, SortDirection>> sortings,
            int maximumRows,
            int startRowIndex)
        {
            var fieldBuilder = new QueryFieldBuilder<Empleado>();
            
            var query = new Query<Empleado>();

            // Simple text and integer fields. The field names come from the select expression.
            query.Fields.Add(fieldBuilder.Create(x => x.Nombre).Instance); // Field name = "Nombre"
            query.Fields.Add(fieldBuilder.Create(x => x.Apellido).Instance); // Field name = "Apellido"
            query.Fields.Add(fieldBuilder.Create(x => x.Dni).Instance);

            // Date filter with truncated time. We need to specify the name of the field because we can not get it from the select expression.
            query.Fields.Add(fieldBuilder.Create("FechaNacimiento").Select(x => x.FechaNacimiento.Date).Instance);

            // List filter targeting an enum
            query.Fields.Add(fieldBuilder.Create("EstadoCivil")
                .Select(x => x.EstadoCivil_Id)  // enums in EF5 for .NET 4.0
                .FilterAs(FilterType.List)      // since the select targets an int we need this to force a list filter instead of an integer filter.
                // Return constants for specific values of the target, not necesary but allows us to translates the enum values that will be shown.
                .SelectWhen(this.GetEstadoCivilTranslations(), string.Empty)
                .Instance);

            query.Fields.Add(fieldBuilder.Create(x => x.Edad).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Salario).Instance);

            var empleados = this.GetEmpleados().AsQueryable();

            empleados = query.Filter(empleados, filters);
            empleados = query.OrderBy(empleados, sortings);

            if (maximumRows > 0)
            {
                empleados = empleados.Skip(startRowIndex).Take(maximumRows);
            }

            var projection = query.Project(empleados);

            return projection.ToDataTable();
        }

        public int GetCount(
            Dictionary<string, string> filters,
            List<KeyValuePair<string, SortDirection>> sortings)
        {
            return this.GetAll(filters, null, 0, 0).Rows.Count;
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

        private Dictionary<object, object> GetEstadoCivilTranslations()
        {
            return new Dictionary<object, object>
                {
                    {(int)EstadoCivil.Soltero, "Soltero"},
                    {(int)EstadoCivil.Casado, "Casado"},
                    {(int)EstadoCivil.Separado, "Separado"},
                    {(int)EstadoCivil.Divorciado, "Divorciado"},
                    {(int)EstadoCivil.Viudo, "Viudo"}
                };
        }
    }
}