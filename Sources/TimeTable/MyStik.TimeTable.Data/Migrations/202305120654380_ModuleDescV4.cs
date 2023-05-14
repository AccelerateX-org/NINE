namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModuleAccreditation", "Description", c => c.String());
            AddColumn("dbo.CurriculumSlot", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CurriculumSlot", "Description");
            DropColumn("dbo.ModuleAccreditation", "Description");
        }
    }
}
