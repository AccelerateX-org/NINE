using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.Horst;
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
        public ActionResult Index(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumViewModel();

            model.Curriculum = curr;
            model.Semester = SemesterService.GetSemester(DateTime.Today);

            model.ActiveSemesters.AddRange(Db.Semesters.Where(s => s.Groups.Any(g => g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)));


            // hier muss überprüft werden, ob der aktuelle Benutzer
            // der Fakultät des Studiengangs angehört oder nicht
            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);

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
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Terms(Guid id)
        {
            var org = GetMyOrganisation();

            var cur = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var criteria = Db.Criterias.Where(x => x.Requirement.Option.Package.Curriculum.Id == cur.Id).GroupBy(x => x.Term).ToList();

            var model = new CurriculumTermViewModel
            {
                Curriculum = cur,
                Terms = criteria
            };


            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Modules(Guid id)
        {
            var org = GetMyOrganisation();

            var model = Db.Curricula.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Milestones(Guid id)
        {
            var org = GetMyOrganisation();

            var model = Db.Curricula.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Requirement(Guid id)
        {
            var model = Db.Requirements.SingleOrDefault(x => x.Id == id);

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
            ViewBag.Semester = SemesterService.GetSemester(DateTime.Today);


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
            var semester = SemesterService.GetSemester(DateTime.Today);

            ViewBag.Semesters = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = semester.Name,
                    Value = semester.Name
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
                Semester = semester.Name
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


        [HttpPost]
        public JsonResult OrgList(Guid? orgId, string number)
        {
            List<ActivityOrganiser> orgList;
            if (orgId.HasValue)
            {
                orgList = Db.Organisers.Where(x => x.Id != orgId).ToList();
            }
            else
            {
                orgList = Db.Organisers.ToList();
            }


            var list = orgList.Where(l => l.ShortName.ToUpper().Contains(number.ToUpper()))
                .OrderBy(l => l.ShortName)
                .Select(l => new
                {
                    name = l.ShortName,
                    id = l.Id
                })
                .Take(10);
            return Json(list);
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
            var semester = SemesterService.GetSemester(semId);

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
        /// <param name="semId"></param>
        /// <param name="currId"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult SemesterTopicList(Guid? semId, Guid currId, bool activeOnly = true)
        {
            var curr = Db.Curricula.SingleOrDefault(c => c.Id == currId);
            var semester = SemesterService.GetSemester(semId);

            var model = new CurriculumViewModel
            {
                Curriculum = curr,
            };

            model.Topics = Db.SemesterTopics.Where(x =>
                x.Semester.Id == semester.Id &&
                x.Topic.Chapter.Curriculum.Id == curr.Id).ToList();

            return PartialView("_TopicList", model);
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
            var semester = SemesterService.GetSemester(semId);
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            var semService = new SemesterService(Db);
            var currs = semService.GetActiveCurricula(org, semester, activeOnly);

            var model = currs
                .OrderBy(g => g.ShortName)
                .ToList();

            return PartialView("_CurriculumSelectList", model);
        }

        [HttpPost]
        public PartialViewResult CurriculaList2(Guid orgId)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            var currs = org.Curricula.ToList();

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
            var org = GetMyOrganisation();


            IEnumerable<Room> roomList;

            if (useFree.HasValue && date.HasValue && @from.HasValue && until.HasValue)
            {
                DateTime begin = date.Value.AddHours(@from.Value.Hours).AddMinutes(@from.Value.Minutes);
                DateTime end = date.Value.AddHours(until.Value.Hours).AddMinutes(until.Value.Minutes);

                roomList = useFree.Value ? new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(org.Id, isOrgAdmin, begin, end) : new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(isOrgAdmin);
                
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
            var semester = SemesterService.GetSemester(DateTime.Today);

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
            var user = AppUser;

            var courses = Db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any(g =>
                g.Id == semGroupId)).OrderBy(c => c.Name).ToList();

            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);
            var semester = semGroup.Semester;

            var model = new List<CourseSummaryModel>();

            var courseService = new CourseService(Db);

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);

                summary.State = ActivityService.GetActivityState(course.Occurrence, user);
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

            /*
            ViewBag.Degrees = Db.Degrees.Select(c => new SelectListItem
            {
                Text = c.ShortName + " (" + c.Name + ")",
                Value = c.Id.ToString(),
            });
            */

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
            cur.Version = model.Version;
            Db.SaveChanges();

            return RedirectToAction("Structure", new {id = cur.Id});
        }

        public ActionResult Import(Guid id)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumImportModel
            {
                Curriculum = cur
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Import(CurriculumImportModel model)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentStructure?.SaveAs(tempFile);

            var lines = System.IO.File.ReadAllLines(tempFile, Encoding.Default);

            foreach (var line in lines)
            {
                var words = line.Split(';');
                var orgName = words[0].Trim();
                var curName = words[1].Trim();
                var pckName = words[2].Trim();
                var optName = words[3].Trim();
                var reqName = words[4].Trim();
                var reqEcts = int.TryParse(words[5], out var result) ? result : 0;
                var critName = words[6].Trim();
                var critTerm = int.TryParse(words[7], out var result2) ? result2 : 0;
                var subName = words[8].Trim();
                var subLVs = words[9].Trim();
                var subExams = words[10].Trim();

                var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(orgName));

                var cur = org?.Curricula.SingleOrDefault(x => x.ShortName.Equals(curName));
                if (cur == null)
                    continue;

                var pck = cur.Packages.SingleOrDefault(x => x.Name.Equals(pckName));
                if (pck == null)
                {
                    pck = new CurriculumPackage
                    {
                        Name = pckName,
                        Curriculum = cur,
                    };
                    cur.Packages.Add(pck);
                }

                var option = pck.Options.SingleOrDefault(x => x.Name.Equals(optName));
                if (option == null)
                {
                    option = new PackageOption
                    {
                        Name = optName,
                        Package = pck,
                    };

                    pck.Options.Add(option);
                }

                var req = option.Requirements.SingleOrDefault(x => x.Name.Equals(reqName));
                if (req == null)
                {
                    req = new CurriculumRequirement
                    {
                        Option = option,
                        Name = reqName
                    };
                    option.Requirements.Add(req);
                }

                // ECTS nur setzen, wenn größer als 0
                if (reqEcts > 0)
                    req.ECTS = reqEcts;

                var crit = req.Criterias.SingleOrDefault(x => x.Name.Equals(critName));
                if (crit == null)
                {
                    crit = new CurriculumCriteria
                    {
                        Name = critName,
                        Requirement = req
                    };

                    req.Criterias.Add(crit);
                }

                // Semester nur wenn größer als 0
                if (critTerm > 0)
                    crit.Term = critTerm;

                // Module => suchen und akkreditieren
                // innerhalb der Module noch die Infos setzen

                // Modul in den Akkreditierungen suchen
                // wenn keine Akkreditierung vorhanden, dann
                // Modul anlegen und automatisch akkreditieren
                // Aufräumen ist spezielle Adminaufgabe

                // gibt es das Modul schon als Akkreditierung in diesem Studiengang?
                var allAccs = Db.Accreditations.Where(x => x.Criteria.Requirement.Option.Package.Curriculum.Id == cur.Id && x.Module.Name.Equals(subName)).ToList();
                if (!allAccs.Any())
                {
                    // Modul anlegen
                    var module = new CurriculumModule();
                    module.Name = subName;
                    // LVs und Prüfungen

                    if (!string.IsNullOrEmpty(subLVs))
                    {
                        var lvs = subLVs.Split(',');

                        foreach (var lv in lvs)
                        {
                            var c = new ModuleCourse();
                            c.Name = lv;

                            module.ModuleCourses.Add(c);
                            Db.ModuleCourses.Add(c);
                        }
                    }


                    if (!string.IsNullOrEmpty(subExams))
                    {
                        var exs = subExams.Split(',');

                        foreach (var ex in exs)
                        {
                            var c = new ModuleExam();
                            c.ExternalId = ex;

                            module.ModuleExams.Add(c);
                            Db.ModuleExams.Add(c);
                        }
                    }

                    Db.CurriculumModules.Add(module);

                    // Akkreditierung anlegen
                    var acc = new ModuleAccreditation();
                    acc.Module = module;
                    acc.Criteria = crit;

                    Db.Accreditations.Add(acc);
                }
            }


            Db.SaveChanges();


            return RedirectToAction("Index", new { id = model.Curriculum.Id });
        }

       
        public JsonResult Export(Guid id)
        {
            var service = new ExportService(Db);

            var prog = service.GetProgram(id, 33);

            return Json(prog, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Delete(Guid id)
        {
            var cur2 = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var modules = new List<CurriculumModule>();

            foreach (var pck2 in cur2.Packages.ToList())
            {
                foreach (var option2 in pck2.Options.ToList())
                {
                    foreach (var req2 in option2.Requirements.ToList())
                    {
                        foreach (var crit2 in req2.Criterias.ToList())
                        {
                            foreach (var acc2 in crit2.Accreditations.ToList())
                            {
                                if (!modules.Contains(acc2.Module))
                                {
                                    modules.Add(acc2.Module);
                                }

                                Db.Accreditations.Remove(acc2);
                            }
                            Db.Criterias.Remove(crit2);
                        }
                        Db.Requirements.Remove(req2);
                    }
                    Db.PackageOptions.Remove(option2);
                }
                Db.CurriculumPackages.Remove(pck2);
            }

            // jetzt noch die Module
            foreach (var module in modules)
            {
                if (!module.Accreditations.Any())
                {
                    foreach (var course in module.ModuleCourses.ToList())
                    {
                        Db.ModuleCourses.Remove(course);
                    }

                    foreach (var exam in module.ModuleExams.ToList())
                    {
                        Db.ModuleExams.Remove(exam);
                    }

                    Db.CurriculumModules.Remove(module);
                }

            }

            Db.SaveChanges();




            return RedirectToAction("Index", new {id = id});
        }

        public ActionResult Transfer(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == id);
            var curricula = Db.Curricula.Where(x => x.Organiser.Id == curriculum.Id && x.Id != curriculum.Id)
                .OrderBy(f => f.ShortName);

            var model = new CurriculumTransferModel
            {
                Curriculum = curriculum,
                TargetCurrId = curricula.First().Id
            };


            ViewBag.Curricula = curricula.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            return View(model);
        }

        public ActionResult Destroy(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == id);

            return View();
        }

    }
}
