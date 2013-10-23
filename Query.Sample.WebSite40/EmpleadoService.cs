using System;
using System.Collections.Generic;
using System.Linq;
using Query.Common;
using Query.Sample.Model;
using Query.Core;

namespace Query.Sample.WebSite40
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
        public List<object> GetAll(
            Dictionary<string, string> filters,
            List<KeyValuePair<string, SortDirection>> sortings,
            int maximumRows,
            int startRowIndex)
        {
            var query = new Query<Empleado>();

            // Simple text and integer fields. The field names come from the select expression.
            query.AddField(x => x.Nombre).AddWhere(x => x.Apellido); // Field name = "Nombre"
            query.AddField(x => x.Apellido).AddWhere(x => x.Nombre).CaseSensitive(); // Field name = "Apellido" - case sensitive
            query.AddField(x => x.Dni);

            // A field that concatenates two properties.
            query.AddField("FullName").Select(x => x.Nombre + " " + x.Apellido);

            // Date filter with truncated time.
            query.AddField(x => x.FechaNacimiento).TruncateTime();

            // List filter targeting an enum
            query.AddField("EstadoCivil")
                .Select(x => x.EstadoCivil_Id) // enums in EF5 for .NET 4.0
                // since the select targets an int we need the following to force a list filter instead of an integer filter.
                .FilterAs(FilterType.List)
                // Return constants for specific values of the target, not necesary but allows us to translates the enum values that will be shown.
                .SelectWhen(this.GetEstadoCivilTranslations(), string.Empty)
                .FilterWhen(10, x => x.EstadoCivil_Id.Equals((int)EstadoCivil.Casado) || // custom value filter
                                       x.EstadoCivil_Id.Equals((int)EstadoCivil.Separado));

            query.AddField(x => x.Edad);
            query.AddField(x => x.Salario);
            query.AddField("AttachmentCount").Select(x => x.Attachment.Items.Count(item => !item.Deleted && item.Location_Id.Equals((int)AttachmentLocation.Creation)));
            query.AddField(x => x.Cuit);
            query.AddField(x => x.AverageHourlyWage);

            using (var db = new SampleContext())
            {
                var empleados = (IQueryable<Empleado>)db.Set<Empleado>();

                // Filtering
                empleados = query.Filter(empleados, filters);

                // Sorting and pagination
                if (maximumRows > 0)
                {
                    empleados = query.OrderBy(empleados, sortings, x => x.Id); // x => x.Id acts as default sort when sortings is empty, required to use Skip and Take on EF
                    empleados = empleados.Skip(startRowIndex).Take(maximumRows);
                }

                // Projection. theResult is an untyped IQueryable. The ElementType is an annonymous type based in the defined fields
                var theResult = query.Project(empleados);
                
                // Calling ToList on the final IQueryable so the SQL query is performed by EF.
                return Enumerable.Cast<object>(theResult).ToList();
            }
        }

        public int GetCount(
            Dictionary<string, string> filters,
            List<KeyValuePair<string, SortDirection>> sortings)
        {
            return this.GetAll(filters, null, 0, 0).Count;
        }

        private List<Empleado> GetEmpleadosMock()
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

        private void CreateEmpleados()
        {
            using (var db = new SampleContext())
            {
                db.Empleados.Add(this.GetEmpleadosMock()[0]);
                db.Empleados.Add(this.GetEmpleadosMock()[1]);
                db.Empleados.Add(this.GetEmpleadosMock()[2]);
                db.SaveChanges();
            }
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
