using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class EventService
    {
        private TimeTableDbContext db = new TimeTableDbContext();
        private Event _course;

        //private UserService userService = new UserService();

        public EventService()
        {
            
        }

        public EventService(Guid courseId)
        {
            _course = db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == courseId);
        }


        internal List<OccurrenceGroupCapacityModel> GetSubscriptionGroups()
        {
            var groups = new List<OccurrenceGroupCapacityModel>();

            foreach (var semGroup in _course.SemesterGroups)
            {
                var occGroup = _course.Occurrence.Groups.SingleOrDefault(g => g.SemesterGroups.Any(s => s.Id == semGroup.Id));

                // u.U. fehlende Gruppen automatisch ergänzen
                if (occGroup == null)
                {
                    occGroup = new OccurrenceGroup
                    {
                        FitToCurriculumOnly = false,
                        Capacity = 0,
                    };
                    occGroup.SemesterGroups.Add(semGroup);
                    _course.Occurrence.Groups.Add(occGroup);
                    db.SaveChanges();
                }

                groups.Add(new OccurrenceGroupCapacityModel
                {
                    CourseId = _course.Id,
                    SemesterGroupId = semGroup.Id,
                    Curriculum = semGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName,
                    Group = semGroup.GroupName,
                    Capacity = occGroup.Capacity
                });
            }

            return groups;
        }

    }
}
