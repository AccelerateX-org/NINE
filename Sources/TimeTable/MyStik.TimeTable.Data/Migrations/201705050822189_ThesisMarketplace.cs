namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ThesisMarketplace : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThesisAnnouncement",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Department = c.String(),
                        Company = c.String(),
                        Provider_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ThesisProvider", t => t.Provider_Id)
                .Index(t => t.Provider_Id);
            
            CreateTable(
                "dbo.ThesisProvider",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ThesisFeedback",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Earning = c.Int(nullable: false),
                        Provider_Id = c.Guid(),
                        Workflow_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ThesisProvider", t => t.Provider_Id)
                .ForeignKey("dbo.ThesisWorkflow", t => t.Workflow_Id)
                .Index(t => t.Provider_Id)
                .Index(t => t.Workflow_Id);
            
            CreateTable(
                "dbo.ThesisWorkflow",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        Created = c.DateTime(nullable: false),
                        IsAccepted = c.Boolean(),
                        IsFinished = c.Boolean(),
                        Announcement_Id = c.Guid(),
                        Examiner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ThesisAnnouncement", t => t.Announcement_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Examiner_Id)
                .Index(t => t.Announcement_Id)
                .Index(t => t.Examiner_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThesisFeedback", "Workflow_Id", "dbo.ThesisWorkflow");
            DropForeignKey("dbo.ThesisWorkflow", "Examiner_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ThesisWorkflow", "Announcement_Id", "dbo.ThesisAnnouncement");
            DropForeignKey("dbo.ThesisFeedback", "Provider_Id", "dbo.ThesisProvider");
            DropForeignKey("dbo.ThesisAnnouncement", "Provider_Id", "dbo.ThesisProvider");
            DropIndex("dbo.ThesisWorkflow", new[] { "Examiner_Id" });
            DropIndex("dbo.ThesisWorkflow", new[] { "Announcement_Id" });
            DropIndex("dbo.ThesisFeedback", new[] { "Workflow_Id" });
            DropIndex("dbo.ThesisFeedback", new[] { "Provider_Id" });
            DropIndex("dbo.ThesisAnnouncement", new[] { "Provider_Id" });
            DropTable("dbo.ThesisWorkflow");
            DropTable("dbo.ThesisFeedback");
            DropTable("dbo.ThesisProvider");
            DropTable("dbo.ThesisAnnouncement");
        }
    }
}
