namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class GroupTemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupAlias",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .Index(t => t.Curriculum_Id);
            
            CreateTable(
                "dbo.GroupTemplate",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CurriculumGroupName = c.String(),
                        SemesterGroupName = c.String(),
                        Alias_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GroupAlias", t => t.Alias_Id)
                .Index(t => t.Alias_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupTemplate", "Alias_Id", "dbo.GroupAlias");
            DropForeignKey("dbo.GroupAlias", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.GroupTemplate", new[] { "Alias_Id" });
            DropIndex("dbo.GroupAlias", new[] { "Curriculum_Id" });
            DropTable("dbo.GroupTemplate");
            DropTable("dbo.GroupAlias");
        }
    }
}
