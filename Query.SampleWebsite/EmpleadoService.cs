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
        public DataTable GetAll(Dictionary<string, string> filters, Dictionary<string, Tuple<int, SortDirection>> sortings,  int maximumRows, int startRowIndex)
        {
            var fieldBuilder = new QueryFieldBuilder<Empleado>();
            
            var query = new Query<Empleado>();

            query.Fields.Add(fieldBuilder.Create(x => x.Nombre).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Apellido).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Dni).Instance);
            query.Fields.Add(fieldBuilder.Create("FechaNacimiento").Select(x => x.FechaNacimiento.Date).Instance);
            query.Fields.Add(fieldBuilder.Create("EstadoCivil")
                .FilterAs(FilterType.List)
                .Select(x => x.EstadoCivil_Id)
                .SelectWhen((int)EstadoCivil.Soltero, "Soltero")
                .SelectWhen((int)EstadoCivil.Casado, "Casado")
                .SelectWhen((int)EstadoCivil.Separado, "Separado")
                .SelectWhen((int)EstadoCivil.Divorciado, "Divorciado")
                .SelectWhen((int)EstadoCivil.Viudo, "Viudo")
                .SelectElse(string.Empty)
                .Instance);

            query.Fields.Add(fieldBuilder.Create(x => x.Edad).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Salario).Instance);

            var empleados = new List<Empleado>
                {
                    new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero, Edad = 29, Salario = 150.33m, FechaNacimiento = DateTime.Today},
                    new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado, Edad = 35, Salario = 200.94m, FechaNacimiento = DateTime.Today.AddDays(1)},
                    new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero, Edad = 24, Salario = 300.44m, FechaNacimiento = DateTime.Today.AddDays(2)},
                };

            return query.Apply(empleados.AsQueryable(), filters).ToDataTable();
        }

        public int GetCount(Dictionary<string, string> filters, Dictionary<string, Tuple<int, SortDirection>> sortings, int maximumRows, int startRowIndex)
        {
            return this.GetAll(filters, null, maximumRows, startRowIndex).Rows.Count;
        }
    }
}