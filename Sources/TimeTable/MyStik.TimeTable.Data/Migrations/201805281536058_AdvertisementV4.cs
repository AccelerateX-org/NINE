namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvertisementV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Student", "IsPartTime", c => c.Boolean(nullable: false));
            AddColumn("dbo.Student", "IsDual", c => c.Boolean(nullable: false));
            AddColumn("dbo.Student", "HasCompleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Student", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.Student", "FirstSemester_Id", c => c.Guid());
            AddColumn("dbo.Student", "LastSemester_Id", c => c.Guid());
            AddColumn("dbo.Advertisement", "ForWorkingStudent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisement", "ForTutor", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisement", "ForCompetition", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Student", "FirstSemester_Id");
            CreateIndex("dbo.Student", "LastSemester_Id");
            AddForeignKey("dbo.Student", "FirstSemester_Id", "dbo.Semester", "Id");
            AddForeignKey("dbo.Student", "LastSemester_Id", "dbo.Semester", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Student", "LastSemester_Id", "dbo.Semester");
            DropForeignKey("dbo.Student", "FirstSemester_Id", "dbo.Semester");
            DropIndex("dbo.Student", new[] { "LastSemester_Id" });
            DropIndex("dbo.Student", new[] { "FirstSemester_Id" });
            DropColumn("dbo.Advertisement", "ForCompetition");
            DropColumn("dbo.Advertisement", "ForTutor");
            DropColumn("dbo.Advertisement", "ForWorkingStudent");
            DropColumn("dbo.Student", "LastSemester_Id");
            DropColumn("dbo.Student", "FirstSemester_Id");
            DropColumn("dbo.Student", "Created");
            DropColumn("dbo.Student", "HasCompleted");
            DropColumn("dbo.Student", "IsDual");
            DropColumn("dbo.Student", "IsPartTime");
        }
    }
}
