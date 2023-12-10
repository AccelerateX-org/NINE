namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV10 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.OrganiserMemberActivityDate", newName: "ActivityDateOrganiserMember");
            RenameTable(name: "dbo.OccurrenceGroupSemesterGroup", newName: "SemesterGroupOccurrenceGroup");
            DropPrimaryKey("dbo.ActivityDateOrganiserMember");
            DropPrimaryKey("dbo.SemesterGroupOccurrenceGroup");
            CreateTable(
                "dbo.SubjectAccreditation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Slot_Id = c.Guid(),
                        Subject_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumSlot", t => t.Slot_Id)
                .ForeignKey("dbo.ModuleSubject", t => t.Subject_Id)
                .Index(t => t.Slot_Id)
                .Index(t => t.Subject_Id);
            
            CreateTable(
                "dbo.SlotExecution",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Course_Id = c.Guid(),
                        Slot_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .ForeignKey("dbo.CurriculumSlot", t => t.Slot_Id)
                .Index(t => t.Course_Id)
                .Index(t => t.Slot_Id);
            
            CreateTable(
                "dbo.SubjectTeaching",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Course_Id = c.Guid(),
                        Subject_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .ForeignKey("dbo.ModuleSubject", t => t.Subject_Id)
                .Index(t => t.Course_Id)
                .Index(t => t.Subject_Id);
            
            AddColumn("dbo.OccurrenceGroup", "MinCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.OccurrenceGroup", "MaxCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.OccurrenceGroup", "Curriculum_Id", c => c.Guid());
            AddColumn("dbo.Occurrence", "UseWaitingList", c => c.Boolean(nullable: false));
            AddColumn("dbo.Occurrence", "AllocationMethod", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ActivityDateOrganiserMember", new[] { "ActivityDate_Id", "OrganiserMember_Id" });
            AddPrimaryKey("dbo.SemesterGroupOccurrenceGroup", new[] { "SemesterGroup_Id", "OccurrenceGroup_Id" });
            CreateIndex("dbo.OccurrenceGroup", "Curriculum_Id");
            AddForeignKey("dbo.OccurrenceGroup", "Curriculum_Id", "dbo.Curriculum", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubjectAccreditation", "Subject_Id", "dbo.ModuleSubject");
            DropForeignKey("dbo.SubjectTeaching", "Subject_Id", "dbo.ModuleSubject");
            DropForeignKey("dbo.SubjectTeaching", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.SlotExecution", "Slot_Id", "dbo.CurriculumSlot");
            DropForeignKey("dbo.SlotExecution", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.OccurrenceGroup", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.SubjectAccreditation", "Slot_Id", "dbo.CurriculumSlot");
            DropIndex("dbo.SubjectTeaching", new[] { "Subject_Id" });
            DropIndex("dbo.SubjectTeaching", new[] { "Course_Id" });
            DropIndex("dbo.SlotExecution", new[] { "Slot_Id" });
            DropIndex("dbo.SlotExecution", new[] { "Course_Id" });
            DropIndex("dbo.OccurrenceGroup", new[] { "Curriculum_Id" });
            DropIndex("dbo.SubjectAccreditation", new[] { "Subject_Id" });
            DropIndex("dbo.SubjectAccreditation", new[] { "Slot_Id" });
            DropPrimaryKey("dbo.SemesterGroupOccurrenceGroup");
            DropPrimaryKey("dbo.ActivityDateOrganiserMember");
            DropColumn("dbo.Occurrence", "AllocationMethod");
            DropColumn("dbo.Occurrence", "UseWaitingList");
            DropColumn("dbo.OccurrenceGroup", "Curriculum_Id");
            DropColumn("dbo.OccurrenceGroup", "MaxCapacity");
            DropColumn("dbo.OccurrenceGroup", "MinCapacity");
            DropTable("dbo.SubjectTeaching");
            DropTable("dbo.SlotExecution");
            DropTable("dbo.SubjectAccreditation");
            AddPrimaryKey("dbo.SemesterGroupOccurrenceGroup", new[] { "OccurrenceGroup_Id", "SemesterGroup_Id" });
            AddPrimaryKey("dbo.ActivityDateOrganiserMember", new[] { "OrganiserMember_Id", "ActivityDate_Id" });
            RenameTable(name: "dbo.SemesterGroupOccurrenceGroup", newName: "OccurrenceGroupSemesterGroup");
            RenameTable(name: "dbo.ActivityDateOrganiserMember", newName: "OrganiserMemberActivityDate");
        }
    }
}
