using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.SampleModel;

namespace Query.Test
{
    [TestClass]
    public class QueryFieldTest
    {
        [TestMethod]
        public void FilterNotValidFilter()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero},
            };

            var result = field.Filter(empleados.AsQueryable(), new Filter {Valid = false});

            Assert.AreEqual(empleados.Count, result.Count());
        }

        [TestMethod]
        public void FilterWhenCasado()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.When.Add(EstadoCivil.Casado, empleado => empleado.EstadoCivil.Equals(EstadoCivil.Casado));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 31333555, EstadoCivil = EstadoCivil.Soltero},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 28444555, EstadoCivil = EstadoCivil.Casado},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 34123321, EstadoCivil = EstadoCivil.Soltero},
            };

            var filter = new Filter {Valid = true, Value = EstadoCivil.Casado};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(EstadoCivil.Casado, result[0].EstadoCivil);
        }

        [TestMethod]
        public void FilterWhereIntEqual()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 1},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 2},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 3},
            };

            const int dni = 1;
            var filter = new Filter {Valid = true, Value = dni, Operator = FilterOperator.Equal};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(dni, result[0].Dni);
        }

        [TestMethod]
        public void FilterWhereIntNotEqual()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 1},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 2},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 3},
            };

            var filter = new Filter {Valid = true, Value = 1, Operator = FilterOperator.NotEqual};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].Dni);
            Assert.AreEqual(3, result[1].Dni);
        }

        [TestMethod]
        public void FilterWhereIntGreaterThan()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 1},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 2},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 3},
            };

            var filter = new Filter {Valid = true, Value = 2, Operator = FilterOperator.GreaterThan};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(3, result[0].Dni);
        }

        [TestMethod]
        public void FilterWhereIntGreaterThanEqual()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 1},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 2},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 3},
            };

            var filter = new Filter {Valid = true, Value = 2, Operator = FilterOperator.GreaterThanEqual};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].Dni);
            Assert.AreEqual(3, result[1].Dni);
        }

        [TestMethod]
        public void FilterWhereIntLessThan()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 1},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 2},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 3},
            };

            var filter = new Filter {Valid = true, Value = 2, Operator = FilterOperator.LessThan};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Dni);
        }

        [TestMethod]
        public void FilterWhereIntLessThanEqual()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Nombre = "Andres", Apellido = "Chort", Dni = 1},
                new Empleado {Nombre = "Matias", Apellido = "Gieco", Dni = 2},
                new Empleado {Nombre = "Neri", Apellido = "Diaz", Dni = 3},
            };

            var filter = new Filter {Valid = true, Value = 2, Operator = FilterOperator.LessThanEqual};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Dni);
            Assert.AreEqual(2, result[1].Dni);
        }

        [TestMethod]
        public void FilterWhereIntBetween()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, int>(x => x.Dni));

            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Dni = 1},
                new Empleado {Dni = 2},
                new Empleado {Dni = 3},
                new Empleado {Dni = 4},
                new Empleado {Dni = 5},
            };

            var filter = new Filter {Valid = true, Operator = FilterOperator.Between};
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
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Apellido = "Chort", Dni = 1},
                new Empleado {Apellido = "Gieco", Dni = 2},
                new Empleado {Apellido = "Diaz", Dni = 3},
            };

            var filter = new Filter {Valid = true, Value = "o", Operator = FilterOperator.Contains};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Chort", result[0].Apellido);
            Assert.AreEqual("Gieco", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereContainsCaseInsensitive()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Apellido = "Chort", Dni = 1},
                new Empleado {Apellido = "Gieco", Dni = 2},
                new Empleado {Apellido = "Diaz", Dni = 3},
            };

            var filter = new Filter {Valid = true, Value = "O", Operator = FilterOperator.Contains};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Chort", result[0].Apellido);
            Assert.AreEqual("Gieco", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereStartsWith()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Apellido = "Chort", Dni = 1},
                new Empleado {Apellido = "Gieco", Dni = 2},
                new Empleado {Apellido = "Diaz", Dni = 3},
                new Empleado {Apellido = "Dominguez", Dni = 4},
            };

            var filter = new Filter {Valid = true, Value = "D", Operator = FilterOperator.StartsWith};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("Dominguez", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereStartsWithCaseInsensitive()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Apellido = "Chort", Dni = 1},
                new Empleado {Apellido = "Gieco", Dni = 2},
                new Empleado {Apellido = "Diaz", Dni = 3},
                new Empleado {Apellido = "Dominguez", Dni = 4},
            };

            var filter = new Filter {Valid = true, Value = "d", Operator = FilterOperator.StartsWith};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("Dominguez", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereEndsWith()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Apellido = "Chort", Dni = 1},
                new Empleado {Apellido = "Gieco", Dni = 2},
                new Empleado {Apellido = "Diaz", Dni = 3},
                new Empleado {Apellido = "Dominguez", Dni = 4},
            };

            var filter = new Filter {Valid = true, Value = "z", Operator = FilterOperator.EndsWith};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("Dominguez", result[1].Apellido);
        }

        [TestMethod]
        public void FilterWhereEndsWithCaseInsensitive()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
            
            field.Where.Add(ExpressionBuilder.Build<Empleado, string>(x => x.Apellido));
            List<Empleado> empleados = new List<Empleado>
            {
                new Empleado {Apellido = "Chort", Dni = 1},
                new Empleado {Apellido = "Gieco", Dni = 2},
                new Empleado {Apellido = "Diaz", Dni = 3},
                new Empleado {Apellido = "Dominguez", Dni = 4},
            };

            var filter = new Filter {Valid = true, Value = "Z", Operator = FilterOperator.EndsWith};
            
            var result = field.Filter(empleados.AsQueryable(), filter).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Diaz", result[0].Apellido);
            Assert.AreEqual("Dominguez", result[1].Apellido);
        }
    }
}