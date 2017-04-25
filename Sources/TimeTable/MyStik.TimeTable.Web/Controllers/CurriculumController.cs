using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using PagedList;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CurriculumController : BaseController
    {
        private readonly TimeTableInfoService _service = new TimeTableInfoService();


        public ActionResult Index()
        {
            var model = new CurriculumViewModel();

            ViewBag.Curricula = Db.Curricula.Where(c => c.Organiser.ShortName.Equals("FK 09")).Select(c => new SelectListItem
            {
                Text = c.ShortName + " (" + c.Name + ")",
                Value = c.Id.ToString(),
            });

            model.Curriculum = Db.Curricula.First();
            
            ViewBag.UserRight = GetUserRight();

            return View(model);
        }





        //
        // GET: /Course/
        public ActionResult Programs()
        {
            var user = AppUser;

            
            var progs = _service.GetCurriculums();
            ViewBag.Curriculums = progs;
            ViewBag.Semester = GetSemester();


            ViewBag.Faculties = Db.Organisers.Select(f => new SelectListItem
            {
                Text = f.ShortName,
                Value = f.ShortName,
            });

            /*
            ViewBag.Semesters = Db.Semesters.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name
            });
             */

            ViewBag.Semesters = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = GetSemester().Name,
                    Value = GetSemester().Name
                }
            };


            ViewBag.Curricula = Db.Curricula.Where(c => c.Organiser.ShortName.Equals("FK 09")).Select(c => new SelectListItem
            {
                Text = c.ShortName + " (" + c.Name + ")",
                Value = c.Id.ToString(),
            });



            /* das lassen wir weg, bis zur Neuimplementierung der Seite
            var curr = string.IsNullOrEmpty(user.Curriculum) ?
                Db.Curricula.First() :
                Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(user.Curriculum));
            */

            var curr = Db.Curricula.First();

            if (curr != null)
            {
                var semester = GetSemester();

                var semesterGroups = Db.SemesterGroups.Where(g =>
                    g.Semester.Id == semester.Id &&
                    g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id).OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


                var semGroups = semesterGroups.Select(semGroup => new SelectListItem
                {
                    Text = semGroup.FullName,
                    Value = semGroup.Id.ToString()
                }).ToList();

                ViewBag.Groups = semGroups;
            }




            GroupSelectionViewModel model = new GroupSelectionViewModel
            {
                Faculty = "FK 09",
                Curriculum = curr.ShortName,
                Group = "",
                Semester = GetSemester().Name
            };

            
            return View(model);
        }

        public ActionResult Lecturers()
        {
            return View();
        }

        public ActionResult Rooms()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult GroupList(Guid? semId, Guid currId)
        {
            var curr = Db.Curricula.SingleOrDefault(c => c.Id == currId);
            var semester = semId == null ? GetSemester() : Db.Semesters.SingleOrDefault(s => s.Id == semId.Value);

            var model = new CurriculumViewModel
            {
                Curriculum = curr,
            };

            if (curr != null && semester != null)
            {
                model.SemesterGroups = Db.SemesterGroups
                    .Where(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == model.Curriculum.Id)
                    .OrderBy(g => g.CapacityGroup.CurriculumGroup.Name)
                    .ThenBy(g => g.CapacityGroup.Name)
                    .ToList();
            }
            else
            {
                model.SemesterGroups = new List<SemesterGroup>();
            }

            return PartialView("_GroupList", model);
        }


        [HttpPost]
        public PartialViewResult CurriculaList(Guid orgId)
        {
            var model = new CurriculumViewModel();

            model.Curricula = Db.Curricula
                .Where(g => g.Organiser.Id == orgId)
                .OrderBy(g => g.ShortName)
                .ToList();

            return PartialView("_CurriculumList", model);
        }



        [HttpPost]
        public JsonResult LecturerList(Guid? orgId, string name)
        {

            var org = orgId != null
                ? Db.Organisers.SingleOrDefault(o => o.Id == orgId.Value)
                : Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

            if (org != null)
            {
                var list2 = org.Members.Where(l => (!string.IsNullOrEmpty(l.ShortName) && l.ShortName.StartsWith(name.ToUpper())) ||
                                                  (!string.IsNullOrEmpty(l.Name) && l.Name.ToUpper().StartsWith(name.ToUpper())))
                    .OrderBy(l => l.Name)
                    .Select(l => new
                    {
                        name = l.Name,
                        id = l.Id,
                    })
                    .Take(10);

                return Json(list2);
            }

            return null;
        }

        [HttpPost]
        public JsonResult RoomList(string number, DateTime? date, TimeSpan? from, TimeSpan? until, bool? useFree)
        {
            // Den User ermitteln
            var isOrgAdmin = IsOrgAdmin();


            IEnumerable<Room> roomList;

            if (useFree.HasValue && date.HasValue && @from.HasValue && until.HasValue)
            {
                DateTime begin = date.Value.AddHours(@from.Value.Hours).AddMinutes(@from.Value.Minutes);
                DateTime end = date.Value.AddHours(until.Value.Hours).AddMinutes(until.Value.Minutes);

                roomList = useFree.Value ? new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(begin, end, isOrgAdmin) : new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(isOrgAdmin);
                
            }
            else
            {
                roomList = new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(isOrgAdmin);
            }

            if (useFree.HasValue && useFree.Value)
            {
                var list = roomList.Where(l => l.Number.ToUpper().Contains(number.ToUpper()))
                    .OrderByDescending(l => l.Capacity)
                    .ThenBy(l => l.Number)
                    .Select(l => new
                    {
                        name = l.Number,
                        capacity = Math.Abs(l.Capacity),
                    })
                    .Take(10);
                return Json(list);
            }
            else
            {
                var list = roomList.Where(l => l.Number.ToUpper().Contains(number.ToUpper()))
                    .OrderBy(l => l.Number)
                    .Select(l => new
                    {
                        name = l.Number,
                        capacity = Math.Abs(l.Capacity),
                    })
                    .Take(10);
                return Json(list);
            }
        }

        [HttpPost]
        public JsonResult RoomListByOrg(Guid orgId, string number)
        {
            var isOrgAdmin = IsOrgAdmin(orgId);

            var roomList = new MyStik.TimeTable.Web.Services.RoomService().GetRooms(orgId, isOrgAdmin);

            var list = roomList.Where(l => l.Number.ToUpper().Contains(number.ToUpper()))
                .OrderBy(l => l.Number)
                .Select(l => new
                {
                    name = l.Number,
                    capacity = Math.Abs(l.Capacity),
                    id = l.Id
                })
                .Take(10);
            return Json(list);
        }



        [HttpPost]
        public JsonResult RoomListComplete(string number)
        {
            var roomList = new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(true);
            var list = roomList.Where(l => l.Number.ToUpper().Contains(number.ToUpper()))
                .OrderBy(l => l.Number)
                .Select(l => new
                {
                    name = l.Number,
                    capacity = Math.Abs(l.Capacity),
                })
                .Take(10);
            return Json(list);
        }

        [HttpPost]
        public JsonResult RoomListForDay(string number, string sem, int? day, TimeSpan? from, TimeSpan? until)
        {
            IEnumerable<Room> roomList;



           var allRooms = Db.Rooms.Where(r => r.Number.ToUpper().StartsWith(number.ToUpper())).OrderBy(r => r.Number).ToList();

            // Bei kurzer Eingabe nicht prüfen => performancegründe
            if (number.Length < 3)
            {
                var list = allRooms
                    .Select(l => new
                    {
                        name = l.Number,
                        capacity = Math.Abs(l.Capacity),
                    })
                    .Take(10)
                    .ToList();

                return Json(list);
            }

            
            
            var semester = new SemesterService().GetSemester(sem);

           if (semester != null && day.HasValue && @from.HasValue && until.HasValue)
           {
               var dayOfWeek = (DayOfWeek)day.Value;

               roomList = new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(dayOfWeek, from.Value, until.Value, semester, IsOrgAdmin(), allRooms);
           }
           else
           {
               roomList = new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(IsOrgAdmin(), allRooms);
           }


            var list2 = roomList
                .Select(l => new
                {
                    name = l.Number,
                    capacity = Math.Abs(l.Capacity),
                })
                .Take(10)
                .ToList();

            return Json(list2);
        }

        [HttpPost]
        public JsonResult CurriculumGroupList(string name)
        {
            var logger = LogManager.GetLogger("Course");

            // das sind nur die Gruppen, die es in diesem Semester auch gibt!
            var semester = GetSemester();

            var dummy = semester.Groups.ToList().OrderBy(g => g.FullName).ToList();

            var list = dummy.Where(g => g.FullName.ToUpper().StartsWith(name.ToUpper()))
                .Select(g => new
                {
                    name = g.FullName,
                }).ToList();

            return Json(list);
        }



        [HttpPost]
        public PartialViewResult CourseListByProgram(Guid semGroupId, bool compact=false)
        {
            var semester = GetSemester();
            var user = AppUser;

            var courses = Db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any(g =>
                g.Id == semGroupId)).OrderBy(c => c.Name).ToList();

            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);

            var model = new List<CourseSummaryModel>();

            var courseService = new CourseService(UserManager);

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);

                summary.State = ActivityService.GetActivityState(course.Occurrence, user, semester);
                summary.SemesterGroup = semGroup;

                var subscriptions = course.Occurrence.Subscriptions.ToList();
                summary.SubscriptionCountFit = 0;
                foreach (var subscription in subscriptions)
                {
                    var isInGroup = Db.Subscriptions.OfType<SemesterSubscription>()
                        .Any(s => s.UserId.Equals(subscription.UserId) && s.SemesterGroup.Id == semGroupId);

                    if (isInGroup)
                        summary.SubscriptionCountFit++;
                }

                model.Add(summary);
            }

            if (compact)
                return PartialView("_CourseListSummaryCompact", model);

            return PartialView("_CourseListSummary", model);
        }


        [HttpPost]
        public PartialViewResult CourseListByLecturer(string dozId)
        {
            var orgId = "FK 09";
            var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(orgId.ToUpper()));
            var member = organiser.Members.SingleOrDefault(m => m.ShortName.Equals(dozId));
            if (member == null)
                return PartialView("");

            // Alle Vorlesungen
            // Alle Sprechstunden
            // Alle Newsletter
            // Alle Veranstaltungen

            // => 

            var model = new LecturerCharacteristicModel { Lecturer = member };

            var semester = GetSemester();
            var user = AppUser;

            var courseService = new CourseService(UserManager);

            model.Courses = courseService.GetCourses(semester.Name, member);

            var ac = new ActivityService();
            foreach (var course in model.Courses)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();
                course.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();
                course.Rooms.AddRange(rooms);

                course.State = ActivityService.GetActivityState(course.Course.Occurrence, user, semester);
            }

            // Sprechstunden
            model.OfficeHours = courseService.GetOfficeHours(member);


            return PartialView("_MemberLecturer", model);
        }

        [HttpPost]
        public PartialViewResult CourseListByRoom(string roomId)
        {
            var model = new List<CourseSummaryModel>();

            var user = AppUser;
            var semester = GetSemester();

            var room = Db.Rooms.SingleOrDefault(l => l.Number.Equals(roomId));

            if (room != null)
            {
                var courses =
                    Db.Activities //.OfType<Course>()
                        .Where(c => c.Dates.Any(oc => oc.Rooms.Any(l => l.Id == room.Id)))
                        .ToList();

                foreach (var course in courses)
                {
                    var lectures =
                        Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

                    var summary = new CourseSummaryModel {Course = course};
                    summary.Lecturers.AddRange(lectures);

                    var days = (from occ in course.Dates
                        select
                            new
                            {
                                Day = occ.Begin.DayOfWeek,
                                Begin = occ.Begin.TimeOfDay,
                                End = occ.End.TimeOfDay,
                            }).Distinct();

                    foreach (var day in days)
                    {
                        var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                        var courseDate = new CourseDateModel
                        {
                            DayOfWeek = day.Day,
                            StartTime = day.Begin,
                            EndTime = day.End,
                            DefaultDate = defaultDay.Begin
                        };

                        summary.Dates.Add(courseDate);
                    }

                    summary.State = ActivityService.GetActivityState(course.Occurrence, user, semester);

                    model.Add(summary);
                }
            }
            return PartialView("_CourseListSummary", model);
        }

        public ActionResult List()
        {
            var model = Db.Curricula.ToList();

            return View(model);
        }

        [HttpPost]
        public PartialViewResult Details(Guid currId)
        {
            var model = Db.Curricula.SingleOrDefault(c => c.Id == currId);

            return PartialView("_CurriculumDetails", model);
        }

        public ActionResult AddAlias(Guid id)
        {
            var model = new CurriculumCreateAliasModel
            {
                CurriculumId = id
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddAlias(CurriculumCreateAliasModel model)
        {
            /*
            var curriculum = Db.Curricula.SingleOrDefault(c => c.Id == model.CurriculumId);

            var alias = curriculum.GroupAliases.SingleOrDefault(a => a.Name.Equals(model.AliasName));

            if (alias == null)
            {
                alias = new GroupAlias
                {
                    Curriculum = curriculum,
                    Name = model.AliasName,
                };

                Db.GroupAliases.Add(alias);
            }

            var tpl = alias.GroupTemplates.SingleOrDefault(g => g.CurriculumGroupName.Equals(model.CurrGroupName));
            if (tpl == null)
            {
                tpl = new GroupTemplate
                {
                    Alias = alias,
                    CurriculumGroupName = model.CurrGroupName,
                    SemesterGroupName = model.SemGroupName
                };
                Db.GroupTemplates.Add(tpl);
            }

            Db.SaveChanges();
            */
            return RedirectToAction("Index");
        }

    }
}