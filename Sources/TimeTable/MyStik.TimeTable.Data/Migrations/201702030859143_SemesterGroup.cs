namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SemesterGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SemesterGroup", "IsAvailable", c => c.Boolean(nullable: false));

            // Abwärtskompatibel: alle bestehenden Semestergruppen sind auch freiegegeben
            Sql("UPDATE dbo.SemesterGroup SET IsAvailable  = 'True'");

        }
        
        public override void Down()
        {
            DropColumn("dbo.SemesterGroup", "IsAvailable");
        }
    }
}
