namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV09 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SubjectOpportunity", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.SubjectOpportunity", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.SubjectOpportunity", "Subject_Id", "dbo.ModuleSubject");
            DropIndex("dbo.SubjectOpportunity", new[] { "Course_Id" });
            DropIndex("dbo.SubjectOpportunity", new[] { "Semester_Id" });
            DropIndex("dbo.SubjectOpportunity", new[] { "Subject_Id" });
            AddColumn("dbo.CatalogResponsibility", "IsHead", c => c.Boolean(nullable: false));
            AddColumn("dbo.ModuleResponsibility", "IsHead", c => c.Boolean(nullable: false));
            AddColumn("dbo.TeachingDescription", "MinCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.TeachingDescription", "MaxCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.TeachingDescription", "Curriculum_Id", c => c.Guid());
            CreateIndex("dbo.TeachingDescription", "Curriculum_Id");
            AddForeignKey("dbo.TeachingDescription", "Curriculum_Id", "dbo.Curriculum", "Id");
            DropTable("dbo.SubjectOpportunity");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SubjectOpportunity",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Course_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                        Subject_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.TeachingDescription", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.TeachingDescription", new[] { "Curriculum_Id" });
            DropColumn("dbo.TeachingDescription", "Curriculum_Id");
            DropColumn("dbo.TeachingDescription", "MaxCapacity");
            DropColumn("dbo.TeachingDescription", "MinCapacity");
            DropColumn("dbo.ModuleResponsibility", "IsHead");
            DropColumn("dbo.CatalogResponsibility", "IsHead");
            CreateIndex("dbo.SubjectOpportunity", "Subject_Id");
            CreateIndex("dbo.SubjectOpportunity", "Semester_Id");
            CreateIndex("dbo.SubjectOpportunity", "Course_Id");
            AddForeignKey("dbo.SubjectOpportunity", "Subject_Id", "dbo.ModuleSubject", "Id");
            AddForeignKey("dbo.SubjectOpportunity", "Semester_Id", "dbo.Semester", "Id");
            AddForeignKey("dbo.SubjectOpportunity", "Course_Id", "dbo.Activity", "Id");
        }
    }
}
