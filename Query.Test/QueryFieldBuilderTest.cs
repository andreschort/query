using System;
using System.Linq.Expressions;
using Common.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.SampleModel;

namespace Query.Test
{
    [TestClass]
    public class QueryFieldBuilderTest
    {
        [TestMethod]
        public void CreateWithName()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Name");

            Assert.AreEqual("Name", builder.Instance.Name);
        }

        [TestMethod]
        public void CreateWithExpression()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.Apellido);

            Assert.AreEqual("Apellido", builder.Instance.Name);
            Assert.AreEqual(typeof(string), builder.Instance.Select.ReturnType);
        }

        [TestMethod]
        public void Select()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Campo").Select(x => x.Apellido);
            
            Assert.AreEqual("Campo", builder.Instance.Name);
            Assert.AreEqual(typeof(string), builder.Instance.Select.ReturnType);
            Assert.AreEqual(typeof(string), builder.Instance.Where);
            Assert.AreEqual(typeof(string), builder.Instance.OrderBy);
        }
    }
}