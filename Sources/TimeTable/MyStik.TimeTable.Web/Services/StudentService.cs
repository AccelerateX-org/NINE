using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        /// <summary>
        /// Der zuletzt angelegte Studiengang
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Student GetCurrentStudent(string userId)
        {
            return Db.Students.Where(x => x.UserId.Equals(userId)).OrderByDescending(x => x.Created)
                .FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Student GetCurrentStudent(ApplicationUser user)
        {
            if (user == null)
                return null;

            return Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                .FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Student> GetStudentHistory(ApplicationUser user)
        {
            return Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created).ToList();
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