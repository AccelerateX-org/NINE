namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CourseLink : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CapacityCourse",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ShortName = c.String(),
                        Course_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModuleCourse", t => t.Course_Id)
                .Index(t => t.Course_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CapacityCourse", "Course_Id", "dbo.ModuleCourse");
            DropIndex("dbo.CapacityCourse", new[] { "Course_Id" });
            DropTable("dbo.CapacityCourse");
        }
    }
}
