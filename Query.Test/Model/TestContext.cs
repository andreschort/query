﻿using System.Data.Common;
using System.Data.Entity;

namespace Query.Test.Model
{
    public class TestContext : DbContext
    {
        public DbSet<Empleado> Empleados { get; set; }

        public TestContext()
        {
        }

        public TestContext(DbConnection existingConnection)
            : base(existingConnection, true)
        {
        }
    }
}