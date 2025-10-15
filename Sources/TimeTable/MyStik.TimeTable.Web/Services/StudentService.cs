using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class StudentService : BaseService
    {
        public StudentService() : base(new TimeTableDbContext())
        {
            
        }

        public StudentService(TimeTableDbContext db) : base(db)
        {
        }

        public ICollection<Student> GetStudent(string userId)
        {
            // alt return Db.Students.Where(x => x.UserId.Equals(userId) && x.LastSemester == null).OrderByDescending(x => x.Created)
            if (string.IsNullOrEmpty(userId))
                return new List<Student>();

            return Db.Students
                .Where(x =>
                    x.UserId.Equals(userId))
                .OrderByDescending(x => x.Created)
                .ToList();
        }


        /// <summary>
        /// Der zuletzt angelegte Studiengang
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ICollection<Student> GetCurrentStudent(string userId)
        {
            // alt return Db.Students.Where(x => x.UserId.Equals(userId) && x.LastSemester == null).OrderByDescending(x => x.Created)
            if (string.IsNullOrEmpty(userId))
                return new List<Student>();
            
            return Db.Students
                .Where(x => 
                    x.UserId.Equals(userId) && 
                    x.FirstSemester != null && x.LastSemester == null)
                .OrderBy(x => x.Created)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ICollection<Student> GetCurrentStudent(ApplicationUser user)
        {
            if (user == null)
                return new List<Student>();

            return GetCurrentStudent(user.Id);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<string> GetStudents(SemesterGroup group)
        {

            var courses = Db.Activities.OfType<Course>().Where(x =>
                x.SemesterGroups.Any(g => g.Id == group.Id)).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var course in courses)
            {
                allSubscriptions.AddRange(course.Occurrence.Subscriptions);
            }

            var allStudents = allSubscriptions.Select(s => s.UserId).Distinct().ToList();
            return allStudents;
        }


    }
}