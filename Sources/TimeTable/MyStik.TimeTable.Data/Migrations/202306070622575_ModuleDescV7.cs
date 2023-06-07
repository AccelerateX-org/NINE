namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CurriculumModule", "NameEn", c => c.String());
            AddColumn("dbo.CurriculumModule", "PrerequisitesEn", c => c.String());
            AddColumn("dbo.CurriculumModule", "ApplicablenessEn", c => c.String());
            AddColumn("dbo.ModuleDescription", "DescriptionEn", c => c.String());
            AddColumn("dbo.TeachingDescription", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeachingDescription", "Description");
            DropColumn("dbo.ModuleDescription", "DescriptionEn");
            DropColumn("dbo.CurriculumModule", "ApplicablenessEn");
            DropColumn("dbo.CurriculumModule", "PrerequisitesEn");
            DropColumn("dbo.CurriculumModule", "NameEn");
        }
    }
}
