using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Areas.HelpPage;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using RoomService = MyStik.TimeTable.Web.Services.RoomService;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ConflictsController : BaseController
    {
        // GET: Conflicts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rooms(Guid id, string day, Guid? segId)
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

            if (string.IsNullOrEmpty(day))
            {
                if (segId.HasValue)
                {
                    var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segId.Value);
                    day = segment.From.ToString("yyyy-MM-dd", new CultureInfo("de-DE"));
                }
            }


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
            {
                var model = new RoomConflictModel();
                model.Course = date.Activity as Course;
                model.Date = date;
                model.Alternatives = new List<RoomAlternativeModel>();

                var room = date.Rooms.FirstOrDefault();
                var org = date.Activity.Organiser;
                var sem = date.Activity.Semester;

                var roomService = new RoomService();

                var dayList = new List<DateTime>();
                foreach (var activityDate in date.Activity.Dates.OrderBy(x => x.Begin))
                {
                    /*
                    var freeRoomsDay = roomService.GetFreeRooms(org.Id, true, activityDate.Begin, activityDate.End);

                    var alt = new RoomAlternativeModel
                    {
                        Date = activityDate,
                        AvailableRooms = freeRoomsDay.ToList()
                    };

                    model.Alternatives.Add(alt);
                    */

                    dayList.Add(activityDate.Begin.Date);
                }
                var freeRoomsSeries = roomService.GetFreeRooms(org.Id, true, dayList, date.Begin.TimeOfDay, date.End.TimeOfDay);
                model.AvailableRooms = freeRoomsSeries.ToList();


                return PartialView("_Date", model);
            }


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
                Db.Curricula.Where(x => x.Organiser.Id == org.Id && !x.IsDeprecated).OrderBy(x => x.ShortName).FirstOrDefault();

            
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

        public ActionResult Segments(Guid orgId, Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganiser(orgId);


            var model = new SemesterActiveViewModel
            {
                Curriculum = null,
                Semester = semester,
                Organiser = org,
                Label = null,
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


            return View(model);
        }

        public ActionResult AssignSegment(Guid courseId, Guid segId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segId);

            var sem = course.Semester;
            var org = course.Organiser;

            if (segment.Semester.Id == sem.Id)
            {
                course.Segment = segment;
                Db.SaveChanges();
            }

            return RedirectToAction("Segments", new { orgId = org.Id, semId = sem.Id });
        }


        public ActionResult DeleteDate(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);

            var course = date.Activity as Course;

            if (course != null)
            {
                var segment = course.Segment;

                var sem = course.Semester;
                var org = course.Organiser;

                DeleteService.DeleteActivityDate(id);

                return RedirectToAction("Segments", new { orgId = org.Id, semId = sem.Id });

            }

            return null;
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

            var cs = new CourseInfoService();
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

            var cs = new CourseInfoService();
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
        public PartialViewResult GetConflictTable(Guid orgId, Guid semId, Guid segId)
        {
            var semester = SemesterService.GetSemester(semId);
            var segment = Db.SemesterDates.SingleOrDefault(x => x.Id == segId);

            var courses = new List<Course>();

            var org = GetOrganiser(orgId);

            var segBorderFrom = segment.From.Date;
            var segBorderTo = segment.To.Date.AddDays(1);

            courses.AddRange(Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id &&
                    x.Organiser.Id == org.Id &&
                    (x.Segment == null || (x.Segment != null && x.Segment.Id == segment.Id)) &&
                    x.Dates.Any(d => d.Begin < segBorderFrom || d.End > segBorderTo))
                .ToList());

            var cs = new CourseInfoService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }

            var model = new SemesterActiveViewModel
            {
                Curriculum = null,
                Semester = semester,
                Segment = segment,
                Organiser = org,
                Label = null,
                Courses = courseSummaries
            };

            ViewBag.UserRight = GetUserRight(org);


            return PartialView("_ConflictTable", model);
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

        /// <summary>
        /// Calculates the conflicts for the course
        /// optional against a given sequence of regular time span
        /// </summary>
        /// <param name="courseId">Course the conflicts should be calculated</param>
        /// <param name="dayOfWeek"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        /*
        [HttpPost]
        public PartialViewResult GetConflicts(Guid courseId, Guid? segId, int? nDayOfWeek, string sBegin, string sEnd)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);

            var courseService = new CourseInfoService(Db);

            var ownConflicts = courseService.GetConflicts(course);
            var dayOfWeek = (DayOfWeek)Enum.GetValues(typeof(DayOfWeek)).GetValue(nDayOfWeek.Value);
            var begin = DateTime.Parse(sBegin);
            var end = DateTime.Parse(sEnd);

            // Berechnung der neuen Termine aus Segement und Wochentag
            var altPeriods = SemesterService.GetDatePeriods(segId.Value, dayOfWeek, begin, end);
            var altConflicts  = courseService.GetConflicts(course, altPeriods.ToList());

            var model = new ConflictSummaryModel
            {
                Course = course,
                OwnConflicts = ownConflicts,
                AltConflicts = altConflicts
            };
            

            return PartialView("_ConflictGrid", model);

        }
        */


        public PartialViewResult GetRoomsForDate(Guid dateId)
        {
            // erster Versuch => ein konkretes Datum
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);

            if (date != null)
            {
                var model = new RoomConflictModel();
                model.Course = date.Activity as Course;
                model.Date = date;
                model.Alternatives = new List<RoomAlternativeModel>();

                var room = date.Rooms.FirstOrDefault();
                var org = date.Activity.Organiser;
                var sem = date.Activity.Semester;

                var roomService = new RoomService();

                var dayList = new List<DateTime>();
                foreach (var activityDate in date.Activity.Dates.OrderBy(x => x.Begin))
                {
                    var freeRoomsDay = roomService.GetFreeRooms(org.Id, true, activityDate.Begin, activityDate.End);

                    var alt = new RoomAlternativeModel
                    {
                        Date = activityDate,
                        AvailableRooms = freeRoomsDay.ToList()
                    };

                    model.Alternatives.Add(alt);

                    dayList.Add(activityDate.Begin.Date);
                }

                return PartialView("_DateList", model);
            }

            return PartialView("_NotFound");
        }

        [HttpPost]
        public PartialViewResult ShowDates(Guid dateId, int weekDay, string begin)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == dateId);

            var date = course.Dates.FirstOrDefault();
            var duration = date.End - date.Begin;


            var org = course.Organiser;
            var segment = course.Segment;
            var from = DateTime.Parse(begin);
            var to = from.Add(duration);

            var dayofWeek = DayOfWeek.Monday;
            switch (weekDay)
            {
                case 1: dayofWeek = DayOfWeek.Monday; break;
                case 2: dayofWeek = DayOfWeek.Tuesday; break;
                case 3: dayofWeek = DayOfWeek.Wednesday; break;
                case 4: dayofWeek = DayOfWeek.Thursday; break;
                case 5: dayofWeek = DayOfWeek.Friday; break;
            }

            var datePeriods = SemesterService.GetDatePeriods(segment.Id, dayofWeek, from, to);

            var model = new List<DateAlternativeModel>();

            foreach (var datePeriod in datePeriods)
            {
                var alt = new DateAlternativeModel
                {
                    Course = course,
                    DatePeriod = datePeriod,
                    Rooms = new List<Room>()
                };

                model.Add(alt);
            }


            return PartialView("_Alternative", model);
        }


        [HttpPost]
        public PartialViewResult Move(Guid dateId, int weekDay, string begin)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == dateId);

            var date = course.Dates.FirstOrDefault();
            var duration = date.End - date.Begin;

            var from = DateTime.Parse(begin);
            var to = from.Add(duration);

            var dayofWeek = DayOfWeek.Monday;
            switch (weekDay)
            {
                case 1: dayofWeek = DayOfWeek.Monday; break;
                case 2: dayofWeek = DayOfWeek.Tuesday; break;
                case 3: dayofWeek = DayOfWeek.Wednesday; break;
                case 4: dayofWeek = DayOfWeek.Thursday; break;
                case 5: dayofWeek = DayOfWeek.Friday; break;
            }

            var model = MoveDates(course, dayofWeek, from, to);

            return PartialView("_Alternative", model);
        }


        private List<DateAlternativeModel> MoveDates(Course course, DayOfWeek dayofWeek, DateTime from, DateTime to)
        {
            var courseService = new CourseInfoService(Db);
            var summary = courseService.GetCourseSummary(course);

            var org = course.Organiser;
            var segment = course.Segment;

            var orderedDates = course.Dates.OrderBy(x => x.Begin).ToList();
            var refDate = orderedDates.FirstOrDefault();

            var model = new List<DateAlternativeModel>();

            if (refDate == null)
                return model;

            if (refDate.Begin.DayOfWeek == dayofWeek)
            {
                // nur Zeiten tauschen
                foreach (var courseDate in orderedDates)
                {
                    courseDate.Begin = courseDate.Begin.Date.Add(from.TimeOfDay);
                    courseDate.End = courseDate.End.Date.Add(to.TimeOfDay);

                    var alt = new DateAlternativeModel
                    {
                        Course = course,
                        DatePeriod = new DatePeriod { Begin = courseDate.Begin, End = courseDate.End},
                        Rooms = new List<Room>()
                    };
                    model.Add(alt);

                }
            }
            else
            {
                var datePeriods = SemesterService.GetDatePeriods(segment.Id, dayofWeek, from, to).OrderBy(x => x.Begin).ToList();

                var nDates = Math.Min(orderedDates.Count, datePeriods.Count);

                for (var i = 0; i < nDates; i++)
                {
                    orderedDates[i].Begin = datePeriods[i].Begin;
                    orderedDates[i].End = datePeriods[i].End;

                    var alt = new DateAlternativeModel
                    {
                        Course = course,
                        DatePeriod = datePeriods[i],
                        Rooms = new List<Room>()
                    };

                    model.Add(alt);
                }

                // Kurs hat jetzt mehr Termine
                if (datePeriods.Count > orderedDates.Count)
                {
                    // neue Dates anfügen
                    for (var i = nDates; i < datePeriods.Count; i++)
                    {
                        var activityDate = new ActivityDate
                        {
                            Activity = course,
                            Begin = datePeriods[i].Begin,
                            End = datePeriods[i].End,
                            Occurrence = new Occurrence
                            {
                                Capacity = int.MaxValue,
                                IsAvailable = true,
                                IsCanceled = false,
                                IsMoved = false,
                                FromIsRestricted = false,
                                UntilIsRestricted = false,
                                UseGroups = false,
                            }
                        };

                        foreach (var room in summary.Rooms)
                        {
                            activityDate.Rooms.Add(room);
                        }

                        foreach (var lecturer in summary.Lecturers)
                        {
                            activityDate.Hosts.Add(lecturer);
                        }

                        Db.ActivityDates.Add(activityDate);

                        var alt = new DateAlternativeModel
                        {
                            Course = course,
                            DatePeriod = datePeriods[i],
                            Rooms = new List<Room>()
                        };

                        model.Add(alt);
                    }
                }

                // Kurs hat jetzt weniger Termine
                if (datePeriods.Count < orderedDates.Count)
                {
                    // Kurstermine löschen
                    for (var i = nDates; i < orderedDates.Count; i++)
                    {
                        var courseDate = orderedDates[i];
                        courseDate.Hosts.Clear();
                        courseDate.Rooms.Clear();

                        var bookings = Db.RoomBookings.Where(x => x.Date.Id == courseDate.Id).ToList();

                        foreach (var booking in bookings)
                        {
                            Db.RoomBookings.Remove(booking);
                        }

                        course.Dates.Remove(courseDate);

                        Db.ActivityDates.Remove(courseDate);
                    }
                }
            }


            Db.SaveChanges();

            return model;
        }



        [HttpPost]
        public PartialViewResult MoveFromDrop(Guid dateId, string begin)
        {
            var course = Db.Activities.OfType<Course>().Include(activity => activity.Dates).SingleOrDefault(x => x.Id == dateId);

            var date = course.Dates.FirstOrDefault();
            var duration = date.End - date.Begin;

            var from = DateTime.Parse(begin);
            var to = from.Add(duration);

            var dayofWeek = from.DayOfWeek;

            var model = MoveDates(course, dayofWeek, from, to);

            return null;
        }


        [HttpPost]
        public PartialViewResult MoveFromResize(Guid dateId, string begin, string end)
        {
            var course = Db.Activities.OfType<Course>().Include(activity => activity.Dates).SingleOrDefault(x => x.Id == dateId);

            var from = DateTime.Parse(begin);
            var to = DateTime.Parse(end);

            var dayofWeek = from.DayOfWeek;

            var model = MoveDates(course, dayofWeek, from, to);

            return null;
        }



        [HttpPost]
        public PartialViewResult CheckAlternative(Guid dateId, int weekDay, string begin)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == dateId);

            var date = course.Dates.FirstOrDefault();
            var duration = date.End - date.Begin;


            var org = course.Organiser;
            var segment = course.Segment;
            var from = DateTime.Parse(begin);
            var to = from.Add(duration);

            var dayofWeek = DayOfWeek.Monday;
            switch (weekDay)
            {
                case 1: dayofWeek = DayOfWeek.Monday; break;
                case 2: dayofWeek = DayOfWeek.Tuesday; break;
                case 3: dayofWeek = DayOfWeek.Wednesday; break;
                case 4: dayofWeek = DayOfWeek.Thursday; break;
                case 5: dayofWeek = DayOfWeek.Friday; break;
            }

            var roomService = new RoomService();

            var datePeriods = SemesterService.GetDatePeriods(segment.Id, dayofWeek, from, to);

            var model = new List<DateAlternativeModel>();

            foreach (var datePeriod in datePeriods)
            {
                var alt = new DateAlternativeModel
                {
                    Course = course,
                    DatePeriod = datePeriod,
                    Rooms = new List<Room>()
                };

                var rooms = roomService.GetAvaliableRooms(org.Id, datePeriod.Begin, datePeriod.End, true);

                foreach (var room in rooms)
                {
                    alt.Rooms.Add(room.Room);
                }

                model.Add(alt);
            }


            return PartialView("_Alternative", model);
        }


    }
}