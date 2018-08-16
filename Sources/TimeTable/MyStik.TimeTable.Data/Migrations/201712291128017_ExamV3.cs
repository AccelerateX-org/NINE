namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ExamV3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lottery", "IsAvailable", c => c.Boolean(nullable: false));
            AddColumn("dbo.ExamPaper", "Title", c => c.String());
            AddColumn("dbo.ExamPaper", "Description", c => c.String());
            AddColumn("dbo.ExamPaper", "Version", c => c.String());
            AddColumn("dbo.ExamPaper", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ExamPaper", "LastChangeDate", c => c.DateTime());
            AddColumn("dbo.ExamPaper", "LastChangeUserId", c => c.String());
            AddColumn("dbo.ExamPaper", "ReleaseDate", c => c.DateTime());

            // Abwärtskompatibel: alle bestehenden Semestergruppen sind auch freiegegeben
            Sql("UPDATE dbo.Lottery SET IsAvailable  = 'True'");

        }

        public override void Down()
        {
            DropColumn("dbo.ExamPaper", "ReleaseDate");
            DropColumn("dbo.ExamPaper", "LastChangeUserId");
            DropColumn("dbo.ExamPaper", "LastChangeDate");
            DropColumn("dbo.ExamPaper", "CreateDate");
            DropColumn("dbo.ExamPaper", "Version");
            DropColumn("dbo.ExamPaper", "Description");
            DropColumn("dbo.ExamPaper", "Title");
            DropColumn("dbo.Lottery", "IsAvailable");
        }
    }
}
