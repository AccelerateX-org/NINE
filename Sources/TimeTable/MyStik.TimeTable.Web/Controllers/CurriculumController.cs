using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using Newtonsoft.Json;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumController : BaseController
    {
        public ActionResult Details(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            if (curr.BulletinBoard == null)
            {
                var board = new BulletinBoard
                {
                    Autonomy = curr.Autonomy,   // somit haben automatisch alle Gremien des Studiengangs auf den Schaukasten Zugang
                    Name = curr.Name,
                    Description = "Aushänge relevant für alle Studierende des Studiengangs"
                };

                curr.BulletinBoard = board;

                Db.BulletinBoards.Add(board);
                Db.SaveChanges();
            }

            var model = new CurriculumViewModel();

            model.Curriculum = curr;
            model.Semester = SemesterService.GetSemester(DateTime.Today);
            model.NextSemester = SemesterService.GetNextSemester(model.Semester);
            model.PreviousSemester = SemesterService.GetPreviousSemester(model.Semester);

            var user = GetCurrentUser();

            if (user.MemberState == MemberState.Staff)
            {
                model.ActiveSemesters.AddRange(Db.Semesters.Where(s =>
                    s.Groups.Any(g => g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)));
            }
            else
            {
                model.ActiveSemesters.AddRange(Db.Semesters.Where(s =>
                    s.Groups.Any(g => g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id && g.IsAvailable)));
            }


            var assessments = Db.Assessments.Where(x =>
                x.Curriculum.Id == curr.Id &&
                x.Stages.Any(s => s.ClosingDateTime != null && s.ClosingDateTime.Value >= DateTime.Today)).ToList();

            model.Assessments = assessments;

            // die Labels sammlen
            var labels = new List<ItemLabel>();
            foreach (var section in curr.Sections)
            {
                foreach (var slot in section.Slots)
                {
                    if (slot.LabelSet != null && slot.LabelSet.ItemLabels.Any())
                    {
                        foreach (var label in slot.LabelSet.ItemLabels)
                        {
                            if (!labels.Contains(label))
                            {
                                labels.Add(label);
                            }

                        }
                    }

                }
            }

            ViewBag.FilterLabels = labels.OrderBy(x => x.Name);

            model.Areas = new List<AreaSelectViewModel>();

            var allAreasWithOptions = curr.Areas.Where(x => x.Options.Count > 1).ToList();
            foreach (var area in allAreasWithOptions)
            {
                var optList = area.Options.ToList();
                optList.Shuffle();
                var option = optList.First();

                var selectOption = new AreaSelectViewModel
                {
                    Area = area,
                    Option = option,
                };

                model.Areas.Add(selectOption);
            }




            // hier muss überprüft werden, ob der aktuelle Benutzer
            // der Fakultät des Studiengangs angehört oder nicht
            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);

            return View(model);
        }

        [HttpPost]
        public PartialViewResult LoadModulePlan(Guid currId, string label)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new CurriculumViewModel();

            model.Curriculum = curr;

            model.FilterLabel =  curr.LabelSet.ItemLabels.FirstOrDefault(x => x.Name.Equals(label));


            return PartialView("_ModulePlan", model);
        }


        [HttpPost]
        public PartialViewResult LoadModulePlanAreas(Guid currId, Guid[] optIds)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new CurriculumViewModel();

            model.Curriculum = curr;

            model.Areas = new List<AreaSelectViewModel>();

            foreach (var optId in optIds)
            {
                var option = Db.AreaOptions.SingleOrDefault(x => x.Id == optId);

                var selectOption = new AreaSelectViewModel
                {
                    Area = option.Area,
                    Option = option,
                };

                model.Areas.Add(selectOption);
            }

            return PartialView("_ModuleAreaPlan", model);
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
        public ActionResult Scheme(Guid id)
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

            var model = new CurriculumTermViewModel
            {
                Curriculum = cur,
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


            ViewBag.Curricula = Db.Curricula.Where(c => c.Organiser.ShortName.Equals("FK 09")).Select(c =>
                new SelectListItem
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
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)
                    .OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


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
        public PartialViewResult GroupList(Guid? semId, Guid currId, bool activeOnly = true)
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
        public PartialViewResult CurriculaList(Guid? semId, Guid orgId, bool activeOnly = true)
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

        [HttpPost]
        public PartialViewResult CurriculaList3(Guid orgId)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            var currs = org.Curricula.ToList();

            var model = currs
                .OrderBy(g => g.ShortName)
                .ToList();

            return PartialView("_CurriculumListGroup", model);
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

        [HttpPost]
        public PartialViewResult SemesterList3(Guid orgId)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            List<Semester> semesters;

            // aktuelles Semester
            var currentSemester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(currentSemester);

            // vom NextSemester 8 zurück

            semesters = Db.Semesters.Where(x => x.StartCourses <= nextSemester.StartCourses)
                .OrderByDescending(x => x.EndCourses)
                .Take(8).ToList();

            return PartialView("_SemesterListGroup", semesters);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="name"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult LecturerList(Guid? orgId, string name, bool hasAccount = false)
        {

            var org = orgId != null
                ? Db.Organisers.SingleOrDefault(o => o.Id == orgId.Value)
                : GetMyOrganisation();

            if (org != null)
            {
                if (hasAccount)
                {
                    var list2 = org.Members.Where(l =>
                            (!string.IsNullOrEmpty(l.UserId)) && (
                            (!string.IsNullOrEmpty(l.ShortName) && l.ShortName.StartsWith(name.ToUpper())) ||
                            (!string.IsNullOrEmpty(l.Name) && l.Name.ToUpper().StartsWith(name.ToUpper()))))
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
                else
                {
                    var list2 = org.Members.Where(l =>
                            (!string.IsNullOrEmpty(l.ShortName) && l.ShortName.StartsWith(name.ToUpper())) ||
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
            var org = GetMyOrganisation();
            var member = MemberService.GetMember(org.Id);
            var isOrgAdmin = member?.IsAdmin ?? false;


            IEnumerable<Room> roomList;

            if (useFree.HasValue && date.HasValue && @from.HasValue && until.HasValue)
            {
                DateTime begin = date.Value.AddHours(@from.Value.Hours).AddMinutes(@from.Value.Minutes);
                DateTime end = date.Value.AddHours(until.Value.Hours).AddMinutes(until.Value.Minutes);

                roomList = useFree.Value
                    ? new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(org.Id, isOrgAdmin, begin, end)
                    : new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(isOrgAdmin);

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
            var user = GetCurrentUser();
            var org = GetOrganiser(orgId);
            var member = MemberService.GetOrganiserMember(org.Id, user.Id);
            var isOrgAdmin = false;
            if (member != null)
            {
                if (member.IsRoomAdmin || member.IsAdmin)
                {
                    isOrgAdmin = true;
                }
            }

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
            var org = GetMyOrganisation();
            var member = MemberService.GetMember(org.Id);
            var isOrgAdmin = false;
            if (member != null)
            {
                if (member.IsRoomAdmin || member.IsAdmin)
                {
                    isOrgAdmin = true;
                }
            }



            var allRooms = Db.Rooms.Where(r => r.Number.ToUpper().StartsWith(number.ToUpper())).OrderBy(r => r.Number)
                .ToList();

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
                var dayOfWeek = (DayOfWeek) day.Value;

                roomList = new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(dayOfWeek, from.Value,
                    until.Value, semester, isOrgAdmin, allRooms);
            }
            else
            {
                roomList = new MyStik.TimeTable.Web.Services.RoomService().GetAllRooms(isOrgAdmin, allRooms);
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
        public PartialViewResult CourseListByProgram(Guid semGroupId, bool compact = false)
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
        public ActionResult Edit(Guid id)
        {
            var model = Db.Curricula.SingleOrDefault(x => x.Id == id);

            // Achtung: vertauschen der Anzeige für "IsDeprecated"
            model.IsDeprecated = !model.IsDeprecated;

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

            cur.Tag = model.Tag;
            cur.Name = model.Name;
            cur.ShortName = model.ShortName;
            cur.Description = model.Description;
            cur.Version = model.Version;
            cur.ThesisDuration = model.ThesisDuration;

            cur.IsDeprecated = !model.IsDeprecated;

            Db.SaveChanges();

            return RedirectToAction("Admin", new {id = cur.Id});
        }

        public ActionResult ImportSections(Guid id)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumImportModel
            {
                Curriculum = cur
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ImportSections(CurriculumImportModel model)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentStructure?.SaveAs(tempFile);

            CurriculumModulePlan plan = null;
            
            using (StreamReader file = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                plan = (CurriculumModulePlan)serializer.Deserialize(file, typeof(CurriculumModulePlan));
            }
            

            if (plan == null)
                return View();

            // Im Augenblick ist es ein Import für einen existierenden Studiengang
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);
            if (curr.Sections.Any())
                return View();

            // Ergänzung von Tags
            curr.Tag = plan.tag;
            curr.Version = plan.version;
            curr.Organiser.Tag = plan.institution;
            curr.EctsTarget = plan.ectsTarget;

            if (curr.Degree == null)
            {
                var deg = Db.Degrees.FirstOrDefault(x => x.Name.Equals(plan.level));

                if (deg == null)
                {
                    deg = new Degree
                    {
                        Name = plan.level
                    };

                    Db.Degrees.Add(deg);
                }

                curr.Degree = deg;
            }



            foreach (var section in plan.sections)
            {
                var currSection = new CurriculumSection
                {
                    Name = section.name,
                    Order = section.order,
                    Curriculum = curr
                };

                foreach (var slot in section.slots)
                {
                    var currSlot = new CurriculumSlot
                    {
                        ECTS = slot.ects,
                        Position = slot.position,
                        Tag = slot.tag,
                        Name = slot.name,
                        CurriculumSection = currSection
                    };

                    // das Label
                    if (!string.IsNullOrEmpty(slot.label))
                    {
                        // Test, ob Studiengang schon Labelset hat
                        var currLabelSet = curr.LabelSet;
                        if (currLabelSet == null)
                        {
                            currLabelSet = new ItemLabelSet();
                            curr.LabelSet = currLabelSet;
                            Db.ItemLabelSets.Add(currLabelSet);
                        }

                        // gibt es das Label schon im Studiengang?
                        var label = currLabelSet.ItemLabels.FirstOrDefault(x => x.Name.Equals(slot.label));

                        if (label == null)
                        {
                            label = new ItemLabel
                            {
                                Name = slot.label,
                                HtmlColor = "#ff0000"
                            };

                            currLabelSet.ItemLabels.Add(label);

                            Db.ItemLabels.Add(label);
                        }

                        // das Label dem Slot zuordnen
                        var slotLabelSet = new ItemLabelSet();

                        currSlot.LabelSet = slotLabelSet;
                        slotLabelSet.ItemLabels.Add(label);
                    }

                    Db.CurriculumSlots.Add(currSlot);
                }

                Db.CurriculumSections.Add(currSection);
            }


            Db.SaveChanges();

            return RedirectToAction("Details", new {id = model.Curriculum.Id});
        }



        public ActionResult ImportAreas(Guid id)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumImportModel
            {
                Curriculum = cur
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ImportAreas(CurriculumImportModel model)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentStructure?.SaveAs(tempFile);

            CurriculumModulePlan plan = null;

            using (var file = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                plan = (CurriculumModulePlan)serializer.Deserialize(file, typeof(CurriculumModulePlan));
            }


            if (plan == null)
                return View();

            // Im Augenblick ist es ein Import für einen existierenden Studiengang
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);


            // Ergänzung von Tags
            curr.Tag = plan.tag;
            curr.Version = plan.version;
            curr.Organiser.Tag = plan.institution;
            curr.EctsTarget = plan.ectsTarget;

            if (curr.Degree == null)
            {
                var deg = Db.Degrees.FirstOrDefault(x => x.Name.Equals(plan.level));

                if (deg == null)
                {
                    deg = new Degree
                    {
                        Name = plan.level
                    };

                    Db.Degrees.Add(deg);
                }

                curr.Degree = deg;
            }



            foreach (var area in plan.areas)
            {
                var currArea = curr.Areas.FirstOrDefault(x => x.Tag.ToUpper().Equals(area.tag));

                if (currArea == null)
                {
                    currArea = new CurriculumArea()
                    {
                        Name = area.name,
                        Tag = area.tag,
                        Curriculum = curr
                    };
                    Db.CurriculumAreas.Add(currArea);
                }

                foreach (var option in area.options)
                {
                    var currOption = currArea.Options.FirstOrDefault(x => x.Tag.ToUpper().Equals(option.tag.ToUpper()));

                    if (currOption == null)
                    {
                        currOption = new AreaOption()
                        {
                            Tag = option.tag,
                            Area = currArea
                        };
                        Db.AreaOptions.Add(currOption);
                    }

                    foreach (var slot in option.slots)
                    {
                        var currSlot = currOption.Slots.FirstOrDefault(x => x.Tag.ToUpper().Equals(slot.tag.ToUpper()));

                        if (currSlot == null)
                        {
                            currSlot = new CurriculumSlot
                            {
                                ECTS = slot.ects,
                                Semester = slot.semester,
                                Tag = slot.tag,
                                Name = slot.name,
                                AreaOption = currOption
                            };
                            Db.CurriculumSlots.Add(currSlot);
                        }
                    }
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = model.Curriculum.Id });
        }


        

        public ActionResult ImportMoveAccr(Guid id)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumImportModel
            {
                Curriculum = cur
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ImportMoveAccr(CurriculumImportModel model)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentStructure?.SaveAs(tempFile);

            AccreditationImportModel plan = null;

            using (var file = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                plan = (AccreditationImportModel)serializer.Deserialize(file, typeof(AccreditationImportModel));
            }

            if (plan == null)
                return View();



            foreach (var accredition in plan.accreditions)
            {
                var catWordsSource = accredition.sourceslot.Split('#');
                if (catWordsSource.Length != 5) continue;

                var catWordsTarget = accredition.targetslot.Split('#');
                if (catWordsTarget.Length != 5) continue;

                var institutionNameSource = catWordsSource[0];
                var currNameSource = catWordsSource[1];
                var areaNameSource = catWordsSource[2];
                var optionNameSource = catWordsSource[3];
                var moduleNameSource = catWordsSource[4];

                var institutionNameTarget = catWordsTarget[0];
                var currNameTarget = catWordsTarget[1];
                var areaNameTarget = catWordsTarget[2];
                var optionNameTarget = catWordsTarget[3];
                var moduleNameTarget = catWordsTarget[4];

                var slotSource = Db.CurriculumSlots.SingleOrDefault(x =>
                    x.AreaOption != null &&
                    x.AreaOption.Area.Curriculum.Organiser.Tag.Equals(institutionNameSource) &&
                    x.AreaOption.Area.Curriculum.Tag.Equals(currNameSource) &&
                    x.AreaOption.Area.Tag.Equals(areaNameSource) &&
                    x.AreaOption.Tag.Equals(optionNameSource) &&
                    x.Tag.Equals(moduleNameSource));


                var slotTarget = Db.CurriculumSlots.SingleOrDefault(x =>
                    x.AreaOption != null &&
                    x.AreaOption.Area.Curriculum.Organiser.Tag.Equals(institutionNameTarget) &&
                    x.AreaOption.Area.Curriculum.Tag.Equals(currNameTarget) &&
                    x.AreaOption.Area.Tag.Equals(areaNameTarget) &&
                    x.AreaOption.Tag.Equals(optionNameTarget) &&
                    x.Tag.Equals(moduleNameTarget));

                if (slotSource != null && slotTarget != null)
                {
                    foreach (var accreditation in slotSource.ModuleAccreditations.ToList())
                    {
                        slotSource.ModuleAccreditations.Remove(accreditation);
                        slotTarget.ModuleAccreditations.Add(accreditation);
                    }
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = model.Curriculum.Id });
        }

        public ActionResult DeleteSlot(Guid id)
        {
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == id);
            var option = slot.AreaOption;

            foreach (var accreditation in slot.ModuleAccreditations.ToList())
            {
                foreach (var exam in accreditation.ExaminationDescriptions.ToList())
                {
                    Db.ExaminationDescriptions.Remove(exam);
                }

                foreach (var teaching in accreditation.TeachingDescriptions.ToList())
                {
                    Db.TeachingDescriptions.Remove(teaching);
                }

                Db.Accreditations.Remove(accreditation);
            }


            Db.CurriculumSlots.Remove(slot);
            Db.SaveChanges();

            return RedirectToAction("Option", new { id = option.Id });
        }

        public ActionResult DeleteOption(Guid id)
        {
            var option = Db.AreaOptions.SingleOrDefault(x => x.Id == id);
            var area = option.Area;

            Db.AreaOptions.Remove(option);
            Db.SaveChanges();

            return RedirectToAction("Area", new { id = area.Id });
        }

        public ActionResult DeleteArea(Guid id)
        {
            var area = Db.CurriculumAreas.SingleOrDefault(x => x.Id == id);
            var curr = area.Curriculum;

            Db.CurriculumAreas.Remove(area);
            Db.SaveChanges();


            return RedirectToAction("Areas", new {id = curr.Id});
        }


        public ActionResult DeleteModulePlan(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumDeleteModel
            {
                Curriculum = curr
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteModulePlan(CurriculumDeleteModel model)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);

            if (!cur.Tag.Equals(model.Code))
            {
                var model2 = new CurriculumDeleteModel { Curriculum = cur };

                return View(model2);
            }


            foreach (var section in cur.Sections.ToList())
            {
                foreach (var slot in section.Slots.ToList())
                {
                    foreach (var accreditation in slot.ModuleAccreditations.ToList())
                    {
                        Db.Accreditations.Remove(accreditation);
                    }

                    Db.CurriculumSlots.Remove(slot);
                }

                Db.CurriculumSections.Remove(section);
            }

            foreach (var area in cur.Areas.ToList())
            {
                foreach (var option in area.Options.ToList())
                {
                    foreach (var slot in option.Slots.ToList())
                    {
                        foreach (var accreditation in slot.ModuleAccreditations.ToList())
                        {
                            foreach(var exam in accreditation.ExaminationDescriptions.ToList())
                            {
                                Db.ExaminationDescriptions.Remove(exam);
                            }
                            
                            foreach(var teaching in accreditation.TeachingDescriptions.ToList())
                            {
                                Db.TeachingDescriptions.Remove(teaching);
                            }
                            
                            Db.Accreditations.Remove(accreditation);
                        }

                        Db.CurriculumSlots.Remove(slot);
                    }

                    Db.AreaOptions.Remove(option);
                }

                Db.CurriculumAreas.Remove(area);
            }


            Db.SaveChanges();

            return RedirectToAction("Details", new { id = cur.Id });
        }



        public ActionResult ImportAccreditations(Guid id)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumImportModel
            {
                Curriculum = cur
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ImportAccreditations(CurriculumImportModel model)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentStructure?.SaveAs(tempFile);

            AccreditationImportModel plan = null;

            using (StreamReader file = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                plan = (AccreditationImportModel)serializer.Deserialize(file, typeof(AccreditationImportModel));
            }


            if (plan == null)
                return View();

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);

            var allAreaSlots = Db.CurriculumSlots
                .Where(x => x.CurriculumSection == null && x.AreaOption != null &&
                            x.AreaOption.Area.Curriculum.Id == curr.Id).ToList();



            foreach (var accreditation in plan.accreditions)
            {
                var slot = allAreaSlots.FirstOrDefault(x =>
                    x.FullTag.Equals(accreditation.slot));
                if (slot == null) continue;

                var catWords = accreditation.module.Split('#');
                if (catWords.Length != 3) continue;

                var institutionName = catWords[0];
                var catalogName = catWords[1];
                var moduleName = catWords[2];


                var module = Db.CurriculumModules.FirstOrDefault(x => 
                    x.Tag.Equals(moduleName) &&
                    x.Catalog.Tag.Equals(catalogName) &&
                    x.Catalog.Organiser.Tag.Equals(institutionName)
                    );

                if (module == null) continue;

                var moduleAccredition = slot.ModuleAccreditations.FirstOrDefault(x => x.Module.Id == module.Id);

                if (moduleAccredition == null)
                {
                    moduleAccredition = new ModuleAccreditation
                    {
                        Slot = slot,
                        Module = module,
                    };
                    Db.Accreditations.Add(moduleAccredition);
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = model.Curriculum.Id });
        }


        public ActionResult Opportunities(Guid id)
        {
            var cur = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumImportModel
            {
                Curriculum = cur
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Opportunities(CurriculumImportModel model)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentStructure?.SaveAs(tempFile);

            OpportunityImportModel plan = null;

            using (StreamReader file = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                plan = (OpportunityImportModel)serializer.Deserialize(file, typeof(OpportunityImportModel));
            }


            if (plan == null)
                return View();

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);
            var sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(plan.semester));

            foreach (var accreditation in plan.opportunities)
            {
                var catWords = accreditation.subject.Split('#');
                if (catWords.Length != 4) continue;

                var orgName = catWords[0];
                var catalogName = catWords[1];
                var moduleName = catWords[2];
                var subjectName = catWords[3];


                var module = Db.ModuleCourses.FirstOrDefault(x =>
                    x.Tag.Equals(subjectName) && x.Module != null &&
                    x.Module.Tag.Equals(moduleName) && x.Module.Catalog != null &&
                    x.Module.Catalog.Tag.Equals(catalogName) &&
                    x.Module.Catalog.Organiser.Tag.Equals(orgName)
                    );

                if (module == null) continue;

                var courses = Db.Activities.OfType<Course>().Where(x => 
                    x.ShortName.Equals(accreditation.course) &&
                    x.SemesterGroups.Any(g => 
                        g.Semester.Id == sem.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)
                    ).ToList();

                foreach (var course in courses)
                {
                    var subjectOpportunity = module.Opportunities.FirstOrDefault(x =>
                        x.Course.Id == course.Id && 
                        x.Semester.Id == sem.Id && 
                        x.Subject.Id == module.Id);

                    if (subjectOpportunity == null)
                    {
                        subjectOpportunity = new SubjectOpportunity
                        {
                            Course = course,
                            Subject = module,
                            Semester = sem
                        };

                        Db.SubjectOpportunities.Add(subjectOpportunity);
                    }
                }

            }

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = model.Curriculum.Id });
        }







        public ActionResult Transfer(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == id);
            var curricula = Db.Curricula.Where(x => x.Organiser.Id == curriculum.Organiser.Id && x.Id != curriculum.Id)
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


        [HttpPost]
        public ActionResult Transfer(CurriculumTransferModel model)
        {
            var sourceCur = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);
            var targetCur = Db.Curricula.SingleOrDefault(x => x.Id == model.TargetCurrId);


            var allStudents = Db.Students.Where(x => x.Curriculum.Id == sourceCur.Id);

            foreach (var student in allStudents)
            {
                student.Curriculum = targetCur;
            }

            Db.SaveChanges();


            return RedirectToAction("Students", new {id = sourceCur.Id});
        }




        public ActionResult Destroy(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == id);

            return View();
        }

        public ActionResult Admin(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(curr.Organiser);

            return View(curr);
        }

        /*
        public ActionResult AdminSubject(Guid id)
        {
            var subject = Db.CertificateSubjects.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(subject.CertificateModule.Curriculum.Organiser);

            return View(subject);
        }



        public ActionResult EditModule(Guid id)
        {
            var model = Db.CertificateModules.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditModule(CertificateModule model)
        {
            var module = Db.CertificateModules.SingleOrDefault(x => x.Id == model.Id);

            module.Name = model.Name;
            module.Weight = model.Weight;

            Db.SaveChanges();

            return RedirectToAction("Admin", new {id = module.Curriculum.Id});
        }

        public ActionResult DeleteModule(Guid id)
        {
            var model = Db.CertificateModules.SingleOrDefault(x => x.Id == id);

            var curr = model.Curriculum;

            Db.CertificateModules.Remove(model);
            Db.SaveChanges();

            return RedirectToAction("Admin", new {id = curr.Id});
        }



        public ActionResult EditSubject(Guid id)
        {
            var model = Db.CertificateSubjects.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditSubject(CertificateSubject model)
        {
            var subject = Db.CertificateSubjects.SingleOrDefault(x => x.Id == model.Id);

            subject.Name = model.Name;
            subject.Ects = model.Ects;
            subject.Term = model.Term;

            Db.SaveChanges();

            return RedirectToAction("Admin", new {id = subject.CertificateModule.Curriculum.Id});
        }

        public ActionResult DeleteSubject(Guid id)
        {
            var model = Db.CertificateSubjects.SingleOrDefault(x => x.Id == id);

            var curr = model.CertificateModule.Curriculum;

            foreach (var contentModule in model.ContentModules.ToList())
            {
                Db.Accreditations.Remove(contentModule);
            }

            Db.CertificateSubjects.Remove(model);
            Db.SaveChanges();

            return RedirectToAction("Admin", new {id = curr.Id});
        }

        public ActionResult ModuleDetails(Guid id)
        {
            var model = Db.CertificateModules.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);

            return View(model);
        }

        public ActionResult CreateSubject(Guid id)
        {
            var module = Db.CertificateModules.SingleOrDefault(x => x.Id == id);

            var model = new CertificateSubject
            {
                CertificateModule = module
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSubject(CertificateSubject model)
        {
            var module = Db.CertificateModules.SingleOrDefault(x => x.Id == model.CertificateModule.Id);

            var subject = new CertificateSubject
            {
                Name = model.Name,
                Ects = model.Ects,
                CertificateModule = module
            };

            Db.CertificateSubjects.Add(subject);
            Db.SaveChanges();

            return RedirectToAction("ModuleDetails", new {id = module.Id});
        }

        public ActionResult AdminContentModule(Guid id)
        {
            var model = Db.Accreditations.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(model.CertificateSubject.CertificateModule.Curriculum.Organiser);

            return View(model);
        }
        */


        public ActionResult AssignModules(Guid id)
        {
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == id);

            var model = new ModuleAssignViewModel
            {
                Slot = slot,
                Organisers = Db.Organisers.Where(x => x.ModuleCatalogs.Any()).ToList()
            };

            return View(model);
        }



        [HttpPost]
        public ActionResult AssignModulesSave(Guid slotId, Guid[] moduleIds)
        {
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == slotId);

            foreach (var moduleId in moduleIds)
            {
                var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);

                if (slot == null || module == null) continue;

                bool isModulePresent = slot.ModuleAccreditations.Any(x => x.Module.Id == module.Id);

                if (isModulePresent) continue;

                var accr = new ModuleAccreditation
                {
                    Module = module,
                    Slot = slot

                };

                Db.Accreditations.Add(accr);
            }

            Db.SaveChanges();

            return null;
        }





        public ActionResult Students(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumSummaryModel
            {
                Curriculum = curriculum,
                Students = Db.Students.Where(x => x.Curriculum.Id == curriculum.Id).ToList()
            };


            return View(model);
        }

        public ActionResult Slot(Guid id)
        {
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == id);

            if (slot.CurriculumSection != null)
            {
                ViewBag.UserRight = GetUserRight(slot.CurriculumSection.Curriculum.Organiser);
            }

            if (slot.AreaOption != null)
            {
                ViewBag.UserRight = GetUserRight(slot.AreaOption.Area.Curriculum.Organiser);
            }

            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);

            return View(slot);
        }

        public ActionResult EditSlot(Guid id)
        {
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(slot.AreaOption.Area.Curriculum.Organiser);

            var model = new CurriculumAreaCreateModel
            {
                SlotId = slot.Id,
                Description = slot.Description,
                Name = slot.Name,
                Tag = slot.Tag,
                Semester = slot.Semester,
                Ects = slot.ECTS
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditSlot(CurriculumAreaCreateModel model)
        {
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == model.SlotId);

            if (string.IsNullOrEmpty(model.Tag))
                return View(model);

            var option = slot.AreaOption;
            var doubleTag = option.Slots.FirstOrDefault(x => x.Id != slot.Id && x.Tag.Equals(model.Tag.ToUpper()));
            if (doubleTag != null)
                return View(model);

            slot.Tag = model.Tag.ToUpper();
            slot.Name = model.Name;
            slot.Description = model.Description;
            slot.Semester = model.Semester;
            slot.ECTS = model.Ects;

            Db.SaveChanges();

            return RedirectToAction("Slot", new { id = slot.Id });
        }



        [HttpPost]
        public PartialViewResult LoadModuleList(Guid slotId, string label)
        {
            var slot = Db.CurriculumSlots.FirstOrDefault(x => x.Id == slotId);


            var labels = new List<ItemLabel>();
            foreach (var accreditation in slot.ModuleAccreditations)
            {
                if (accreditation.LabelSet != null && accreditation.LabelSet.ItemLabels.Any())
                {
                    foreach (var aclabel in accreditation.LabelSet.ItemLabels)
                    {
                        if (!labels.Contains(aclabel))
                        {
                            labels.Add(aclabel);
                        }
                    }
                }
            }


            ViewBag.FilterLabel = labels.FirstOrDefault(x => x.Name.Equals(label));


            return PartialView("_ModuleList", slot);
        }



        public ActionResult Labels(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            return View(curr);
        }

        public ActionResult Areas(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(curr.Organiser);

            return View(curr);
        }

        public ActionResult Area(Guid id)
        {
            var curr = Db.CurriculumAreas.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(curr.Curriculum.Organiser);

            return View(curr);
        }

        public ActionResult CreateArea(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(curr.Organiser);

            var model = new CurriculumAreaCreateModel
            {
                CurrId = curr.Id,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateArea(CurriculumAreaCreateModel model)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);

            if (string.IsNullOrEmpty(model.Tag))
                return View(model);

            var doubleTag = curr.Areas.FirstOrDefault(x => x.Tag.Equals(model.Tag.ToUpper()));
            if (doubleTag != null)
                return View(model);

            var area = new CurriculumArea
            {
                Curriculum = curr,
                Tag = model.Tag.ToUpper(),
                Name = model.Name,
                Description = model.Description
            };

            Db.CurriculumAreas.Add(area);

            Db.SaveChanges();

            return RedirectToAction("Area", new { id = area.Id });
        }


        public ActionResult EditArea(Guid id)
        {
            var area = Db.CurriculumAreas.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(area.Curriculum.Organiser);

            var model = new CurriculumAreaCreateModel
            {
                AreaId = area.Id,
                Description = area.Description,
                Name = area.Name,
                Tag = area.Tag
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditArea(CurriculumAreaCreateModel model)
        {
            var area = Db.CurriculumAreas.SingleOrDefault(x => x.Id == model.AreaId);

            if (string.IsNullOrEmpty(model.Tag))
                return View(model);

            var curr = area.Curriculum;
            var doubleTag = curr.Areas.FirstOrDefault(x => x.Id != area.Id && x.Tag.Equals(model.Tag.ToUpper()));
            if (doubleTag != null)
                return View(model);

            area.Tag = model.Tag.ToUpper();
            area.Name = model.Name;
            area.Description = model.Description;

            Db.SaveChanges();

            return RedirectToAction("Area", new {id = area.Id});
        }

        public ActionResult Option(Guid id)
        {
            var curr = Db.AreaOptions.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(curr.Area.Curriculum.Organiser);

            return View(curr);
        }

        public ActionResult CreateOption(Guid id)
        {
            var area = Db.CurriculumAreas.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(area.Curriculum.Organiser);

            var model = new CurriculumAreaCreateModel
            {
                AreaId = area.Id,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateOption(CurriculumAreaCreateModel model)
        {
            var area = Db.CurriculumAreas.SingleOrDefault(x => x.Id == model.AreaId);

            if (string.IsNullOrEmpty(model.Tag))
                return View(model);

            var doubleTag = area.Options.FirstOrDefault(x => x.Tag.Equals(model.Tag.ToUpper()));
            if (doubleTag != null)
                return View(model);

            var option = new AreaOption
            {
                Area = area,
                Tag = model.Tag.ToUpper(),
                Name = model.Name,
                Description = model.Description
            };

            Db.AreaOptions.Add(option);

            Db.SaveChanges();

            return RedirectToAction("Option", new { id = option.Id });
        }



        public ActionResult EditOption(Guid id)
        {
            var option = Db.AreaOptions.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(option.Area.Curriculum.Organiser);

            var model = new CurriculumAreaCreateModel
            {
                OptionId = option.Id,
                Description = option.Description,
                Name = option.Name,
                Tag = option.Tag
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditOption(CurriculumAreaCreateModel model)
        {
            var option = Db.AreaOptions.SingleOrDefault(x => x.Id == model.OptionId);

            if (string.IsNullOrEmpty(model.Tag))
                return View(model);

            var area = option.Area;
            var doubleTag = area.Options.FirstOrDefault(x => x.Id != option.Id && x.Tag.Equals(model.Tag.ToUpper()));
            if (doubleTag != null)
                return View(model);

            option.Tag = model.Tag.ToUpper();
            option.Name = model.Name;
            option.Description = model.Description;

            Db.SaveChanges();

            return RedirectToAction("Option", new { id = option.Id });
        }

        public ActionResult MoveSlots(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var model = new MoveSlotModel
            {
                Curriculum = curr
            };

            return View(model);
        }


        [HttpPost]
        public PartialViewResult GetOptions(Guid areaId)
        {
            var org = Db.CurriculumAreas.SingleOrDefault(x => x.Id == areaId);

            var currs = org.Options.ToList();

            var model = currs
                .OrderBy(g => g.Tag)
                .ToList();

            return PartialView("_OptionSelectList", model);
        }

        [HttpPost]
        public PartialViewResult GetSlots(Guid optionId, string side)
        {
            var catalog = Db.AreaOptions.SingleOrDefault(x => x.Id == optionId);

            var model = catalog.Slots
                .OrderBy(g => g.Tag)
                .ToList();

            ViewBag.ListName = side + "ModuleList";

            return PartialView("_SlotListGroup", model);
        }

        [HttpPost]
        public ActionResult MoveSlotsSave(Guid optionId, Guid[] slotIds)
        {
            var option = Db.AreaOptions.SingleOrDefault(x => x.Id == optionId);

            foreach (var slotId in slotIds)
            {
                var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == slotId);

                if (option == null || slot == null) continue;

                if (option.Slots.All(x => x.Id != slot.Id))
                {
                    slot.AreaOption = option;
                }
            }

            Db.SaveChanges();

            return null;
        }

        public ActionResult DeleteAccredition(Guid id)
        {
            var accr = Db.Accreditations.SingleOrDefault(x => x.Id == id);

            var slot = accr.Slot;

            foreach (var exam in accr.ExaminationDescriptions.ToList())
            {
                Db.ExaminationDescriptions.Remove(exam);
            }

            foreach (var teaching in accr.TeachingDescriptions.ToList())
            {
                Db.TeachingDescriptions.Remove(teaching);
            }

            Db.Accreditations.Remove(accr);

            Db.SaveChanges();


            return RedirectToAction("Slot", new {id=slot.Id});
        }


            /* Noch etwas aufheben
            public ActionResult HackBABW(Guid id)
            {
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);
                var org = curr.Organiser;

                if (org.Tag.Equals("10") && curr.ShortName.Equals("BABW"))
                {
                    // alle die mit BABW starten aber nicht BABW sind
                    var allBABWSubCatalogs = org.ModuleCatalogs.Where(x => x.Tag.StartsWith("BABW") && !x.Tag.Equals("BABW")).ToList();

                    var babwMainCatalog = org.ModuleCatalogs.SingleOrDefault(x => x.Tag.Equals("BABW"));

                    if (babwMainCatalog == null)
                    {
                        babwMainCatalog = new CurriculumModuleCatalog
                        {
                            Name = "BABW alle Module",
                            Tag = "BABW",
                            Organiser = org
                        };
                        Db.CurriculumModuleCatalogs.Add(babwMainCatalog);
                    }

                    foreach (var subCatalog in allBABWSubCatalogs.ToList())
                    {
                        foreach (var module in subCatalog.Modules.ToList())
                        {
                            subCatalog.Modules.Remove(module);
                            babwMainCatalog.Modules.Add(module);
                        }

                        org.ModuleCatalogs.Remove(subCatalog);
                        Db.CurriculumModuleCatalogs.Remove(subCatalog);
                    }

                    Db.SaveChanges();
                }

                return RedirectToAction("Admin", new { id = id });
            }
            */
        }
    }