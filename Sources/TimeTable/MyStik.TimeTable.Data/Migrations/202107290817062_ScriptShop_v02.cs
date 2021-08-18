namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScriptShop_v02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScriptDocument", "Course_Id", "dbo.Activity");
            DropIndex("dbo.ScriptDocument", new[] { "Course_Id" });
            DropColumn("dbo.ScriptDocument", "Course_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScriptDocument", "Course_Id", c => c.Guid());
            CreateIndex("dbo.ScriptDocument", "Course_Id");
            AddForeignKey("dbo.ScriptDocument", "Course_Id", "dbo.Activity", "Id");
        }
    }
}
