namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurriculumV3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Degree",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        Description = c.String(),
                        IsUndergraduate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentSemesterGroup",
                c => new
                    {
                        Student_Id = c.Guid(nullable: false),
                        SemesterGroup_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Student_Id, t.SemesterGroup_Id })
                .ForeignKey("dbo.Student", t => t.Student_Id, cascadeDelete: true)
                .ForeignKey("dbo.SemesterGroup", t => t.SemesterGroup_Id, cascadeDelete: true)
                .Index(t => t.Student_Id)
                .Index(t => t.SemesterGroup_Id);
            
            AddColumn("dbo.Curriculum", "AsPartTime", c => c.Boolean(nullable: false));
            AddColumn("dbo.Curriculum", "IsQualification", c => c.Boolean(nullable: false));
            AddColumn("dbo.Curriculum", "AsDual", c => c.Boolean(nullable: false));
            AddColumn("dbo.Curriculum", "Degree_Id", c => c.Guid());
            CreateIndex("dbo.Curriculum", "Degree_Id");
            AddForeignKey("dbo.Curriculum", "Degree_Id", "dbo.Degree", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Curriculum", "Degree_Id", "dbo.Degree");
            DropForeignKey("dbo.StudentSemesterGroup", "SemesterGroup_Id", "dbo.SemesterGroup");
            DropForeignKey("dbo.StudentSemesterGroup", "Student_Id", "dbo.Student");
            DropIndex("dbo.StudentSemesterGroup", new[] { "SemesterGroup_Id" });
            DropIndex("dbo.StudentSemesterGroup", new[] { "Student_Id" });
            DropIndex("dbo.Curriculum", new[] { "Degree_Id" });
            DropColumn("dbo.Curriculum", "Degree_Id");
            DropColumn("dbo.Curriculum", "AsDual");
            DropColumn("dbo.Curriculum", "IsQualification");
            DropColumn("dbo.Curriculum", "AsPartTime");
            DropTable("dbo.StudentSemesterGroup");
            DropTable("dbo.Degree");
        }
    }
}
