namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Curriculum_v8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExaminationAid",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        ExaminationUnit_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExaminationUnit", t => t.ExaminationUnit_Id)
                .Index(t => t.ExaminationUnit_Id);
            
            AddColumn("dbo.TeachingForm", "Capacity", c => c.Int(nullable: false));

            /*
            AddColumn("dbo.TeachingUnit", "Name", c => c.String());
            AddColumn("dbo.TeachingUnit", "Description", c => c.String());
            */
        }

        public override void Down()
        {
            /*
            DropColumn("dbo.TeachingUnit", "Name");
            DropColumn("dbo.TeachingUnit", "Description");
            */

            DropForeignKey("dbo.ExaminationAid", "ExaminationUnit_Id", "dbo.ExaminationUnit");
            DropIndex("dbo.ExaminationAid", new[] { "ExaminationUnit_Id" });
            DropColumn("dbo.TeachingForm", "Capacity");
            DropTable("dbo.ExaminationAid");
        }
    }
}
