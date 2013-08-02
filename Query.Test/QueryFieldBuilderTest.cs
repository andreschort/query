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
            Assert.AreEqual(FilterType.Text, builder.Instance.FilterType);
        }

        [TestMethod]
        public void Select()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Campo").Select(x => x.Apellido);
            
            Assert.AreEqual("Campo", builder.Instance.Name);
            Assert.AreEqual(typeof(string), builder.Instance.Select.ReturnType);
            Assert.AreEqual(1, builder.Instance.Where.Count);
            Assert.AreEqual(typeof(string), builder.Instance.Where[0].ReturnType);
            Assert.AreEqual(FilterType.Text, builder.Instance.FilterType);
        }

        [TestMethod]
        public void WhereAfterSelect()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Campo").Select(x => x.EstadoCivil).Where(x => x.EstadoCivil_Id);
            
            Assert.AreEqual("Campo", builder.Instance.Name);
            Assert.AreEqual(typeof(EstadoCivil), builder.Instance.Select.ReturnType);
            Assert.AreEqual(1, builder.Instance.Where.Count);
            Assert.AreEqual(typeof(int), builder.Instance.Where[0].ReturnType);
            Assert.AreEqual(FilterType.Integer, builder.Instance.FilterType);
        }

        [TestMethod]
        public void WhereBeforeSelect()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Campo").Where(x => x.EstadoCivil_Id).Select(x => x.EstadoCivil);
            
            Assert.AreEqual("Campo", builder.Instance.Name);
            Assert.AreEqual(typeof(EstadoCivil), builder.Instance.Select.ReturnType);
            Assert.AreEqual(1, builder.Instance.Where.Count);
            Assert.AreEqual(typeof(int), builder.Instance.Where[0].ReturnType);
            Assert.AreEqual(FilterType.Integer, builder.Instance.FilterType);
        }

        [TestMethod]
        public void SelectWhen()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id)
                   .SelectWhen(EstadoCivil.Separado, "Separado");

            Assert.AreEqual(1, builder.Instance.SelectWhen.Count);
            Assert.AreEqual("Separado", builder.Instance.SelectWhen[EstadoCivil.Separado]);
        }

        [TestMethod]
        public void SelectElse()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id).SelectElse("Separado");

            Assert.AreEqual(1, builder.Instance.SelectWhen.Count);
            Assert.AreEqual("Separado", builder.Instance.SelectWhen[EstadoCivil.Separado]);
        }
    }
}