namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ModuleSubject", "SWS", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ModuleSubject", "SWS", c => c.Int(nullable: false));
        }
    }
}
