namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MemberLinkingv1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberExport",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ShortName = c.String(),
                        Member_Id = c.Guid(),
                        Organiser_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.ActivityOrganiser", t => t.Organiser_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Organiser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemberExport", "Organiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.MemberExport", "Member_Id", "dbo.OrganiserMember");
            DropIndex("dbo.MemberExport", new[] { "Organiser_Id" });
            DropIndex("dbo.MemberExport", new[] { "Member_Id" });
            DropTable("dbo.MemberExport");
        }
    }
}
