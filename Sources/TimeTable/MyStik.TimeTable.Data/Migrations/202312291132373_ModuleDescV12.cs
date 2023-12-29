namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV12 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ActivityDateOrganiserMember", newName: "OrganiserMemberActivityDate");
            RenameTable(name: "dbo.SemesterGroupOccurrenceGroup", newName: "OccurrenceGroupSemesterGroup");
            DropForeignKey("dbo.ModuleAccreditation", "Criteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.ModuleAccreditation", "Slot_Id", "dbo.CurriculumSlot");
            DropForeignKey("dbo.ModuleAccreditation", "Module_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.TeachingDescription", "Accreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.TeachingDescription", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.TeachingDescription", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.TeachingDescription", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.TeachingDescription", "Subject_Id", "dbo.ModuleSubject");
            DropForeignKey("dbo.CurriculumCriteria", "Chapter_Id", "dbo.CurriculumChapter");
            DropForeignKey("dbo.CurriculumGroup", "CurriculumCriteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.ExaminationDescription", "Accreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.ModuleAccreditation", "LabelSet_Id", "dbo.ItemLabelSet");
            DropIndex("dbo.ModuleAccreditation", new[] { "Criteria_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "Slot_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "Module_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "LabelSet_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "Chapter_Id" });
            DropIndex("dbo.CurriculumGroup", new[] { "CurriculumCriteria_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Accreditation_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Course_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Curriculum_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Semester_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Subject_Id" });
            DropIndex("dbo.ExaminationDescription", new[] { "Accreditation_Id" });
            DropPrimaryKey("dbo.OrganiserMemberActivityDate");
            DropPrimaryKey("dbo.OccurrenceGroupSemesterGroup");
            AddPrimaryKey("dbo.OrganiserMemberActivityDate", new[] { "OrganiserMember_Id", "ActivityDate_Id" });
            AddPrimaryKey("dbo.OccurrenceGroupSemesterGroup", new[] { "OccurrenceGroup_Id", "SemesterGroup_Id" });
            DropColumn("dbo.CurriculumGroup", "CurriculumCriteria_Id");
            DropColumn("dbo.ExaminationDescription", "Accreditation_Id");
            DropTable("dbo.ModuleAccreditation");
            DropTable("dbo.CurriculumCriteria");
            DropTable("dbo.TeachingDescription");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TeachingDescription",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MinCapacity = c.Int(nullable: false),
                        MaxCapacity = c.Int(nullable: false),
                        Description = c.String(),
                        Accreditation_Id = c.Guid(),
                        Course_Id = c.Guid(),
                        Curriculum_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                        Subject_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurriculumCriteria",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Term = c.Int(nullable: false),
                        ShortName = c.String(),
                        MinECTS = c.Int(nullable: false),
                        MaxECTS = c.Int(nullable: false),
                        Option = c.Int(nullable: false),
                        Chapter_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModuleAccreditation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ShortName = c.String(),
                        Description = c.String(),
                        Number = c.String(),
                        IsMandatory = c.Boolean(nullable: false),
                        Criteria_Id = c.Guid(),
                        Slot_Id = c.Guid(),
                        Module_Id = c.Guid(),
                        LabelSet_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ExaminationDescription", "Accreditation_Id", c => c.Guid());
            AddColumn("dbo.CurriculumGroup", "CurriculumCriteria_Id", c => c.Guid());
            DropPrimaryKey("dbo.OccurrenceGroupSemesterGroup");
            DropPrimaryKey("dbo.OrganiserMemberActivityDate");
            AddPrimaryKey("dbo.OccurrenceGroupSemesterGroup", new[] { "SemesterGroup_Id", "OccurrenceGroup_Id" });
            AddPrimaryKey("dbo.OrganiserMemberActivityDate", new[] { "ActivityDate_Id", "OrganiserMember_Id" });
            CreateIndex("dbo.ExaminationDescription", "Accreditation_Id");
            CreateIndex("dbo.TeachingDescription", "Subject_Id");
            CreateIndex("dbo.TeachingDescription", "Semester_Id");
            CreateIndex("dbo.TeachingDescription", "Curriculum_Id");
            CreateIndex("dbo.TeachingDescription", "Course_Id");
            CreateIndex("dbo.TeachingDescription", "Accreditation_Id");
            CreateIndex("dbo.CurriculumGroup", "CurriculumCriteria_Id");
            CreateIndex("dbo.CurriculumCriteria", "Chapter_Id");
            CreateIndex("dbo.ModuleAccreditation", "LabelSet_Id");
            CreateIndex("dbo.ModuleAccreditation", "Module_Id");
            CreateIndex("dbo.ModuleAccreditation", "Slot_Id");
            CreateIndex("dbo.ModuleAccreditation", "Criteria_Id");
            AddForeignKey("dbo.ModuleAccreditation", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
            AddForeignKey("dbo.ExaminationDescription", "Accreditation_Id", "dbo.ModuleAccreditation", "Id");
            AddForeignKey("dbo.CurriculumGroup", "CurriculumCriteria_Id", "dbo.CurriculumCriteria", "Id");
            AddForeignKey("dbo.CurriculumCriteria", "Chapter_Id", "dbo.CurriculumChapter", "Id");
            AddForeignKey("dbo.TeachingDescription", "Subject_Id", "dbo.ModuleSubject", "Id");
            AddForeignKey("dbo.TeachingDescription", "Semester_Id", "dbo.Semester", "Id");
            AddForeignKey("dbo.TeachingDescription", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.TeachingDescription", "Course_Id", "dbo.Activity", "Id");
            AddForeignKey("dbo.TeachingDescription", "Accreditation_Id", "dbo.ModuleAccreditation", "Id");
            AddForeignKey("dbo.ModuleAccreditation", "Module_Id", "dbo.CurriculumModule", "Id");
            AddForeignKey("dbo.ModuleAccreditation", "Slot_Id", "dbo.CurriculumSlot", "Id");
            AddForeignKey("dbo.ModuleAccreditation", "Criteria_Id", "dbo.CurriculumCriteria", "Id");
            RenameTable(name: "dbo.OccurrenceGroupSemesterGroup", newName: "SemesterGroupOccurrenceGroup");
            RenameTable(name: "dbo.OrganiserMemberActivityDate", newName: "ActivityDateOrganiserMember");
        }
    }
}
