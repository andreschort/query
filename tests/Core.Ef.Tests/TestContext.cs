using Microsoft.EntityFrameworkCore;

namespace QueryTables.Test.Model
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Empleado> Empleados { get; set; }
    }
}
