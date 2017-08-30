namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfficeHourSlots : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "SlotsPerDate", c => c.Int());
            AddColumn("dbo.Activity", "FutureSubscriptions", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activity", "FutureSubscriptions");
            DropColumn("dbo.Activity", "SlotsPerDate");
        }
    }
}
