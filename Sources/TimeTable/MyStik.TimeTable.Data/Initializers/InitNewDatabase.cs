using MyStik.TimeTable.Data.DefaultData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data.Initializers
{
    public class InitNewDatabase : CreateDatabaseIfNotExists<TimeTableDbContext>
    {
        protected override void Seed(TimeTableDbContext context)
        {
            InfrastructureData.InitOrganisation(context);
            InfrastructureData.InitCurriculum(context);

            base.Seed(context);
        }
    }
}
