using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ConflictsController : BaseController
    {
        // GET: Conflicts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rooms(Guid id, string day)
        {
            var user = GetCurrentUser();

            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var orgs = model.Assignments.Select(x => x.Organiser).Distinct().ToList();
            var org = orgs.Where(x =>
                    x.Members.Any(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id) && m.IsRoomAdmin))
                .FirstOrDefault();

            if (org == null)
                org = orgs.FirstOrDefault();

            if (org == null)
                org = GetMyOrganisation();

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Organiser = org;
            ViewBag.Day = day;

            return View(model);
        }

        public PartialViewResult Date(Guid dateId)
        {
            // erster Versuch => ein konkretes Datum
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);

            if (date != null)
                return PartialView("_Date", date);

            // zweiter Versuch => die Activity
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == dateId);
            if (course != null)
                return PartialView("_Course", course);

            return PartialView("_NotFound");
        }


        public ActionResult Label(Guid orgId, Guid semId, Guid? currId, Guid? labelId, string day)
        {
            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganiser(orgId);

            var curr = currId.HasValue ? 
                Db.Curricula.SingleOrDefault(x => x.Id == currId.Value) : 
                Db.Curricula.FirstOrDefault(x => x.Organiser.Id == org.Id && !x.IsDeprecated);

            
            var label = labelId.HasValue ? 
                Db.ItemLabels.SingleOrDefault(x => x.Id == labelId.Value) : 
                curr.LabelSet.ItemLabels.FirstOrDefault();

            var model = new SemesterActiveViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Organiser = org,
                Label = label,
                Courses = new List<CourseSummaryModel>()
            };

            var segments = Db.SemesterDates.Where(x => x.Semester.Id == semId &&
                                                       x.Organiser != null && x.Organiser.Id == orgId && x.HasCourses)
                .ToList();

            if (segments.Any())
            {
                var maxSegment = segments.First();
                var maxDays = 0.0;

                foreach (var segment in segments)
                {
                    var days = (segment.To.Date - segment.From.Date).TotalDays;
                    if (days > maxDays)
                    {
                        maxDays = days;
                        maxSegment = segment;
                    }
                }

                model.Segment = maxSegment;
            }

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Organisers = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });
            ViewBag.Semesters = Db.Semesters.Where(x => x.Id == semester.Id).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            ViewBag.Segments = segments.Select(c => new SelectListItem
            {
                Text = c.Description,
                Value = c.Id.ToString(),
            });
            ViewBag.Curricula = Db.Curricula.Where(x => x.Organiser.Id == org.Id && !x.IsDeprecated).OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });
            ViewBag.Labels = curr.LabelSet.ItemLabels.OrderBy(x => x.Name).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        public ActionResult Catalog(Guid orgId, Guid semId, Guid? catId, string day)
        {
            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganiser(orgId);

            var curr = Db.Curricula.FirstOrDefault(x => x.Organiser.Id == org.Id && !x.IsDeprecated);
            var label = curr.LabelSet.ItemLabels.FirstOrDefault();

            var catalog = org.ModuleCatalogs.FirstOrDefault();

            var model = new SemesterActiveViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Organiser = org,
                Label = label,
                Catalog = catalog,
                Courses = new List<CourseSummaryModel>()
            };

            var segments = Db.SemesterDates.Where(x => x.Semester.Id == semId &&
                                                       x.Organiser != null && x.Organiser.Id == orgId && x.HasCourses)
                .ToList();

            if (segments.Any())
            {
                var maxSegment = segments.First();
                var maxDays = 0.0;

                foreach (var segment in segments)
                {
                    var days = (segment.To.Date - segment.From.Date).TotalDays;
                    if (days > maxDays)
                    {
                        maxDays = days;
                        maxSegment = segment;
                    }
                }

                model.Segment = maxSegment;
            }

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Organisers = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });
            ViewBag.Semesters = Db.Semesters.Where(x => x.Id == semester.Id).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            ViewBag.Segments = segments.Select(c => new SelectListItem
            {
                Text = c.Description,
                Value = c.Id.ToString(),
            });
            ViewBag.Catalogs = Db.CurriculumModuleCatalogs.Where(x => x.Organiser.Id == org.Id).OrderBy(x => x.Tag).Select(c => new SelectListItem
            {
                Text = c.Tag,
                Value = c.Id.ToString(),
            });
            ViewBag.Curricula = Db.Curricula.Where(x => x.Organiser.Id == org.Id && !x.IsDeprecated).OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });
            ViewBag.Labels = curr.LabelSet.ItemLabels.OrderBy(x => x.Name).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });


            return View(model);
        }



        [HttpPost]
        public PartialViewResult GetPlanGridLabel(Guid orgId, Guid semId, Guid segId, Guid currId, Guid labelId)
        {
            var semester = SemesterService.GetSemester(semId);
            var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            var courses = new List<Course>();

            var org = GetOrganiser(orgId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            courses.AddRange(Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id &&
                    x.Organiser.Id == org.Id &&
                    x.LabelSet != null &&
                    x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                .ToList());

            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }

            var model = new SemesterActiveViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Segment = segment,
                Organiser = org,
                Label = label,
                Courses = courseSummaries
            };

            ViewBag.UserRight = GetUserRight(org);


            return PartialView("_PlanGrid", model);
        }


        [HttpPost]
        public PartialViewResult GetPlanGridCatalog(Guid orgId, Guid semId, Guid segId, Guid catId, Guid currId, Guid labelId)
        {
            var semester = SemesterService.GetSemester(semId);
            var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == catId);

            var courses = new List<Course>();

            var org = GetOrganiser(orgId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            courses.AddRange(Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id && 
                    x.Organiser.Id == catalog.Organiser.Id &&
                    x.SubjectTeachings.Any(t => t.Subject.Module.Catalog.Id == catalog.Id))
                .ToList());

            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }

            var model = new SemesterActiveViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Segment = segment,
                Catalog = catalog,
                Organiser = org,
                Label = label,
                Courses = courseSummaries
            };

            ViewBag.UserRight = GetUserRight(org);


            return PartialView("_PlanGrid", model);
        }



        [HttpPost]
        public PartialViewResult GetSegments(Guid orgId, Guid semId)
        {
            var model = Db.SemesterDates.Where(x => x.Semester.Id == semId &&
                                                    x.Organiser != null && x.Organiser.Id == orgId && x.HasCourses)
                .OrderBy(x => x.From)
                .ToList();

            return PartialView("_SegmentSelectList", model);
        }

        [HttpPost]
        public PartialViewResult GetCurricula(Guid orgId, Guid semId)
        {
            var model = Db.Curricula.Where(x => x.Organiser != null && x.Organiser.Id == orgId && !x.IsDeprecated)
                .OrderBy(x => x.ShortName)
                .ToList();

            return PartialView("_CurriculaSelectList", model);
        }

        [HttpPost]
        public PartialViewResult GetLabels(Guid orgId, Guid semId, Guid currId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            if (curr.LabelSet != null)
            {
                return PartialView("_LabelSelectList", curr.LabelSet.ItemLabels.OrderBy(x => x.Name).ToList());
            }

            return PartialView("_LabelSelectList", new List<ItemLabel>());

        }


    }
}