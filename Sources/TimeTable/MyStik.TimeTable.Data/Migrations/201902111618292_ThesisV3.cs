namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThesisV3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Advisor",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Contact_Id = c.Guid(),
                        Thesis_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contact", t => t.Contact_Id)
                .ForeignKey("dbo.Thesis", t => t.Thesis_Id)
                .Index(t => t.Contact_Id)
                .Index(t => t.Thesis_Id);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        Department_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Department", t => t.Department_Id)
                .Index(t => t.Department_Id);
            
            CreateTable(
                "dbo.Supervisor",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Member_Id = c.Guid(),
                        Thesis_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.Thesis", t => t.Thesis_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Thesis_Id);
            
            CreateTable(
                "dbo.Department",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Organisation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisation", t => t.Organisation_Id)
                .Index(t => t.Organisation_Id);
            
            CreateTable(
                "dbo.Organisation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Thesis", "RequestDate", c => c.DateTime());
            AddColumn("dbo.Thesis", "ResponseDate", c => c.DateTime());
            AddColumn("dbo.Thesis", "RequestMessage", c => c.String());
            AddColumn("dbo.Thesis", "IsPassed", c => c.Boolean());
            AddColumn("dbo.Thesis", "AcceptanceDate", c => c.DateTime());
            AddColumn("dbo.Thesis", "IsAccepted", c => c.Boolean());
            AddColumn("dbo.Thesis", "RequestAuthority_Id", c => c.Guid());
            AlterColumn("dbo.Thesis", "IssueDate", c => c.DateTime());
            AlterColumn("dbo.Thesis", "ExpirationDate", c => c.DateTime());
            CreateIndex("dbo.Thesis", "RequestAuthority_Id");
            AddForeignKey("dbo.Thesis", "RequestAuthority_Id", "dbo.OrganiserMember", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Department", "Organisation_Id", "dbo.Organisation");
            DropForeignKey("dbo.Contact", "Department_Id", "dbo.Department");
            DropForeignKey("dbo.Supervisor", "Thesis_Id", "dbo.Thesis");
            DropForeignKey("dbo.Supervisor", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.Thesis", "RequestAuthority_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.Advisor", "Thesis_Id", "dbo.Thesis");
            DropForeignKey("dbo.Advisor", "Contact_Id", "dbo.Contact");
            DropIndex("dbo.Department", new[] { "Organisation_Id" });
            DropIndex("dbo.Supervisor", new[] { "Thesis_Id" });
            DropIndex("dbo.Supervisor", new[] { "Member_Id" });
            DropIndex("dbo.Contact", new[] { "Department_Id" });
            DropIndex("dbo.Advisor", new[] { "Thesis_Id" });
            DropIndex("dbo.Advisor", new[] { "Contact_Id" });
            DropIndex("dbo.Thesis", new[] { "RequestAuthority_Id" });
            AlterColumn("dbo.Thesis", "ExpirationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Thesis", "IssueDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Thesis", "RequestAuthority_Id");
            DropColumn("dbo.Thesis", "IsAccepted");
            DropColumn("dbo.Thesis", "AcceptanceDate");
            DropColumn("dbo.Thesis", "IsPassed");
            DropColumn("dbo.Thesis", "RequestMessage");
            DropColumn("dbo.Thesis", "ResponseDate");
            DropColumn("dbo.Thesis", "RequestDate");
            DropTable("dbo.Organisation");
            DropTable("dbo.Department");
            DropTable("dbo.Supervisor");
            DropTable("dbo.Contact");
            DropTable("dbo.Advisor");
        }
    }
}
