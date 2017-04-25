namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alumnae : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityOwner",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsLocked = c.Boolean(nullable: false),
                        Activity_Id = c.Guid(),
                        Member_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Activity_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .Index(t => t.Activity_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.Alumnus",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        Curriculum_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.Semester_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alumnus", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.Alumnus", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.ActivityOwner", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ActivityOwner", "Activity_Id", "dbo.Activity");
            DropIndex("dbo.Alumnus", new[] { "Semester_Id" });
            DropIndex("dbo.Alumnus", new[] { "Curriculum_Id" });
            DropIndex("dbo.ActivityOwner", new[] { "Member_Id" });
            DropIndex("dbo.ActivityOwner", new[] { "Activity_Id" });
            DropTable("dbo.Alumnus");
            DropTable("dbo.ActivityOwner");
        }
    }
}
