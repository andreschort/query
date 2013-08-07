using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.SampleModel;

namespace Query.Test
{
    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void ProjectResultingType()
        {
            Query<Empleado> query = new Query<Empleado>();

            query.Fields.Add(new QueryField<Empleado>
                {
                    Name = "FullName",
                    Select = ExpressionBuilder.Build<Empleado, string>(x => x.Nombre + " " + x.Apellido)
                });

            query.Fields.Add(new QueryField<Empleado>
                {
                    Name = "Dni",
                    Select = ExpressionBuilder.Build<Empleado, int>(x => x.Dni)
                });

            var empleados = new List<Empleado>
                {
                    new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero},
                    new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado},
                    new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero},
                };

            // Test target method
            var queryable = query.Project(empleados.AsQueryable());
            var result = queryable.Cast<object>().ToList();

            // Assertions for the target method
            Assert.AreEqual(empleados.Count, result.Count);

            var fields = result[0].GetType().GetFields();

            Assert.AreEqual("FullName", fields[0].Name);
            Assert.AreEqual(typeof(string), fields[0].FieldType);
            Assert.AreEqual("Dni", fields[1].Name);
            Assert.AreEqual(typeof(int), fields[1].FieldType);
        }

        [TestMethod]
        public void ProjectResultingProjection()
        {
            // Init test vars
            Query<Empleado> query = new Query<Empleado>();

            query.Fields.Add(new QueryField<Empleado>
            {
                Name = "FullName",
                Select = ExpressionBuilder.Build<Empleado, string>(x => x.Nombre + " " + x.Apellido)
            });

            query.Fields.Add(new QueryField<Empleado>
            {
                Name = "Dni",
                Select = ExpressionBuilder.Build<Empleado, int>(x => x.Dni)
            });

            var empleados = new List<Empleado>
                {
                    new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero},
                    new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado},
                    new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero},
                };

            // Test target method
            var queryable = query.Project(empleados.AsQueryable());
            List<dynamic> result = queryable.Cast<object>().ToList();

            // Assertions for the target method
            Assert.AreEqual("Andres Chort", result[0].FullName);
            Assert.AreEqual(31333555, result[0].Dni);
        }

        [TestMethod]
        public void ProjectWhen()
        {
            // Init test vars
            Query<Empleado> query = new Query<Empleado>();

            var field = new QueryField<Empleado>
                {
                    Name = "EstadoCivil",
                    Select = ExpressionBuilder.Build<Empleado, EstadoCivil>(x => x.EstadoCivil),
                };
            field.SelectWhen.Add(EstadoCivil.Soltero, "Soltero");
            field.SelectWhen.Add(EstadoCivil.Casado, "Casado");
            field.SelectElse = "no se";

            query.Fields.Add(field);

            var empleados = new List<Empleado>
                {
                    new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero},
                    new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado},
                    new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Divorciado},
                };

            // Test target method
            var queryable = query.Project(empleados.AsQueryable());
            List<dynamic> result = queryable.Cast<object>().ToList();

            // Assertions for the target method
            Assert.AreEqual(EstadoCivil.Soltero.ToString(), result[0].EstadoCivil);
            Assert.AreEqual(EstadoCivil.Casado.ToString(), result[1].EstadoCivil);
            Assert.AreEqual("no se", result[2].EstadoCivil);
        }

    }
}
