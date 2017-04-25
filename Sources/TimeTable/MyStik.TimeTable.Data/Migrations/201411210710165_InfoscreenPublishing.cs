namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InfoscreenPublishing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "Published", c => c.Boolean(defaultValue:false));
            AddColumn("dbo.Activity", "FromDateTime", c => c.DateTime());
            AddColumn("dbo.Activity", "FromTimeSpan", c => c.Time(precision: 7));

            Sql("UPDATE dbo.Activity SET Published = 'False'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activity", "FromTimeSpan");
            DropColumn("dbo.Activity", "FromDateTime");
            DropColumn("dbo.Activity", "Published");
        }
    }
}
