using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Contracts;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class TimeTableInfoService : ITimeTableInfoService
    {
        private readonly TimeTableDbContext _db;

        public TimeTableInfoService(TimeTableDbContext db)
        {
            _db = db;
        }

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
                g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == orgId)).ToList();
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

        public ICollection<Data.Curriculum> GetCurriculums()
        {
            return _db.Curricula.OrderBy(c => c.Name).ToList();
        }
        
        public Data.Curriculum GetCurriculum(string name)
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


        public void DeleteActivity(Activity activity)
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

                foreach (var change in date.Changes.ToList())
                {
                    foreach (var changeNotificationState in change.NotificationStates.ToList())
                    {
                        change.NotificationStates.Remove(changeNotificationState);
                        _db.NotificationStates.Remove(changeNotificationState);
                    }

                    date.Changes.Remove(change);
                    _db.DateChanges.Remove(change);
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

            foreach (var subscription in subs)
            {
                var allDrawings = _db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
                foreach (var drawing in allDrawings)
                {
                    _db.SubscriptionDrawings.Remove(drawing);
                }
            }
            subs.ForEach(s => _db.Subscriptions.Remove(s));

            var drawings = _db.OccurrenceDrawings.Where(x => x.Occurrence.Id == occ.Id).ToList();
            foreach (var drawing in drawings)
            {
                _db.OccurrenceDrawings.Remove(drawing);
            }

            // Aus Lotterien austragen
            foreach (var lottery in occ.Lotteries.ToList())
            {
                lottery.Occurrences.Remove(occ);
            }


            // jetzt die Ocurrence wegwerfen
            _db.Occurrences.Remove(occ);
        }

        public void DeleteCurriculum(Data.Curriculum curriculum)
        {
            var cuurGroups = curriculum.CurriculumGroups.ToList();
            foreach (var curriculumGroup in cuurGroups)
            {
                var capGroups = curriculumGroup.CapacityGroups.ToList();
                foreach (var capacityGroup in capGroups)
                {
                    var ext = capacityGroup.Aliases.ToList();
                    foreach (var groupAliase in ext)
                    {
                        _db.GroupAliases.Remove(groupAliase);
                    }
                    _db.CapacityGroups.Remove(capacityGroup);
                }

                var semGroups = curriculumGroup.SemesterGroups.ToList();
                foreach (var semesterGroup in semGroups)
                {
                    _db.SemesterGroups.Remove(semesterGroup);
                }

                _db.CurriculumGroups.Remove(curriculumGroup);

            }

            var modules = curriculum.Modules.ToList();
            foreach (var module in modules)
            {
                var courses = module.ModuleCourses.ToList();
                foreach (var moduleCourse in courses)
                {
                    var c2 = moduleCourse.Courses.ToList();
                    foreach (var course in c2)
                    {
                        _db.Activities.Remove(course);
                    }

                    _db.ModuleCourses.Remove(moduleCourse);
                }
                
                _db.CurriculumModules.Remove(module);

            }
            
            _db.Curricula.Remove(curriculum);

            _db.SaveChanges();
        }

    }
}
