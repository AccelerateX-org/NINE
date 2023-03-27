namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExaminationAid", "ExaminationUnit_Id", "dbo.ExaminationUnit");
            DropForeignKey("dbo.ExaminationUnit", "Form_Id", "dbo.ExaminationForm");
            DropForeignKey("dbo.ExaminationUnit", "ModuleDescription_Id", "dbo.ModuleDescription");
            DropIndex("dbo.ExaminationUnit", new[] { "Form_Id" });
            DropIndex("dbo.ExaminationUnit", new[] { "ModuleDescription_Id" });
            DropIndex("dbo.ExaminationAid", new[] { "ExaminationUnit_Id" });
            CreateTable(
                "dbo.ChangeLog",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserIdAmendment = c.String(),
                        UserIdApproval = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastEdited = c.DateTime(nullable: false),
                        Approved = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ModuleDescription", "ChangeLog_Id", c => c.Guid());
            CreateIndex("dbo.ModuleDescription", "ChangeLog_Id");
            AddForeignKey("dbo.ModuleDescription", "ChangeLog_Id", "dbo.ChangeLog", "Id");
            DropTable("dbo.ExaminationUnit");
            DropTable("dbo.ExaminationAid");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExaminationUnit",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Duration = c.Int(),
                        Weight = c.Double(nullable: false),
                        Form_Id = c.Guid(),
                        ModuleDescription_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.ModuleDescription", "ChangeLog_Id", "dbo.ChangeLog");
            DropIndex("dbo.ModuleDescription", new[] { "ChangeLog_Id" });
            DropColumn("dbo.ModuleDescription", "ChangeLog_Id");
            DropTable("dbo.ChangeLog");
            CreateIndex("dbo.ExaminationAid", "ExaminationUnit_Id");
            CreateIndex("dbo.ExaminationUnit", "ModuleDescription_Id");
            CreateIndex("dbo.ExaminationUnit", "Form_Id");
            AddForeignKey("dbo.ExaminationUnit", "ModuleDescription_Id", "dbo.ModuleDescription", "Id");
            AddForeignKey("dbo.ExaminationUnit", "Form_Id", "dbo.ExaminationForm", "Id");
            AddForeignKey("dbo.ExaminationAid", "ExaminationUnit_Id", "dbo.ExaminationUnit", "Id");
        }
    }
}
