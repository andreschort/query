using System.Data.Common;
using System.Data.Entity;

namespace Query.Sample.Model
{
    public class SampleContext : DbContext
    {
        public DbSet<Empleado> Empleados { get; set; }

        public SampleContext()
        {
        }

        public SampleContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {
        }
    }
}
