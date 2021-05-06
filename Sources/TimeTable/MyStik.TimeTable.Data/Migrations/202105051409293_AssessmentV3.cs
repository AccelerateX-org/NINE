namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssessmentV3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssessmentStage", "FileTypes", c => c.String());
            AddColumn("dbo.AssessmentStage", "MaxFileCount", c => c.Int(nullable: false));
            AddColumn("dbo.AssessmentStage", "NaxPxSize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssessmentStage", "NaxPxSize");
            DropColumn("dbo.AssessmentStage", "MaxFileCount");
            DropColumn("dbo.AssessmentStage", "FileTypes");
        }
    }
}
