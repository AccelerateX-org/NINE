namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlumniV3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InfoscreenPage", "RoomAllocation_Id", "dbo.RoomAllocation");
            DropIndex("dbo.InfoscreenPage", new[] { "RoomAllocation_Id" });
            CreateTable(
                "dbo.MemberAvailability",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Begin = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Priority = c.Int(nullable: false),
                        Remarks = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                        Member_Id = c.Guid(),
                        Segment_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.SemesterDate", t => t.Segment_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Segment_Id)
                .Index(t => t.Semester_Id);
            
            AddColumn("dbo.InfoscreenPage", "RoomAllocationGroup_Id", c => c.Guid());
            CreateIndex("dbo.InfoscreenPage", "RoomAllocationGroup_Id");
            AddForeignKey("dbo.InfoscreenPage", "RoomAllocationGroup_Id", "dbo.RoomAllocationGroup", "Id");
            DropColumn("dbo.InfoscreenPage", "RoomAllocation_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InfoscreenPage", "RoomAllocation_Id", c => c.Guid());
            DropForeignKey("dbo.MemberAvailability", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.MemberAvailability", "Segment_Id", "dbo.SemesterDate");
            DropForeignKey("dbo.InfoscreenPage", "RoomAllocationGroup_Id", "dbo.RoomAllocationGroup");
            DropForeignKey("dbo.MemberAvailability", "Member_Id", "dbo.OrganiserMember");
            DropIndex("dbo.InfoscreenPage", new[] { "RoomAllocationGroup_Id" });
            DropIndex("dbo.MemberAvailability", new[] { "Semester_Id" });
            DropIndex("dbo.MemberAvailability", new[] { "Segment_Id" });
            DropIndex("dbo.MemberAvailability", new[] { "Member_Id" });
            DropColumn("dbo.InfoscreenPage", "RoomAllocationGroup_Id");
            DropTable("dbo.MemberAvailability");
            CreateIndex("dbo.InfoscreenPage", "RoomAllocation_Id");
            AddForeignKey("dbo.InfoscreenPage", "RoomAllocation_Id", "dbo.RoomAllocation", "Id");
        }
    }
}
