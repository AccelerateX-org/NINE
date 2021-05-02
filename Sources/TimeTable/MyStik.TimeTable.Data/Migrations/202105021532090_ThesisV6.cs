namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThesisV6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Thesis", "ProlongRequestDate", c => c.DateTime());
            AddColumn("dbo.Thesis", "ProlongReason", c => c.String());
            AddColumn("dbo.Thesis", "ProlongExtensionDate", c => c.DateTime());
            AddColumn("dbo.Thesis", "ProlongSupervisorAccepted", c => c.Boolean());
            AddColumn("dbo.Thesis", "ProlongExaminationBoardAccepted", c => c.Boolean());
            AddColumn("dbo.Thesis", "ProlongRejection", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Thesis", "ProlongRejection");
            DropColumn("dbo.Thesis", "ProlongExaminationBoardAccepted");
            DropColumn("dbo.Thesis", "ProlongSupervisorAccepted");
            DropColumn("dbo.Thesis", "ProlongExtensionDate");
            DropColumn("dbo.Thesis", "ProlongReason");
            DropColumn("dbo.Thesis", "ProlongRequestDate");
        }
    }
}
