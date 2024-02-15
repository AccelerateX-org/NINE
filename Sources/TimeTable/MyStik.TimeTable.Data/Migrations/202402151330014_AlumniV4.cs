namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlumniV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Alumnus", "Program", c => c.String());
            AddColumn("dbo.Alumnus", "Student_Id", c => c.Guid());
            AlterColumn("dbo.Alumnus", "Code", c => c.String());
            CreateIndex("dbo.Alumnus", "Student_Id");
            AddForeignKey("dbo.Alumnus", "Student_Id", "dbo.Student", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alumnus", "Student_Id", "dbo.Student");
            DropIndex("dbo.Alumnus", new[] { "Student_Id" });
            AlterColumn("dbo.Alumnus", "Code", c => c.Int(nullable: false));
            DropColumn("dbo.Alumnus", "Student_Id");
            DropColumn("dbo.Alumnus", "Program");
        }
    }
}
