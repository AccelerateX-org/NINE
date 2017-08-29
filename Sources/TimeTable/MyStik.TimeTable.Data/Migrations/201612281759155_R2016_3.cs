namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class R2016_3 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SemesterGroupActivity", newName: "ActivitySemesterGroup");
            DropForeignKey("dbo.CurriculumModule", "Group_Id", "dbo.CurriculumGroup");
            DropIndex("dbo.CurriculumModule", new[] { "Group_Id" });
            DropPrimaryKey("dbo.ActivitySemesterGroup");
            CreateTable(
                "dbo.ModuleAccreditation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ECTS = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        Criteria_Id = c.Guid(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumCriteria", t => t.Criteria_Id)
                .ForeignKey("dbo.CurriculumModule", t => t.Module_Id)
                .Index(t => t.Criteria_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.CurriculumCriteria",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ECTS = c.Int(nullable: false),
                        Weight = c.Double(nullable: false),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .Index(t => t.Curriculum_Id);
            
            CreateTable(
                "dbo.Institution",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Domain = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Building",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        Institution_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Institution", t => t.Institution_Id)
                .Index(t => t.Institution_Id);
            
            CreateTable(
                "dbo.CriteriaRule",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AtEarliest = c.Boolean(),
                        AtLatest = c.Boolean(),
                        Criteria_Id = c.Guid(),
                        Group_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumCriteria", t => t.Criteria_Id)
                .ForeignKey("dbo.CurriculumGroup", t => t.Group_Id)
                .Index(t => t.Criteria_Id)
                .Index(t => t.Group_Id);
            
            CreateTable(
                "dbo.ModuleTrial",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Mark = c.Int(),
                        Course_Id = c.Guid(),
                        Mapping_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .ForeignKey("dbo.ModuleMapping", t => t.Mapping_Id)
                .Index(t => t.Course_Id)
                .Index(t => t.Mapping_Id);
            
            CreateTable(
                "dbo.CurriculumModuleCurriculumGroup",
                c => new
                    {
                        CurriculumModule_Id = c.Guid(nullable: false),
                        CurriculumGroup_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CurriculumModule_Id, t.CurriculumGroup_Id })
                .ForeignKey("dbo.CurriculumModule", t => t.CurriculumModule_Id, cascadeDelete: true)
                .ForeignKey("dbo.CurriculumGroup", t => t.CurriculumGroup_Id, cascadeDelete: true)
                .Index(t => t.CurriculumModule_Id)
                .Index(t => t.CurriculumGroup_Id);
            
            CreateTable(
                "dbo.InstitutionActivityOrganiser",
                c => new
                    {
                        Institution_Id = c.Guid(nullable: false),
                        ActivityOrganiser_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Institution_Id, t.ActivityOrganiser_Id })
                .ForeignKey("dbo.Institution", t => t.Institution_Id, cascadeDelete: true)
                .ForeignKey("dbo.ActivityOrganiser", t => t.ActivityOrganiser_Id, cascadeDelete: true)
                .Index(t => t.Institution_Id)
                .Index(t => t.ActivityOrganiser_Id);
            
            AddColumn("dbo.Activity", "IsBookable", c => c.Boolean());
            AddColumn("dbo.Activity", "RommBooked", c => c.String());
            AddColumn("dbo.Activity", "Info", c => c.String());
            AddColumn("dbo.CurriculumModule", "Lecturer", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Language", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "SWS", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Work", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Requirements", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Skills", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Topic", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Leistung", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Books", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Curriculum_Id", c => c.Guid());
            AddColumn("dbo.BinaryStorage", "Building_Id", c => c.Guid());
            AddColumn("dbo.Semester", "CoursePlan_Id", c => c.Guid());
            AddColumn("dbo.Room", "Building_Id", c => c.Guid());
            AddColumn("dbo.CoursePlan", "IsFavorit", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoursePlan", "Curriculum_Id", c => c.Guid());
            AddColumn("dbo.ModuleMapping", "IsCharged", c => c.Boolean(nullable: false));
            AddColumn("dbo.ModuleMapping", "CurriculumSemester_Id", c => c.Guid());
            AddPrimaryKey("dbo.ActivitySemesterGroup", new[] { "Activity_Id", "SemesterGroup_Id" });
            CreateIndex("dbo.CurriculumModule", "Curriculum_Id");
            CreateIndex("dbo.BinaryStorage", "Building_Id");
            CreateIndex("dbo.Room", "Building_Id");
            CreateIndex("dbo.Semester", "CoursePlan_Id");
            CreateIndex("dbo.CoursePlan", "Curriculum_Id");
            CreateIndex("dbo.ModuleMapping", "CurriculumSemester_Id");
            AddForeignKey("dbo.BinaryStorage", "Building_Id", "dbo.Building", "Id");
            AddForeignKey("dbo.Room", "Building_Id", "dbo.Building", "Id");
            AddForeignKey("dbo.CurriculumModule", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.CoursePlan", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.ModuleMapping", "CurriculumSemester_Id", "dbo.Semester", "Id");
            AddForeignKey("dbo.Semester", "CoursePlan_Id", "dbo.CoursePlan", "Id");
            DropColumn("dbo.CurriculumModule", "Group_Id");
            DropColumn("dbo.ModuleMapping", "Mark");
            DropColumn("dbo.ModuleMapping", "Trial");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ModuleMapping", "Trial", c => c.Int(nullable: false));
            AddColumn("dbo.ModuleMapping", "Mark", c => c.Int());
            AddColumn("dbo.CurriculumModule", "Group_Id", c => c.Guid());
            DropForeignKey("dbo.Semester", "CoursePlan_Id", "dbo.CoursePlan");
            DropForeignKey("dbo.ModuleTrial", "Mapping_Id", "dbo.ModuleMapping");
            DropForeignKey("dbo.ModuleTrial", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.ModuleMapping", "CurriculumSemester_Id", "dbo.Semester");
            DropForeignKey("dbo.CoursePlan", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CurriculumModule", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CriteriaRule", "Group_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.CriteriaRule", "Criteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.InstitutionActivityOrganiser", "ActivityOrganiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.InstitutionActivityOrganiser", "Institution_Id", "dbo.Institution");
            DropForeignKey("dbo.Room", "Building_Id", "dbo.Building");
            DropForeignKey("dbo.BinaryStorage", "Building_Id", "dbo.Building");
            DropForeignKey("dbo.Building", "Institution_Id", "dbo.Institution");
            DropForeignKey("dbo.CurriculumModuleCurriculumGroup", "CurriculumGroup_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.CurriculumModuleCurriculumGroup", "CurriculumModule_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.ModuleAccreditation", "Module_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.CurriculumCriteria", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.ModuleAccreditation", "Criteria_Id", "dbo.CurriculumCriteria");
            DropIndex("dbo.InstitutionActivityOrganiser", new[] { "ActivityOrganiser_Id" });
            DropIndex("dbo.InstitutionActivityOrganiser", new[] { "Institution_Id" });
            DropIndex("dbo.CurriculumModuleCurriculumGroup", new[] { "CurriculumGroup_Id" });
            DropIndex("dbo.CurriculumModuleCurriculumGroup", new[] { "CurriculumModule_Id" });
            DropIndex("dbo.ModuleTrial", new[] { "Mapping_Id" });
            DropIndex("dbo.ModuleTrial", new[] { "Course_Id" });
            DropIndex("dbo.ModuleMapping", new[] { "CurriculumSemester_Id" });
            DropIndex("dbo.CoursePlan", new[] { "Curriculum_Id" });
            DropIndex("dbo.CriteriaRule", new[] { "Group_Id" });
            DropIndex("dbo.CriteriaRule", new[] { "Criteria_Id" });
            DropIndex("dbo.Semester", new[] { "CoursePlan_Id" });
            DropIndex("dbo.Room", new[] { "Building_Id" });
            DropIndex("dbo.Building", new[] { "Institution_Id" });
            DropIndex("dbo.BinaryStorage", new[] { "Building_Id" });
            DropIndex("dbo.CurriculumModule", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "Curriculum_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "Module_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "Criteria_Id" });
            DropPrimaryKey("dbo.ActivitySemesterGroup");
            DropColumn("dbo.ModuleMapping", "CurriculumSemester_Id");
            DropColumn("dbo.ModuleMapping", "IsCharged");
            DropColumn("dbo.CoursePlan", "Curriculum_Id");
            DropColumn("dbo.CoursePlan", "IsFavorit");
            DropColumn("dbo.Room", "Building_Id");
            DropColumn("dbo.Semester", "CoursePlan_Id");
            DropColumn("dbo.BinaryStorage", "Building_Id");
            DropColumn("dbo.CurriculumModule", "Curriculum_Id");
            DropColumn("dbo.CurriculumModule", "Books");
            DropColumn("dbo.CurriculumModule", "Leistung");
            DropColumn("dbo.CurriculumModule", "Topic");
            DropColumn("dbo.CurriculumModule", "Skills");
            DropColumn("dbo.CurriculumModule", "Requirements");
            DropColumn("dbo.CurriculumModule", "Work");
            DropColumn("dbo.CurriculumModule", "SWS");
            DropColumn("dbo.CurriculumModule", "Language");
            DropColumn("dbo.CurriculumModule", "Lecturer");
            DropColumn("dbo.Activity", "Info");
            DropColumn("dbo.Activity", "RommBooked");
            DropColumn("dbo.Activity", "IsBookable");
            DropTable("dbo.InstitutionActivityOrganiser");
            DropTable("dbo.CurriculumModuleCurriculumGroup");
            DropTable("dbo.ModuleTrial");
            DropTable("dbo.CriteriaRule");
            DropTable("dbo.Building");
            DropTable("dbo.Institution");
            DropTable("dbo.CurriculumCriteria");
            DropTable("dbo.ModuleAccreditation");
            AddPrimaryKey("dbo.ActivitySemesterGroup", new[] { "SemesterGroup_Id", "Activity_Id" });
            CreateIndex("dbo.CurriculumModule", "Group_Id");
            AddForeignKey("dbo.CurriculumModule", "Group_Id", "dbo.CurriculumGroup", "Id");
            RenameTable(name: "dbo.ActivitySemesterGroup", newName: "SemesterGroupActivity");
        }
    }
}
