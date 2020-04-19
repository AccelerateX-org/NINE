using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Helpers;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class DashboardController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var userRight = GetUserRight();
            ViewBag.UserRight = userRight;

            if (User.IsInRole("SysAdmin"))
            {
                if (Session["OrgAdminId"] == null)
                {
                    return View("DashboardSysAdmin");
                }

                return View("DashboardOrgMember", CreateDashboardModelOrgMember(userRight));
            }

            var meberships = GetMyMemberships();
            var student = StudentService.GetCurrentStudent(userRight.User.Id);
            var alumni = GetMyAlumni();

            // wenn alles nix, dann erster Besuch
            if (!meberships.Any() && student == null && !alumni.Any())
            {
                return View("FirstVisit");
            }

            // alle anderen Fälle
            // wer  member ist ist Dozent => Check mit Fachschaft
            // wer nur student ist Student
            // aber auch: nochmal check auf "eine Sicht für alle"





            switch (userRight.User.MemberState)
            {
                case MemberState.Student:
                {
                    return View("DashboardStudentNew", CreateDashboardModelStudentNew(userRight));
                }

                case MemberState.Staff:
                    return View("DashboardOrgMemberNew", CreateDashboardModelOrgMemberNew(userRight));
                default:
                    return View("DashboardDefault", CreateDashboardModelDefault(userRight));
            }
        }

        public ActionResult Schedule(Guid id)
        {
            var userRight = GetUserRight();
            ViewBag.UserRight = userRight;

            var semSubService = new SemesterSubscriptionService();

            var user = AppUser;

            // das übergebene Semester
            var currentSemester = SemesterService.GetSemester(id);

            var model = new DashboardStudentViewModel();
            model.User = user;
            model.Organiser = GetMyOrganisation();

            model.Semester = currentSemester;
            model.SemesterGroup = semSubService.GetSemesterGroup(user.Id, currentSemester);

            return View(model);
        }



        private DashboardViewModel CreateDashboardModelDefault(UserRight userRight)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var model = new DashboardViewModel
            {
                Semester = semester,
                User = userRight.User
            };
            return model;
        }

        private DashboardViewModel CreateDashboardModelOrgMember(UserRight userRight)
        {
            var user = userRight.User;
            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);
            var previousSemester = SemesterService.GetPreviousSemester(semester);
            var org = GetMyOrganisation();
            var member = GetMyMembership();


            var model = new DashboardViewModel
            {
                Semester = semester,
                User = user,
                Member = member,
                Organiser = org,
                ThisSemesterActivities = {Semester = semester}
            };

            if (member == null)
                return model;

            var ohs = new OfficeHourInfoService(UserManager);

            // alle Aktivitäten, im aktuellen Semester
            var officeHour = new OfficeHourService(Db).GetOfficeHour(member, semester);
            model.ThisSemesterActivities.MyOfficeHour = officeHour;
            if (officeHour != null)
            {
                model.ThisSemesterActivities.NextOfficeHourDate = ohs.GetPreviewNextDate(officeHour);
            }

            model.ThisSemesterActivities.MyCourses.AddRange(GetLecturerCourses(semester, userRight.User));
            model.ThisSemesterActivities.MyEvents.AddRange(GetLecturerEvents(semester, userRight.User));
            model.ThisSemesterActivities.MyReservations.AddRange(GetLecturerReservations(semester, userRight.User));
            model.ThisSemesterActivities.MyExams.AddRange(GetLecturerExams(semester, userRight.User));

            // und für das nächste Semester
            // nur wenn es Stundenpläne gibt, diese müssen noch nicht freigegeben sein, Existenz reicht
            if (nextSemester != null)
            {
                var activeCurriclula = SemesterService.GetActiveCurricula(org, nextSemester, false);
                if (activeCurriclula.Any())
                {
                    model.NextSemester = nextSemester;

                    officeHour = new OfficeHourService(Db).GetOfficeHour(member, nextSemester);

                    model.NextSemesterActivities.MyOfficeHour = officeHour;
                    model.NextSemesterActivities.Semester = nextSemester;
                    if (officeHour != null)
                    {
                        model.NextSemesterActivities.NextOfficeHourDate = ohs.GetPreviewNextDate(officeHour);
                    }

                    model.NextSemesterActivities.MyCourses.AddRange(GetLecturerCourses(nextSemester, userRight.User));
                    model.NextSemesterActivities.MyEvents.AddRange(GetLecturerEvents(nextSemester, userRight.User));
                    model.NextSemesterActivities.MyReservations.AddRange(GetLecturerReservations(nextSemester, userRight.User));
                    model.NextSemesterActivities.MyExams.AddRange(GetLecturerExams(nextSemester, userRight.User));
                }
            }

            // es gibt (noch) kein nächstes Semester, dann das vorherige prüfen
            if (model.NextSemester == null && previousSemester != null)
            {
                var activeCurriclula = SemesterService.GetActiveCurricula(org, previousSemester, false);
                if (activeCurriclula.Any())
                {
                    model.PreviousSemester = previousSemester;

                    officeHour = new OfficeHourService(Db).GetOfficeHour(member, previousSemester);

                    model.PreviousSemesterActivities.MyOfficeHour = officeHour;
                    model.PreviousSemesterActivities.Semester = previousSemester;
                    if (officeHour != null)
                    {
                        model.PreviousSemesterActivities.NextOfficeHourDate = ohs.GetPreviewNextDate(officeHour);
                    }

                    model.PreviousSemesterActivities.MyCourses.AddRange(GetLecturerCourses(previousSemester, userRight.User));
                    model.PreviousSemesterActivities.MyEvents.AddRange(GetLecturerEvents(previousSemester, userRight.User));
                    model.PreviousSemesterActivities.MyReservations.AddRange(GetLecturerReservations(previousSemester, userRight.User));
                    model.PreviousSemesterActivities.MyExams.AddRange(GetLecturerExams(previousSemester, userRight.User));
                }
            }


            // Was läuft gerade?
            // alles was jetzt läuft
            // alles was in den nächsten 60 min beginnt
            var now = DateTime.Now;
            var endOfDay = now.AddHours(1);

            var nowPlaying = Db.ActivityDates.Where(d =>
                d.Activity is Course &&
                (d.Begin <= now && now < d.End || d.Begin > now && d.Begin < endOfDay) &&
                d.Activity.SemesterGroups.Any(g =>
                    g.CapacityGroup != null &&
                    g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
            .OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();

            model.NowPlayingDates = nowPlaying;

            return model;
        }


        private List<ActivitySummary> GetLecturerCourses(Semester semester, ApplicationUser user)
        {
            List<ActivitySummary> model = new List<ActivitySummary>();

            var lectureActivities =
                Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();

            foreach (var activity in lectureActivities)
            {
                var summary = new ActivitySummary { Activity = activity };

                // nur die, bei denen es noch Termine in der Zukunft gibt
                if (activity.Dates.Any(x => x.End >= DateTime.Now))
                {
                    var currentDate =
                        activity.Dates.Where(d => d.Begin <= DateTime.Now && DateTime.Now <= d.End)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();
                    var nextDate =
                        activity.Dates.Where(d => d.Begin >= DateTime.Now)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();

                    if (currentDate != null)
                    {
                        summary.CurrentDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = currentDate },
                            State = ActivityService.GetSubscriptionState(currentDate.Occurrence, currentDate.Begin, currentDate.End),
                        };
                    }

                    if (nextDate != null)
                    {
                        summary.NextDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = nextDate },
                            State = ActivityService.GetSubscriptionState(nextDate.Occurrence, nextDate.Begin, nextDate.End),
                        };
                    }
                }
                model.Add(summary);
            }

            return model;
        }


        private List<ActivitySummary> GetLecturerEvents(Semester semester, ApplicationUser user)
        {
            List<ActivitySummary> model = new List<ActivitySummary>();

            var lectureActivities =
                Db.Activities.OfType<Event>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();

            foreach (var activity in lectureActivities)
            {
                var summary = new ActivitySummary { Activity = activity };

                // nur die, bei denen es noch Termine in der Zukunft gibt
                if ((activity.Dates.Any() && activity.Dates.OrderBy(d => d.End).Last().End >= DateTime.Today))
                {
                    var currentDate =
                        activity.Dates.Where(d => d.Begin <= DateTime.Now && DateTime.Now <= d.End)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();
                    var nextDate =
                        activity.Dates.Where(d => d.Begin >= DateTime.Now)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();

                    if (currentDate != null)
                    {
                        summary.CurrentDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = currentDate },
                            State = ActivityService.GetSubscriptionState(currentDate.Occurrence, currentDate.Begin, currentDate.End),
                        };
                    }

                    if (nextDate != null)
                    {
                        summary.NextDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = nextDate },
                            State = ActivityService.GetSubscriptionState(nextDate.Occurrence, nextDate.Begin, nextDate.End),
                        };
                    }
                }
                model.Add(summary);
            }

            return model;
        }

        private List<ActivitySummary> GetLecturerReservations(Semester semester, ApplicationUser user)
        {
            List<ActivitySummary> model = new List<ActivitySummary>();

            var lectureActivities =
                Db.Activities.OfType<Reservation>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();

            foreach (var activity in lectureActivities)
            {
                var summary = new ActivitySummary { Activity = activity };

                // nur die, bei denen es noch Termine in der Zukunft gibt
                if ((activity.Dates.Any() && activity.Dates.OrderBy(d => d.End).Last().End >= DateTime.Today))
                {
                    var currentDate =
                        activity.Dates.Where(d => d.Begin <= DateTime.Now && DateTime.Now <= d.End)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();
                    var nextDate =
                        activity.Dates.Where(d => d.Begin >= DateTime.Now)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();

                    if (currentDate != null)
                    {
                        summary.CurrentDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = currentDate },
                            State = ActivityService.GetSubscriptionState(currentDate.Occurrence, currentDate.Begin, currentDate.End),
                        };
                    }

                    if (nextDate != null)
                    {
                        summary.NextDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = nextDate },
                            State = ActivityService.GetSubscriptionState(nextDate.Occurrence, nextDate.Begin, nextDate.End),
                        };
                    }
                }
                model.Add(summary);
            }

            return model;
        }


        private List<ActivitySummary> GetLecturerExams(Semester semester, ApplicationUser user)
        {
            List<ActivitySummary> model = new List<ActivitySummary>();

            var lectureActivities =
                //Db.Activities.OfType<Exam>().ToList();
                
                Db.Activities.OfType<Exam>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Owners.Any(d => d.Member.UserId.Equals(user.Id))).ToList();
                

            foreach (var activity in lectureActivities)
            {
                var summary = new ActivitySummary { Activity = activity };

                model.Add(summary);
            }

            return model;
        }



        private DashboardStudentViewModel CreateDashboardModelStudent(UserRight userRight)
        {
            var semSubService = new SemesterSubscriptionService();

            var currentSemester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(DateTime.Today);

            var model = new DashboardStudentViewModel
            {
                User = userRight.User,
                Semester = currentSemester,
                SemesterGroup = semSubService.GetSemesterGroup(userRight.User.Id, currentSemester),
                Student = Db.Students.Where(x => x.UserId.Equals(userRight.User.Id)).OrderByDescending(x => x.Created).FirstOrDefault()
            };

            // keine Semestergruppe gewählt => aktive Pläne suchen
            if (model.SemesterGroup == null)
            {
                model.ActiveOrgsSemester = SemesterService.GetActiveOrganiser(currentSemester, true);
            }


            // das nächste Semester nur anzeigen, wenn es einen veröffentlichsten Stundenplan für die letzte Fakultät gibt!
            var nextSemesterOrgs = SemesterService.GetActiveOrganiser(nextSemester, true);
            if (nextSemesterOrgs.Any())
            {
                model.NextSemester = nextSemester;
                model.NextSemesterGroup = semSubService.GetSemesterGroup(userRight.User.Id, nextSemester);
                if (model.NextSemesterGroup == null)
                {
                    model.ActiveOrgsNextSemester = nextSemesterOrgs;
                }
            }



            // Alle Anfragen zu Abschlussarbeiten
            var supervisions = Db.Activities.OfType<Supervision>()
                .Where(x => x.Occurrence.Subscriptions.Any(y => y.UserId.Equals(userRight.User.Id))).ToList();

            foreach (var supervision in supervisions)
            {
                var request = new SupervisionRequestModel();

                request.Supervision = supervision;
                request.Lecturer = supervision.Owners.First().Member;
                request.Subscription =
                    supervision.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userRight.User.Id));

                model.Requests.Add(request);
            }

            // Alle Abschlussarbeiten
            var theses = Db.Theses.Where(x => x.Student.UserId.Equals(userRight.User.Id)).ToList();

            foreach (var thesis in theses)
            {
                var tModel = new ThesisDetailModel();

                tModel.Thesis = thesis;
                tModel.Lecturer = thesis.Supervision.Owners.First().Member;

                model.Theses.Add(tModel);
            }

            // Alle heutigen Termine
            // Alle Eintragungen
            var begin = DateTime.Now;
            var end = DateTime.Today.AddDays(1);

            var allDates =
                Db.ActivityDates.Where(d =>
                    (d.Activity.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRight.User.Id)) ||
                     d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRight.User.Id)) ||
                     d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRight.User.Id)))) &&
                    d.End >= begin && d.End <= end).OrderBy(d => d.Begin).ToList();

            foreach (var date in allDates)
            {
                var act = new AgendaActivityViewModel
                {
                    Date = date,
                    Slot = date.Slots.FirstOrDefault(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRight.User.Id)))
                };

                model.TodaysActivities.Add(act);
            }

            if (model.Student != null)
            {
                var org = model.Student.Curriculum.Organiser;
                

                // Alle Platzverlosungen
                // letzte 90 Tage
                var lastEnd = DateTime.Today;
                var alLotteries = Db.Lotteries.Where(x =>
                    x.IsActiveUntil >= lastEnd && x.IsAvailable &&
                    x.Organiser != null && x.Organiser.Id == org.Id).OrderBy(x => x.FirstDrawing).ToList();

                foreach (var lottery in alLotteries)
                {
                    var courseList = new List<Course>();
                    courseList.AddRange(
                        lottery.Occurrences.Select(
                            occurrence => Db.Activities.OfType<Course>().SingleOrDefault(
                                c => c.Occurrence.Id == occurrence.Id)).Where(course => course != null));

                    var hasFit = 
                    courseList.Any(c => c.SemesterGroups.Any(g =>
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == model.Student.Curriculum.Id));

                    if (hasFit)
                    {
                        model.Lotteries.Add(lottery);
                    }
                }
            }


            return model;
        }


        private DashboardStudentViewModel CreateDashboardModelStudentNew(UserRight userRight)
        {
            var student = StudentService.GetCurrentStudent(userRight.User.Id);

            var currentSemester = student != null ? SemesterService.GetNewestSemester(student.Curriculum.Organiser) : SemesterService.GetSemester(DateTime.Today);
            var prevSemester = SemesterService.GetPreviousSemester(currentSemester);



            var model = new DashboardStudentViewModel
            {
                User = userRight.User,
                Semester = prevSemester,
                NextSemester = currentSemester,
                Student = student,
                Organiser = student?.Curriculum.Organiser
            };

            if (student != null)
            {
                var limit = DateTime.Today.AddDays(-7);
                model.Advertisements = Db.Advertisements.Where(x => x.Owner.Organiser.Id == student.Curriculum.Organiser.Id &&
                                                       x.Created >= limit).ToList();
            }
            else
            {
                model.Advertisements = new List<Advertisement>();
            }



            return model;
        }


        private DashboardStudentViewModel CreateDashboardModelOrgMemberNew(UserRight userRight)
        {
            var org = GetMyOrganisation();

            var currentSemester = SemesterService.GetNewestSemester(org);
            var prevSemester = SemesterService.GetPreviousSemester(currentSemester);

            var model = new DashboardStudentViewModel
            {
                User = userRight.User,
                Semester = prevSemester,
                NextSemester = currentSemester,
                Organiser = org
            };

            var limit = DateTime.Today.AddDays(-7);
            model.Advertisements = Db.Advertisements.Where(x => x.Owner.Organiser.Id == org.Id &&
                                                                x.Created >= limit).ToList();


            return model;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hmemail"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult SendVerificationMail(string hmemail)
        {
            bool IsHM = hmemail.ToLower().EndsWith("@hm.edu");

            if (IsHM)
            {
                var user = UserManager.FindByEmail(hmemail.ToLower());
                var me = UserManager.FindByName(User.Identity.Name);

                if (user == null)
                {
                    // Adresse noch nicht vergeben => E-Mail versenden
                    string code = UserManager.GenerateEmailConfirmationToken(me.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    // das mache ich wieder selber 
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    var mailModel = new ConfirmEmailMailModel
                    {
                        User = me,
                        Token = code,
                    };

                    new MailController().VerfiyHmEMail(mailModel).Deliver();


                    return PartialView("_SuccessMailSent");
                }
                else
                {
                    if (user.Id.Equals(me.Id))
                    {
                        // unwahrscheinlich, das darf nicht passieren, wenn doch dann E-Mail versenden
                        return PartialView("_Error");
                    }
                    else
                    {
                        // Fehlermeldung "E-Mail Adresse bereits vergeben"
                        return PartialView("_ErrorMailTaken");
                    }
                }

            }
            else
            {
                // keine hm.edu Adresse
                return PartialView("_ErrorInvalidMail");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ActivityDetails(Guid id)
        {
            var act = Db.Activities.SingleOrDefault(a => a.Id == id);

            if (act != null)
            {
                if (act is Course)
                {
                    return RedirectToAction("Details", "Course", new { id = id} );
                }
                else if (act is Event)
                {
                    return RedirectToAction("Details", "Event", new { id = id });
                }
                else if (act is Reservation)
                {
                    return RedirectToAction("Index", "Reservation");
                }
            }

            return new EmptyResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GetNotificationList(string userId)
        {
            List<ActivityDateChange> data = new List<ActivityDateChange>();
            data = Db.DateChanges.Where(a => a.NotificationStates.Any(b => b.UserId.Equals(userId)) 
                && DateTime.Compare(DateTime.Now, a.NewEnd) < 0)
                .OrderByDescending(a => a.NotificationStates.FirstOrDefault(n => n.UserId.Equals(userId)).IsNew).ToList();
            //return PartialView("_NotificationList", data);
            ViewBag.UserId = userId;
            return PartialView("_NotificationList", data);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changeId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult MarkAsRead(string userId, string changeId)
        {
            // NotificationState als gelesen markieren
            var notificationService = new NotificationService();
            notificationService.MarkAsRead(userId, changeId);

            
            // Aktuellen Status der DB abfragen 
            List<ActivityDateChange> data = new List<ActivityDateChange>();
            data = Db.DateChanges.Where(a => a.NotificationStates.Any(b => b.UserId == userId)
                && DateTime.Compare(DateTime.Now, a.NewEnd) < 0)
                .OrderByDescending(a => a.NotificationStates.FirstOrDefault(n => n.UserId.Equals(userId)).IsNew).ToList();

            
            ViewBag.UserId = userId;

            return PartialView("_NotificationList", data);
        }

    }
}