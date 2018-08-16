using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class SemesterGroupService : BaseService
    {
        public SemesterGroupService(TimeTableDbContext db) : base(db)
        {
        }

        public List<SemesterGroup> GetSemesterGroups(ApplicationUser user)
        {
            return Db.SemesterGroups.Where(x =>
                    x.Activities.OfType<Course>()
                        .Any(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id))))
                .ToList();
        }

    }
}