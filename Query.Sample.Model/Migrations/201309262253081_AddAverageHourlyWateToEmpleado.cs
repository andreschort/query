namespace Query.Sample.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAverageHourlyWateToEmpleado : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Empleadoes", "AverageHourlyWage", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Empleadoes", "AverageHourlyWage");
        }
    }
}
