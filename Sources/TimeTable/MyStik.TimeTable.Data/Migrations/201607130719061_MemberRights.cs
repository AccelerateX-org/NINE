namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MemberRights : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrganiserMember", "IsMemberAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsRoomAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsSemesterAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsCurriculumAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsCourseAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsStudentAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsAlumniAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsEventAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "IsNewsAdmin", c => c.Boolean(nullable: false));

            Sql("UPDATE dbo.OrganiserMember SET IsMemberAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsRoomAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsSemesterAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsCurriculumAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsCourseAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsStudentAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsAlumniAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsEventAdmin  = 'False'");
            Sql("UPDATE dbo.OrganiserMember SET IsNewsAdmin  = 'False'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrganiserMember", "IsNewsAdmin");
            DropColumn("dbo.OrganiserMember", "IsEventAdmin");
            DropColumn("dbo.OrganiserMember", "IsAlumniAdmin");
            DropColumn("dbo.OrganiserMember", "IsStudentAdmin");
            DropColumn("dbo.OrganiserMember", "IsCourseAdmin");
            DropColumn("dbo.OrganiserMember", "IsCurriculumAdmin");
            DropColumn("dbo.OrganiserMember", "IsSemesterAdmin");
            DropColumn("dbo.OrganiserMember", "IsRoomAdmin");
            DropColumn("dbo.OrganiserMember", "IsMemberAdmin");
        }
    }
}
