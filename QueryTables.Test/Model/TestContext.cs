using Microsoft.EntityFrameworkCore;

namespace QueryTables.Test.Model
{
    public class TestContext : DbContext
    {
        public TestContext()
        {
        }

        public TestContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {
        }

        public DbSet<Empleado> Empleados { get; set; }
    }
}
