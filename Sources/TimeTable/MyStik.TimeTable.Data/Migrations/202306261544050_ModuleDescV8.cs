namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChangeLog", "IsVisible", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChangeLog", "IsVisible");
        }
    }
}
