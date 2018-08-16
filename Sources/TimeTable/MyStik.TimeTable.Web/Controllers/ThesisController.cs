using System;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisController : BaseController
    {
        /// <summary>
        /// Der Status meiner Abschlussarbeit
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // der aktuelle Benutzer
            var user = GetCurrentUser();
            var org = GetMyOrganisation();

            var theses = Db.Theses.Where(x => 
                x.Supervision.Owners.Any(o => o.Member.Organiser.Id == org.Id)).ToList();

            ViewBag.UserRight = GetUserRight();

            return View(theses);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var org = GetMyOrganisation();

            var model = new ThesisOverviewModel
            {
                Semester = semester,
                Organiser = org
            };

            var thesis = Db.Activities.OfType<Exam>().Where(oh =>
                    oh.SemesterGroups.Any(x => x.Semester.Id == semester.Id) &&
                    oh.Owners.Any(m => m.Member.Organiser.Id == org.Id))
                .ToList();

            foreach (var thesi in thesis)
            {

                var tm = new ThesisOfferViewModel
                {
                    Thesis = thesi,
                    Curriculum = thesi.SemesterGroups.First().CapacityGroup.CurriculumGroup.Curriculum,
                    Lecturer = thesi.Owners.First().Member
                };

                model.Thesis.Add(tm);
            }

            ViewBag.UserRight = GetUserRight(org);


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateThesis(Guid? id)
        {
            var sem = SemesterService.GetSemester(id);

            var org = GetMyOrganisation();

            CourseCreateModel2 model = new CourseCreateModel2();

            model.SemesterId = sem.Id;
            model.OrganiserId = org.Id;
            model.OrganiserId2 = org.Id;
            model.OrganiserId3 = org.Id;

            // Liste aller Fakultäten
            ViewBag.Organiser = Db.Organisers.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Liste aller Fakultäten, auf die Zugriff auf Räume bestehen
            // aktuell nur meine
            ViewBag.RoomOrganiser = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Alle Semester, die in Zukunft enden
            ViewBag.Semester = Db.Semesters.Where(x => x.EndCourses >= DateTime.Today).OrderBy(s => s.StartCourses)
                .Take(5).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            model.SemesterId = sem.Id;

            ViewBag.Curricula = null;
            ViewBag.Groups = null;
            ViewBag.Chapters = null;
            ViewBag.Topics = null;




            // bei der ersten Anzeige wird kein onChange ausgelöst
            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).ToList();
            var curr = currList.FirstOrDefault();
            if (curr != null)
            {
                model.CurriculumId = curr.Id;
                ViewBag.Curricula = currList.Select(c => new SelectListItem
                {
                    Text = c.ShortName,
                    Value = c.Id.ToString(),
                });


                // alle Studiengruppen
                var currGroups = curr.CurriculumGroups.ToList();
                var group = currGroups.FirstOrDefault();
                model.CurrGroupId = @group?.Id ?? Guid.Empty;
                ViewBag.CurrGroups = currGroups.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });

                // alle Kapazitätsgruppen

                if (group != null)
                {
                    var capGroups = group.CapacityGroups.ToList();
                    var firstGroup = capGroups.FirstOrDefault();
                    model.CapGroupId = firstGroup?.Id ?? Guid.Empty;

                    ViewBag.CapGroups = capGroups.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    });
                }


                var chapters = curr.Chapters.ToList();
                var chapter = chapters.FirstOrDefault();
                model.ChapterId = chapter?.Id ?? Guid.Empty;
                ViewBag.Chapters = chapters.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });

                if (chapter != null)
                {
                    var topics = chapter.Topics.ToList();
                    var topic = topics.FirstOrDefault();
                    model.TopicId = topic?.Id ?? Guid.Empty;

                    ViewBag.Topics = topics.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    });
                }
            }


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateThesis(CourseCreateModelExtended model)
        {
            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(model.SemesterId);

            var course = new Exam
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Organiser = org,
                Occurrence = new Occurrence
                {
                    Capacity = -1,
                    IsAvailable = true,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    IsCanceled = false,
                    IsMoved = false,
                    UseGroups = false,
                },
            };

            // da kommen Kapazitätsgruppen
            // d.h. Semestergruppe suchen und ggf. anlegen
            if (model.GroupIds != null)
            {
                foreach (var groupId in model.GroupIds)
                {
                    var capGroup = Db.CapacityGroups.SingleOrDefault(g => g.Id == groupId);

                    var semGroup =
                        Db.SemesterGroups.SingleOrDefault(
                            g => g.Semester.Id == semester.Id && g.CapacityGroup.Id == groupId);

                    // die Semestergruppe gibt es nicht => anlegen
                    if (semGroup == null)
                    {
                        semGroup = new SemesterGroup
                        {
                            CapacityGroup = capGroup,
                            Semester = semester,
                            IsAvailable = false,
                        };
                        Db.SemesterGroups.Add(semGroup);
                    }
                    course.SemesterGroups.Add(semGroup);

                    var occGroup = new OccurrenceGroup
                    {
                        Capacity = 0,
                        FitToCurriculumOnly = true,
                        Occurrence = course.Occurrence
                    };
                    occGroup.SemesterGroups.Add(semGroup);
                    semGroup.OccurrenceGroups.Add(occGroup);
                    course.Occurrence.Groups.Add(occGroup);
                    Db.OccurrenceGroups.Add(occGroup);
                }
            }



            var member = GetMyMembership();

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = course,
                    Member = me,
                    IsLocked = false
                };

                course.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }


            Db.Activities.Add(course);
            Db.SaveChanges();

            return PartialView("_CreateThesisSuccess", course);
        }

        /// <summary>
        /// Die Sicht des Studierenden
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var model = new ThesisDetailModel
            {
                Thesis = thesis,
                Lecturer = thesis.Supervision.Owners.First().Member
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }




        public ActionResult Edit(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var model = new ThesisEditModel
            {
                Thesis = thesis,
                TitleDe = thesis.TitleDe,
                TitleEn = thesis.TitleEn,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ThesisEditModel model)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);
            thesis.TitleDe = model.TitleDe;
            thesis.TitleEn = model.TitleEn;
            Db.SaveChanges();

            return RedirectToAction("Details", new {id = model.Thesis.Id});
        }

        /*
        public ActionResult Request(Guid id)
        {
            var user = GetCurrentUser();
            var thesis = Db.Activities.OfType<Exam>().SingleOrDefault(x => x.Id == id);

            var subscription = thesis.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

            if (subscription != null)
                return RedirectToAction("Details", new { id = id });

            // Eintragen
            // Warteliste
            // Dozent manipuliert onWaitingList
            // PKV manipuliert IsConfirmed
            subscription = new OccurrenceSubscription
            {
                UserId = user.Id,
                TimeStamp = DateTime.Now,
                OnWaitingList = true,
                IsConfirmed = false
            };

            thesis.Occurrence.Subscriptions.Add(subscription);
            Db.SaveChanges();

            // Mail an Betreuer
            if (thesis.Owners.Any())
            {
                var member = thesis.Owners.First().Member;
                var hostUser = GetUser(member.UserId);

                var mailModel = new ThesisRequestMailModel
                {
                    Thesis = thesis,
                    Requester = user, // der anfragende Student
                    User = hostUser, // der Betreuuer
                };

                var mail = new MailController();
                mail.ThesisRequestEMail(mailModel).Deliver();
            }


            return RedirectToAction("Details", new { id = id });
        }
        */

        public ActionResult Reject(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);
            var thesis = Db.Activities.OfType<Exam>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);
            var user = GetUser(subscription.UserId);


            var model = new ThesisRejectModel
            {
                Thesis = thesis,
                Subscription = subscription,
                User = user
            };

            return View(model);
        }


        public ActionResult RejectConfiremd(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);
            var thesis = Db.Activities.OfType<Exam>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);
            var user = GetUser(subscription.UserId);

            thesis.Occurrence.Subscriptions.Remove(subscription);
            Db.Subscriptions.Remove(subscription);
            Db.SaveChanges();


            var mailModel = new ThesisRejectMailModel
            {
                Thesis = thesis,
                Lecturer = GetCurrentUser(),
                User = user, // der Student
            };

            var mail = new MailController();
            mail.ThesisRejectEMail(mailModel).Deliver();

            return RedirectToAction("Admin", new {id = thesis.Id});
        }


        public ActionResult Accept(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);
            var thesis = Db.Activities.OfType<Exam>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);
            var user = GetUser(subscription.UserId);

            // Akzeptieren heisst "Teilnehmer"
            // nicht mehr auf Warteliste 
            subscription.OnWaitingList = false;

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = thesis.Id });
        }


        public ActionResult Issue(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);
            var supervision = Db.Activities.OfType<Supervision>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);
            var user = GetUser(subscription.UserId);
            var subInfoService = new SemesterSubscriptionService();
            var semester = SemesterService.GetSemester(DateTime.Today);



            var model = new ThesisAcceptModel
            {
                Subscription = subscription,
                Supervision = supervision,
                User = user
            };

            model.Curriculum = subInfoService.GetBestCurriculum(subscription.UserId, semester);
            model.Lecturer = supervision.Owners.First().Member;

            int period = 0;

            bool success = int.TryParse(model.Curriculum.Version, out period);
            if (!success || period == 0)
            {
                period = 3;
            }

            model.Period = period;
            model.IssueDate = DateTime.Today;
            model.DeliveryDate = model.IssueDate.AddMonths(period);


            return View(model);
        }


        [HttpPost]
        public ActionResult Issue(ThesisAcceptModel model)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == model.Subscription.Id);
            var supervision = Db.Activities.OfType<Supervision>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);
            var user = GetUser(subscription.UserId);

            // aus der subscription eine Thesis machen
            var subInfoService = new SemesterSubscriptionService();
            var semester = SemesterService.GetSemester(DateTime.Today);
            var curriculum = subInfoService.GetBestCurriculum(subscription.UserId, semester);

            int period = 0;
            bool success = int.TryParse(curriculum.Version, out period);
            if (!success || period == 0)
            {
                period = 3;
            }

            var student = Db.Students.FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Curriculum.Id == curriculum.Id);
            if (student == null)
            {
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == curriculum.Id);
                student = new Student
                {
                    UserId = user.Id,
                    Curriculum = curr
                };

                Db.Students.Add(student);
            }


            var thesis = new Thesis();
            thesis.TitleDe = model.Title;
            thesis.Supervision = supervision;
            thesis.IssueDate = DateTime.Today;
            thesis.ExpirationDate = thesis.IssueDate.AddMonths(period);
            thesis.Student = student;

            Db.Theses.Add(thesis);

            // Subscription und alle anderen löschen
            var list = Db.Activities.OfType<Supervision>().Where(x =>
                x.Occurrence.Subscriptions.Any(u => u.UserId.Equals(subscription.UserId))).ToList();

            foreach (var item in list)
            {
                var sub = item.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                Db.Subscriptions.Remove(sub);
            }

            Db.SaveChanges();

            // oder zur Detailseite
            return RedirectToAction("Thesis", "Lecturer");
        }

        public ActionResult Delete(Guid id)
        {
            var exam = Db.StudentExams.SingleOrDefault(x => x.Id == id);

            var thesis = exam.Exam;

            // alle paper löschen
            var papers = exam.ExamPapers.ToList();

            foreach (var paper in papers)
            {
                // TODO Dokumemnt löschen
                //paper.Document
                exam.ExamPapers.Remove(paper);

                Db.ExamPapers.Remove(paper);
            }
            Db.StudentExams.Remove(exam);
            Db.SaveChanges();



            return RedirectToAction("Admin", new { id = thesis.Id });
        }
    }
    }