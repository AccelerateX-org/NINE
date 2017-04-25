using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.GpUntis;
using MyStik.TimeTable.DataServices.GpUntis.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using WebGrease.Css.Extensions;

namespace MyStik.TimeTable.Web.Controllers
{
    public class SemesterController : BaseController
    {
        //
        // GET: /Semester/
        public ActionResult Index()
        {
            ViewBag.SemesterList = Db.Semesters.OrderByDescending(s => s.StartCourses).Select(f => new SelectListItem
            {
                Text = f.BookingEnabled ? f.Name : f.Name + "*",
                Value = f.Name,
            });


            ViewBag.UserRight = GetUserRight();

            var model = new SemesterViewModel
            {
                Semester = GetSemester()
            };

            return View("Index2", model);
        }


        [HttpPost]
        public PartialViewResult DateList(string semGroupId)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Name.Equals(semGroupId));

            ViewBag.UserRight = GetUserRight();

            return PartialView("_DateList", model);
        }



        public ActionResult Details(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

            return View(model);
        }


        public ActionResult GroupList(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

            return View(model);
        }

        /// <summary>
        /// Ein Semester anlegen
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var model = new SemesterCreateViewModel();


            if (DateTime.Today.Month >= 3 && DateTime.Today.Month < 10)
            {
                var date = new DateTime(DateTime.Today.Year, 10, 1);
                // heute liegt im SS => das nächste ist das WS
                model.Name = string.Format("WS{0}", DateTime.Today.Year - 2000);
                model.StartCourses = date.Date.ToShortDateString();
                model.EndCourses = date.AddDays(100).Date.ToShortDateString();
            }
            else
            {
                // offenbar im WS => das nächste ist das SS
                if (DateTime.Today.Month < 3) // "dieses Jahr"
                {
                    model.Name = string.Format("SS{0}", DateTime.Today.Year - 2000);
                }
                else
                {
                    // nächstes Jahr
                    model.Name = string.Format("SS{0}", DateTime.Today.Year - 1999);
                }
                var date = new DateTime(DateTime.Today.Year, 3, 15);
                model.StartCourses = date.Date.ToShortDateString();
                model.EndCourses = date.AddDays(100).Date.ToShortDateString();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SemesterCreateViewModel model)
        {
            // Vorbedingungen geprüft
            if (!ModelState.IsValid)
                return View(model);

            var from = DateTime.Parse(model.StartCourses);
            var to = DateTime.Parse(model.EndCourses);


            if (from >= to)
            {
                ModelState.AddModelError("StartCourses", "Vorlesungsbeginn liegt später als Vorlesungsende");
                ModelState.AddModelError("EndCourses", "Vorlesungsende liegt früher als Vorlesungsbeginn");
            }

            var semExists = Db.Semesters.SingleOrDefault(s => s.Name.ToUpper().Equals(model.Name.ToUpper()));
            if (semExists != null)
            {
                ModelState.AddModelError("Name", "Ein Semester mit diesem Namen existiert bereits");
            }

            semExists = Db.Semesters.SingleOrDefault(s => s.EndCourses >= from);
            if (semExists != null)
            {
                ModelState.AddModelError("StartCourses",
                    string.Format("Überschneidung mit Vorlesungsende von Semester {0}: {1}", semExists.Name,
                        semExists.EndCourses));
            }

            // Nachbedingunen geprüft
            if (ModelState.IsValid)
            {
                // wenn alles ok, dann einen Redirect auf Index
                var sem = Db.Semesters.Add(new Semester
                {
                    Name = model.Name,
                    StartCourses = from,
                    EndCourses = to,
                    BookingEnabled = false,
                });
                Db.SaveChanges();

                return RedirectToAction("Details", new {id=sem.Id});
            }

            return View(model);
        }

        /// <summary>
        /// Semester verändern
        /// </summary>
        /// <param name="id">SemesterId</param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var model = new SemesterEditViewModel();

            var semester = Db.Semesters.SingleOrDefault(s => s.Id == id);

            model.SemesterId = semester.Id;
            model.Name = semester.Name;
            model.StartCourses = semester.StartCourses.ToShortDateString();
            model.EndCourses = semester.EndCourses.ToShortDateString();


            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SemesterEditViewModel model)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == model.SemesterId);

            semester.Name = model.Name;
            semester.StartCourses = DateTime.Parse(model.StartCourses);
            semester.EndCourses = DateTime.Parse(model.EndCourses);

            Db.SaveChanges();


            return RedirectToAction("Details", new { id = semester.Id });
        }


        public ActionResult Delete(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

            return View(model);
        }

        public ActionResult DeleteConfirmed(string id)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Name.Equals(id));

            Db.Semesters.Remove(semester);

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Ein Semesterdatum anlegen
        /// </summary>
        /// <param name="id">SemesterId</param>
        /// <returns></returns>
        public ActionResult CreateDate(Guid id)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == id);

            var model = new SemesterDateViewModel
            {
                SemesterId = semester.Id,
                Start = semester.StartCourses.Date.ToShortDateString(),
                End = semester.StartCourses.Date.ToShortDateString(),
                HasCourses = false
            };

            ViewBag.Semester = semester;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateDate(SemesterDateViewModel model)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == model.SemesterId);

            var semDate = new SemesterDate
            {
                Description = model.Description,
                From = DateTime.Parse(model.Start),
                To = DateTime.Parse(model.End),
                HasCourses = model.HasCourses,
                Semester = semester
            };

            semester.Dates.Add(semDate);

            Db.SemesterDates.Add(semDate);
            Db.SaveChanges();



            return RedirectToAction("Details", new { id = semester.Id });
        }


        public ActionResult DeleteDate(Guid id)
        {
            var date = Db.SemesterDates.SingleOrDefault(d => d.Id.Equals(id));

            var semester = date.Semester;

            if (semester != null)
            {
                semester.Dates.Remove(date);
            }

            Db.SemesterDates.Remove(date);

            Db.SaveChanges();

            if (semester == null)
            {
                semester = GetSemester();
            }

            return RedirectToAction("Details", new {id = semester.Id});
        }



        [AllowAnonymous]
        public ActionResult Group(Guid id)
        {
            var group = Db.SemesterGroups.SingleOrDefault(g => g.Id == id);
            var courses = group.Activities.OfType<Course>().OrderBy(c => c.Name).ToList();

            var semester = GetSemester();
            var user = AppUser;


            var model = new List<CourseSummaryModel>();
            foreach (var course in courses)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();

                var summary = new CourseSummaryModel();
                summary.Course = course;
                summary.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
                summary.Rooms.AddRange(rooms);

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

            ViewBag.GroupName = group.FullName;
            ViewBag.Semester = group.Semester;
            
            return View(model);
        }

        public ActionResult Enable()
        {
            new SemesterService().EnableNewestSemester();

            return RedirectToAction("Index", "Activity");
        }

        public ActionResult Disable()
        {
            new SemesterService().DisableNewestSemester();

            return RedirectToAction("Index", "Activity");
        }

        public ActionResult Switch(Guid id)
        {
            var sem = Db.Semesters.SingleOrDefault(s => s.Id == id);

            if (sem != null)
            {
                sem.BookingEnabled = !sem.BookingEnabled;
                Db.SaveChanges();
            }

            return RedirectToAction("Details", new {id=sem.Id});
        }


        public ActionResult Init(Guid id)
        {
            return RedirectToAction("List", "Curriculum");
        }

        public ActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGroup(string semname, string groupname)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Name.Equals(semname));
            if (semester == null)
                return RedirectToAction("Index");

            string prog = null;
            string sg = null;

            if (!string.IsNullOrEmpty(groupname))
            {
                var elems = groupname.Split('-');
                if (elems.Count() == 2)
                {
                    prog = elems[0].Trim();
                    sg = elems[1].Trim();
                }
            }

            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Semester.Id == semester.Id &&
                                                                  g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(prog) &&
                                                                  g.CapacityGroup.CurriculumGroup.Name.Equals(sg));

            if (semGroup != null)
                return RedirectToAction("Index");

            var group =
                Db.CurriculumGroups.SingleOrDefault(g => g.Curriculum.ShortName.Equals(prog) &&
                                                          g.Name.Equals(sg));

            if (group != null)
            {
                var curr = Db.SemesterGroups.SingleOrDefault(g =>
                    g.Semester.Id == semester.Id &&
                    g.CapacityGroup.CurriculumGroup.Id == group.Id
                    );

                if (curr == null)
                {
                    var mySem = Db.Semesters.SingleOrDefault(s => s.Id == semester.Id);

                    curr = new SemesterGroup()
                    {
                        Semester = mySem,
                        CurriculumGroup = group
                    };
                    Db.SemesterGroups.Add(curr);
                    Db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }


        public ActionResult DeleteGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteGroup(string groupname)
        {
            string prog = null;
            string sg = null;

            if (!string.IsNullOrEmpty(groupname))
            {
                var elems = groupname.Split('-');
                if (elems.Count() == 2)
                {
                    prog = elems[0].Trim();
                    sg = elems[1].Trim();
                }
            }


            foreach (var semester in Db.Semesters.ToList())
            {
                var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Semester.Id == semester.Id &&
                                                                      g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(prog) &&
                                                                      g.CapacityGroup.CurriculumGroup.Name.Equals(sg));

                if (semGroup != null && !semGroup.Activities.Any())
                {
                    Db.SemesterGroups.Remove(semGroup);
                    Db.SaveChanges();

                }
            }


            var group =
                Db.CurriculumGroups.SingleOrDefault(g => g.Curriculum.ShortName.Equals(prog) &&
                                                          g.Name.Equals(sg));

            if (group != null && !group.SemesterGroups.Any())
            {
                Db.CurriculumGroups.Remove(group);
                Db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }


        [HttpPost]
        public PartialViewResult DeleteGroup2(Guid id)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == id);

            if (semGroup != null && !semGroup.Activities.Any())
            {
                Db.SemesterGroups.Remove(semGroup);
                Db.SaveChanges();
            }

            return PartialView("_EmptyRow");
        }



        public ActionResult Update(string id)
        {
            var model = new Semester();

            var semester = Db.Semesters.SingleOrDefault(s => s.Name.Equals(id));
            // ab hier sinnlos () Lucas is schuld!
            if(semester != null)
            {
                model.Name = semester.Name;
                model.StartCourses = semester.StartCourses;
                model.EndCourses = semester.EndCourses;

                return View(model);
            }
            else
            {
                // Error ausgeben
                return RedirectToAction("Index");
            }
        }

        public ActionResult TransferGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TransferGroup(string sourcegroup, string targetgroup)
        {
            string sourceProg = null;
            string sourceSemGroup = null;

            if (!string.IsNullOrEmpty(sourcegroup))
            {
                var elems = sourcegroup.Split('-');
                if (elems.Count() == 2)
                {
                    sourceProg = elems[0].Trim();
                    sourceSemGroup = elems[1].Trim();
                }
            }

            string targetProg = null;
            string targetSemGroup = null;

            if (!string.IsNullOrEmpty(targetgroup))
            {
                var elems = targetgroup.Split('-');
                if (elems.Count() == 2)
                {
                    targetProg = elems[0].Trim();
                    targetSemGroup = elems[1].Trim();
                }
            }

            foreach (var semester in Db.Semesters.ToList())
            {
                var sourceGroup = Db.SemesterGroups.SingleOrDefault(g => g.Semester.Id == semester.Id &&
                                                                      g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(sourceProg) &&
                                                                      g.CapacityGroup.CurriculumGroup.Name.Equals(sourceSemGroup));

                var targetGroup = Db.SemesterGroups.SingleOrDefault(g => g.Semester.Id == semester.Id &&
                                                                      g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(targetProg) &&
                                                                      g.CapacityGroup.CurriculumGroup.Name.Equals(targetSemGroup));

                if (sourceGroup != null && targetGroup != null)
                {
                    foreach (var act in sourceGroup.Activities)
                    {
                        act.SemesterGroups.Remove(sourceGroup);
                        act.SemesterGroups.Add(targetGroup);
                    }
                }

                Db.SaveChanges();
            }



            return RedirectToAction("Index");
        }

        public ActionResult InitGroups(Guid id)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == id);

            if (semester == null)
                return RedirectToAction("Index");

            var isWS = semester.Name.StartsWith("WS");


            // Alle Curricula durchgehen
            foreach (var curriculum in Db.Curricula.ToList())
            {
                foreach (var curriculumGroup in curriculum.CurriculumGroups.ToList())
                {
                    foreach (var capacityGroup in curriculumGroup.CapacityGroups.ToList())
                    {
                        if ((capacityGroup.InWS && isWS) || (capacityGroup.InSS && !isWS))
                        {
                            var exist = semester.Groups.Any(g => g.CapacityGroup.Id == capacityGroup.Id);

                            if (!exist)
                            {
                                var semGroup = new SemesterGroup
                                {
                                    CapacityGroup = capacityGroup,
                                    CurriculumGroup = capacityGroup.CurriculumGroup,        // nur noch aus Gründen der Sicherheit
                                    Semester = semester
                                };

                                semester.Groups.Add(semGroup);
                                Db.SemesterGroups.Add(semGroup);
                            }
                        }
                    }
                }
            }

            Db.SaveChanges();
            
            
            return RedirectToAction("GroupList", new {id = id});
        }


        public ActionResult BugFixing()
        {
            return View();
        }

        public ActionResult RepairGroups()
        {
            return RedirectToAction("Index");
        }


        private CurriculumGroup InsertGroup(string curr, string name)
        {
            var mba = Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(curr));

            if (mba != null)
            {
                var group = mba.CurriculumGroups.SingleOrDefault(g => g.Name.Equals(name));
                if (group == null)
                {

                    group = new CurriculumGroup
                    {
                        Name = name,
                    };

                    mba.CurriculumGroups.Add(group);

                    Db.SaveChanges();
                }

                return group;
            }

            return null;
        }



        public ActionResult ClearFiles(Guid id)
        {
            string tempDir = Path.GetTempPath();

            var semester = new SemesterService().GetSemester(id);

            tempDir = Path.Combine(tempDir, semester.Name);

            if (Directory.Exists(tempDir))
            {
                var fileNames = Directory.EnumerateFiles(tempDir);
                foreach (var fileName in fileNames)
                {
                    System.IO.File.Delete(fileName);
                }
                Directory.Delete(tempDir);
            }

            return RedirectToAction("GpUntis", new { id = id });
        }

        public ActionResult Report(Guid id)
        {
            var semester = new SemesterService().GetSemester(id);

            var allCourses =
                Db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any(g => g.Semester.Id == id)).ToList();


            return View();
        }
    
    }
}