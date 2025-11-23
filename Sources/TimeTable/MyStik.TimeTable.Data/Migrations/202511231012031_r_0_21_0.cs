namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_21_0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Availability",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlanningGrid",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        Availability_Id = c.Guid(),
                        Organiser_Id = c.Guid(),
                        Segment_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Availability", t => t.Availability_Id)
                .ForeignKey("dbo.ActivityOrganiser", t => t.Organiser_Id)
                .ForeignKey("dbo.SemesterDate", t => t.Segment_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Availability_Id)
                .Index(t => t.Organiser_Id)
                .Index(t => t.Segment_Id)
                .Index(t => t.Semester_Id);
            
            CreateTable(
                "dbo.SlotLoading",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Slot_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumSlot", t => t.Slot_Id)
                .Index(t => t.Slot_Id);
            
            CreateTable(
                "dbo.SlotLoadingChip",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ECTS = c.Int(nullable: false),
                        Loading_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SlotLoading", t => t.Loading_Id)
                .Index(t => t.Loading_Id);
            
            CreateTable(
                "dbo.PlanningSlot",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        From = c.Time(precision: 7),
                        To = c.Time(precision: 7),
                        DayOfWeek = c.Int(),
                        Date = c.DateTime(),
                        Renark = c.String(),
                        PlanningGrid_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlanningGrid", t => t.PlanningGrid_Id)
                .Index(t => t.PlanningGrid_Id);
            
            AddColumn("dbo.OrganiserMember", "Availability_Id", c => c.Guid());
            AddColumn("dbo.Curriculum", "Availability_Id", c => c.Guid());
            AddColumn("dbo.CurriculumSlot", "Weight", c => c.Double(nullable: false));
            AddColumn("dbo.SubjectAccreditation", "IncludeExamination", c => c.Boolean(nullable: false));
            AddColumn("dbo.SubjectAccreditation", "Chip_Id", c => c.Guid());
            AddColumn("dbo.Student", "Availability_Id", c => c.Guid());
            AddColumn("dbo.Room", "Availability_Id", c => c.Guid());
            CreateIndex("dbo.OrganiserMember", "Availability_Id");
            CreateIndex("dbo.Curriculum", "Availability_Id");
            CreateIndex("dbo.SubjectAccreditation", "Chip_Id");
            CreateIndex("dbo.Student", "Availability_Id");
            CreateIndex("dbo.Room", "Availability_Id");
            AddForeignKey("dbo.SubjectAccreditation", "Chip_Id", "dbo.SlotLoadingChip", "Id");
            AddForeignKey("dbo.Student", "Availability_Id", "dbo.Availability", "Id");
            AddForeignKey("dbo.Curriculum", "Availability_Id", "dbo.Availability", "Id");
            AddForeignKey("dbo.Room", "Availability_Id", "dbo.Availability", "Id");
            AddForeignKey("dbo.OrganiserMember", "Availability_Id", "dbo.Availability", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganiserMember", "Availability_Id", "dbo.Availability");
            DropForeignKey("dbo.PlanningGrid", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.PlanningGrid", "Segment_Id", "dbo.SemesterDate");
            DropForeignKey("dbo.PlanningSlot", "PlanningGrid_Id", "dbo.PlanningGrid");
            DropForeignKey("dbo.PlanningGrid", "Organiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.Room", "Availability_Id", "dbo.Availability");
            DropForeignKey("dbo.Curriculum", "Availability_Id", "dbo.Availability");
            DropForeignKey("dbo.Student", "Availability_Id", "dbo.Availability");
            DropForeignKey("dbo.SubjectAccreditation", "Chip_Id", "dbo.SlotLoadingChip");
            DropForeignKey("dbo.SlotLoadingChip", "Loading_Id", "dbo.SlotLoading");
            DropForeignKey("dbo.SlotLoading", "Slot_Id", "dbo.CurriculumSlot");
            DropForeignKey("dbo.PlanningGrid", "Availability_Id", "dbo.Availability");
            DropIndex("dbo.PlanningSlot", new[] { "PlanningGrid_Id" });
            DropIndex("dbo.Room", new[] { "Availability_Id" });
            DropIndex("dbo.Student", new[] { "Availability_Id" });
            DropIndex("dbo.SlotLoadingChip", new[] { "Loading_Id" });
            DropIndex("dbo.SubjectAccreditation", new[] { "Chip_Id" });
            DropIndex("dbo.SlotLoading", new[] { "Slot_Id" });
            DropIndex("dbo.Curriculum", new[] { "Availability_Id" });
            DropIndex("dbo.PlanningGrid", new[] { "Semester_Id" });
            DropIndex("dbo.PlanningGrid", new[] { "Segment_Id" });
            DropIndex("dbo.PlanningGrid", new[] { "Organiser_Id" });
            DropIndex("dbo.PlanningGrid", new[] { "Availability_Id" });
            DropIndex("dbo.OrganiserMember", new[] { "Availability_Id" });
            DropColumn("dbo.Room", "Availability_Id");
            DropColumn("dbo.Student", "Availability_Id");
            DropColumn("dbo.SubjectAccreditation", "Chip_Id");
            DropColumn("dbo.SubjectAccreditation", "IncludeExamination");
            DropColumn("dbo.CurriculumSlot", "Weight");
            DropColumn("dbo.Curriculum", "Availability_Id");
            DropColumn("dbo.OrganiserMember", "Availability_Id");
            DropTable("dbo.PlanningSlot");
            DropTable("dbo.SlotLoadingChip");
            DropTable("dbo.SlotLoading");
            DropTable("dbo.PlanningGrid");
            DropTable("dbo.Availability");
        }
    }
}
