using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class EventService
    {
        private TimeTableDbContext db = new TimeTableDbContext();
        private Event _course;

        
        /// <summary>
        /// 
        /// </summary>
        public EventService()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
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
