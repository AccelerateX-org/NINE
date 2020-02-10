namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThesisV5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Thesis", "PlannedBegin", c => c.DateTime());
            AddColumn("dbo.Thesis", "PlannedEnd", c => c.DateTime());
            AddColumn("dbo.Thesis", "LastPlanChange", c => c.DateTime());
            AddColumn("dbo.Degree", "IsCertificate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Degree", "IsPhD", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Degree", "IsPhD");
            DropColumn("dbo.Degree", "IsCertificate");
            DropColumn("dbo.Thesis", "LastPlanChange");
            DropColumn("dbo.Thesis", "PlannedEnd");
            DropColumn("dbo.Thesis", "PlannedBegin");
        }
    }
}
