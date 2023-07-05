namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV5 : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Curriculum", "Description", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.Curriculum", "Description");
        }
    }
}
