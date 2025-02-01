using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Alle Semester mit Stundenplänen, die in der Zukunft enden
            var semesterList = Db.Semesters.Where(x =>
                x.EndCourses >= DateTime.Today &&
                x.Groups.Any(g => g.IsAvailable)).ToList();

            var semSubService = new SemesterSubscriptionService(Db);

            var model = new List<SemesterSubscriptionOverviewModel>();
            foreach (var semester in semesterList)
            {
                var semGroup = semSubService.GetSemesterGroup(AppUser.Id, semester);

                var m = new SemesterSubscriptionOverviewModel
                {
                    Semester = semester,
                    Group = semGroup
                };

                model.Add(m);
            }


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Subscribe(Guid id)
        {
            var semester = SemesterService.GetSemester(id);

            var semSubService = new SemesterSubscriptionService(Db);

            // Liegt eine Einschreibung vor?
            var semGroup = semSubService.GetSemesterGroup(AppUser.Id, semester);

            SemesterSubscriptionViewModel model;
            if (semGroup == null)
            {
                model = GetModel(semester);
            }
            else
            {
                model = new SemesterSubscriptionViewModel
                {
                    Group = semGroup,
                    Semester = semester.Id.ToString()
                };
            }

            ViewBag.Semester = semester;

            return View("Semester", model);
        }

        private SemesterSubscriptionViewModel GetModel(Semester semester)
        {
            var model = new SemesterSubscriptionViewModel
            {
                Semester = semester.Id.ToString(),
                Group = null,
                Faculty = string.Empty,
                Curriculum = string.Empty,
                CurrGroup = string.Empty,
            };

            var semService = new SemesterService(Db);
            var semSubService = new SemesterSubscriptionService(Db);

            // Alle Fakultäten, die aktive Semestergruppen haben
            var acticeorgs = semService.GetActiveOrganiser(semester);

            ViewBag.Semesters = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = semester.Name,
                    Value = semester.Id.ToString()
                }
            };


            // Immer alle anzeigen, die was haben
            ViewBag.Faculties = acticeorgs.OrderBy(f => f.ShortName).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            // Liste der Studiengänge
            var actOrg = acticeorgs.FirstOrDefault();
            var activecurr = semService.GetActiveCurricula(actOrg, semester, true);

            // Liste der Gruppen hängt jetzt von der Einschreibung ab

            // keine Einschreibung vorhanden
            if (!activecurr.Any())
            {
                model.HasData = false;
            }
            else
            {
                model.HasData = true;

                ViewBag.Curricula = activecurr.OrderBy(f => f.ShortName).Select(f => new SelectListItem
                {
                    Text = string.Format("{0} ({1})", f.ShortName, f.Name),
                    Value = f.Id.ToString(),
                });


                // nimm den ersten
                var myCurr = activecurr.First();

                var semesterGroups = Db.SemesterGroups.Where(g =>
                        g.Semester.Id == semester.Id &&
                        g.IsAvailable &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == myCurr.Id)
                    .OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


                var semGroups = semesterGroups.Select(s => new SelectListItem
                {
                    Text = s.FullName,
                    Value = s.Id.ToString()
                }).ToList();

                ViewBag.Groups = semGroups;
            }

            ViewBag.Semester = semester;

            return model;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSemesterGroup(SemesterSubscriptionViewModel model)
        {
            var user = AppUser;

            var semSubService = new SemesterSubscriptionService();

            if (user != null)
            {
                var group = Db.SemesterGroups.SingleOrDefault(g => g.Id.ToString().Equals(model.CurrGroup));

                if (group != null)
                {
                    semSubService.Subscribe(user.Id, group.Id);
                    model.Group = group;
                }
            }


            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UnSubscribe(Guid id)
        {
            var user = AppUser;
            var semester = SemesterService.GetSemester(id);

            if (user != null)
            {
                // Alle Semestergruppen löschen 
                var group = Db.Subscriptions.OfType<SemesterSubscription>().Where(g =>
                    g.UserId.Equals(user.Id) &&
                    g.SemesterGroup.Semester.Id == semester.Id).ToList();

                foreach (var sub in group)
                {
                    sub.SemesterGroup.Subscriptions.Remove(sub);
                    Db.Subscriptions.Remove(sub);
                }

                Db.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        public ActionResult StartCurriculum()
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var user = GetCurrentUser();

            var semSubService = new SemesterSubscriptionService();
            var semesterSubscription = semSubService.GetSemesterGroup(user.Id, semester);

            var model = new CurriculumSubscriptionViewModel
            {
                IsFullTime = true
            };

            var orgs = Db.Organisers.Where(x => x.IsFaculty && x.Curricula.Any()).OrderBy(f => f.ShortName).ToList();
            var org = semesterSubscription?.CapacityGroup.CurriculumGroup.Curriculum.Organiser ?? orgs.FirstOrDefault();


            var orgSelect = new List<SelectListItem>
            {
                new SelectListItem { Text = "Fakultät wählen", Value = Guid.Empty.ToString() }
            };
            orgSelect.AddRange(orgs.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            }));
            orgSelect.First().Selected = true;

            ViewBag.Faculties = orgSelect;

            ViewBag.Curricula = new List<SelectListItem>();

            /*
            var nextDate = DateTime.Today.AddDays(70);

            ViewBag.Semesters = Db.Semesters.Where(x => x.StartCourses <= nextDate).OrderByDescending(x => x.EndCourses)
                .Take(8)
                .Select(f => new SelectListItem
                    {
                        Text = f.Name,
                        Value = f.Id.ToString(),
                    }
                );
            */

            var currentSemester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(currentSemester);

            // vom NextSemester 8 zurück

            var semesters = Db.Semesters.Where(x => x.StartCourses <= nextSemester.StartCourses)
                .OrderByDescending(x => x.EndCourses)
                .Take(8).ToList();

            var semSelect = new List<SelectListItem>();
            semSelect.Add(new SelectListItem { Text = "Semester wählen", Value = Guid.Empty.ToString() });
            semSelect.AddRange(semesters.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            }));
            semSelect.First().Selected = true;
            ViewBag.Semesters = semSelect;

            return View("Change", model);
        }

        [HttpPost]
        public ActionResult StartCurriculumConfirm(CurriculumSubscriptionViewModel model)
        {
            model.Semester = Db.Semesters.SingleOrDefault(x => x.Id == model.SemId);
            model.Curriculum = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);
            model.Organiser = Db.Organisers.SingleOrDefault(x => x.Id == model.OrgId);

            return View("ChangeConfirm", model);
        }


        [HttpPost]
        public ActionResult StartCurriculum(CurriculumSubscriptionViewModel model)
        {
            var user = GetCurrentUser();
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == model.SemId);
            var nextCurr = Db.Curricula.Include(curriculum =>
                curriculum.Organiser.Institution.LabelSet.ItemLabels).SingleOrDefault(x => x.Id == model.CurrId);

            // gibt es ein aktuelles Studium
            var student = StudentService.GetCurrentStudent(user);
            if (student != null)
            {
                // dieses Studium abschliessen
                if (student.LastSemester == null)
                {
                    student.LastSemester = SemesterService.GetPreviousSemester(semester);
                    student.HasCompleted = true;
                }
            }

            var currentCurr = student.Curriculum;

            if (currentCurr.Id == nextCurr.Id)
            {
                return View("Invalid", student);
            }


            // neuen Studiengang beginnen
            student = new Student
            {
                UserId = user.Id
            };

            Db.Students.Add(student);

            student.Created = DateTime.Now;
            student.FirstSemester = semester;
            student.Curriculum = nextCurr;
            student.IsDual = model.IsDual;
            student.IsPartTime = !model.IsFullTime;
            student.HasCompleted = false;

            // CIE
            if (model.IsIncomer)
            {
                if (student.LabelSet == null)
                {
                    student.LabelSet = new ItemLabelSet();
                    //Db.ItemLabelSets.Add(student.LabelSet);
                }

                var inst = student.Curriculum.Organiser.Institution;

                var cieLabel = inst?.LabelSet.ItemLabels.FirstOrDefault(x => x.Name.Equals("CIE"));

                if (cieLabel != null)
                {
                    student.LabelSet.ItemLabels.Add(cieLabel);
                }
            }

            Db.SaveChanges();

            var nStudent = Db.Students.Count(x => x.UserId.Equals(user.Id));
            if (nStudent == 1)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToAction("Curricula");
        }


        public ActionResult Curricula()
        {
            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);

            var myStudents = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderBy(x => x.Created).ToList();

            var model = new List<AlumniViewModel>();

            foreach (var student in myStudents)
            {
                // nach Alumni suche
                var alumni = Db.Alumnae.SingleOrDefault(x =>
                    x.UserId.Equals(user.Id) && x.Curriculum.Id == student.Curriculum.Id);


                var m = new AlumniViewModel
                {
                    Student = student,
                    Alumni = alumni
                };

                model.Add(m);
            }

            ViewBag.Semester = semester;
            ViewBag.NextSemester = nextSemester;


            return View(model);
        }


        public ActionResult ChangeNumber(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var user = GetCurrentUser();

            if (!user.Id.Equals(student.UserId)) 
                return RedirectToAction("Index", "Dashboard");
            
            
            var model = new CurriculumSubscriptionViewModel();
            model.User = user;
            model.Student = student;
            model.Number = student.Number;

            return View(model);

        }


        [HttpPost]
        public ActionResult ChangeNumber(CurriculumSubscriptionViewModel model)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == model.Student.Id);
            var user = GetCurrentUser();

            if (!user.Id.Equals(student.UserId))
                return RedirectToAction("Index", "Dashboard");

            student.Number = model.Number;
            Db.SaveChanges();

            return RedirectToAction("Curricula");
        }


        public ActionResult ChangeCurriculum(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);


            var semester = SemesterService.GetSemester(DateTime.Today);
            var user = GetCurrentUser();


            var model = new CurriculumSubscriptionViewModel
            {
                OrgId = student.Curriculum.Organiser.Id,
                CurrId = student.Curriculum.Id,
                SemId = student.FirstSemester.Id,
                Student = student
            };

            var orgs = Db.Organisers.Where(x => x.IsFaculty && x.Curricula.Any()).OrderBy(f => f.ShortName).ToList();
            var org = student.Curriculum.Organiser;

            ViewBag.Faculties = orgs.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });


            ViewBag.Curricula = org.Curricula.OrderBy(f => f.ShortName).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            var nextDate = DateTime.Today.AddDays(70);

            ViewBag.Semesters = Db.Semesters.Where(x => x.StartCourses <= nextDate).OrderByDescending(x => x.EndCourses)
                .Select(f => new SelectListItem
                    {
                        Text = f.Name,
                        Value = f.Id.ToString(),
                    }
                );


            return View(model);
        }


        [HttpPost]
        public ActionResult ChangeCurriculum(CurriculumSubscriptionViewModel model)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == model.Student.Id);

            student.FirstSemester = Db.Semesters.SingleOrDefault(x => x.Id == model.SemId);
            student.Curriculum = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);
            student.IsDual = model.IsDual;
            student.IsPartTime = model.IsPartTime;
            student.HasCompleted = false;

            Db.SaveChanges();


            return RedirectToAction("Curricula");
        }

        public ActionResult ChangeLabel(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);


            return View(student);
        }

        public ActionResult LeaveCurriculum(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);

            return View(student);
        }

        public ActionResult EndCurriculum(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);

            student.LastSemester = SemesterService.GetSemester(DateTime.Today);
            student.HasCompleted = true;

            Db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }

    }
}