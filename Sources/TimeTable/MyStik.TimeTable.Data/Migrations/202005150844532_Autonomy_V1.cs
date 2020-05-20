namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Autonomy_V1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CurriculumGroupCurriculumCriteria", newName: "CurriculumCriteriaCurriculumGroup");
            RenameTable(name: "dbo.ActivitySemesterGroup", newName: "SemesterGroupActivity");
            DropPrimaryKey("dbo.CurriculumCriteriaCurriculumGroup");
            DropPrimaryKey("dbo.SemesterGroupActivity");
            CreateTable(
                "dbo.Autonomy",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurriculumAccreditation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Curriculum_Id = c.Guid(),
                        Program_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.CurriculumProgram", t => t.Program_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.Program_Id);
            
            CreateTable(
                "dbo.CurriculumProgram",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Autonomy_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Autonomy", t => t.Autonomy_Id)
                .Index(t => t.Autonomy_Id);
            
            AddColumn("dbo.Curriculum", "Autonomy_Id", c => c.Guid());
            AddColumn("dbo.Activity", "Committee_Id", c => c.Guid());
            AddColumn("dbo.ActivityOrganiser", "Autonomy_Id", c => c.Guid());
            AddColumn("dbo.Institution", "Autonomy_Id", c => c.Guid());
            AddColumn("dbo.Committee", "Autonomy_Id", c => c.Guid());
            AddColumn("dbo.CommitteeMember", "IsSubstitute", c => c.Boolean(nullable: false));
            AddColumn("dbo.CommitteeMember", "HasVotingRight", c => c.Boolean(nullable: false));
            AddPrimaryKey("dbo.CurriculumCriteriaCurriculumGroup", new[] { "CurriculumCriteria_Id", "CurriculumGroup_Id" });
            AddPrimaryKey("dbo.SemesterGroupActivity", new[] { "SemesterGroup_Id", "Activity_Id" });
            CreateIndex("dbo.Curriculum", "Autonomy_Id");
            CreateIndex("dbo.Committee", "Autonomy_Id");
            CreateIndex("dbo.Activity", "Committee_Id");
            CreateIndex("dbo.ActivityOrganiser", "Autonomy_Id");
            CreateIndex("dbo.Institution", "Autonomy_Id");
            AddForeignKey("dbo.Committee", "Autonomy_Id", "dbo.Autonomy", "Id");
            AddForeignKey("dbo.ActivityOrganiser", "Autonomy_Id", "dbo.Autonomy", "Id");
            AddForeignKey("dbo.Institution", "Autonomy_Id", "dbo.Autonomy", "Id");
            AddForeignKey("dbo.Activity", "Committee_Id", "dbo.Committee", "Id");
            AddForeignKey("dbo.Curriculum", "Autonomy_Id", "dbo.Autonomy", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurriculumProgram", "Autonomy_Id", "dbo.Autonomy");
            DropForeignKey("dbo.CurriculumAccreditation", "Program_Id", "dbo.CurriculumProgram");
            DropForeignKey("dbo.CurriculumAccreditation", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.Curriculum", "Autonomy_Id", "dbo.Autonomy");
            DropForeignKey("dbo.Activity", "Committee_Id", "dbo.Committee");
            DropForeignKey("dbo.Institution", "Autonomy_Id", "dbo.Autonomy");
            DropForeignKey("dbo.ActivityOrganiser", "Autonomy_Id", "dbo.Autonomy");
            DropForeignKey("dbo.Committee", "Autonomy_Id", "dbo.Autonomy");
            DropIndex("dbo.CurriculumProgram", new[] { "Autonomy_Id" });
            DropIndex("dbo.CurriculumAccreditation", new[] { "Program_Id" });
            DropIndex("dbo.CurriculumAccreditation", new[] { "Curriculum_Id" });
            DropIndex("dbo.Institution", new[] { "Autonomy_Id" });
            DropIndex("dbo.ActivityOrganiser", new[] { "Autonomy_Id" });
            DropIndex("dbo.Activity", new[] { "Committee_Id" });
            DropIndex("dbo.Committee", new[] { "Autonomy_Id" });
            DropIndex("dbo.Curriculum", new[] { "Autonomy_Id" });
            DropPrimaryKey("dbo.SemesterGroupActivity");
            DropPrimaryKey("dbo.CurriculumCriteriaCurriculumGroup");
            DropColumn("dbo.CommitteeMember", "HasVotingRight");
            DropColumn("dbo.CommitteeMember", "IsSubstitute");
            DropColumn("dbo.Committee", "Autonomy_Id");
            DropColumn("dbo.Institution", "Autonomy_Id");
            DropColumn("dbo.ActivityOrganiser", "Autonomy_Id");
            DropColumn("dbo.Activity", "Committee_Id");
            DropColumn("dbo.Curriculum", "Autonomy_Id");
            DropTable("dbo.CurriculumProgram");
            DropTable("dbo.CurriculumAccreditation");
            DropTable("dbo.Autonomy");
            AddPrimaryKey("dbo.SemesterGroupActivity", new[] { "Activity_Id", "SemesterGroup_Id" });
            AddPrimaryKey("dbo.CurriculumCriteriaCurriculumGroup", new[] { "CurriculumGroup_Id", "CurriculumCriteria_Id" });
            RenameTable(name: "dbo.SemesterGroupActivity", newName: "ActivitySemesterGroup");
            RenameTable(name: "dbo.CurriculumCriteriaCurriculumGroup", newName: "CurriculumGroupCurriculumCriteria");
        }
    }
}
