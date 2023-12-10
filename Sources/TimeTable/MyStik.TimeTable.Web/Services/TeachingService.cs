using System;
using System.Collections.Generic;
using System.Linq;
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

            // die ich halte
            var courses = 
                _db.Activities.OfType<Course>()
                    .Where(x => x.Semester.Id == semester.Id &&
                         (x.Dates.Any(d => d.Hosts.Any(h => h.UserId.Equals(user.Id)))))
                .ToList();

            foreach (var course in courses)
            {
                var summary = courseSummaryService.GetCourseSummary(course);

                summary.Subscription = course.Occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(user.Id));
                
                model.Courses.Add(summary);
            }

            // wo ich eingeschrieben bin
            courses =
                _db.Activities.OfType<Course>()
                    .Where(x => x.Semester.Id == semester.Id &&
                                 x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                    .ToList();

            foreach (var course in courses)
            {
                // doppelte raus
                if (model.Courses.Any(x => x.Course.Id == course.Id)) continue;
                
                var summary = courseSummaryService.GetCourseSummary(course);

                summary.Subscription =
                    course.Occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(user.Id));

                model.Courses.Add(summary);
            }


            var officeHours = _db.Activities.OfType<OfficeHour>().Where(x =>
                x.Semester.Id == semester.Id && 
                (x.Owners.Any(y => y.Member.UserId.Equals(user.Id))))
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




            /*
            var modules = new List<CurriculumModule>();
            foreach (var member in members)
            {
                modules.AddRange(_db.CurriculumModules.Where(x => 
                    x.ModuleResponsibilities.Any(m => 
                        m.Member.Id == member.Id)).ToList());
            }

            foreach (var module in modules)
            {
                var mModel = new TeachingModuleSemesterModel
                {
                    Module = module
                };

                model.Modules.Add(mModel);
            }
            */

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