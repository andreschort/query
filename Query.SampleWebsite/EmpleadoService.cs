using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common.Extension;
using Query.SampleModel;

namespace Query.SampleWebSite
{
    public class EmpleadoService
    {
        public DataTable GetAll(Dictionary<string, string> values, int maximumRows, int startRowIndex)
        {
            var fieldBuilder = new QueryFieldBuilder<Empleado>();
            
            var query = new Query<Empleado>();

            query.Fields.Add(fieldBuilder.Create(x => x.Nombre).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Apellido).Instance);
            query.Fields.Add(fieldBuilder.Create(x => x.Dni).Instance);
            query.Fields.Add(
                fieldBuilder.Create("EstadoCivil")
                            .Select(x => x.EstadoCivil_Id.Equals((int) EstadoCivil.Soltero)
                                             ? "Soltero"
                                             : x.EstadoCivil_Id.Equals((int) EstadoCivil.Casado)
                                                   ? "Casado"
                                                   : x.EstadoCivil_Id.Equals((int) EstadoCivil.Separado)
                                                         ? "Separado"
                                                         : x.EstadoCivil_Id.Equals((int) EstadoCivil.Divorciado)
                                                               ? "Divorciado"
                                                               : x.EstadoCivil_Id.Equals((int) EstadoCivil.Viudo)
                                                                     ? "Viudo"
                                                                     : string.Empty)
                            .FilterAs(FilterType.List)
                            .Where(x => x.EstadoCivil_Id)
                            .Instance);

            var empleados = new List<Empleado>
                {
                    new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero},
                    new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado},
                    new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero},
                };

            return query.Apply(empleados.AsQueryable(), values).ToDataTable();
        }

        public int GetCount(Dictionary<string, string> filters, int maximumRows, int startRowIndex)
        {
            return this.GetAll(filters, maximumRows, startRowIndex).Rows.Count;
        }
    }
}