namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScriptShop_v03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderPeriod", "Title", c => c.String());
            AddColumn("dbo.OrderPeriod", "Organiser_Id", c => c.Guid());
            AddColumn("dbo.OrderPeriod", "Semester_Id", c => c.Guid());
            CreateIndex("dbo.OrderPeriod", "Organiser_Id");
            CreateIndex("dbo.OrderPeriod", "Semester_Id");
            AddForeignKey("dbo.OrderPeriod", "Organiser_Id", "dbo.ActivityOrganiser", "Id");
            AddForeignKey("dbo.OrderPeriod", "Semester_Id", "dbo.Semester", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderPeriod", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.OrderPeriod", "Organiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.OrderPeriod", new[] { "Semester_Id" });
            DropIndex("dbo.OrderPeriod", new[] { "Organiser_Id" });
            DropColumn("dbo.OrderPeriod", "Semester_Id");
            DropColumn("dbo.OrderPeriod", "Organiser_Id");
            DropColumn("dbo.OrderPeriod", "Title");
        }
    }
}
