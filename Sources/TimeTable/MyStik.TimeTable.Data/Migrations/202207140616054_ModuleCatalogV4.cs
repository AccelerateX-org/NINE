namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV4 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ModuleCourse", newName: "ModuleSubject");
            CreateTable(
                "dbo.TeachingFormat",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Tag = c.String(),
                        Description = c.String(),
                        CWN = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CurriculumModuleCatalog", "Tag", c => c.String());
            AddColumn("dbo.CurriculumModuleCatalog", "Description", c => c.String());
            AddColumn("dbo.ModuleSubject", "TeachingFormat_Id", c => c.Guid());
            CreateIndex("dbo.ModuleSubject", "TeachingFormat_Id");
            AddForeignKey("dbo.ModuleSubject", "TeachingFormat_Id", "dbo.TeachingFormat", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleSubject", "TeachingFormat_Id", "dbo.TeachingFormat");
            DropIndex("dbo.ModuleSubject", new[] { "TeachingFormat_Id" });
            DropColumn("dbo.ModuleSubject", "TeachingFormat_Id");
            DropColumn("dbo.CurriculumModuleCatalog", "Description");
            DropColumn("dbo.CurriculumModuleCatalog", "Tag");
            DropTable("dbo.TeachingFormat");
            RenameTable(name: "dbo.ModuleSubject", newName: "ModuleCourse");
        }
    }
}
