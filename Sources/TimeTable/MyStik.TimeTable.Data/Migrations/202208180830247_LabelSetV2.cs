namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LabelSetV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CurriculumSlot", "LabelSet_Id", c => c.Guid());
            CreateIndex("dbo.CurriculumSlot", "LabelSet_Id");
            AddForeignKey("dbo.CurriculumSlot", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurriculumSlot", "LabelSet_Id", "dbo.ItemLabelSet");
            DropIndex("dbo.CurriculumSlot", new[] { "LabelSet_Id" });
            DropColumn("dbo.CurriculumSlot", "LabelSet_Id");
        }
    }
}
