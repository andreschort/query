﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryTables.Common.Util;
using QueryTables.Core;
using QueryTables.Core.Filters;
using QueryTables.Test.Model;

namespace QueryTables.Test
{
    [TestClass]
    public class QueryFieldTest
    {
        [TestMethod]
        public void FilterNotValidFilter()
        {
            var field = new QueryField<Empleado>();
            
            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
            };

            var result = field.Filter(empleados.AsQueryable(), new Filter { Valid = false });

            Assert.AreEqual(empleados.Count, result.Count());
        }

        [TestMethod]
        public void FilterWhenCasado()
        {
            var field = new QueryField<Empleado>();
            
            field.When.Add(EstadoCivil.Casado, empleado => empleado.EstadoCivil.Equals(EstadoCivil.Casado));

            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero },
            };

            var filter = new Filter { Valid = true, Value = EstadoCivil.Casado };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(EstadoCivil.Casado, result[0].EstadoCivil);
        }

        [TestMethod]
        public void FilterWhereIntEqual()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 1 },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 2 },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 3 },
            };

            const int Dni = 1;
            var filter = new Filter { Valid = true, Value = Dni, Operator = FilterOperator.Equal };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(Dni, result[0].Dni);
        }

        [TestMethod]
        public void FilterWhereIntNotEqual()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 1 },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 2 },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 3 },
            };

            var filter = new Filter { Valid = true, Value = 1, Operator = FilterOperator.NotEqual };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].Dni);
            Assert.AreEqual(3, result[1].Dni);
        }

        [TestMethod]
        public void FilterWhereIntGreaterThan()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 1 },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 2 },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 3 },
            };

            var filter = new Filter { Valid = true, Value = 2, Operator = FilterOperator.GreaterThan };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(3, result[0].Dni);
        }

        [TestMethod]
        public void FilterWhereIntGreaterThanEqual()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 1 },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 2 },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 3 },
            };

            var filter = new Filter { Valid = true, Value = 2, Operator = FilterOperator.GreaterThanEqual };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].Dni);
            Assert.AreEqual(3, result[1].Dni);
        }

        [TestMethod]
        public void FilterWhereIntLessThan()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 1 },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 2 },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 3 },
            };

            var filter = new Filter { Valid = true, Value = 2, Operator = FilterOperator.LessThan };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Dni);
        }

        [TestMethod]
        public void FilterWhereIntLessThanEqual()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            var empleados = new List<Empleado>
            {
                new Empleado { Nombre = "Andres", Apellido = "Chort", Dni = 1 },
                new Empleado { Nombre = "Matias", Apellido = "Gieco", Dni = 2 },
                new Empleado { Nombre = "Neri", Apellido = "Diaz", Dni = 3 },
            };

            var filter = new Filter { Valid = true, Value = 2, Operator = FilterOperator.LessThanEqual };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Dni);
            Assert.AreEqual(2, result[1].Dni);
        }

        [TestMethod]
        public void FilterWhereIntBetween()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            var empleados = new List<Empleado>
            {
                new Empleado { Dni = 1 },
                new Empleado { Dni = 2 },
                new Empleado { Dni = 3 },
                new Empleado { Dni = 4 },
                new Empleado { Dni = 5 },
            };

            var filter = new Filter { Valid = true, Operator = FilterOperator.Between };
            filter.Values.Add(2);
            filter.Values.Add(4);
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(2, result[0].Dni);
            Assert.AreEqual(3, result[1].Dni);
            Assert.AreEqual(4, result[2].Dni);
        }

        [TestMethod]
        public void FilterWhereContains()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
            };

            var filter = new Filter { Valid = true, Value = "o", Operator = FilterOperator.Contains };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Chort", result[0].Apellido);
            Assert.AreEqual("Gieco", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereContainsCaseInsensitive()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
            };

            var filter = new Filter { Valid = true, Value = "O", Operator = FilterOperator.Contains };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Chort", result[0].Apellido);
            Assert.AreEqual("Gieco", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereStartsWith()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
                new Empleado { Apellido = "Dominguez", Dni = 4 },
            };

            var filter = new Filter { Valid = true, Value = "D", Operator = FilterOperator.StartsWith };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("Dominguez", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereStartsWithCaseInsensitive()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
                new Empleado { Apellido = "Dominguez", Dni = 4 },
            };

            var filter = new Filter { Valid = true, Value = "d", Operator = FilterOperator.StartsWith };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("Dominguez", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereEndsWith()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
                new Empleado { Apellido = "Dominguez", Dni = 4 },
            };

            var filter = new Filter { Valid = true, Value = "z", Operator = FilterOperator.EndsWith };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("Dominguez", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereEndsWithCaseInsensitive()
        {
            var field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
                new Empleado { Apellido = "DomingueZ", Dni = 4 },
            };

            var filter = new Filter { Valid = true, Value = "Z", Operator = FilterOperator.EndsWith };
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("DomingueZ", result[1].Apellido);
        }

        [TestMethod]
        public void FilterCaseSensitive()
        {
            var field = new QueryField<Empleado> { CaseSensitive = true };
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
                new Empleado { Apellido = "DomingueZ", Dni = 4 },
            };

            var filter = new Filter { Valid = true, Value = "Z", Operator = FilterOperator.EndsWith };

            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("DomingueZ", result[0].Apellido);
        }

        [TestMethod]
        public void FilterDate()
        {
            var field = new QueryField<Empleado>();
            field.Where.Add(ExpressionBuilder.Build<Empleado, DateTime>(x => x.FechaNacimiento.Date));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", FechaNacimiento = DateTime.Today },
                new Empleado { Apellido = "Gieco", FechaNacimiento = DateTime.Today.AddDays(-1) },
                new Empleado { Apellido = "Diaz", FechaNacimiento = DateTime.Today.AddDays(-2) },
                new Empleado { Apellido = "DomingueZ", FechaNacimiento = DateTime.Today },
            };

            var filter = new Filter
            {
                Valid = true,
                Value = DateTime.Today,
                Operator = FilterOperator.GreaterThanEqual
            };

            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(x => x.Apellido.Equals("Chort")));
            Assert.IsTrue(result.Any(x => x.Apellido.Equals("DomingueZ")));
        }

        [TestMethod]
        public void FilterDouble()
        {
            var field = new QueryField<Empleado>();
            field.Where.Add(ExpressionBuilder.Build<Empleado, double>(x => x.AverageHourlyWage));
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", AverageHourlyWage = 10 },
                new Empleado { Apellido = "Gieco", AverageHourlyWage = 20 },
                new Empleado { Apellido = "Diaz",  AverageHourlyWage = 30 },
                new Empleado { Apellido = "DomingueZ", AverageHourlyWage = 40 },
            };

            var filter = new Filter
            {
                Valid = true,
                Value = 20d,
                Operator = FilterOperator.GreaterThan
            };

            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(x => x.Apellido.Equals("Diaz")));
            Assert.IsTrue(result.Any(x => x.Apellido.Equals("DomingueZ")));
        }

        [TestMethod]
        public void FilterNullSafe()
        {
            var field = new QueryField<Empleado> { NullSafe = true };
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
                new Empleado(),
            };

            var filter = new Filter { Valid = true, Value = "o", Operator = FilterOperator.Contains };

            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Chort", result[0].Apellido);
            Assert.AreEqual("Gieco", result[1].Apellido);
        }

        [TestMethod]
        public void FilterNullSafeNotNullable()
        {
            var field = new QueryField<Empleado> { NullSafe = true };
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));
            
            var empleados = new List<Empleado>
            {
                new Empleado { Apellido = "Chort", Dni = 1 },
                new Empleado { Apellido = "Gieco", Dni = 2 },
                new Empleado { Apellido = "Diaz", Dni = 3 },
                new Empleado(),
            };

            var filter = new Filter { Valid = true, Value = 1, Operator = FilterOperator.Equal };

            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Dni);
        }
    }
}