using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query.SampleModel;

namespace Query.Test
{
    [TestClass]
    public class QueryFieldTest
    {
        [TestMethod]
        public void FilterSimpleInt()
        {
            QueryField<Empleado> field = new QueryField<Empleado>();
        }
    }
}
