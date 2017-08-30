using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.Curriculum;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumController : BaseController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new CurriculumViewModel();
            var org = GetMyOrganisation();

            ViewBag.Curricula = Db.Curricula.Where(c => c.Organiser.ShortName.Equals("FK 09")).Select(c => new SelectListItem
            {
                Text = c.ShortName + " (" + c.Name + ")",
                Value = c.Id.ToString(),
            });

            model.Curriculum = Db.Curricula.First();
            
            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Structure(Guid id)
        {
            var org = GetMyOrganisation();

            var model = Db.Curricula.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Programs()
        {
            var user = AppUser;

            var _service = new TimeTableInfoService(Db);
            
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
                Faculty = "FK 01",
                Curriculum = curr.ShortName,
                Group = "",
                Semester = GetSemester().Name
            };

            
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Lecturers()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Rooms()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="currId"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GroupList(Guid? semId, Guid currId, bool activeOnly=true)
        {
            var curr = Db.Curricula.SingleOrDefault(c => c.Id == currId);
            var semester = semId == null ? GetSemester() : Db.Semesters.SingleOrDefault(s => s.Id == semId.Value);

            var model = new CurriculumViewModel
            {
                Curriculum = curr,
            };

            if (curr != null && semester != null)
            {
                if (activeOnly)
                {
                    model.SemesterGroups = Db.SemesterGroups
                        .Where(g =>
                            g.Semester.Id == semester.Id &&
                            g.IsAvailable &&
                            g.CapacityGroup.CurriculumGroup.Curriculum.Id == model.Curriculum.Id)
                        .OrderBy(g => g.CapacityGroup.CurriculumGroup.Name)
                        .ThenBy(g => g.CapacityGroup.Name)
                        .ToList();
                }
                else
                {
                    model.SemesterGroups = Db.SemesterGroups
                        .Where(g =>
                            g.Semester.Id == semester.Id &&
                            g.CapacityGroup.CurriculumGroup.Curriculum.Id == model.Curriculum.Id)
                        .OrderBy(g => g.CapacityGroup.CurriculumGroup.Name)
                        .ThenBy(g => g.CapacityGroup.Name)
                        .ToList();
                }
            }
            else
            {
                model.SemesterGroups = new List<SemesterGroup>();
            }

            return PartialView("_GroupList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult TopicList(Guid groupId)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == groupId);

            var model = new CurriculumViewModel();

            if (semGroup != null)
            {
                var curr = semGroup.CapacityGroup.CurriculumGroup.Curriculum;
                var semester = semGroup.Semester;

                model.Curriculum = curr;


                model.Topics = Db.SemesterTopics.Where(x =>
                    x.Semester.Id == semester.Id &&
                    x.Topic.Chapter.Curriculum.Id == curr.Id &&
                    x.Activities.Any(s => s.SemesterGroups.Any(g => g.Id == semGroup.Id))
                ).ToList();
            }
            else
            {
                model.Topics = new List<SemesterTopic>();
            }

            return PartialView("_TopicList", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CurriculaList(Guid? semId, Guid orgId, bool activeOnly=true)
        {
            var semester = semId == null ? GetSemester() : Db.Semesters.SingleOrDefault(s => s.Id == semId.Value);
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            ICollection<Curriculum> currs;
            if (activeOnly)
            {
                var semService = new SemesterService(Db);
                currs = semService.GetActiveCurricula(org, semester);

            }
            else
            {
                currs = Db.Curricula.Where(x => x.Organiser.Id == orgId).ToList();
            }

            var model = currs
                .OrderBy(g => g.ShortName)
                .ToList();

            return PartialView("_CurriculumSelectList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CurriculumGroupSelectList(Guid currId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var groups = curr.CurriculumGroups.ToList();

            var model = groups
                .OrderBy(g => g.Name)
                .ToList();

            return PartialView("_CurriculumGroupSelectList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CapacityGroupSelectList(Guid currGroupId)
        {
            var curr = Db.CurriculumGroups.SingleOrDefault(x => x.Id == currGroupId);

            var groups = curr.CapacityGroups.ToList();

            var model = groups
                .OrderBy(g => g.Name)
                .ToList();

            return PartialView("_CapacityGroupSelectList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CurriculumChapterSelectList(Guid currId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var groups = curr.Chapters.ToList();

            var model = groups
                .OrderBy(g => g.Name)
                .ToList();

            return PartialView("_ChapterSelectList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CurriculumTopicSelectList(Guid chapterId)
        {
            var curr = Db.CurriculumChapters.SingleOrDefault(x => x.Id == chapterId);

            var groups = curr.Topics.ToList();

            var model = groups
                .OrderBy(g => g.Name)
                .ToList();

            return PartialView("_TopicSelectList", model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>

        [HttpPost]
        public PartialViewResult SemesterList(Guid orgId, bool activeOnly = true)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            List<Semester> semesters;

            if (activeOnly)
            {
                semesters = Db.Semesters.Where(x => x.EndCourses >= DateTime.Today &&
                                                    x.Groups.Any(g =>
                                                        g.IsAvailable &&
                                                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id ==
                                                        org.Id)).ToList();
            }
            else
            {
                semesters = Db.Semesters.Where(x => x.EndCourses >= DateTime.Today &&
                                                    x.Groups.Any(g =>
                                                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id ==
                                                        org.Id)).ToList();
            }


            var model = semesters
                .OrderBy(g => g.Name)
                .ToList();

            return PartialView("_SemesterSelectList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="name"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult LecturerList(Guid? orgId, string name)
        {

            var org = orgId != null
                ? Db.Organisers.SingleOrDefault(o => o.Id == orgId.Value)
                : GetMyOrganisation();

            if (org != null)
            {
                var list2 = org.Members.Where(l => (!string.IsNullOrEmpty(l.ShortName) && l.ShortName.StartsWith(name.ToUpper())) ||
                                                  (!string.IsNullOrEmpty(l.Name) && l.Name.ToUpper().StartsWith(name.ToUpper())))
                    .OrderBy(l => l.Name)
                    .Select(l => new
                    {
                        name = l.Name,
                        shortname = l.ShortName,
                        id = l.Id,
                    })
                    .Take(10);

                return Json(list2);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <param name="useFree"></param>
        /// <returns></returns>
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
                        name = l.FullName,
                        capacity = Math.Abs(l.Capacity),
                    })
                    .Take(10);
                return Json(list);
            }
            else
            {
                var list = roomList.Where(l => l.Number.ToUpper().Contains(number.ToUpper()))
                    .OrderBy(l => l.FullName)
                    .Select(l => new
                    {
                        name = l.Number,
                        capacity = Math.Abs(l.Capacity),
                    })
                    .Take(10);
                return Json(list);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RoomListByOrg(Guid orgId, string number)
        {
            var isOrgAdmin = IsRoomAdmin(orgId);

            var roomList = new MyStik.TimeTable.Web.Services.RoomService().GetRooms(orgId, isOrgAdmin);

            var list = roomList.Where(l => l.Number.ToUpper().Contains(number.ToUpper()))
                .OrderBy(l => l.Number)
                .Select(l => new
                {
                    name = l.FullName,
                    capacity = Math.Abs(l.Capacity),
                    id = l.Id
                })
                .Take(10);
            return Json(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult RoomListComplete(string number)
        {
            var roomList = new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(true);
            var list = roomList.Where(l => l.Number.ToUpper().Contains(number.ToUpper()))
                .OrderBy(l => l.Number)
                .Select(l => new
                {
                    name = l.FullName,
                    capacity = Math.Abs(l.Capacity),
                })
                .Take(10);
            return Json(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="sem"></param>
        /// <param name="day"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
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
                        name = l.FullName,
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
                    name = l.FullName,
                    capacity = Math.Abs(l.Capacity),
                })
                .Take(10)
                .ToList();

            return Json(list2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <param name="compact"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dozId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var model = Db.Curricula.ToList();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Details(Guid currId)
        {
            var model = Db.Curricula.SingleOrDefault(c => c.Id == currId);

            return PartialView("_CurriculumDetails", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddAlias(Guid id)
        {
            var model = new CurriculumCreateAliasModel
            {
                CurriculumId = id
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Criterias(Guid id)
        {
            var model = Db.Curricula.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ImportModuleCatalog(Guid id)
        {
            var curriculum =
                Db.Curricula.SingleOrDefault(x => x.Id == id);
            if (curriculum == null)
                return RedirectToAction("Criterias", new { id = id });

            // Hole Modulkatalog
            var mcs = new ModuleCatalogService();
            var mc = mcs.GetCatalog(curriculum.Organiser.ShortName, curriculum.ShortName);
            if (mc == null)
                return RedirectToAction("Criterias", new { id = id });

            // Importiere den Katalog
            var acs = new AccreditationService();
            // aus jedem Modul ein Kriterium machen
            // alle WPMs werden zu einem Kriterium zusammengefasst
            acs.ImportModuleCatalogSingle(curriculum.Organiser.ShortName, curriculum.ShortName);


            // Zurück zur Startseite
            return RedirectToAction("Criterias", new {id = id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteModuleCatalog(Guid id)
        {
            var curriculum =
                Db.Curricula.SingleOrDefault(x => x.Id == id);


            var allCriteria = curriculum.Criterias.ToList();
            foreach (var criteria in allCriteria)
            {
                foreach (var accreditation in criteria.Accreditations.ToList())
                {
                    var curriculumModule = accreditation.Module;

                    var allCourses = curriculumModule.ModuleCourses.ToList();

                    foreach (var moduleCourse in allCourses)
                    {
                        foreach (var capCourse in moduleCourse.CapacityCourses.ToList())
                        {
                            moduleCourse.CapacityCourses.Remove(capCourse);
                            capCourse.Course = null;

                            Db.CapacityCourses.Remove(capCourse);
                        }

                        Db.ModuleCourses.Remove(moduleCourse);
                    }

                    Db.CurriculumModules.Remove(curriculumModule);
                    Db.Accreditations.Remove(accreditation);
                }
                Db.Criterias.Remove(criteria);
            }
            Db.SaveChanges();


            // Zurück zur Startseite
            return RedirectToAction("Criterias", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var model = Db.Curricula.SingleOrDefault(x => x.Id == id);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Curriculum model)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == model.Id);

            cur.Name = model.Name;
            cur.ShortName = model.ShortName;
            Db.SaveChanges();

            return RedirectToAction("Structure", new {id = cur.Id});
        }

    }
}
