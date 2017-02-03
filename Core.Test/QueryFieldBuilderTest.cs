using System.Collections.Generic;
using Xunit;
using QueryTables.Core;
using QueryTables.Core.Filters.Builders;
using QueryTables.Test.Model;

namespace QueryTables.Test
{
    public class QueryFieldBuilderTest
    {
        [Fact]
        public void CreateWithName()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Name");

            Assert.Equal("Name", builder.Instance.Name);
        }

        [Fact]
        public void CreateWithExpression()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.Apellido);

            Assert.Equal("Apellido", builder.Instance.Name);
            Assert.Equal(typeof(string), builder.Instance.Select.ReturnType);
            Assert.Equal(typeof(TextFilterBuilder), builder.Instance.FilterBuilder.GetType());
        }

        [Fact]
        public void CreateWithExpressionNullableInt()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.Cuit);

            Assert.Equal("Cuit", builder.Instance.Name);
            Assert.Equal(typeof(int?), builder.Instance.Select.ReturnType);
            Assert.Equal(typeof(NumericFilterBuilder), builder.Instance.FilterBuilder.GetType());
        }

        [Fact]
        public void Select()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Campo").Select(x => x.Apellido);
            
            Assert.Equal("Campo", builder.Instance.Name);
            Assert.Equal(typeof(string), builder.Instance.Select.ReturnType);
            Assert.Equal(1, builder.Instance.Where.Count);
            Assert.Equal(typeof(string), builder.Instance.Where[0].ReturnType);
            Assert.Equal(typeof(TextFilterBuilder), builder.Instance.FilterBuilder.GetType());
        }

        [Fact]
        public void WhereAfterSelect()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Campo").Select(x => x.EstadoCivil).Where(x => x.EstadoCivil_Id);
            
            Assert.Equal("Campo", builder.Instance.Name);
            Assert.Equal(typeof(EstadoCivil), builder.Instance.Select.ReturnType);
            Assert.Equal(1, builder.Instance.Where.Count);
            Assert.Equal(typeof(int), builder.Instance.Where[0].ReturnType);
            Assert.Equal(typeof(NumericFilterBuilder), builder.Instance.FilterBuilder.GetType());
        }

        [Fact]
        public void WhereBeforeSelect()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create("Campo").Where(x => x.EstadoCivil_Id).Select(x => x.EstadoCivil);
            
            Assert.Equal("Campo", builder.Instance.Name);
            Assert.Equal(typeof(EstadoCivil), builder.Instance.Select.ReturnType);
            Assert.Equal(1, builder.Instance.Where.Count);
            Assert.Equal(typeof(int), builder.Instance.Where[0].ReturnType);
            Assert.Equal(typeof(NumericFilterBuilder), builder.Instance.FilterBuilder.GetType());
        }

        [Fact]
        public void SelectWhen()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id)
                   .SelectWhen(EstadoCivil.Separado, "Separado");

            Assert.Equal(1, builder.Instance.SelectWhen.Count);
            Assert.Equal("Separado", builder.Instance.SelectWhen[EstadoCivil.Separado]);
        }

        [Fact]
        public void SelectElse()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id).SelectElse("Separado");

            Assert.Equal("Separado", builder.Instance.SelectElse);
        }

        [Fact]
        public void SelectWhenDictionary()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id).SelectWhen(this.GetEstadoCivilTranslations(), string.Empty);

            Assert.Equal(this.GetEstadoCivilTranslations().Count, builder.Instance.SelectWhen.Count);
        }
        
        [Fact]
        public void CaseSensitive()
        {
            var builder = new QueryFieldBuilder<Empleado>();

            builder.Create(x => x.EstadoCivil_Id).CaseSensitive();

            Assert.Equal(true, builder.Instance.CaseSensitive);
        }

        private Dictionary<object, object> GetEstadoCivilTranslations()
        {
            return new Dictionary<object, object>
                {
                    { EstadoCivil.Soltero, "Soltero" },
                    { EstadoCivil.Casado, "Casado" },
                    { EstadoCivil.Separado, "Separado" },
                    { EstadoCivil.Divorciado, "Divorciado" },
                    { EstadoCivil.Viudo, "Viudo" }
                };
        }
    }
}