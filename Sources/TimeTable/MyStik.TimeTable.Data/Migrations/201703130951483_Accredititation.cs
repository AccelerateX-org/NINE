namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Accredititation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurriculumGroupModuleAccreditation",
                c => new
                    {
                        CurriculumGroup_Id = c.Guid(nullable: false),
                        ModuleAccreditation_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CurriculumGroup_Id, t.ModuleAccreditation_Id })
                .ForeignKey("dbo.CurriculumGroup", t => t.CurriculumGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.ModuleAccreditation", t => t.ModuleAccreditation_Id, cascadeDelete: true)
                .Index(t => t.CurriculumGroup_Id)
                .Index(t => t.ModuleAccreditation_Id);
            
            AddColumn("dbo.CurriculumCriteria", "ShortName", c => c.String());
            AddColumn("dbo.CurriculumCriteria", "MinECTS", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumCriteria", "MaxECTS", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumCriteria", "Option", c => c.Int(nullable: false));
            DropColumn("dbo.ModuleAccreditation", "ECTS");
            DropColumn("dbo.ModuleAccreditation", "IsDefault");
            DropColumn("dbo.CurriculumCriteria", "ECTS");
            DropColumn("dbo.CurriculumCriteria", "Weight");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CurriculumCriteria", "Weight", c => c.Double(nullable: false));
            AddColumn("dbo.CurriculumCriteria", "ECTS", c => c.Int(nullable: false));
            AddColumn("dbo.ModuleAccreditation", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.ModuleAccreditation", "ECTS", c => c.Int(nullable: false));
            DropForeignKey("dbo.CurriculumGroupModuleAccreditation", "ModuleAccreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.CurriculumGroupModuleAccreditation", "CurriculumGroup_Id", "dbo.CurriculumGroup");
            DropIndex("dbo.CurriculumGroupModuleAccreditation", new[] { "ModuleAccreditation_Id" });
            DropIndex("dbo.CurriculumGroupModuleAccreditation", new[] { "CurriculumGroup_Id" });
            DropColumn("dbo.CurriculumCriteria", "Option");
            DropColumn("dbo.CurriculumCriteria", "MaxECTS");
            DropColumn("dbo.CurriculumCriteria", "MinECTS");
            DropColumn("dbo.CurriculumCriteria", "ShortName");
            DropTable("dbo.CurriculumGroupModuleAccreditation");
        }
    }
}
