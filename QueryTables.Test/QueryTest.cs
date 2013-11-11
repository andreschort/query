using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTables.Common;
using QueryTables.Common.Extension;
using QueryTables.Common.Util;
using QueryTables.Core;
using QueryTables.Test.Model;
using TestContext = QueryTables.Test.Model.TestContext;

namespace QueryTables.Test
{
    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void ProjectResultingType()
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

            var empleados = new List<Empleado>
                {
                    new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                    new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                    new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
                };

            // Test target method
            var queryable = query.Project(empleados.AsQueryable());
            var result = queryable.Cast<object>().ToList();

            // Assertions for the target method
            Assert.AreEqual(empleados.Count, result.Count);

            var fields = result[0].GetType().GetProperties();

            Assert.AreEqual("FullName", fields[0].Name);
            Assert.AreEqual(typeof(string), fields[0].PropertyType);
            Assert.AreEqual("Dni", fields[1].Name);
            Assert.AreEqual(typeof(int), fields[1].PropertyType);
        }

        [TestMethod]
        public void ProjectResultingProjection()
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

            var empleados = new List<Empleado>
                {
                    new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                    new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                    new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
                }.AsQueryable();

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
            var query = new Query<Empleado>();

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
                    new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                    new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                    new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Divorciado },
                };

            // Test target method
            var queryable = query.Project(empleados.AsQueryable());
            List<dynamic> result = queryable.Cast<object>().ToList();

            // Assertions for the target method
            Assert.AreEqual(EstadoCivil.Soltero.ToString(), result[0].EstadoCivil);
            Assert.AreEqual(EstadoCivil.Casado.ToString(), result[1].EstadoCivil);
            Assert.AreEqual("no se", result[2].EstadoCivil);
        }

        [TestMethod]
        public void OrderBy()
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

            var empleados = new List<Empleado>
                {
                    new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                    new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                    new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
                };

            var sortings = new List<KeyValuePair<string, SortDirection>>
            {
                new KeyValuePair<string, SortDirection>("FullName", SortDirection.Descending)
            };

            // Test target method
            var result = query.OrderBy(empleados.AsQueryable(), sortings).ToList();

            empleados = empleados.OrderByDescending(x => x.Nombre + x.Apellido).ToList();

            // Assertions for the target method
            Assert.AreEqual(empleados[0].Nombre, result[0].Nombre);
        }

        [TestMethod]
        public void OrderByThenBy()
        {
            // Init test vars
            var query = new Query<Empleado>();

            query.Fields.Add(new QueryField<Empleado>
            {
                Name = "Salario",
                Select = ExpressionBuilder.Build<Empleado, decimal>(x => x.Salario)
            });

            query.Fields.Add(new QueryField<Empleado>
            {
                Name = "Edad",
                Select = ExpressionBuilder.Build<Empleado, int>(x => x.Edad)
            });

            var empleados = new List<Empleado>
                {
                    new Empleado { Nombre = "Andres", Salario = 150, Edad = 29 },
                    new Empleado { Nombre = "Matias", Salario = 200, Edad = 35 },
                    new Empleado { Nombre = "Neri", Salario = 300, Edad = 24 },
                };

            var sortings = new List<KeyValuePair<string, SortDirection>>
            {
                new KeyValuePair<string, SortDirection>("Salario", SortDirection.Descending),
                new KeyValuePair<string, SortDirection>("Edad", SortDirection.Ascending),
            };

            // Test target method
            var result = query.OrderBy(empleados.AsQueryable(), sortings).ToList();

            empleados = empleados.OrderByDescending(x => x.Salario).ThenBy(x => x.Edad).ToList();

            // Assertions for the target method
            Assert.AreEqual(empleados[0].Nombre, result[0].Nombre);
        }

        [TestMethod]
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

            var connection = Effort.DbConnectionFactory.CreateTransient();

            List<object> result;
            using (var db = new TestContext(connection))
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
            var fields = result[0].GetType().GetProperties();

            Assert.AreEqual("FullName", fields[0].Name);
            Assert.AreEqual(typeof(string), fields[0].PropertyType);
            Assert.AreEqual("Dni", fields[1].Name);
            Assert.AreEqual(typeof(int), fields[1].PropertyType);
        }

        [TestMethod]
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

            var connection = Effort.DbConnectionFactory.CreateTransient();

            List<dynamic> result;
            using (var db = new TestContext(connection))
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
            Assert.AreEqual("Andres Chort", result[0].FullName);
            Assert.AreEqual(31333555, result[0].Dni);
        }

        [TestMethod]
        public void ProjectNamedType()
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

            var empleados = new List<Empleado>
                {
                    new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                    new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                    new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
                }.AsQueryable();

            // Test target method
            var result = query.Project<EmpleadoDTO>(empleados).ToList();

            // Assertions for the target method
            Assert.AreEqual("Andres Chort", result[0].FullName);
            Assert.AreEqual(31333555, result[0].Dni);
        }
    }
}
