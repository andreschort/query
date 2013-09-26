namespace Query.Sample.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Empleadoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        Dni = c.Int(nullable: false),
                        Edad = c.Int(nullable: false),
                        FechaNacimiento = c.DateTime(nullable: false),
                        Salario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstadoCivil_Id = c.Int(nullable: false),
                        Company = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                        Phone = c.String(),
                        Fax = c.String(),
                        Email = c.String(),
                        Web = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Empleadoes");
        }
    }
}
