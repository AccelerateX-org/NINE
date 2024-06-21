namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrModules : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Curriculum", "InWinterTerm", c => c.Boolean(nullable: false));
            AddColumn("dbo.Curriculum", "InSummerTerm", c => c.Boolean(nullable: false));
            AddColumn("dbo.CurriculumModule", "HasCourses", c => c.Boolean(nullable: false));

            Sql("UPDATE dbo.Curriculum SET InWinterTerm = 'True'");
            Sql("UPDATE dbo.Curriculum SET InSummerTerm = 'True'");
            Sql("UPDATE dbo.CurriculumModule SET HasCourses = 'True'");

        }

        public override void Down()
        {
            DropColumn("dbo.CurriculumModule", "HasCourses");
            DropColumn("dbo.Curriculum", "InSummerTerm");
            DropColumn("dbo.Curriculum", "InWinterTerm");
        }
    }
}
