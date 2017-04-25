using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data.DefaultData;

namespace MyStik.TimeTable.Data.Initializers
{
    public class InitDevDatabase : DropCreateDatabaseAlways<TimeTableDbContext>
    {
        protected override void Seed(TimeTableDbContext context)
        {
            InfrastructureData.InitOrganisation(context);
            InfrastructureData.InitCurriculum(context);

            base.Seed(context);
        }
    }
}
