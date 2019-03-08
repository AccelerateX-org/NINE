namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThesisV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Thesis", "AbstractDe", c => c.String());
            AddColumn("dbo.Thesis", "AbstractEn", c => c.String());
            AddColumn("dbo.Supervisor", "AcceptanceDate", c => c.DateTime());
            AddColumn("dbo.Supervisor", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Supervisor", "Remark");
            DropColumn("dbo.Supervisor", "AcceptanceDate");
            DropColumn("dbo.Thesis", "AbstractEn");
            DropColumn("dbo.Thesis", "AbstractDe");
        }
    }
}
