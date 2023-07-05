namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CurriculumModule", "Applicableness", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CurriculumModule", "Applicableness");
        }
    }
}
