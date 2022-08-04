namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityOrganiser", "Tag", c => c.String());
            AddColumn("dbo.Institution", "Tag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Institution", "Tag");
            DropColumn("dbo.ActivityOrganiser", "Tag");
        }
    }
}
