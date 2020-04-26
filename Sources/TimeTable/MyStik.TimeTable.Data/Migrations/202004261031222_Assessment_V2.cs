namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assessment_V2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CandidatureStage", "AssessmentStage_Id", c => c.Guid());
            CreateIndex("dbo.CandidatureStage", "AssessmentStage_Id");
            AddForeignKey("dbo.CandidatureStage", "AssessmentStage_Id", "dbo.AssessmentStage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CandidatureStage", "AssessmentStage_Id", "dbo.AssessmentStage");
            DropIndex("dbo.CandidatureStage", new[] { "AssessmentStage_Id" });
            DropColumn("dbo.CandidatureStage", "AssessmentStage_Id");
        }
    }
}
