namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurriculumModuleCatalog",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Organiser_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityOrganiser", t => t.Organiser_Id)
                .Index(t => t.Organiser_Id);
            
            CreateTable(
                "dbo.SubjectOpportunity",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Course_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                        Subject_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .ForeignKey("dbo.ModuleCourse", t => t.Subject_Id)
                .Index(t => t.Course_Id)
                .Index(t => t.Semester_Id)
                .Index(t => t.Subject_Id);
            
            AddColumn("dbo.ModuleAccreditation", "Slot_Id", c => c.Guid());
            AddColumn("dbo.CurriculumModule", "Catalog_Id", c => c.Guid());
            AddColumn("dbo.Lottery", "blockPartTime", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lottery", "blockFullTime", c => c.Boolean(nullable: false));
            CreateIndex("dbo.ModuleAccreditation", "Slot_Id");
            CreateIndex("dbo.CurriculumModule", "Catalog_Id");
            AddForeignKey("dbo.CurriculumModule", "Catalog_Id", "dbo.CurriculumModuleCatalog", "Id");
            AddForeignKey("dbo.ModuleAccreditation", "Slot_Id", "dbo.CurriculumSlot", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubjectOpportunity", "Subject_Id", "dbo.ModuleCourse");
            DropForeignKey("dbo.SubjectOpportunity", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.SubjectOpportunity", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.ModuleAccreditation", "Slot_Id", "dbo.CurriculumSlot");
            DropForeignKey("dbo.CurriculumModuleCatalog", "Organiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.CurriculumModule", "Catalog_Id", "dbo.CurriculumModuleCatalog");
            DropIndex("dbo.SubjectOpportunity", new[] { "Subject_Id" });
            DropIndex("dbo.SubjectOpportunity", new[] { "Semester_Id" });
            DropIndex("dbo.SubjectOpportunity", new[] { "Course_Id" });
            DropIndex("dbo.CurriculumModuleCatalog", new[] { "Organiser_Id" });
            DropIndex("dbo.CurriculumModule", new[] { "Catalog_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "Slot_Id" });
            DropColumn("dbo.Lottery", "blockFullTime");
            DropColumn("dbo.Lottery", "blockPartTime");
            DropColumn("dbo.CurriculumModule", "Catalog_Id");
            DropColumn("dbo.ModuleAccreditation", "Slot_Id");
            DropTable("dbo.SubjectOpportunity");
            DropTable("dbo.CurriculumModuleCatalog");
        }
    }
}
