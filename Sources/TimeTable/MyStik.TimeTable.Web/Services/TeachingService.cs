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

        public TeachingSemesterSummaryModel GetActivities(Semester semester, ApplicationUser user, ICollection<OrganiserMember> members)
        {
            var model = new TeachingSemesterSummaryModel
            {
                Semester = semester,
            };

            var courseSummaryService = new CourseService(_db);

            var courses = 
                _db.Activities.OfType<Course>()
                    .Where(x => x.SemesterGroups.Any(g => 
                         g.Semester.Id == semester.Id) &&
                         x.Dates.Any(d => d.Hosts.Any(h => h.UserId.Equals(user.Id))))
                .ToList();

            foreach (var course in courses)
            {
                var summary = courseSummaryService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }


            var officeHours = _db.Activities.OfType<OfficeHour>().Where(x =>
                x.Semester.Id == semester.Id && x.Owners.Any(y => y.Member.UserId.Equals(user.Id)))
                .ToList();


            foreach (var oh in officeHours)
            {
                var ohSummary = new OfficeHourCharacteristicModel()
                {
                    OfficeHour = oh
                };

                ohSummary.Date = oh.Dates.Where(x => x.Begin > DateTime.Now).OrderBy(x => x.Begin).FirstOrDefault();


                model.OfficeHours.Add(ohSummary);
            }





            var modules = new List<CurriculumModule>();
            foreach (var member in members)
            {
                modules.AddRange(_db.CurriculumModules.Where(x => x.MV.Id == member.Id).ToList());
            }


            foreach (var module in modules)
            {
                var mModel = new TeachingModuleSemesterModel
                {
                    Module = module
                };


                foreach (var moduleCourse in module.ModuleCourses)
                {
                    var semCourses =
                        moduleCourse.Nexus.Where(x => x.Course.SemesterGroups.Any(g => g.Semester.Id == semester.Id))
                            .Select(x => x.Course).Distinct().ToList();

                    foreach (var course in semCourses)
                    {
                        var summary = courseSummaryService.GetCourseSummary(course);
                        mModel.Courses.Add(summary);
                    }
                }

                model.Modules.Add(mModel);
            }

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