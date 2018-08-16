namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ExamV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExamModuleExam",
                c => new
                    {
                        Exam_Id = c.Guid(nullable: false),
                        ModuleExam_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Exam_Id, t.ModuleExam_Id })
                .ForeignKey("dbo.Activity", t => t.Exam_Id, cascadeDelete: true)
                .ForeignKey("dbo.ModuleExam", t => t.ModuleExam_Id, cascadeDelete: true)
                .Index(t => t.Exam_Id)
                .Index(t => t.ModuleExam_Id);
            
            AddColumn("dbo.LotteryBet", "AmountConsumed", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExamModuleExam", "ModuleExam_Id", "dbo.ModuleExam");
            DropForeignKey("dbo.ExamModuleExam", "Exam_Id", "dbo.Activity");
            DropIndex("dbo.ExamModuleExam", new[] { "ModuleExam_Id" });
            DropIndex("dbo.ExamModuleExam", new[] { "Exam_Id" });
            DropColumn("dbo.LotteryBet", "AmountConsumed");
            DropTable("dbo.ExamModuleExam");
        }
    }
}
