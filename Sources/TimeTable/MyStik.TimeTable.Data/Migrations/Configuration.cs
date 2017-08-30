namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<TimeTableDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "MyStik.TimeTable.Data.TimeTableDbContext";
        }

        protected override void Seed(TimeTableDbContext context)
        {
            base.Seed(context);
        }
    }
}
