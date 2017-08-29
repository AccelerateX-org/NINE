namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryDrawingReports2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OccurrenceDrawing", "Occurrence_Id", c => c.Guid());
            CreateIndex("dbo.OccurrenceDrawing", "Occurrence_Id");
            AddForeignKey("dbo.OccurrenceDrawing", "Occurrence_Id", "dbo.Occurrence", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OccurrenceDrawing", "Occurrence_Id", "dbo.Occurrence");
            DropIndex("dbo.OccurrenceDrawing", new[] { "Occurrence_Id" });
            DropColumn("dbo.OccurrenceDrawing", "Occurrence_Id");
        }
    }
}
