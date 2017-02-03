using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Xunit;
using QueryTables.Common;
using QueryTables.Common.Extension;
using QueryTables.Common.Util;
using QueryTables.Core;
using QueryTables.Test.Model;
using TestContext = QueryTables.Test.Model.TestContext;

namespace QueryTables.Test
{
    public class QueryTest
    {
        [Fact]
        public void ProjectResultingTypeEF()
        {
            var query = new Query<Empleado>();

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

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("true");

            List<object> result;
            using (var db = new TestContext(optionsBuilder.Options))
            {
                db.Empleados.Add(new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero, FechaNacimiento = DateTime.Today });
                db.Empleados.Add(new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado, FechaNacimiento = DateTime.Today });
                db.Empleados.Add(new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero, FechaNacimiento = DateTime.Today });
                db.SaveChanges();

                // Test target method
                var queryable = query.Project(db.Empleados);
                result = Enumerable.Cast<object>(queryable).ToList();
            }
            
            // Assertions for the target method
            var fields = result[0].GetType().GetTypeInfo().GetType().GetProperties();

            Assert.Equal("FullName", fields[0].Name);
            Assert.Equal(typeof(string), fields[0].PropertyType);
            Assert.Equal("Dni", fields[1].Name);
            Assert.Equal(typeof(int), fields[1].PropertyType);
        }

        [Fact]
        public void ProjectResultingProjectionEF()
        {
            // Init test vars
            var query = new Query<Empleado>();

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

            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseInMemoryDatabase("true");

            List<dynamic> result;
            using (var db = new TestContext(optionsBuilder.Options))
            {
                db.Empleados.Add(new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero, FechaNacimiento = DateTime.Today });
                db.Empleados.Add(new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado, FechaNacimiento = DateTime.Today });
                db.Empleados.Add(new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero, FechaNacimiento = DateTime.Today });
                db.SaveChanges();

                // Test target method
                var queryable = query.Project(db.Empleados);
                result = queryable.ToDynamic();
            }

            // Assertions for the target method
            Assert.Equal("Andres Chort", result[0].FullName);
            Assert.Equal(31333555, result[0].Dni);
        }
    }
}
