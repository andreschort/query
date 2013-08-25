using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Query.SampleModel
{
    public class SampleContext : DbContext
    {
        public DbSet<Empleado> Empleados { get; set; }
    }
}
