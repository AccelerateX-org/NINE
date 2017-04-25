using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Contracts;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class TimeTableInfoService : ITimeTableInfoService
    {
        readonly TimeTableDbContext _db = new TimeTableDbContext();
        public ICollection<Course> GetAllUnassignedCourses()
        {
            return _db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any() == false).ToList();
        }

        public ICollection<Course> GetCourses(string semester)
        {
            return _db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any(g => g.Semester.Name.Equals(semester))).ToList();
        }

        public ICollection<Course> GetCourses(Guid semId, Guid orgId)
        {
            return _db.Activities.OfType<Course>().Where(c => 
                c.SemesterGroups.Any(g => g.Semester.Id == semId &&
                g.CurriculumGroup.Curriculum.Organiser.Id == orgId)).ToList();
        }

        public ICollection<Course> GetCourses(string semester, string curriculumShortName)
        {
            return _db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any(g => 
                g.Semester.Name.Equals(semester) &&
                g.CapacityGroup != null && 
                g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(curriculumShortName)
                )).ToList();
        }


        public ICollection<Course> GetCourses(string semester, string curriculumShortName, int number)
        {
            throw new NotImplementedException();
        }

        public ICollection<Course> GetCourses(string semester, string curriculumShortName, int number, string group)
        {
            throw new NotImplementedException();
        }

        public ICollection<Course> GetCourses(string semester, string curriculumShortName, string groupName)
        {
            throw new NotImplementedException();
            /*
            return _db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any(g =>
                g.Semester.Name.Equals(semester) &&
                g.CurriculumGroup.Curriculum.ShortName.Equals(curriculumShortName) &&
                g.CurriculumGroup.Name.Equals(groupName)
                )).ToList();
             */
        }

        public ICollection<Curriculum> GetCurriculums()
        {
            return _db.Curricula.OrderBy(c => c.Name).ToList();
        }
        
        public Curriculum GetCurriculum(string name)
        {
            return _db.Curricula.SingleOrDefault(c => c.ShortName.ToUpper().Equals(name.ToUpper()));
        }

        public ICollection<CurriculumGroup> GetCurriculmGroups(string curriculumShortName)
        {
            return _db.CurriculumGroups.Where(g => g.Curriculum.ShortName.Equals(curriculumShortName)).ToList();
        }

        public ICollection<OrganiserMember> GetLecturers(Guid courseId)
        {
            return _db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == courseId)).ToList();
        }

        public void DeleteCourse(Guid courseId)
        {
            var course = _db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
            if (course == null)
                return;

            DeleteActivity(course);
        }

        public void DeleteEvent(Guid courseId)
        {
            var course = _db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == courseId);
            if (course == null)
                return;

            DeleteActivity(course);
        }


        private void DeleteActivity(Activity activity)
        {
            // alle termine durchgehen
            foreach (var date in activity.Dates.ToList())
            {
                foreach (var slot in date.Slots.ToList())
                {
                    DeleteOccurrence(slot.Occurrence);
                    date.Slots.Remove(slot);
                    _db.ActivitySlots.Remove(slot);
                }

                DeleteOccurrence(date.Occurrence);
                activity.Dates.Remove(date);
                _db.ActivityDates.Remove(date);
            }

            DeleteOccurrence(activity.Occurrence);

            foreach (var activityOwner in activity.Owners.ToList())
            {
                _db.ActivityOwners.Remove(activityOwner);
            }
            
            _db.Activities.Remove(activity);

            _db.SaveChanges();
        }

        private void DeleteOccurrence(Occurrence occ)
        {
            if (occ == null)
                return;

            // Occurrence Groups löschen
            var occGroups = occ.Groups.ToList();
            occGroups.ForEach(g => _db.OccurrenceGroups.Remove(g));

            // Alle Eintragungen des Kurses
            var subs = occ.Subscriptions.ToList();
            subs.ForEach(s => _db.Subscriptions.Remove(s));

            // jetzt die Ocurrence wegwerfen
            _db.Occurrences.Remove(occ);
        }

    }
}
