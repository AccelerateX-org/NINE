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
                return View("DashboardSysAdmin");
            }


            switch (userRight.User.MemberState)
            {
                case MemberState.Student:
                    return View("DashboardStudent", CreateDashboardModelStudent(userRight));
                case MemberState.Staff:
                    return View("DashboardOrgMember", CreateDashboardModelOrgMember(userRight));
                default:
                    return View("DashboardDefault", CreateDashboardModelDefault(userRight));
            }
        }


        private DashboardViewModel CreateDashboardModelDefault(UserRight userRight)
        {
            var ohs = new OfficeHourInfoService(UserManager);
            var semester = GetSemester();

            var model = new DashboardViewModel();
            model.Semester = semester;
            model.User = userRight.User;

            // Sprechstunden
            model.OfficeHours =
            Db.Activities.OfType<OfficeHour>().Where(a =>
                a.Semester.Id == semester.Id).ToList();

          
            
            return model;
        }

        private DashboardViewModel CreateDashboardModelOrgMember(UserRight userRight)
        {
            var ohs = new OfficeHourInfoService(UserManager);
            var semester = GetSemester();
            var lastSemester = GetPreviousSemester();
            var org = GetMyOrganisation();
            var member = GetMember(User.Identity.Name, org.ShortName);

            var model = new DashboardViewModel();
            model.IsProf = HasUserRole(User.Identity.Name, org.ShortName, "Prof");
            model.Semester = semester;
            model.User = userRight.User;
            model.Member = member;

            if (member != null)
            {
                var officeHour = new OfficeHourService().GetOfficeHour(member, semester);
                model.OfficeHour = officeHour;
                if (officeHour != null)
                {
                    model.NextOfficeHourDate = ohs.GetPreviewNextDate(officeHour);
                }

                // die Fakultät
                model.Organiser = member.Organiser;

                model.MyModules = Db.CurriculumModules.Where(x => x.MV != null && x.MV.Id == member.Id).ToList();
            }


            // alle Aktivitäten, im aktuellen Semester
            model.MyActivities.AddRange(GetLecturerActivities(semester, userRight.User));
            model.MyPreviousActivities.AddRange(GetLecturerActivities(lastSemester, userRight.User));
            model.PreviousSemester = lastSemester;


            // Was läuft gerade
            var now = DateTime.Now;
            var endOfDay = DateTime.Today.AddDays(1);

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


        private List<ActivitySummary> GetLecturerActivities(Semester semester, ApplicationUser user)
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
                if ((activity.Dates.Any() && activity.Dates.OrderBy(d => d.End).Last().End >= GlobalSettings.Today))
                {
                    var currentDate =
                        activity.Dates.Where(d => d.Begin <= GlobalSettings.Now && GlobalSettings.Now <= d.End)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();
                    var nextDate =
                        activity.Dates.Where(d => d.Begin >= GlobalSettings.Now)
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



        private DashboardStudentViewModel CreateDashboardModelStudent(UserRight userRight)
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = GetSemester();
            var user = AppUser;


            var model = new DashboardStudentViewModel();

            model.Semester = GetSemester();
            model.User = user;
            model.SemesterGroup = semSubService.GetSemesterGroup(model.User.Id, model.Semester);


            // Eintragungen auf Aktivitätsebene => Lehrveranstaltungen
            var activities = Db.Activities.OfType<Course>().Where(a => 
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();
            foreach (var activity in activities)
            {
                model.MyCourseSubscriptions.Add(new ActivitySubscriptionStateModel
                {
                    Activity = new ActivitySummary { Activity = activity },
                });
            }

            model.NextCourseDate =
            Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id)))
                .Select(officeHour => officeHour.Dates.FirstOrDefault(x => x.End > DateTime.Now)).Where(date => date != null)
                .OrderBy(x => x.Begin).FirstOrDefault();



            // Die Veranstaltungen => Annahme, dass das nur Termine sind
            /*
            var dates = Db.ActivityDates.Where(d => d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();
            foreach (var activityDate in dates)
            {
                if (activityDate.End >= GlobalSettings.Today && !(activityDate.Activity is OfficeHour))
                {
                    model.MySubscriptions.Add(new ActivitySubscriptionStateModel
                    {
                        Activity = new ActivityDateSummary { Date = activityDate },
                        State = ActivityService.GetActivityState(activityDate.Occurrence, user, semester)
                    });
                }
            }
             */

            /*
            var slots = Db.ActivitySlots.Where(s => s.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();
            foreach (var activitySlot in slots)
            {
                if (activitySlot.ActivityDate.End >= GlobalSettings.Today)
                {
                    model.MySubscriptions.Add(new ActivitySubscriptionModel
                    {
                        Activity = new ActivitySlotSummary { Slot = activitySlot },
                        State = ac.GetActivityState(activitySlot.Occurrence, UserManager.FindByName(User.Identity.Name))
                    });
                }
            }
             */

            // Der nächste Sprechstundentermin
            var allOfficeHours =
                Db.Activities.OfType<OfficeHour>().Where(a =>
                    a.Semester.Id == semester.Id &&
                    a.Dates.Any(d =>
                        d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)) ||
                        d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)))
                    )).ToList();

            var allDates = new List<ActivityDate>();
            foreach (var officeHour in allOfficeHours)
            {
                var nextDate = officeHour.Dates
                    .Where(d =>
                        d.End > DateTime.Now && (
                            d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)) ||
                            d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)))))
                    .OrderBy(d => d.Begin).FirstOrDefault();

                if (nextDate != null)
                {
                    allDates.Add(nextDate);
                }
            }

            model.NextOfficeHourDate = allDates.OrderBy(d => d.Begin).FirstOrDefault();

            var nextSemesterDate = semester.Dates.Where(x => x.From >= DateTime.Now).OrderBy(x => x.From).FirstOrDefault();
            if (nextSemesterDate == null)
            {
                if (DateTime.Today > semester.StartCourses)
                {
                    model.NextSemesterDate = semester.EndCourses;
                    model.NextSemesterDateDescription = "Vorlesungsende";
                }
                else
                {
                    model.NextSemesterDate = semester.StartCourses;
                    model.NextSemesterDateDescription = "Vorlesungsbeginn";
                }
            }
            else
            {
                if (nextSemesterDate.From > semester.StartCourses)
                {
                    model.NextSemesterDate = nextSemesterDate.From;
                    model.NextSemesterDateDescription = nextSemesterDate.Description;
                }
                else
                {
                    model.NextSemesterDate = semester.StartCourses;
                    model.NextSemesterDateDescription = "Vorlesungsbeginn";
                }
            }




            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GroupList(string currId)
        {
            var curr = Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(currId));

            var semester = GetSemester();

            var selectList = new List<SelectListItem>();

            if (curr != null)
            {

                var groupList = Db.SemesterGroups.Where(g => g.Semester.Id == semester.Id &&
                                             g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id).ToList();


                foreach (var group in groupList.OrderBy(g => g.GroupName))
                {
                    if (group.CapacityGroup.CurriculumGroup.IsSubscribable)
                    {
                        selectList.Add(new SelectListItem
                        {
                            Text = group.FullName,
                            Value = group.Id.ToString()
                        });
                    }
                }
            }

            return PartialView("_GroupList", selectList);
        }


        /*
         * Muss umgezogen werden
        [HttpPost]
        public PartialViewResult UpdateSemesterGroup(DashboardViewModel model)
        {
            var user = AppUser;

            var semSubService = new SemesterSubscriptionService();

            if (user != null)
            {
                var group = Db.SemesterGroups.SingleOrDefault(g => g.Id.ToString().Equals(model.CurrGroup));

                if (group != null)
                {
                    semSubService.Subscribe(user.Id, group.Id);
                }
            }

            model.Semester = GetSemester();
            model.User = user;
            model.SemesterGroups = semSubService.GetSemesterGroups(model.User.Id, model.Semester);

            if (!model.SemesterGroups.Any())
            {
                ViewBag.Faculties =
                    Db.Organisers.Where(x => x.Curricula.Any()).OrderBy(f => f.ShortName).Select(f => new SelectListItem
                    {
                        Text = f.ShortName,
                        Value = f.Id.ToString(),
                    });

                ViewBag.Curricula = new SelectList(new object[] {});
                ViewBag.Groups = new SelectList(new object[] {});
            }

            return PartialView("_PortletSemGroup", model);
        }


        [HttpPost]
        public PartialViewResult RemoveSemesterGroup(DashboardViewModel model)
        {
            var user = AppUser;
            var semester = GetSemester();

            if (user != null)
            {
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

            var semSubService = new SemesterSubscriptionService();

            model.Semester = GetSemester();
            model.User = AppUser;
            model.SemesterGroups = semSubService.GetSemesterGroups(model.User.Id, model.Semester);

            var currList = Db.Curricula.Where(x => !x.ShortName.Equals("Export")).ToList();

            ViewBag.Curricula = currList.Select(c => new SelectListItem
            {
                Text = c.ShortName + " (" + c.Name + ")",
                Value = c.ShortName,
            });

            var curr = currList.First();


            if (curr != null)
            {
                var selectList = new List<SelectListItem>();

                var groupList = Db.SemesterGroups.Where(g => g.Semester.Id == semester.Id &&
                                             g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id).ToList();


                foreach (var group in groupList.OrderBy(g => g.GroupName))
                {
                    if (group.CapacityGroup.CurriculumGroup.IsSubscribable)
                    {
                        selectList.Add(new SelectListItem
                        {
                            Text = group.FullName,
                            Value = group.Id.ToString()
                        });
                    }
                }

                ViewBag.Groups = selectList;
            }

            return PartialView("_PortletSemGroup", model);
        }
        */

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
        /// <param name="culture"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
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