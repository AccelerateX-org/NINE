namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ExamV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrganiserMember", "IsExamAdmin", c => c.Boolean(nullable: false));
            Sql("UPDATE dbo.OrganiserMember SET IsExamAdmin  = 'False'");

            DropForeignKey("dbo.StudentExam", "Exam_Id", "dbo.ModuleExam");
            AddForeignKey("dbo.StudentExam", "Exam_Id", "dbo.Activity");
        }

        public override void Down()
        {
            DropColumn("dbo.OrganiserMember", "IsExamAdmin");
        }
    }
}
