namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlumniV1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Alumnus", "Email", c => c.String());
            AddColumn("dbo.Alumnus", "Code", c => c.Int(nullable: false));
            AddColumn("dbo.Alumnus", "CodeExpiryDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Alumnus", "IsValid", c => c.Boolean(nullable: false));
            AddColumn("dbo.Alumnus", "FirstName", c => c.String());
            AddColumn("dbo.Alumnus", "LastName", c => c.String());
            AddColumn("dbo.Alumnus", "Title", c => c.String());
            AddColumn("dbo.Alumnus", "Degree", c => c.String());
            AddColumn("dbo.Alumnus", "FinishingSemester", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Alumnus", "FinishingSemester");
            DropColumn("dbo.Alumnus", "Degree");
            DropColumn("dbo.Alumnus", "Title");
            DropColumn("dbo.Alumnus", "LastName");
            DropColumn("dbo.Alumnus", "FirstName");
            DropColumn("dbo.Alumnus", "IsValid");
            DropColumn("dbo.Alumnus", "CodeExpiryDateTime");
            DropColumn("dbo.Alumnus", "Code");
            DropColumn("dbo.Alumnus", "Email");
        }
    }
}
