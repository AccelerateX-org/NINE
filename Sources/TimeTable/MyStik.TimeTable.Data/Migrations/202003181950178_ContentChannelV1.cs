namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContentChannelV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContentChannel",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        TokenName = c.String(),
                        Token = c.String(),
                        AccessUrl = c.String(),
                        ParticipientsOnly = c.Boolean(nullable: false),
                        Activity_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Activity_Id)
                .Index(t => t.Activity_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContentChannel", "Activity_Id", "dbo.Activity");
            DropIndex("dbo.ContentChannel", new[] { "Activity_Id" });
            DropTable("dbo.ContentChannel");
        }
    }
}
