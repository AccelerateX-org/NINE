using System.Data.Entity;

namespace MyStik.TimeTable.Data.Initializers
{
    public class InitDevDatabase : DropCreateDatabaseAlways<TimeTableDbContext>
    {
        protected override void Seed(TimeTableDbContext context)
        {
            //InfrastructureData.InitOrganisation(context);
            //InfrastructureData.InitCurriculum(context);

            base.Seed(context);
        }
    }
}
