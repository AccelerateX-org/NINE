namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeemsterGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscription", "LikesEMails", c => c.Boolean(nullable:false, defaultValue:false));
            AddColumn("dbo.Subscription", "LikesNotifications", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Subscription", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue: "OccurrenceSubscription"));
            AddColumn("dbo.Subscription", "SemesterGroup_Id", c => c.Guid());
            AddColumn("dbo.SemesterGroup", "Name", c => c.String());
            CreateIndex("dbo.Subscription", "SemesterGroup_Id");
            AddForeignKey("dbo.Subscription", "SemesterGroup_Id", "dbo.SemesterGroup", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscription", "SemesterGroup_Id", "dbo.SemesterGroup");
            DropIndex("dbo.Subscription", new[] { "SemesterGroup_Id" });
            DropColumn("dbo.SemesterGroup", "Name");
            DropColumn("dbo.Subscription", "SemesterGroup_Id");
            DropColumn("dbo.Subscription", "Discriminator");
            DropColumn("dbo.Subscription", "LikesNotifications");
            DropColumn("dbo.Subscription", "LikesEMails");
        }
    }
}
