namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MouleCatalogV5 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SemesterGroupOccurrenceGroup", newName: "OccurrenceGroupSemesterGroup");
            DropPrimaryKey("dbo.OccurrenceGroupSemesterGroup");
            CreateTable(
                "dbo.ModuleDescription",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Description = c.String(),
                        Module_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumModule", t => t.Module_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Module_Id)
                .Index(t => t.Semester_Id);
            
            AddColumn("dbo.CurriculumSlot", "Name", c => c.String());
            AddPrimaryKey("dbo.OccurrenceGroupSemesterGroup", new[] { "OccurrenceGroup_Id", "SemesterGroup_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleDescription", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.ModuleDescription", "Module_Id", "dbo.CurriculumModule");
            DropIndex("dbo.ModuleDescription", new[] { "Semester_Id" });
            DropIndex("dbo.ModuleDescription", new[] { "Module_Id" });
            DropPrimaryKey("dbo.OccurrenceGroupSemesterGroup");
            DropColumn("dbo.CurriculumSlot", "Name");
            DropTable("dbo.ModuleDescription");
            AddPrimaryKey("dbo.OccurrenceGroupSemesterGroup", new[] { "SemesterGroup_Id", "OccurrenceGroup_Id" });
            RenameTable(name: "dbo.OccurrenceGroupSemesterGroup", newName: "SemesterGroupOccurrenceGroup");
        }
    }
}
