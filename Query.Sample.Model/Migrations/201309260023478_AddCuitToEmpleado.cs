namespace Query.Sample.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCuitToEmpleado : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Empleadoes", "Cuit", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Empleadoes", "Cuit");
        }
    }
}
