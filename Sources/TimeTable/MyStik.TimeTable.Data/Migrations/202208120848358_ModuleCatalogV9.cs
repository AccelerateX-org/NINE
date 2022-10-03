namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV9 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CurriculumModule", "MV_Id", "dbo.OrganiserMember");
            DropIndex("dbo.CurriculumModule", new[] { "MV_Id" });
            CreateTable(
                "dbo.ModuleResponsibility",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Member_Id = c.Guid(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.CurriculumModule", t => t.Module_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Module_Id);
            
            AddColumn("dbo.Curriculum", "ThesisDuration", c => c.Int(nullable: false));
            DropColumn("dbo.CurriculumModule", "MV_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CurriculumModule", "MV_Id", c => c.Guid());
            DropForeignKey("dbo.ModuleResponsibility", "Module_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.ModuleResponsibility", "Member_Id", "dbo.OrganiserMember");
            DropIndex("dbo.ModuleResponsibility", new[] { "Module_Id" });
            DropIndex("dbo.ModuleResponsibility", new[] { "Member_Id" });
            DropColumn("dbo.Curriculum", "ThesisDuration");
            DropTable("dbo.ModuleResponsibility");
            CreateIndex("dbo.CurriculumModule", "MV_Id");
            AddForeignKey("dbo.CurriculumModule", "MV_Id", "dbo.OrganiserMember", "Id");
        }
    }
}
