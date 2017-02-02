using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTables.Common;
using QueryTables.Common.Extension;
using QueryTables.Common.Util;
using QueryTables.Core;
using QueryTables.Test.Model;

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

            var fields = result[0].GetType().GetTypeInfo().GetType().GetProperties();

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

        [TestMethod]
        public void AggregateOnTwoFields() {
            var query = new Query<Empleado>();

            query.Fields.Add(new QueryField<Empleado> {
                Name = "Uno",
                Select = ExpressionBuilder.Build<Empleado, int>(x => 1)
            });

            query.Fields.Add(new QueryField<Empleado> {
                Name = "Dos",
                Select = ExpressionBuilder.Build<Empleado, int>(x => 2)
            });

            query.Fields.Add(new QueryField<Empleado> {
                Name = "Tres",
                Select = ExpressionBuilder.Build<Empleado, int>(x => 3)
            });

            query.AggregateOn("Suma", "Uno", "Dos");

            var empleados = new List<Empleado>
                {
                    new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                    new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                    new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
                }.AsQueryable();

            // Test target method
            var queryable = query.Project<UnoDosTres>(empleados.AsQueryable());
            List<UnoDosTres> result = queryable.ToList();

            // Assertions for the target method
            Assert.AreEqual(1, result[0].Uno);
            Assert.AreEqual(2, result[0].Dos);
            Assert.AreEqual(3, result[0].Suma);
        }

        [TestMethod]
        public void AggregateOnThreeFields() {
            var query = new Query<Empleado>();

            query.Fields.Add(new QueryField<Empleado> {
                Name = "Uno",
                Select = ExpressionBuilder.Build<Empleado, int>(x => 1)
            });

            query.Fields.Add(new QueryField<Empleado> {
                Name = "Dos",
                Select = ExpressionBuilder.Build<Empleado, int>(x => 2)
            });

            query.Fields.Add(new QueryField<Empleado> {
                Name = "Tres",
                Select = ExpressionBuilder.Build<Empleado, int>(x => 3)
            });

            query.AggregateOn("Suma", "Uno", "Dos", "Tres");

            var empleados = new List<Empleado>
                {
                    new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                    new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                    new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
                }.AsQueryable();

            // Test target method
            var queryable = query.Project<UnoDosTres>(empleados.AsQueryable());
            List<UnoDosTres> result = queryable.ToList();

            // Assertions for the target method
            Assert.AreEqual(1, result[0].Uno);
            Assert.AreEqual(2, result[0].Dos);
            Assert.AreEqual(3, result[0].Tres);
            Assert.AreEqual(6, result[0].Suma);
        }
    }

    public class UnoDosTres {
        public int Uno { get; set; }
        public int Dos { get; set; }
        public int Tres { get; set; }
        public int Suma { get; set; }
    }
}