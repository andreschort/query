namespace Query.Sample.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentDeletedProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachments", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attachments", "Deleted");
        }
    }
}
