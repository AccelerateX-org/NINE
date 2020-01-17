using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class TeachingService
    {
        private TimeTableDbContext _db;

        public TeachingService(TimeTableDbContext db)
        {
            _db = db;
        }

        public TeachingSemesterSummaryModel GetActivities(Semester semester, ApplicationUser user)
        {
            var courses = 
                _db.Activities.OfType<Course>()
                    .Where(x => x.SemesterGroups.Any(g => 
                         g.Semester.Id == semester.Id) &&
                         x.Dates.Any(d => d.Hosts.Any(h => h.UserId.Equals(user.Id))))
                .ToList();


            var officeHours = _db.Activities.OfType<OfficeHour>().Where(x =>
                x.Semester.Id == semester.Id && x.Owners.Any(y => y.Member.UserId.Equals(user.Id)))
                .ToList();

            var model = new TeachingSemesterSummaryModel
            {
                Semester = semester,
                Courses = courses,
                OfficeHours = officeHours,
            };

            return model;
        }

        public List<Thesis> GetActiveTheses(ApplicationUser user)
        {
            var theses = _db.Theses.Where(x =>
                    x.Supervisors.Any(m => m.Member.UserId.Equals(user.Id)) && // Alle Abschlussarbeiten für den Betreuer
                    x.GradeDate == null // noch nicht benotet
            ).ToList();

            return theses;
        }
    }
}