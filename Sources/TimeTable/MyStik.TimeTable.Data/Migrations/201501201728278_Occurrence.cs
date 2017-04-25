namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Occurrence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Occurrence", "UseGroups", c => c.Boolean(nullable: false));
            AddColumn("dbo.Occurrence", "UseExactFit", c => c.Boolean(nullable: false));

            Sql("UPDATE dbo.Occurrence SET UseGroups = 'False'");
            Sql("UPDATE dbo.Occurrence SET UseExactFit = 'False'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Occurrence", "UseExactFit");
            DropColumn("dbo.Occurrence", "UseGroups");
        }
    }
}
