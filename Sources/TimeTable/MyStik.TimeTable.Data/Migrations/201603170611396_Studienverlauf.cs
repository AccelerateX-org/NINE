namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Studienverlauf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoursePlan",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModuleMapping",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Mark = c.Int(),
                        Trial = c.Int(nullable: false),
                        Module_Id = c.Guid(),
                        Plan_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumModule", t => t.Module_Id)
                .ForeignKey("dbo.CoursePlan", t => t.Plan_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Module_Id)
                .Index(t => t.Plan_Id)
                .Index(t => t.Semester_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleMapping", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.ModuleMapping", "Plan_Id", "dbo.CoursePlan");
            DropForeignKey("dbo.ModuleMapping", "Module_Id", "dbo.CurriculumModule");
            DropIndex("dbo.ModuleMapping", new[] { "Semester_Id" });
            DropIndex("dbo.ModuleMapping", new[] { "Plan_Id" });
            DropIndex("dbo.ModuleMapping", new[] { "Module_Id" });
            DropTable("dbo.ModuleMapping");
            DropTable("dbo.CoursePlan");
        }
    }
}
