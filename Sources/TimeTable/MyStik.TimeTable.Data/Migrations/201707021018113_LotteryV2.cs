namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lottery", "Organiser_Id", c => c.Guid());
            AddColumn("dbo.Lottery", "Semester_Id", c => c.Guid());
            CreateIndex("dbo.Lottery", "Organiser_Id");
            CreateIndex("dbo.Lottery", "Semester_Id");
            AddForeignKey("dbo.Lottery", "Organiser_Id", "dbo.ActivityOrganiser", "Id");
            AddForeignKey("dbo.Lottery", "Semester_Id", "dbo.Semester", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lottery", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.Lottery", "Organiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.Lottery", new[] { "Semester_Id" });
            DropIndex("dbo.Lottery", new[] { "Organiser_Id" });
            DropColumn("dbo.Lottery", "Semester_Id");
            DropColumn("dbo.Lottery", "Organiser_Id");
        }
    }
}
