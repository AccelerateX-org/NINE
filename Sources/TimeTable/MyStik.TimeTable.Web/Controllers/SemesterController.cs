using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterController : BaseController
    {
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.SemesterList = Db.Semesters.OrderByDescending(s => s.StartCourses).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Name,
            });

            var org = GetMyOrganisation();


            ViewBag.UserRight = GetUserRight(org);

            var model = new SemesterViewModel
            {
                Semester = GetSemester()
            };

            return View("Index2", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DateList(string semGroupId)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Name.Equals(semGroupId));

            if (model == null)
            {
                model = Db.Semesters.FirstOrDefault();
            }

            ViewBag.UserRight = GetUserRight();

            return PartialView("_DateList", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GroupList(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                //Start = semester.StartCourses.Date.ToShortDateString(),
                Start = DateTime.Today.ToShortDateString(),
                //End = semester.StartCourses.Date.ToShortDateString(),
                End = DateTime.Today.ToShortDateString(),
                HasCourses = false
            };

            ViewBag.Semester = semester;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                Semester = semester,
                
            };

            semester.Dates.Add(semDate);

            Db.SemesterDates.Add(semDate);
            Db.SaveChanges();



            return RedirectToAction("Details", new { id = semester.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id des Datums, dessen Attributwerte verändert werden solllen</param>
        /// <returns></returns>
        public ActionResult EditDate(Guid id)
        {
            var date = Db.SemesterDates.SingleOrDefault(s => s.Id == id);
            var semester = Db.Semesters.SingleOrDefault(s => s.Dates.Any(d => d.Id == id));

            var model = new SemesterDateViewModel
            {
                DateId = date.Id,
                SemesterId = semester.Id,
                Start = date.From.ToShortDateString(),
                End = date.To.ToShortDateString(),
                Description = date.Description,
                HasCourses = date.HasCourses
            };

            ViewBag.Semester = semester;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDate(SemesterDateViewModel model)
        {
            // wir müssen uns hier wieder das Objekt aus der Datenbank holen
            // Dazu brauchen wir die Id
            // Daher mmüssen wir die Id schon vorher an das Formular übergeben
            var date = Db.SemesterDates.SingleOrDefault(s => s.Id == model.DateId);
            var semester = Db.Semesters.SingleOrDefault(s => s.Dates.Any(d => d.Id == model.DateId));

            // Jetzt aktualisieren wir die Attributwerte des Objekts aus der Datenbank
            // mit den Attributwerten aus dem Formular
            date.Description = model.Description;
            date.From = DateTime.Parse(model.Start);
            date.To = DateTime.Parse(model.End);
            date.HasCourses = model.HasCourses;
            // Reparatur
            if (date.Semester == null)
            {
                date.Semester = semester;
            }

            // Objekt wurde geändert
            // Jetzt speichern wir es in der Datenbank
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = date.Semester.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Init(Guid id)
        {
            return RedirectToAction("List", "Curriculum");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateGroup()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteGroup()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferGroup()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcegroup"></param>
        /// <param name="targetgroup"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult BugFixing()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Erstellt eine Auswertung aller Lehrveranstaltungen nach Dozenten
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Report(Guid id)
        {
            var model = CreateSemesterReport(id);
            var semester = GetSemester(id);
            ViewBag.Semester = semester;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult ReportFile(Guid id)
        {
            var model = CreateSemesterReport(id);
            var semester = GetSemester(id);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write("Studiengang;Gruppe;Dozent;Name;Eintragungen");
            writer.Write(Environment.NewLine);


            foreach (var course in model)
            {

                writer.Write("{0};{1};{2};{3};{4}",
                    course.Curriculum.ShortName, course.Group.FullName,
                    course.Lecturer.Name,
                    course.Course.Name,
                    course.Course.Occurrence.Subscriptions.Count);
                writer.Write(Environment.NewLine);

            }


            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Lehrangebot_");
            sb.Append(semester.Name);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }


        private List<SemesterCourseViewModel> CreateSemesterReport(Guid id)
        {

        var model = new List<SemesterCourseViewModel>();

            // Alle Lehrveranstaltungen in diesem Semester
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(s => s.Semester.Id == id)).ToList();

            // für jede Lehrveranstaltung alle Dozenten
            foreach (var course in courses)
            {
                // Alle Dozenten in dieser LV
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();


                // Für jede Semestergruppe
                foreach (var semesterGroup in course.SemesterGroups)
                {

                    foreach (var lecture in lectures)
                    {
                        var courseModel = new SemesterCourseViewModel
                        {
                            Course = course,
                            Curriculum = semesterGroup.CapacityGroup.CurriculumGroup.Curriculum,
                            Lecturer = lecture,
                            Group = semesterGroup
                        };

                        model.Add(courseModel);

                    }
                }
            }

            model = model.OrderBy(x => x.Curriculum.ShortName).ThenBy(x => x.Lecturer.Name).ToList();

            return model; 
        }


    }
}