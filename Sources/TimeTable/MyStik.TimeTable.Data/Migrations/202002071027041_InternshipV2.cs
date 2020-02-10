namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InternshipV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Internship", "PlannedBegin", c => c.DateTime());
            AddColumn("dbo.Internship", "PlannedEnd", c => c.DateTime());
            AddColumn("dbo.Internship", "LastPlanChange", c => c.DateTime());
            AddColumn("dbo.Internship", "AcceptedDate", c => c.DateTime());
            AddColumn("dbo.Internship", "RealBegin", c => c.DateTime());
            AddColumn("dbo.Internship", "RealEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Internship", "RealEnd");
            DropColumn("dbo.Internship", "RealBegin");
            DropColumn("dbo.Internship", "AcceptedDate");
            DropColumn("dbo.Internship", "LastPlanChange");
            DropColumn("dbo.Internship", "PlannedEnd");
            DropColumn("dbo.Internship", "PlannedBegin");
        }
    }
}
