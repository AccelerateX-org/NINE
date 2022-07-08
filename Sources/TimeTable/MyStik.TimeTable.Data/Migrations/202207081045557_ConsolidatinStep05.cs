namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsolidatinStep05 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.OccurrenceGroupSemesterGroup", newName: "SemesterGroupOccurrenceGroup");
            DropForeignKey("dbo.CurriculumGroup", "CurriculumModule_Id", "dbo.CurriculumModule");
            DropIndex("dbo.CurriculumGroup", new[] { "CurriculumModule_Id" });
            DropPrimaryKey("dbo.SemesterGroupOccurrenceGroup");
            AddColumn("dbo.CurriculumModule", "Tag", c => c.String());
            AddColumn("dbo.ModuleCourse", "Tag", c => c.String());
            AddPrimaryKey("dbo.SemesterGroupOccurrenceGroup", new[] { "SemesterGroup_Id", "OccurrenceGroup_Id" });
            DropColumn("dbo.CurriculumModule", "ECTS");
            DropColumn("dbo.CurriculumModule", "Description");
            DropColumn("dbo.CurriculumModule", "PreRequisites");
            DropColumn("dbo.CurriculumModule", "Competences");
            DropColumn("dbo.CurriculumModule", "Literature");
            DropColumn("dbo.CurriculumGroup", "CurriculumModule_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CurriculumGroup", "CurriculumModule_Id", c => c.Guid());
            AddColumn("dbo.CurriculumModule", "Literature", c => c.String());
            AddColumn("dbo.CurriculumModule", "Competences", c => c.String());
            AddColumn("dbo.CurriculumModule", "PreRequisites", c => c.String());
            AddColumn("dbo.CurriculumModule", "Description", c => c.String());
            AddColumn("dbo.CurriculumModule", "ECTS", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.SemesterGroupOccurrenceGroup");
            DropColumn("dbo.ModuleCourse", "Tag");
            DropColumn("dbo.CurriculumModule", "Tag");
            AddPrimaryKey("dbo.SemesterGroupOccurrenceGroup", new[] { "OccurrenceGroup_Id", "SemesterGroup_Id" });
            CreateIndex("dbo.CurriculumGroup", "CurriculumModule_Id");
            AddForeignKey("dbo.CurriculumGroup", "CurriculumModule_Id", "dbo.CurriculumModule", "Id");
            RenameTable(name: "dbo.SemesterGroupOccurrenceGroup", newName: "OccurrenceGroupSemesterGroup");
        }
    }
}
