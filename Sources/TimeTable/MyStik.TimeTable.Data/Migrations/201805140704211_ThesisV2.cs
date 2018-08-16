namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThesisV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Thesis",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TitleDe = c.String(),
                        TitleEn = c.String(),
                        IssueDate = c.DateTime(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
                        RenewalDate = c.DateTime(),
                        DeliveryDate = c.DateTime(),
                        GradeDate = c.DateTime(),
                        IsCleared = c.Boolean(),
                        Student_Id = c.Guid(),
                        Supervision_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Student", t => t.Student_Id)
                .ForeignKey("dbo.Activity", t => t.Supervision_Id)
                .Index(t => t.Student_Id)
                .Index(t => t.Supervision_Id);
            
            AddColumn("dbo.Student", "Curriculum_Id", c => c.Guid());
            CreateIndex("dbo.Student", "Curriculum_Id");
            AddForeignKey("dbo.Student", "Curriculum_Id", "dbo.Curriculum", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Thesis", "Supervision_Id", "dbo.Activity");
            DropForeignKey("dbo.Thesis", "Student_Id", "dbo.Student");
            DropForeignKey("dbo.Student", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.Thesis", new[] { "Supervision_Id" });
            DropIndex("dbo.Thesis", new[] { "Student_Id" });
            DropIndex("dbo.Student", new[] { "Curriculum_Id" });
            DropColumn("dbo.Student", "Curriculum_Id");
            DropTable("dbo.Thesis");
        }
    }
}
