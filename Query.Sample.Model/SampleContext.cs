using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Query.SampleModel
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
