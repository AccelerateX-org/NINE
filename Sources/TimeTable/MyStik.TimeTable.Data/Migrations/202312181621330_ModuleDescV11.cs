namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV11 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SlotExecution", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.SlotExecution", "Slot_Id", "dbo.CurriculumSlot");
            DropIndex("dbo.SlotExecution", new[] { "Course_Id" });
            DropIndex("dbo.SlotExecution", new[] { "Slot_Id" });
            DropTable("dbo.SlotExecution");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SlotExecution",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Course_Id = c.Guid(),
                        Slot_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.SlotExecution", "Slot_Id");
            CreateIndex("dbo.SlotExecution", "Course_Id");
            AddForeignKey("dbo.SlotExecution", "Slot_Id", "dbo.CurriculumSlot", "Id");
            AddForeignKey("dbo.SlotExecution", "Course_Id", "dbo.Activity", "Id");
        }
    }
}
