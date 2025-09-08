namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_19_0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LotteryEntitlement",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MaxSubscription = c.Int(),
                        MinSubscription = c.Int(),
                        MaxConfirm = c.Int(),
                        MaxExceptionConfirm = c.Int(),
                        ValidFrom = c.DateTime(),
                        ValidUntil = c.DateTime(),
                        Lottery_Id = c.Guid(),
                        Student_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lottery", t => t.Lottery_Id)
                .ForeignKey("dbo.Student", t => t.Student_Id)
                .Index(t => t.Lottery_Id)
                .Index(t => t.Student_Id);
            
            AddColumn("dbo.SubjectAccreditation", "ECTS", c => c.Double(nullable: false));
            AddColumn("dbo.InfoscreenPage", "Index", c => c.Int(nullable: false));
            AddColumn("dbo.RoomAllocationChange", "ReferenceId", c => c.String());
            AddColumn("dbo.RoomAllocationChange", "RoomNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LotteryEntitlement", "Student_Id", "dbo.Student");
            DropForeignKey("dbo.LotteryEntitlement", "Lottery_Id", "dbo.Lottery");
            DropIndex("dbo.LotteryEntitlement", new[] { "Student_Id" });
            DropIndex("dbo.LotteryEntitlement", new[] { "Lottery_Id" });
            DropColumn("dbo.RoomAllocationChange", "RoomNumber");
            DropColumn("dbo.RoomAllocationChange", "ReferenceId");
            DropColumn("dbo.InfoscreenPage", "Index");
            DropColumn("dbo.SubjectAccreditation", "ECTS");
            DropTable("dbo.LotteryEntitlement");
        }
    }
}
