namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurriculumV5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CurriculumCriteria", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CurriculumGroupModuleAccreditation", "CurriculumGroup_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.CurriculumGroupModuleAccreditation", "ModuleAccreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.CurriculumModuleCurriculumGroup", "CurriculumModule_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.CurriculumModuleCurriculumGroup", "CurriculumGroup_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.GroupAlias", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CurriculumModule", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.PackageOptionModuleAccreditation", "PackageOption_Id", "dbo.PackageOption");
            DropForeignKey("dbo.PackageOptionModuleAccreditation", "ModuleAccreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.CurriculumCriteria", "PackageOption_Id", "dbo.PackageOption");
            DropForeignKey("dbo.CurriculumCriteria", "Group_Id", "dbo.CurriculumGroup");
            DropIndex("dbo.CurriculumCriteria", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "PackageOption_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "Group_Id" });
            DropIndex("dbo.GroupAlias", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumModule", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumGroupModuleAccreditation", new[] { "CurriculumGroup_Id" });
            DropIndex("dbo.CurriculumGroupModuleAccreditation", new[] { "ModuleAccreditation_Id" });
            DropIndex("dbo.CurriculumModuleCurriculumGroup", new[] { "CurriculumModule_Id" });
            DropIndex("dbo.CurriculumModuleCurriculumGroup", new[] { "CurriculumGroup_Id" });
            DropIndex("dbo.PackageOptionModuleAccreditation", new[] { "PackageOption_Id" });
            DropIndex("dbo.PackageOptionModuleAccreditation", new[] { "ModuleAccreditation_Id" });
            CreateTable(
                "dbo.CurriculumRequirement",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        ECTS = c.Int(nullable: false),
                        Option_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PackageOption", t => t.Option_Id)
                .Index(t => t.Option_Id);
            
            CreateTable(
                "dbo.CurriculumGroupCurriculumCriteria",
                c => new
                    {
                        CurriculumGroup_Id = c.Guid(nullable: false),
                        CurriculumCriteria_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CurriculumGroup_Id, t.CurriculumCriteria_Id })
                .ForeignKey("dbo.CurriculumGroup", t => t.CurriculumGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.CurriculumCriteria", t => t.CurriculumCriteria_Id, cascadeDelete: true)
                .Index(t => t.CurriculumGroup_Id)
                .Index(t => t.CurriculumCriteria_Id);
            
            AddColumn("dbo.CurriculumCriteria", "Term", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumCriteria", "Requirement_Id", c => c.Guid());
            AddColumn("dbo.CurriculumGroup", "Term", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumGroup", "CurriculumModule_Id", c => c.Guid());
            CreateIndex("dbo.CurriculumCriteria", "Requirement_Id");
            CreateIndex("dbo.CurriculumGroup", "CurriculumModule_Id");
            AddForeignKey("dbo.CurriculumGroup", "CurriculumModule_Id", "dbo.CurriculumModule", "Id");
            AddForeignKey("dbo.CurriculumCriteria", "Requirement_Id", "dbo.CurriculumRequirement", "Id");
            DropColumn("dbo.CurriculumCriteria", "Curriculum_Id");
            DropColumn("dbo.CurriculumCriteria", "PackageOption_Id");
            DropColumn("dbo.CurriculumCriteria", "Group_Id");
            DropColumn("dbo.GroupAlias", "Curriculum_Id");
            DropColumn("dbo.CurriculumModule", "Curriculum_Id");
            DropTable("dbo.CurriculumGroupModuleAccreditation");
            DropTable("dbo.CurriculumModuleCurriculumGroup");
            DropTable("dbo.PackageOptionModuleAccreditation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PackageOptionModuleAccreditation",
                c => new
                    {
                        PackageOption_Id = c.Guid(nullable: false),
                        ModuleAccreditation_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackageOption_Id, t.ModuleAccreditation_Id });
            
            CreateTable(
                "dbo.CurriculumModuleCurriculumGroup",
                c => new
                    {
                        CurriculumModule_Id = c.Guid(nullable: false),
                        CurriculumGroup_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CurriculumModule_Id, t.CurriculumGroup_Id });
            
            CreateTable(
                "dbo.CurriculumGroupModuleAccreditation",
                c => new
                    {
                        CurriculumGroup_Id = c.Guid(nullable: false),
                        ModuleAccreditation_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CurriculumGroup_Id, t.ModuleAccreditation_Id });
            
            AddColumn("dbo.CurriculumModule", "Curriculum_Id", c => c.Guid());
            AddColumn("dbo.GroupAlias", "Curriculum_Id", c => c.Guid());
            AddColumn("dbo.CurriculumCriteria", "Group_Id", c => c.Guid());
            AddColumn("dbo.CurriculumCriteria", "PackageOption_Id", c => c.Guid());
            AddColumn("dbo.CurriculumCriteria", "Curriculum_Id", c => c.Guid());
            DropForeignKey("dbo.CurriculumRequirement", "Option_Id", "dbo.PackageOption");
            DropForeignKey("dbo.CurriculumCriteria", "Requirement_Id", "dbo.CurriculumRequirement");
            DropForeignKey("dbo.CurriculumGroupCurriculumCriteria", "CurriculumCriteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.CurriculumGroupCurriculumCriteria", "CurriculumGroup_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.CurriculumGroup", "CurriculumModule_Id", "dbo.CurriculumModule");
            DropIndex("dbo.CurriculumGroupCurriculumCriteria", new[] { "CurriculumCriteria_Id" });
            DropIndex("dbo.CurriculumGroupCurriculumCriteria", new[] { "CurriculumGroup_Id" });
            DropIndex("dbo.CurriculumRequirement", new[] { "Option_Id" });
            DropIndex("dbo.CurriculumGroup", new[] { "CurriculumModule_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "Requirement_Id" });
            DropColumn("dbo.CurriculumGroup", "CurriculumModule_Id");
            DropColumn("dbo.CurriculumGroup", "Term");
            DropColumn("dbo.CurriculumCriteria", "Requirement_Id");
            DropColumn("dbo.CurriculumCriteria", "Term");
            DropTable("dbo.CurriculumGroupCurriculumCriteria");
            DropTable("dbo.CurriculumRequirement");
            CreateIndex("dbo.PackageOptionModuleAccreditation", "ModuleAccreditation_Id");
            CreateIndex("dbo.PackageOptionModuleAccreditation", "PackageOption_Id");
            CreateIndex("dbo.CurriculumModuleCurriculumGroup", "CurriculumGroup_Id");
            CreateIndex("dbo.CurriculumModuleCurriculumGroup", "CurriculumModule_Id");
            CreateIndex("dbo.CurriculumGroupModuleAccreditation", "ModuleAccreditation_Id");
            CreateIndex("dbo.CurriculumGroupModuleAccreditation", "CurriculumGroup_Id");
            CreateIndex("dbo.CurriculumModule", "Curriculum_Id");
            CreateIndex("dbo.GroupAlias", "Curriculum_Id");
            CreateIndex("dbo.CurriculumCriteria", "Group_Id");
            CreateIndex("dbo.CurriculumCriteria", "PackageOption_Id");
            CreateIndex("dbo.CurriculumCriteria", "Curriculum_Id");
            AddForeignKey("dbo.CurriculumCriteria", "Group_Id", "dbo.CurriculumGroup", "Id");
            AddForeignKey("dbo.CurriculumCriteria", "PackageOption_Id", "dbo.PackageOption", "Id");
            AddForeignKey("dbo.PackageOptionModuleAccreditation", "ModuleAccreditation_Id", "dbo.ModuleAccreditation", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PackageOptionModuleAccreditation", "PackageOption_Id", "dbo.PackageOption", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CurriculumModule", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.GroupAlias", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.CurriculumModuleCurriculumGroup", "CurriculumGroup_Id", "dbo.CurriculumGroup", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CurriculumModuleCurriculumGroup", "CurriculumModule_Id", "dbo.CurriculumModule", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CurriculumGroupModuleAccreditation", "ModuleAccreditation_Id", "dbo.ModuleAccreditation", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CurriculumGroupModuleAccreditation", "CurriculumGroup_Id", "dbo.CurriculumGroup", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CurriculumCriteria", "Curriculum_Id", "dbo.Curriculum", "Id");
        }
    }
}
