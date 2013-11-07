using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.Core;
using Query.Core.Filters.Builders;
using Query.Test.Model;

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
            Assert.AreEqual(typeof(TextFilterBuilder), builder.Instance.FilterBuilder.GetType());
        }

        [TestMethod]
        public void CreateWithExpressionNullableInt()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.Cuit);

            Assert.AreEqual("Cuit", builder.Instance.Name);
            Assert.AreEqual(typeof(int?), builder.Instance.Select.ReturnType);
            Assert.AreEqual(typeof(NumericFilterBuilder), builder.Instance.FilterBuilder.GetType());
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
            Assert.AreEqual(typeof(TextFilterBuilder), builder.Instance.FilterBuilder.GetType());
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
            Assert.AreEqual(typeof(NumericFilterBuilder), builder.Instance.FilterBuilder.GetType());
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
            Assert.AreEqual(typeof(NumericFilterBuilder), builder.Instance.FilterBuilder.GetType());
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

            Assert.AreEqual("Separado", builder.Instance.SelectElse);
        }

        [TestMethod]
        public void SelectWhenDictionary()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id).SelectWhen(this.GetEstadoCivilTranslations(), string.Empty);

            Assert.AreEqual(this.GetEstadoCivilTranslations().Count, builder.Instance.SelectWhen.Count);
        }
        
        [TestMethod]
        public void CaseSensitive()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id).CaseSensitive();

            Assert.AreEqual(true, builder.Instance.CaseSensitive);
        }

        private Dictionary<object, object> GetEstadoCivilTranslations()
        {
            return new Dictionary<object, object>
                {
                    {EstadoCivil.Soltero, "Soltero"},
                    {EstadoCivil.Casado, "Casado"},
                    {EstadoCivil.Separado, "Separado"},
                    {EstadoCivil.Divorciado, "Divorciado"},
                    {EstadoCivil.Viudo, "Viudo"}
                };
        }
    }
}