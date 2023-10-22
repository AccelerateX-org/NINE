namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanningV01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "IsProjected", c => c.Boolean(nullable: false));
            AddColumn("dbo.Activity", "Segment_Id", c => c.Guid());
            AddColumn("dbo.OrganiserMember", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisement", "UserId", c => c.String());
            AddColumn("dbo.Advertisement", "Thumbnail_Id", c => c.Guid());
            AddColumn("dbo.PersonalContact", "HostUserId", c => c.String());
            AddColumn("dbo.ActivityOwner", "PlanningPreferences", c => c.String());
            CreateIndex("dbo.Activity", "Segment_Id");
            CreateIndex("dbo.Advertisement", "Thumbnail_Id");
            AddForeignKey("dbo.Advertisement", "Thumbnail_Id", "dbo.BinaryStorage", "Id");
            AddForeignKey("dbo.Activity", "Segment_Id", "dbo.SemesterDate", "Id");

            Sql("UPDATE dbo.Activity SET IsProjected = 'False'");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Activity", "Segment_Id", "dbo.SemesterDate");
            DropForeignKey("dbo.Advertisement", "Thumbnail_Id", "dbo.BinaryStorage");
            DropIndex("dbo.Advertisement", new[] { "Thumbnail_Id" });
            DropIndex("dbo.Activity", new[] { "Segment_Id" });
            DropColumn("dbo.ActivityOwner", "PlanningPreferences");
            DropColumn("dbo.PersonalContact", "HostUserId");
            DropColumn("dbo.Advertisement", "Thumbnail_Id");
            DropColumn("dbo.Advertisement", "UserId");
            DropColumn("dbo.OrganiserMember", "IsDefault");
            DropColumn("dbo.Activity", "Segment_Id");
            DropColumn("dbo.Activity", "IsProjected");
        }
    }
}
