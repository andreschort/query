namespace Query.SampleModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addattachment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AttachmentItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        Location_Id = c.Int(nullable: false),
                        Attachment_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.Attachment_Id)
                .Index(t => t.Attachment_Id);
            
            AddColumn("dbo.Empleadoes", "Attachment_Id", c => c.Int());
            AddForeignKey("dbo.Empleadoes", "Attachment_Id", "dbo.Attachments", "Id");
            CreateIndex("dbo.Empleadoes", "Attachment_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AttachmentItems", new[] { "Attachment_Id" });
            DropIndex("dbo.Empleadoes", new[] { "Attachment_Id" });
            DropForeignKey("dbo.AttachmentItems", "Attachment_Id", "dbo.Attachments");
            DropForeignKey("dbo.Empleadoes", "Attachment_Id", "dbo.Attachments");
            DropColumn("dbo.Empleadoes", "Attachment_Id");
            DropTable("dbo.AttachmentItems");
            DropTable("dbo.Attachments");
        }
    }
}
