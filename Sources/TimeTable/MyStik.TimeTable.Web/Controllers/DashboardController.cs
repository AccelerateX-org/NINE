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

namespace MyStik.TimeTable.Web.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            var userRight = GetUserRight();
            ViewBag.UserRight = userRight;

            if (User.IsInRole("SysAdmin"))
                return View("DashboardOrgAdmin", CreateDashboardModelOrgAdmin(userRight));


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
            var officeHours =
            Db.Activities.OfType<OfficeHour>().Where(a =>
                a.Semester.Id == semester.Id &&
                a.Dates.Any(d =>
                    d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)) ||
                    d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)))
                )).ToList();

            model.OfficeHours = new List<OfficeHourDateViewModel>();

            foreach (var officeHour in officeHours)
            {
                model.OfficeHours.Add(ohs.GetNextSubscription(officeHour, model.User.Id));
            }
          
            
            return model;
        }

        private DashboardViewModel CreateDashboardModelOrgMember(UserRight userRight)
        {
            var ohs = new OfficeHourInfoService(UserManager);
            var semester = GetSemester();
            var member = GetMember(User.Identity.Name, "FK 09");

            var model = new DashboardViewModel();
            model.IsProf = HasUserRole(User.Identity.Name, "FK 09", "Prof");
            model.Semester = semester;
            model.User = userRight.User;
            model.Member = member;

            if (member != null)
            {
                var officeHour = new OfficeHourService().GetOfficeHour(member.ShortName, semester.Name);
                model.OfficeHour = officeHour;
                if (officeHour != null)
                {
                    model.NextOfficeHourDate = ohs.GetPreviewNextDate(officeHour);
                }

                // die Fakultät
                model.Organiser = member.Organiser;
            }


            // alle Aktivitäten, im aktuellen Semester
            var lectureActivities =
                Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(userRight.User.Id)))).ToList();

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
                model.MyActivities.Add(summary);
            }


            // Was läuft gerade
            model.CurrentDates =
                Db.ActivityDates.Where(d => d.Begin <= GlobalSettings.Now && GlobalSettings.Now <= d.End && d.Occurrence.IsCanceled == false ).OrderBy(d => d.Begin).ToList();

            /*
            model.ActiveMembers = Db.Members.Where(m => m.Dates.Any(d => 
                d.Begin >= GlobalSettings.Today &&
                d.End <= GlobalSettings.Tomorrow)
                ).ToList();
            */

            GetAvailableRooms(model);

            return model;
        }

        private DashboardViewModel CreateDashboardModelOrgAdmin(UserRight userRight)
        {
            var model = new DashboardViewModel();

            model.Semester = GetSemester();

            // Was läuft gerade
            model.CurrentDates =
                Db.ActivityDates.Where(d => d.Begin <= GlobalSettings.Now && GlobalSettings.Now <= d.End && d.Occurrence.IsCanceled == false).OrderBy(d => d.Begin).ToList();

            GetAvailableRooms(model);


            model.CanceledDates =
                Db.ActivityDates.Where(d => 
                    (d.Begin <= GlobalSettings.Now && GlobalSettings.Now <= d.End) && d.Occurrence.IsCanceled
                    ).ToList();
            
            return model;
        }


        private DashboardViewModel CreateDashboardModelStudent(UserRight userRight)
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = GetSemester();
            var user = AppUser;


            var model = new DashboardViewModel();

            model.Semester = GetSemester();
            model.User = user;
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


            // Alle Termine als Teilnehmer in diesem Semester

            // Eintragungen auf Aktivitätsebene => Lehrveranstaltungen
            var activities = Db.Activities.OfType<Course>().Where(a => 
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();
            foreach (var activity in activities)
            {
                // Die Platzverlosungen, bei denen ich dabei bin
                var lottery = Db.Lotteries.SingleOrDefault(l => l.Occurrences.Any(o => o.Id == activity.Occurrence.Id));

                model.MyCourseSubscriptions.Add(new ActivitySubscriptionStateModel
                {
                    Activity = new ActivitySummary { Activity = activity },
                    State = ActivityService.GetActivityState(activity.Occurrence, user, semester),
                    Lottery = lottery
                });
            }


            // Die Veranstaltungen => Annahme, dass das nur Termine sind
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

            // Sprechstunden
            var officeHours = 
            Db.Activities.OfType<OfficeHour>().Where(a => 
                a.Semester.Id == semester.Id &&
                a.Dates.Any(d =>
                    d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)) ||
                    d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(model.User.Id)))
                )).ToList();

            model.OfficeHours = new List<OfficeHourDateViewModel>();

            var ohs = new OfficeHourInfoService(UserManager);

            foreach (var officeHour in officeHours)
            {
                model.OfficeHours.Add(ohs.GetNextSubscription(officeHour, model.User.Id));
            }


            GetAvailableRooms(model);

            return model;
        }


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


        internal void GetAvailableRooms(DashboardViewModel model)
        {
            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var fk09 = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));
            if (fk09 != null)
            {
                var allRooms = roomService.GetAvaliableRoomsNow(fk09.Id);
                // nur R-Bau
                model.AvailableRooms = allRooms.Where(r => r.Room.Number.StartsWith("R")).ToList();
            }
            else
            {
                model.AvailableRooms = new List<RoomInfoModel>();
            }
        }
    }
}