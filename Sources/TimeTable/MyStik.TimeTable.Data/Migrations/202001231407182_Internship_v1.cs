namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Internship_v1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Internship",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        ColloqDate = c.DateTime(),
                        HasColloqPassed = c.Boolean(),
                        RequestDate = c.DateTime(),
                        ResponseDate = c.DateTime(),
                        RequestMessage = c.String(),
                        IsPassed = c.Boolean(),
                        RequestAuthority_Id = c.Guid(),
                        Student_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.RequestAuthority_Id)
                .ForeignKey("dbo.Student", t => t.Student_Id)
                .Index(t => t.RequestAuthority_Id)
                .Index(t => t.Student_Id);
            
            AddColumn("dbo.Advisor", "CorporateName", c => c.String());
            AddColumn("dbo.Advisor", "PersonFirstName", c => c.String());
            AddColumn("dbo.Advisor", "PersonLastName", c => c.String());
            AddColumn("dbo.Advisor", "PersonAction", c => c.String());
            AddColumn("dbo.Advisor", "PersonEMail", c => c.String());
            AddColumn("dbo.Advisor", "PersonPhone", c => c.String());
            AddColumn("dbo.Advisor", "Internship_Id", c => c.Guid());
            CreateIndex("dbo.Advisor", "Internship_Id");
            AddForeignKey("dbo.Advisor", "Internship_Id", "dbo.Internship", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Internship", "Student_Id", "dbo.Student");
            DropForeignKey("dbo.Internship", "RequestAuthority_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.Advisor", "Internship_Id", "dbo.Internship");
            DropIndex("dbo.Internship", new[] { "Student_Id" });
            DropIndex("dbo.Internship", new[] { "RequestAuthority_Id" });
            DropIndex("dbo.Advisor", new[] { "Internship_Id" });
            DropColumn("dbo.Advisor", "Internship_Id");
            DropColumn("dbo.Advisor", "PersonPhone");
            DropColumn("dbo.Advisor", "PersonEMail");
            DropColumn("dbo.Advisor", "PersonAction");
            DropColumn("dbo.Advisor", "PersonLastName");
            DropColumn("dbo.Advisor", "PersonFirstName");
            DropColumn("dbo.Advisor", "CorporateName");
            DropTable("dbo.Internship");
        }
    }
}
