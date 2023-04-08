using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    public class ActivityDateService
    {

        private TimeTableDbContext _db = null;

        /// <summary>
        /// 
        /// </summary>
        public ActivityDateService()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public ActivityDateService(TimeTableDbContext db)
        {
            _db = db;
        }

        private TimeTableDbContext Db
        {
            get { return _db ?? (_db = new TimeTableDbContext()); }
        }


        public ICollection<ActivityDate> GetDates(DateTime atDateTime, ActivityOrganiser forOrganiser)
        {
            var nowPlaying = Db.ActivityDates.Where(d =>
                    (d.Begin <= atDateTime && atDateTime <= d.End) &&                             // alles an diesem Tag
                    (d.Activity.SemesterGroups.Any(g =>                                         // alles was Zugehörigkeit zu einer Semestergruppe hat
                         g.CapacityGroup != null &&
                         g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == forOrganiser.Id) ||
                     (d.Activity.Organiser.Id == forOrganiser.Id)                                     // alle Raumreservierungen, Sprechstunden
                    ))
                .OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();


            return nowPlaying;
        }
    }
}