namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_26_0 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Curriculum", "ValidSince_Id", c => c.Guid());
            CreateIndex("dbo.Curriculum", "ValidSince_Id");
            AddForeignKey("dbo.Curriculum", "ValidSince_Id", "dbo.Semester", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Curriculum", "ValidSince_Id", "dbo.Semester");
            DropIndex("dbo.Curriculum", new[] { "ValidSince_Id" });
            DropColumn("dbo.Curriculum", "ValidSince_Id");
        }
    }
}
