using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Contracts;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class OfficeHourController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SysAdmin")]
        public ActionResult DeleteAll()
        {
            var officeHours = Db.Activities.OfType<OfficeHour>().ToList();
            foreach (var officeHour in officeHours.ToList())
            {
                foreach (var date in officeHour.Dates.ToList())
                {
                    foreach (var slot in date.Slots.ToList())
                    {
                        foreach (var sub in slot.Occurrence.Subscriptions.ToList())
                        {
                            slot.Occurrence.Subscriptions.Remove(sub);
                            Db.Subscriptions.Remove(sub);
                        }
                        Db.Occurrences.Remove(slot.Occurrence);
                        date.Slots.Remove(slot);
                        Db.ActivitySlots.Remove(slot);
                    }

                    foreach (var sub in date.Occurrence.Subscriptions.ToList())
                    {
                        date.Occurrence.Subscriptions.Remove(sub);
                        Db.Subscriptions.Remove(sub);
                    }

                    Db.Occurrences.Remove(date.Occurrence);
                    officeHour.Dates.Remove(date);
                    Db.ActivityDates.Remove(date);
                }

                foreach (var sub in officeHour.Occurrence.Subscriptions.ToList())
                {
                    officeHour.Occurrence.Subscriptions.Remove(sub);
                    Db.Subscriptions.Remove(sub);
                }

                Db.Occurrences.Remove(officeHour.Occurrence);
                Db.Activities.Remove(officeHour);
            }

            Db.SaveChanges();

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Delete(Guid id)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == id);

            if (officeHour != null)
            {
                foreach (var date in officeHour.Dates.ToList())
                {
                    foreach (var slot in date.Slots.ToList())
                    {
                        foreach (var sub in slot.Occurrence.Subscriptions.ToList())
                        {
                            slot.Occurrence.Subscriptions.Remove(sub);
                            Db.Subscriptions.Remove(sub);
                        }
                        Db.Occurrences.Remove(slot.Occurrence);
                        date.Slots.Remove(slot);
                        Db.ActivitySlots.Remove(slot);
                    }

                    foreach (var sub in date.Occurrence.Subscriptions.ToList())
                    {
                        date.Occurrence.Subscriptions.Remove(sub);
                        Db.Subscriptions.Remove(sub);
                    }

                    Db.Occurrences.Remove(date.Occurrence);
                    officeHour.Dates.Remove(date);
                    Db.ActivityDates.Remove(date);
                }

                foreach (var sub in officeHour.Occurrence.Subscriptions.ToList())
                {
                    officeHour.Occurrence.Subscriptions.Remove(sub);
                    Db.Subscriptions.Remove(sub);
                }

                Db.Occurrences.Remove(officeHour.Occurrence);
                Db.Activities.Remove(officeHour);
            }

            Db.SaveChanges();

            return RedirectToAction("Index", "Lecturer");
        }


        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var allMySemester = Db.Semesters.Where(x =>
                x.EndCourses >= DateTime.Today && x.Groups.Any(g =>
                   g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id)).OrderByDescending(x => x.EndCourses).ToList();


            return View(allMySemester);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Semester(Guid? id)
        {
            // Liste aller Sprechstunden
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var officeHours = Db.Activities.OfType<OfficeHour>().Where(oh =>
                    oh.Semester.Id == semester.Id &&
                    oh.Owners.Any(m => m.Member.Organiser.Id == org.Id))
                .ToList();

            var model = new OfficeHourOverviewModel
            {
                Organiser = org,
                Semester = semester,
            };


            var member = GetMyMembership();
            if (member != null)
            {
                model.MyOfficeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(x =>
                    x.Semester.Id == semester.Id && x.Owners.Any(y => y.Member.Id == member.Id));
            }



            foreach (var officeHour in officeHours)
            {
                var lecturer = officeHour.Dates.Any()
                    ? officeHour.Dates.First().Hosts.FirstOrDefault()
                    : officeHour.Owners.First().Member;

                var m = new OfficeHourDateViewModel();

                m.OfficeHour = officeHour;
                m.Lecturer = lecturer;

                m.Date = officeHour.Dates.FirstOrDefault(x => x.End > DateTime.Now);

                model.OfficeHours.Add(m);
            }

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Subscriptions(Guid? id)
        {
            var user = GetCurrentUser();

            if (!id.HasValue)
            {
                var allMySemesterWithOfficeHours = 
                    Db.Activities.OfType<OfficeHour>().Where(x =>
                        x.Semester != null &&
                        x.Dates.Any(d => d.Occurrence.Subscriptions.Any(s =>s.UserId.Equals(user.Id)) ||
                                         d.Slots.Any(s =>s.Occurrence.Subscriptions.Any(g =>g.UserId.Equals(user.Id))))).GroupBy(x =>
                    x.Semester).Select(x => x.Key).OrderByDescending(x => x.EndCourses).ToList();

                return View("SemesterList", allMySemesterWithOfficeHours);
            }

            // Alle in diesem Semester
            var semester = SemesterService.GetSemester(id);

            var allMyOfficeHours =
                Db.Activities.OfType<OfficeHour>().Where(x =>
                    x.Semester != null && x.Semester.Id == id.Value &&
                    x.Dates.Any(d => d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                                     d.Slots.Any(s => s.Occurrence.Subscriptions.Any(g => g.UserId.Equals(user.Id))))).ToList();

            var model = new OfficeHourOverviewModel();
            model.Semester = semester;

            foreach (var officeHour in allMyOfficeHours)
            {
                // alle dates
                var dates = officeHour.Dates.Where(d => d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                    .ToList();

                foreach (var date in dates)
                {
                    var ohDate = new OfficeHourDateViewModel();
                    ohDate.OfficeHour = officeHour;
                    ohDate.Date = date;
                    ohDate.Lecturer = officeHour.Owners.First().Member;
                    ohDate.Subscription = date.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                    model.OfficeHours.Add(ohDate);
                }

                // alle slots
                dates = officeHour.Dates.Where(d => d.Slots.Any(s => s.Occurrence.Subscriptions.Any(g => g.UserId.Equals(user.Id))))
                    .ToList();

                foreach (var date in dates)
                {
                    var slots = date.Slots.Where(d => d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                        .ToList();

                    foreach (var slot in slots)
                    {
                        var ohDate = new OfficeHourDateViewModel();
                        ohDate.OfficeHour = officeHour;
                        ohDate.Date = date;
                        ohDate.Slot = slot;
                        ohDate.Lecturer = officeHour.Owners.First().Member;
                        ohDate.Subscription = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                        model.OfficeHours.Add(ohDate);
                    }
                }

            }



            return View("SemesterDates", model);
        }

        /// <summary>
        /// Terminliste einer bestimmten Sprechstunde
        /// </summary>
        /// <param name="id"></param>
        /// <param name="semId"></param>
        /// <returns></returns>
        public ActionResult History(Guid id, Guid semId)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == id);

            if (officeHour == null)
            {
                return RedirectToAction("Index", "Lecturer");
            }

            // leider stehat das Semester nicht drin => Parameter
            //var semester = officeHour.Semester;
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == semId);

            var hostRequested = Db.Members.FirstOrDefault(l => l.Dates.Any(d => d.Activity.Id == officeHour.Id));

            var userRequesting = UserManager.FindByName(User.Identity.Name);

            var model = new OfficeHourCharacteristicModel
            {
                OfficeHour = officeHour,
                Semester = semester,
                Host = hostRequested,
            };

            FillOfficeHourDateList(model, hostRequested, userRequesting);


            ViewBag.UserRight = GetUserRight(User.Identity.Name, officeHour);


            if (ViewBag.UserRight.IsHost)
            {
                return View("DateListHost", model);
            }

            return View("DateListPublic", model);

        }


        /// <summary>
        /// Sprechstundentermine des Dozenten im aktuellen Semester
        /// </summary>
        /// <param name="id">memberId des Dozenten</param>
        /// <param name="semId"></param>
        /// <returns></returns>
        public ActionResult Lecturer(Guid id, Guid? semId)
        {
            var semester = SemesterService.GetSemester(semId);
            var hostRequested = Db.Members.FirstOrDefault(l => l.Id == id);
            var user = GetCurrentUser();

            if (hostRequested == null)
            {
                return RedirectToAction("Index");
            }

            var ohService = new OfficeHourService(Db);
            var officeHour = ohService.GetOfficeHour(hostRequested, semester);
            if (officeHour == null)
            {
                // TODO: keine Sprechstunde
                return RedirectToAction("Index", "Lecturer");
            }

            var model2 = new OfficeHourSubscriptionViewModel
            {
                OfficeHour = officeHour,
                Semester = semester,
                Host = hostRequested,
            };

            ViewBag.UserRight = GetUserRight(User.Identity.Name, officeHour);

            if (officeHour.ByAgreement)
            {
                return ByAgreement(model2);
            }
            else
            {
                // Generische Anzeige nach Datum
                var infoService = new OfficeHourInfoService(UserManager);
                model2.Dates.AddRange(infoService.GetDates(officeHour, user.Id));

                // Berechnung aller zukünftigen Einschreibungen
                model2.FutureSubCount = model2.Dates.Count(x => x.Date.End > DateTime.Now && x.Subscription != null);


                return View("DateListPublic", model2);
            }
        }

        private ActionResult ByAgreement(OfficeHourSubscriptionViewModel model)
        {
            return View("DateListByAgreement", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubscribeDate(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);
            var user = GetCurrentUser();
            var officeHour = date.Activity as OfficeHour;

            var infoService = new OfficeHourInfoService(UserManager);

            var model = new OfficeHourDateSubscriptionViewModel();

            model.Date = date;
            model.Host = infoService.GetHost(officeHour);

            if (date.Slots.Any())
            {
                model.AvailableSlots = date.Slots.Where(x => !x.Occurrence.Subscriptions.Any()).ToList();
            }

            // Konsistenzprüfungen
            var hasSubscription = infoService.HasSubscription(date, user.Id);
            if (hasSubscription)
            {
                model.Subscription = infoService.GetSubscription(date, user.Id);
                model.Slot = infoService.GetSubscribedSlot(date, user.Id);
                model.Semester = officeHour.Semester;
                return View("HasSubscription", model);
            }


            

            return View(model);
        }

        [HttpPost]
        public ActionResult SubscribeDate(OfficeHourDateSubscriptionViewModel model)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == model.Date.Id);
            var officeHour = date.Activity as OfficeHour;
            var user = GetCurrentUser();
            var infoService = new OfficeHourInfoService(UserManager);

            var host = infoService.GetHost(officeHour);

            // Konsistenzprüfungen
            var hasSubscription = infoService.HasSubscription(date, user.Id);
            if (hasSubscription)
            {
                model.Date = date;
                model.Host = infoService.GetHost(officeHour);
                model.Subscription = infoService.GetSubscription(date, user.Id);
                model.Slot = infoService.GetSubscribedSlot(date, user.Id);
                model.Semester = officeHour.Semester;
                return View("HasSubscription", model);
            }

            // Subscription anlegen
            // die doppelte Eintragung wurde oben bereits geprüft
            var subscription = new OccurrenceSubscription
            {
                UserId = user.Id,
                TimeStamp = DateTime.Now,
                OnWaitingList = false,
                IsConfirmed = true,
                SubscriberRemark = model.Description
            };

            // jetzt noch date oder slot
            if (date.Slots.Any())
            {
                var slot = date.Slots.SingleOrDefault(x => x.Id == model.SlotID);
                if (slot != null)
                {
                    slot.Occurrence.Subscriptions.Add(subscription);
                    Db.SaveChanges();
                }
                else
                {
                    
                }
            }
            else
            {
                date.Occurrence.Subscriptions.Add(subscription);
                Db.SaveChanges();
            }

            return RedirectToAction("Lecturer", new {id=host.Id, semId=officeHour.Semester.Id});
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubscribeSlot(Guid slotId)
        {
            var slot = Db.ActivitySlots.SingleOrDefault(x => x.Id == slotId);
            var user = GetCurrentUser();

            var msg = "";
            if (!slot.Occurrence.Subscriptions.Any())
            {
                var mySub = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                if (mySub == null)
                {
                    var subscription = new OccurrenceSubscription
                    {
                        UserId = user.Id,
                        TimeStamp = DateTime.Now,
                        OnWaitingList = false,
                        IsConfirmed = true
                    };

                    msg = "Eintragung angelegt";

                    slot.Occurrence.Subscriptions.Add(subscription);
                    Db.SaveChanges();
                }
                else
                {
                    msg = "Bereits Eintragung vorhanden";
                }
            }
            else
            {
                msg = "Slot schon besetzt";
            }

            var officeHour = slot.ActivityDate.Activity as OfficeHour;
            var owner = officeHour.Owners.First();

            var logger = LogManager.GetLogger("SubscribeActivity");
            logger.InfoFormat("{0} ({1}) by [{2}]: {3}",
                officeHour.Name, owner.Member.ShortName, User.Identity.Name, msg);

            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// Das ist das was der Studierende macht
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public ActionResult Unsubscribe(Guid slotId)
        {
            var slot = Db.ActivitySlots.SingleOrDefault(x => x.Id == slotId);
            var user = GetCurrentUser();

            var msg = "";

            var mySub = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
            if (mySub != null)
            {
                slot.Occurrence.Subscriptions.Remove(mySub);
                Db.Subscriptions.Remove(mySub);
                Db.SaveChanges();

                msg = "Eintrag gelöscht";
            }
            else
            {
                msg = "Kein Eintrag mehr vorhanden";
            }

            var officeHour = slot.ActivityDate.Activity as OfficeHour;
            var owner = officeHour.Owners.First();

            var logger = LogManager.GetLogger("DischargeActivity");
            logger.InfoFormat("{0} ({1}) by [{2}]: {3}",
                officeHour.Name, owner.Member.ShortName, User.Identity.Name, msg);


            return RedirectToAction("Index", "Dashboard");
        }



 
        private void FillOfficeHourDateList(OfficeHourCharacteristicModel model, OrganiserMember hostRequested,
            ApplicationUser userRequesting)
        {
            var logger = LogManager.GetLogger("OfficeHour");

            var myUser = AppUser;

            if (model.OfficeHour != null)
            {

                var now = DateTime.Now;

                ICollection<ActivityDate> allDates = model.OfficeHour.Dates.OrderBy(d => d.Begin).ToList();


                // Alle Termine
                foreach (var date in allDates)
                {
                    try
                    {
                        var isHistory = date.End < now;

                        if (date.Slots.Any())
                        {
                            #region Slots

                            OfficeHourSlotViewModel firstSlotOnDate = null;
                            var i = 1;

                            // Slots
                            foreach (var ohSlot in date.Slots)
                            {
                                if (ohSlot.Occurrence != null)
                                {
                                    if (ohSlot.Occurrence.Subscriptions.Any())
                                    {
                                        #region Slot mit Eintragungen

                                        OfficeHourSlotViewModel firstSlotOnSlot = null;
                                        var j = 1;
                                        var nSub = 0;

                                        foreach (
                                            var subscription in
                                            ohSlot.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                                        {
                                            var user = UserManager.FindById(subscription.UserId);

                                            OfficeHourSlotViewModel slot;

                                            if (user != null)
                                            {

                                                // pro Subscription eine Zeile
                                                var state = ActivityService.GetActivityState(ohSlot.Occurrence, myUser);

                                                // wenn es nicht der user selbst ist, dann ist der Slot besetzt
                                                if (!User.Identity.Name.Equals(user.UserName))
                                                {
                                                    state.HasError = true;
                                                    state.ErrorMessage = "belegt";
                                                    state.CapacityLeft = 0;
                                                    state.Subscription = null;
                                                }

                                                slot = new OfficeHourSlotViewModel
                                                {
                                                    Date = date.Begin.Date,
                                                    From = ohSlot.Begin.TimeOfDay,
                                                    Until = ohSlot.End.TimeOfDay,
                                                    DateOccurrenceId = date.Occurrence.Id,
                                                    Member = new CourseMemberModel()
                                                    {
                                                        Subscription = subscription,
                                                        User = user,
                                                    },
                                                    RowCount = 1,
                                                    RowNo = i,
                                                    SubscriptionCount = 1,
                                                    SubscriptionNo = j,
                                                    State = state,
                                                    Occurrence = ohSlot.Occurrence,
                                                    ActivityDate = ohSlot.ActivityDate,
                                                    IsHistory = isHistory,
                                                };
                                            }
                                            else
                                            {
                                                slot = new OfficeHourSlotViewModel
                                                {
                                                    Date = date.Begin.Date,
                                                    From = ohSlot.Begin.TimeOfDay,
                                                    Until = ohSlot.End.TimeOfDay,
                                                    DateOccurrenceId = date.Occurrence.Id,
                                                    Member = null,
                                                    RowCount = 1,
                                                    RowNo = i,
                                                    SubscriptionCount = 1,
                                                    SubscriptionNo = 1,
                                                    State =
                                                        ActivityService.GetActivityState(ohSlot.Occurrence, myUser),
                                                    Occurrence = ohSlot.Occurrence,
                                                    ActivityDate = ohSlot.ActivityDate,
                                                    IsHistory = isHistory,
                                                };
                                            }

                                            if (firstSlotOnDate == null)
                                            {
                                                firstSlotOnDate = slot;
                                            }

                                            if (firstSlotOnSlot == null)
                                            {
                                                firstSlotOnSlot = slot;
                                            }

                                            model.CurrentSlots.Add(slot);
                                            i++;
                                            j++;
                                            nSub++;
                                        }
                                        if (firstSlotOnSlot != null)
                                        {
                                            firstSlotOnSlot.SubscriptionCount = j - 1;
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Slot ohne Eintragungen

                                        // nur aufnehmen, wenn generell aktiv und der Owner die Sicht anfordert

                                        if (ohSlot.Occurrence.IsAvailable || (userRequesting != null &&
                                                                              !string.IsNullOrEmpty(
                                                                                  hostRequested.UserId) &&
                                                                              hostRequested.UserId.Equals(
                                                                                  userRequesting.Id)))
                                        {
                                            var slot = new OfficeHourSlotViewModel
                                            {
                                                Date = date.Begin.Date,
                                                From = ohSlot.Begin.TimeOfDay,
                                                Until = ohSlot.End.TimeOfDay,
                                                DateOccurrenceId = date.Occurrence.Id,
                                                Member = null,
                                                RowCount = 1,
                                                RowNo = i,
                                                SubscriptionCount = 1,
                                                SubscriptionNo = 1,
                                                State =
                                                    ActivityService.GetActivityState(ohSlot.Occurrence, myUser),
                                                Occurrence = ohSlot.Occurrence,
                                                ActivityDate = ohSlot.ActivityDate,
                                                IsHistory = isHistory,
                                            };

                                            if (firstSlotOnDate == null)
                                            {
                                                firstSlotOnDate = slot;
                                            }

                                            model.CurrentSlots.Add(slot);
                                            i++;
                                        }

                                        #endregion
                                    }
                                }
                            }

                            if (firstSlotOnDate != null)
                            {
                                firstSlotOnDate.RowCount = i - 1;
                            }

                            #endregion
                        }
                        else
                        {
                            #region Keine Slots

                            // keine Slots
                            if (date.Occurrence.Subscriptions.Any())
                            {
                                OfficeHourSlotViewModel firstSlot = null;
                                var i = 1;
                                var nSub = 0;

                                foreach (var subscription in date.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                                {
                                    var user = UserManager.FindById(subscription.UserId);

                                    var slot = new OfficeHourSlotViewModel
                                    {
                                        Date = date.Begin.Date,
                                        From = date.Begin.TimeOfDay,
                                        Until = date.End.TimeOfDay,
                                        DateOccurrenceId = date.Occurrence.Id,
                                        Member = new CourseMemberModel()
                                        {
                                            Subscription = subscription,
                                            User = user,
                                        },
                                        RowCount = 1,
                                        RowNo = i,
                                        SubscriptionCount = 1,
                                        SubscriptionNo = i,
                                        State = ActivityService.GetActivityState(date.Occurrence, myUser),
                                        // ist der aktuelle User eingetragen?
                                        Occurrence = date.Occurrence,
                                        ActivityDate = date,
                                        IsHistory = isHistory,
                                    };

                                    if (firstSlot == null)
                                    {
                                        firstSlot = slot;
                                    }

                                    model.CurrentSlots.Add(slot);
                                    i++;
                                    nSub++;
                                }

                                firstSlot.RowCount = i - 1;
                                firstSlot.SubscriptionCount = nSub;
                            }
                            else
                            {
                                var slot = new OfficeHourSlotViewModel
                                {
                                    Date = date.Begin.Date,
                                    From = date.Begin.TimeOfDay,
                                    Until = date.End.TimeOfDay,
                                    DateOccurrenceId = date.Occurrence.Id,
                                    Member = null,
                                    RowCount = 1,
                                    RowNo = 1,
                                    SubscriptionCount = 1,
                                    SubscriptionNo = 1,
                                    State = ActivityService.GetActivityState(date.Occurrence, myUser),
                                    Occurrence = date.Occurrence,
                                    ActivityDate = date,
                                    IsHistory = isHistory,
                                };

                                model.CurrentSlots.Add(slot);
                            }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        var sb = new StringBuilder();
                        if (date.Occurrence == null)
                        {
                            sb.Append("Occ des Datums fehlt");
                        }
                        else
                        {
                            sb.Append(ex.Message);
                        }

                        logger.ErrorFormat("Fehler bei Termin {0} für {1}: {2}", date.Begin, model.OfficeHour.ShortName,
                            sb.ToString());
                        logger.Error(ex.StackTrace);
                    }
                }
            }



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id der zugehörigen Lehrveranstaltung</param>
        /// <returns></returns>
        public ActionResult CreateDate(Guid id)
        {
            var memberService = new MemberService(Db, UserManager);

            var model = new OfficeHourCreateModel
            {
                OfficeHourId = id,
                NewDate = DateTime.Today.ToShortDateString(),
                NewDateEnd = DateTime.Today.ToShortDateString(),
                StartTime = "16:00",
                EndTime = "17:00",
                Capacity = 5,
                SlotDuration = 0,
                SpareSlots = 0,
                SubscriptionLimit = 0,
                DateOption = 0,
                Text = "Terminvereinbarung per E-Mail",
                Type = -1,
                IsWeekly = false,
                UseSlots = false
            };

            SetTimeSelections();
            SetRestrictionSelections();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateDate(OfficeHourCreateModel model)
        {
            var oh = Db.Activities.OfType<OfficeHour>().SingleOrDefault(a => a.Id == model.OfficeHourId);
            var semester = oh.Semester;

            // Model in request umsetzen
            var day = DateTime.Parse(model.NewDate);
            var from = DateTime.Parse(model.StartTime);
            var to = DateTime.Parse(model.EndTime);

            var dayEnd = DateTime.Parse(model.NewDateEnd);

            var start = day.Add(from.TimeOfDay);
            var end = day.Add(to.TimeOfDay);

            var infoService = new OfficeHourInfoService(UserManager);
            var host = infoService.GetHost(oh);

            

            var date = new ActivityDate
            {
                Begin = start,
                End = end,
                Activity = oh,
                Hosts = new HashSet<OrganiserMember>
                {
                    host,
                },
                Occurrence = new Occurrence
                {
                    IsAvailable = true,
                    Capacity = model.Capacity,
                    FromIsRestricted = false,
                    UntilIsRestricted = (model.SubscriptionLimit > 0),
                    UntilTimeSpan = (model.SubscriptionLimit > 0)
                        ? new TimeSpan(model.SubscriptionLimit - 1, 59, 0)
                        : new TimeSpan?(),

                    IsCanceled = false,
                    IsMoved = false,
                }

            };

            if (model.UseSlots)
            {
                var ohDuration = end - start;
                var totalMinutes = ohDuration.TotalMinutes;
                var numSlots = (int)(ohDuration.TotalMinutes / model.SlotDuration + 0.01);
                var mySlotDuration = totalMinutes / (double) numSlots;


                for (int i = 1; i <= numSlots; i++)
                {
                    var slotStart = start.AddMinutes((i - 1) * mySlotDuration);
                    var slotEnd = start.AddMinutes(i * mySlotDuration);

                    // i-ter Slot
                    var available = true;
                    if (model.SpareSlots < 0) // Anzahl vom Ende her
                    {
                        available = (i <= numSlots + model.SpareSlots);
                    }
                    else if (model.SpareSlots > 0) // Anzahl vom Anfang
                    {
                        available = (i > model.SpareSlots);
                    }


                    var slot = new ActivitySlot
                    {
                        Begin = slotStart,
                        End = slotEnd,
                        Occurrence = new Occurrence
                        {
                            IsAvailable = available,
                            Capacity = 1,
                            FromIsRestricted = false, // Zeitrestriktionen nur auf dem Activity Date
                            UntilIsRestricted = false,
                            IsCanceled = false,
                            IsMoved = false,
                        }
                    };

                    date.Slots.Add(slot);
                }
            }

            oh.Dates.Add(date);


            if (model.IsWeekly)
            {
                if (semester != null)
                {
                    day = day.AddDays(7);
                    while (day <= dayEnd)
                    {
                        bool isVorlesung = true;
                        foreach (var sd in semester.Dates)
                        {
                            // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                            if (sd.From.Date <= day.Date &&
                                day.Date <= sd.To.Date &&
                                sd.HasCourses == false)
                            {
                                isVorlesung = false;
                            }
                        }

                        if (isVorlesung)
                        {

                            start = day.Add(from.TimeOfDay);
                            end = day.Add(to.TimeOfDay);


                            date = new ActivityDate
                            {
                                Begin = start,
                                End = end,
                                Activity = oh,
                                Hosts = new HashSet<OrganiserMember>
                                {
                                    host,
                                },
                                Occurrence = new Occurrence
                                {
                                    IsAvailable = true,
                                    Capacity = model.Capacity,
                                    FromIsRestricted = false,
                                    UntilIsRestricted = (model.SubscriptionLimit > 0),
                                    UntilTimeSpan = (model.SubscriptionLimit > 0)
                                        ? new TimeSpan(model.SubscriptionLimit - 1, 59, 0)
                                        : new TimeSpan?(),
                                    IsCanceled = false,
                                    IsMoved = false,
                                }

                            };

                            if (model.UseSlots)
                            {
                                var ohDuration = end - start;
                                var totalMinutes = ohDuration.TotalMinutes;
                                var numSlots = (int)(ohDuration.TotalMinutes / model.SlotDuration + 0.01);
                                var mySlotDuration = totalMinutes / (double)numSlots;


                                for (int i = 1; i <= numSlots; i++)
                                {
                                    var slotStart = start.AddMinutes((i - 1) * mySlotDuration);
                                    var slotEnd = start.AddMinutes(i * mySlotDuration);

                                    // i-ter Slot
                                    var available = true;
                                    if (model.SpareSlots < 0) // Anzahl vom Ende her
                                    {
                                        available = (i <= numSlots + model.SpareSlots);
                                    }
                                    else if (model.SpareSlots > 0) // Anzahl vom Anfang
                                    {
                                        available = (i > model.SpareSlots);
                                    }


                                    var slot = new ActivitySlot
                                    {
                                        Begin = slotStart,
                                        End = slotEnd,
                                        Occurrence = new Occurrence
                                        {
                                            IsAvailable = available,
                                            Capacity = 1,
                                            FromIsRestricted = false, // Zeitrestriktionen nur auf dem Activity Date
                                            UntilIsRestricted = false,
                                            IsCanceled = false,
                                            IsMoved = false,
                                        }
                                    };

                                    date.Slots.Add(slot);
                                }
                            }

                            oh.Dates.Add(date);


                        }
                        day = day.AddDays(7);
                    }
                }
            }

            Db.SaveChanges();

            return RedirectToAction("OfficeHour", "Lecturer", new { id=semester.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteDate(Guid id)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == id);

            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == summary.Date.Id);
            var oh = date.Activity as OfficeHour;

            var actSummary = new ActivitySummary {Activity = date.Activity};

            // alle slots löschen
            foreach (var slot in date.Slots.ToList())
            {
                foreach (var sub in slot.Occurrence.Subscriptions.ToList())
                {
                    slot.Occurrence.Subscriptions.Remove(sub);
                    Db.Subscriptions.Remove(sub);
                }
                Db.Occurrences.Remove(slot.Occurrence);
                date.Slots.Remove(slot);
                Db.ActivitySlots.Remove(slot);
            }

            occ.Subscriptions.ForEach(s => Db.Subscriptions.Remove(s));
            Db.Occurrences.Remove(occ);
            date.Occurrence = null;


            date.Activity.Dates.Remove(date);
            date.Hosts.Clear();
            date.Rooms.Clear();
            Db.ActivityDates.Remove(date);

            Db.SaveChanges();

            

            return RedirectToAction("OfficeHour", "Lecturer", new {id = oh.Semester.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CancelDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();

            string sText;
            var email = mailing.CancelOccurrence(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelDate(OccurrenceMailingModel model)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = true;
                Db.SaveChanges();
            }

            var m = new MailingController {ControllerContext = ControllerContext};
            return m.CustomOccurrenceMail(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReactivateDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();

            string sText;
            var email = mailing.ReactivateOccurrence(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReactivateDate(OccurrenceMailingModel model)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = false;
                Db.SaveChanges();
            }

            var m = new MailingController {ControllerContext = ControllerContext};
            return m.CustomOccurrenceMail(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MoveDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var model = new OfficeHourMoveDateModel
            {
                ActivityId = summary.Activity.Id,
                ActivityDateId = summary.Date.Id,
                NewDate = summary.Date.Begin.ToShortDateString(),
                NewBegin = summary.Date.Begin.TimeOfDay.ToString(),
                NewEnd = summary.Date.End.TimeOfDay.ToString(),
                AdjustSlotCount = true,
                OfficeHour = summary.Activity as OfficeHour,
                Date = summary.Date
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MoveDate(OfficeHourMoveDateModel model)
        {
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == model.ActivityDateId);

            // das sind noch die alten Zeiten
            if (activityDate != null)
            {
                // Das sind nun die neuen Zeiten
                var day = DateTime.Parse(model.NewDate);
                var from = DateTime.Parse(model.NewBegin);
                var to = DateTime.Parse(model.NewEnd);


                var start = day.Add(from.TimeOfDay);
                var end = day.Add(to.TimeOfDay);

                if (start > end)
                {
                    var x = start;
                    start = end;
                    end = x;
                }


                var nSlots = activityDate.Slots.Count;
                if (nSlots == 0)
                {
                    // Keine Slots => Anfang und Ende anpassen
                    activityDate.Begin = start;
                    activityDate.End = end;
                }
                else
                {
                    activityDate.Begin = start;
                    activityDate.End = end;

                    // Die Anzahl der Slots wird nicht verändert
                    // Der Status Reserve etc auch nicht
                    // es werden die Zeiten angepasst

                    var ohDuration = activityDate.End - activityDate.Begin;

                    var slotDuration = (int) (ohDuration.TotalMinutes / nSlots + 0.01);

                    var slots = activityDate.Slots.OrderBy(s => s.Begin).ToList();
                    var i = 1;
                    foreach (var slot in slots)
                    {
                        var slotStart = activityDate.Begin.AddMinutes((i - 1) * slotDuration);
                        var slotEnd = activityDate.Begin.AddMinutes(i * slotDuration);

                        slot.Begin = slotStart;
                        slot.End = slotEnd;

                        i++;
                    }

                }

                Db.SaveChanges();

                var officeHour = activityDate.Activity as OfficeHour;
                return RedirectToAction("OfficeHour", "Lecturer", new {id=officeHour.Semester.Id});
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);
            var m = GetMyMembership();

            if (m != null)
            {
                var courseService = new OfficeHourService(Db);
                var myOfficeHour = courseService.GetOfficeHour(m, semester);
                if (myOfficeHour != null)
                {
                    return RedirectToAction("Lecturer", new { id = m.Id });
                }
            }

            var model = new OfficeHourCreateModel
            {
                Semester = semester,
                Member = m
            };

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateOpenSystem(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);
            var m = GetMyMembership();

            if (m != null)
            {
                var courseService = new OfficeHourService(Db);
                var myOfficeHour = courseService.GetOfficeHour(m, semester);
                if (myOfficeHour != null)
                {
                    return RedirectToAction("Lecturer", new {id = m.Id});
                }
            }

            var model = new OfficeHourCreateModel
            {
                DayOfWeek = 1,
                StartTime = "16:00",
                EndTime = "17:00",
                Capacity = 5,
                SlotDuration = 0,
                SpareSlots = 0,
                SubscriptionLimit = 0,
                DateOption = 0,
                Text = string.Empty,
                Semester = semester
            };

            SetTimeSelections();
            SetRestrictionSelections();

            var org = GetMyOrganisation();
            ViewBag.UserRight = GetUserRight(org);
            


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateOpenSystem(OfficeHourCreateModel model)
        {
            var semester = SemesterService.GetSemester(model.Semester.Id);
            var org = GetMyOrganisation();
            var member = GetMyMembership();

            // Model in request umsetzen
            var start = TimeSpan.Parse(model.StartTime);
            var end = TimeSpan.Parse(model.EndTime);

            var ohService = new OfficeHourService(Db);

            var request = new OfficeHourCreateRequest
            {
                DozId = member.Id,
                StartTime = start,
                EndTime = end,
                SubscriptionLimit = model.SubscriptionLimit,
                Capacity = model.Capacity,
                SlotDuration = 0,
                SpareSlots = 0,
                DayOfWeek = (DayOfWeek) model.DayOfWeek,
                ByAgreement = false,
                OrgId = org.Id,
                SemesterId = semester.Id,
                CreateDates = true,
                Text = model.Description
            };

            var officeHour = ohService.CreateOfficeHour(request);

            return RedirectToAction("OfficeHour", "Lecturer", new {id = semester.Id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateSlotSystem(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);


            var dozId = string.Empty;
            var m = GetMyMembership();

            if (m != null)
            {
                var courseService = new OfficeHourService(Db);
                var myOfficeHour = courseService.GetOfficeHour(m, semester);
                if (myOfficeHour != null)
                {
                    return RedirectToAction("Lecturer", new {id = m.Id});
                }
            }

            // Member-Admin darf Sprechstunde für andere (und sich) anlegen
            if (m != null && !m.IsMemberAdmin)
            {
                dozId = m.ShortName;
            }


            var model = new OfficeHourCreateModel
            {
                DayOfWeek = 1,
                StartTime = "16:00",
                EndTime = "17:00",
                Capacity = 1,
                SlotDuration = 15,
                SpareSlots = 0,
                SubscriptionLimit = 0,
                DateOption = 0,
                Text = "Terminvereinbarung per E-Mail",
                SlotsPerDate = 1,
                MaxFutureSlots = 1,
                Semester = semester
            };

            SetTimeSelections();
            SetRestrictionSelections();

            var org = GetMyOrganisation();
            ViewBag.UserRight = GetUserRight(org);


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSlotSystem(OfficeHourCreateModel model)
        {
            var semester = SemesterService.GetSemester(model.Semester.Id);
            var org = GetMyOrganisation();
            var member = GetMyMembership();

            // Model in request umsetzen
            var start = TimeSpan.Parse(model.StartTime);
            var end = TimeSpan.Parse(model.EndTime);

            var ohService = new OfficeHourService(Db);

            var request = new OfficeHourCreateRequest
            {
                DozId = member.Id,
                StartTime = start,
                EndTime = end,
                SubscriptionLimit = model.SubscriptionLimit,
                Capacity = 1,
                SlotDuration = model.SlotDuration,
                SpareSlots = model.SpareSlots,
                DayOfWeek = (DayOfWeek) model.DayOfWeek,
                ByAgreement = false,
                OrgId = org.Id,
                SemesterId = semester.Id,
                CreateDates = true,
                Text = model.Description,
                SlotsPerDate = 1,
                FutureSlots = model.MaxFutureSlots
            };


            var officeHour = ohService.CreateOfficeHour(request);

            return RedirectToAction("OfficeHour", "Lecturer", new { id = semester.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateByAgreement(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);

            var m = GetMyMembership();

            if (m != null)
            {
                var courseService = new OfficeHourService(Db);
                var myOfficeHour = courseService.GetOfficeHour(m, semester);
                if (myOfficeHour != null)
                {
                    return RedirectToAction("Lecturer", new { id = m.Id });
                }
            }

            var model = new OfficeHourCreateModel
            {
                Description = "",
                Semester = semester
            };

            var org = GetMyOrganisation();
            ViewBag.UserRight = GetUserRight(org);


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateByAgreement(OfficeHourCreateModel model)
        {
            var semester = SemesterService.GetSemester(model.Semester.Id);
            var org = GetMyOrganisation();
            var member = GetMyMembership();

            var ohService = new OfficeHourService(Db);

            var request = new OfficeHourCreateRequest
            {
                DozId = member.Id,
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.Zero,
                SubscriptionLimit = 0,
                Capacity = 0,
                SlotDuration = 0,
                SpareSlots = 0,
                DayOfWeek = DayOfWeek.Friday,
                ByAgreement = true,
                OrgId = org.Id,
                SemesterId = semester.Id,
                CreateDates = false,
                Text = model.Description
            };

            var officeHour = ohService.CreateOfficeHour(request);


            return RedirectToAction("OfficeHour", "Lecturer", new { id = semester.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteOfficeHour(Guid id)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(act => act.Id == id);

            if (officeHour != null)
            {
                foreach (var date in officeHour.Dates.ToList())
                {
                    foreach (var slot in date.Slots.ToList())
                    {
                        foreach (var sub in slot.Occurrence.Subscriptions.ToList())
                        {
                            slot.Occurrence.Subscriptions.Remove(sub);
                            Db.Subscriptions.Remove(sub);
                        }
                        Db.Occurrences.Remove(slot.Occurrence);
                        date.Slots.Remove(slot);
                        Db.ActivitySlots.Remove(slot);
                    }

                    foreach (var sub in date.Occurrence.Subscriptions.ToList())
                    {
                        date.Occurrence.Subscriptions.Remove(sub);
                        Db.Subscriptions.Remove(sub);
                    }

                    Db.Occurrences.Remove(date.Occurrence);
                    officeHour.Dates.Remove(date);
                    Db.ActivityDates.Remove(date);
                }

                foreach (var sub in officeHour.Occurrence.Subscriptions.ToList())
                {
                    officeHour.Occurrence.Subscriptions.Remove(sub);
                    Db.Subscriptions.Remove(sub);
                }

                foreach (var owner in officeHour.Owners.ToList())
                {
                    officeHour.Owners.Remove(owner);
                    Db.ActivityOwners.Remove(owner);
                }

                Db.Occurrences.Remove(officeHour.Occurrence);
                Db.Activities.Remove(officeHour);

                Db.SaveChanges();
            }

            // Zurück zur Übersicht
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveSubscription(Guid id, string userId)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);

            if (occurrence != null)
            {
                var subscription =
                    occurrence.Subscriptions.SingleOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    occurrence.Subscriptions.Remove(subscription);
                    Db.Subscriptions.Remove(subscription);
                    Db.SaveChanges();

                    var summary = ActivityService.GetSummary(id);

                    var mailModel = new SubscriptionMailModel
                    {
                        Summary = summary,
                        Subscription = subscription,
                        User = UserManager.FindById(subscription.UserId),
                        SenderUser = UserManager.FindByName(User.Identity.Name),
                    };

                    var mail = new MailController();
                    mail.RemoveSubscription(mailModel).Deliver();

                    // date oder Slot?
                    var date = Db.ActivityDates.SingleOrDefault(x => x.Occurrence.Id == id);
                    if (date != null)
                    {
                        return PartialView("_RemoveSubscriptionDate", date);
                    }
                    else
                    {
                        var slot = Db.ActivitySlots.SingleOrDefault(x => x.Occurrence.Id == id);
                        if (slot != null)
                        {
                            return PartialView("_RemoveSubscriptionSlot", slot);
                        }
                    }

                }
            }

            return PartialView("_RemoveSubscription");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult SubscriptionList(Guid id)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write("Datum;Von;Bis;Name;Vorname;Datum der Eintragung;Anliegen");
            writer.Write(Environment.NewLine);

            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(a => a.Id == id);

            if (officeHour != null)
            {
                var now = DateTime.Now;

                foreach (var date in officeHour.Dates.Where(d => d.End >= now).OrderBy(d => d.Begin))
                {
                    foreach (var slot in date.Slots)
                    {
                        foreach (var sub in slot.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                        {
                            var user = UserManager.FindById(sub.UserId);
                            if (user != null)
                            {
                                writer.Write("{0};{1:hh\\:mm};{2:hh\\:mm};{3};{4};{5};{6}", 
                                    date.Begin.Date.ToShortDateString(), slot.Begin.TimeOfDay, slot.End.TimeOfDay,
                                    user.LastName, user.FirstName, sub.TimeStamp, sub.SubscriberRemark);
                                writer.Write(Environment.NewLine);
                            }
                        }
                    }
                    foreach (var sub in date.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                    {
                        var user = UserManager.FindById(sub.UserId);
                        if (user != null)
                        {
                            writer.Write("{0};{1:hh\\:mm};{2:hh\\:mm};{3};{4};{5};{6}",
                                date.Begin.Date.ToShortDateString(), date.Begin.TimeOfDay, date.End.TimeOfDay,
                                user.LastName,
                                user.FirstName,
                                sub.TimeStamp, sub.SubscriberRemark
                            );
                            writer.Write(Environment.NewLine);
                        }
                    }

                }
            }

            writer.Flush();
            writer.Dispose();

            return File(ms.GetBuffer(), "text/csv", "Teilnehmer.csv");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult SubscriptionDateList(Guid id)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write("Datum;Von;Bis;Name;Vorname;Datum der Eintragung;Anliegen");
            writer.Write(Environment.NewLine);

            var date = Db.ActivityDates.SingleOrDefault(a => a.Id == id);

            foreach (var slot in date.Slots)
            {
                foreach (var sub in slot.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                {
                    var user = UserManager.FindById(sub.UserId);
                    if (user != null)
                    {
                        writer.Write("{0};{1:hh\\:mm};{2:hh\\:mm};{3};{4};{5};{6}",
                            date.Begin.Date.ToShortDateString(), slot.Begin.TimeOfDay, slot.End.TimeOfDay,
                            user.LastName, user.FirstName, sub.TimeStamp, sub.SubscriberRemark);
                        writer.Write(Environment.NewLine);
                    }
                }
            }
            foreach (var sub in date.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
            {
                var user = UserManager.FindById(sub.UserId);
                if (user != null)
                {
                    writer.Write("{0};{1:hh\\:mm};{2:hh\\:mm};{3};{4};{5};{6}",
                        date.Begin.Date.ToShortDateString(), date.Begin.TimeOfDay, date.End.TimeOfDay,
                        user.LastName,
                        user.FirstName,
                        sub.TimeStamp, sub.SubscriberRemark
                    );
                    writer.Write(Environment.NewLine);
                }
            }

            writer.Flush();
            writer.Dispose();

            return File(ms.GetBuffer(), "text/csv", "Teilnehmer.csv");
        }


        private void SetRestrictionSelections()
        {
            var sd = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Slots a 5 min", Value = 5},
                new SelectionHelper {Text = "Slots a 10 min", Value = 10},
                new SelectionHelper {Text = "Slots a 15 min", Value = 15},
                new SelectionHelper {Text = "Slots a 20 min", Value = 20},
                new SelectionHelper {Text = "Slots a 30 min", Value = 30}
            };

            var spare = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Keine", Value = 0},
                new SelectionHelper {Text = "1 am Ende", Value = -1},
                new SelectionHelper {Text = "2 am Ende", Value = -2},
                new SelectionHelper {Text = "3 am Ende", Value = -3},
                new SelectionHelper {Text = "1 am Anfang", Value = 1},
                new SelectionHelper {Text = "2 am Anfang", Value = 2},
                new SelectionHelper {Text = "3 am Anfang", Value = 3},
                new SelectionHelper {Text = "jeweils 1 am Anfang und Ende", Value = -99}
            };

            var cap = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Keine Beschränkung", Value = -1},
                new SelectionHelper {Text = "1", Value = 1},
                new SelectionHelper {Text = "2", Value = 2},
                new SelectionHelper {Text = "3", Value = 3},
                new SelectionHelper {Text = "4", Value = 4},
                new SelectionHelper {Text = "5", Value = 5},
            };

            var limits = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Keine Beschränkung", Value = 0},
                new SelectionHelper {Text = "jeweils 12 h vor Beginn des Termins", Value = 12},
                new SelectionHelper {Text = "jeweils 24 h vor Beginn des Termins", Value = 24},
            };


            var slotLimits = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "1", Value = 1},
                new SelectionHelper {Text = "2", Value = 2},
                new SelectionHelper {Text = "3", Value = 3},
                new SelectionHelper {Text = "Keine Beschränkung", Value = 0},
            };

            var futureSlotLimits = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "1", Value = 1},
                new SelectionHelper {Text = "2", Value = 2},
                new SelectionHelper {Text = "3", Value = 3},
                new SelectionHelper {Text = "Keine Beschränkung", Value = 0},
            };



            ViewBag.SlotDurations = new SelectList(sd, "Value", "Text", "Slots a 10 min");
            ViewBag.SpareSlots = new SelectList(spare, "Value", "Text", "keine");
            ViewBag.Capacities = new SelectList(cap, "Value", "Text", "keine Beschränkung");
            ViewBag.Limits = new SelectList(limits, "Value", "Text", "keine Beschränkung");
            ViewBag.SlotLimits = new SelectList(slotLimits, "Value", "Text", "1");
            ViewBag.FutureSlotLimits = new SelectList(futureSlotLimits, "Value", "Text", "1");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult LockSlot(Guid id)
        {
            var occurrence = Db.ActivitySlots.SingleOrDefault(occ => occ.Id == id);

            if (occurrence != null)
            {
                occurrence.Occurrence.IsAvailable = false;
                Db.SaveChanges();
            }


            return PartialView("_LockedSlot", occurrence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult UnLockSlot(Guid id)
        {
            var occurrence = Db.ActivitySlots.SingleOrDefault(occ => occ.Id == id);

            if (occurrence != null)
            {
                occurrence.Occurrence.IsAvailable = true;
                Db.SaveChanges();
            }

            return PartialView("_LockedSlot", occurrence);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditSlotSystem(Guid id)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == id);

            var limit = 0;
            if (officeHour.Occurrence.UntilIsRestricted && officeHour.Occurrence.UntilTimeSpan.HasValue)
            {
                limit = officeHour.Occurrence.UntilTimeSpan.Value.Hours + 1;
            }


            var model = new OfficeHourCreateModel
            {
                OfficeHourId = officeHour.Id,
                Capacity = officeHour.Occurrence.Capacity,
                SubscriptionLimit = limit,
                MaxFutureSlots = officeHour.FutureSubscriptions.HasValue ? officeHour.FutureSubscriptions.Value : 0
            };

            SetRestrictionSelections();

            var org = GetMyOrganisation();
            var sem = officeHour.Semester;
            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Semester = sem;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditSlotSystem(OfficeHourCreateModel model)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == model.OfficeHourId);

            if (officeHour != null)
            {

                officeHour.Occurrence.UntilIsRestricted = (model.SubscriptionLimit > 0);
                officeHour.Occurrence.UntilTimeSpan =
                    (model.SubscriptionLimit > 0)
                        ? new TimeSpan(model.SubscriptionLimit - 1, 59, 0)
                        : new TimeSpan?();

                officeHour.FutureSubscriptions = model.MaxFutureSlots;
                officeHour.Description = model.Description;

                officeHour.Occurrence.Capacity = model.Capacity;
                Db.SaveChanges();
            }

            return RedirectToAction("OfficeHour", "Lecturer");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditOpenSystem(Guid id)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == id);

            var model = new OfficeHourCreateModel
            {
                OfficeHourId = officeHour.Id,
                MaxFutureSlots = officeHour.FutureSubscriptions ?? -1,
                Description = officeHour.Description,
                SubscriptionLimit = 0,
            };

            SetRestrictionSelections();

            var org = GetMyOrganisation();
            var sem = officeHour.Semester;
            model.Semester = sem;

            ViewBag.UserRight = GetUserRight(org);


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditOpenSystem(OfficeHourCreateModel model)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == model.OfficeHourId);

            officeHour.FutureSubscriptions = model.MaxFutureSlots;
            officeHour.Description = model.Description;
            Db.SaveChanges();

            return RedirectToAction("OfficeHour", "Lecturer", new { id = officeHour.Semester.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditByAgreement(Guid id)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == id);

            var model = new OfficeHourCreateModel
            {
                OfficeHourId = officeHour.Id,
                Description = officeHour.Description
            };

            SetRestrictionSelections();

            var org = GetMyOrganisation();
            var sem = officeHour.Semester;
            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Semester = sem;


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditByAgreement(OfficeHourCreateModel model)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == model.OfficeHourId);

            officeHour.Description = model.Description;
            Db.SaveChanges();

            return RedirectToAction("OfficeHour", "Lecturer");

        }

        public ActionResult SubscriptionDetails(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);
            var user = GetCurrentUser();
            var officeHour = date.Activity as OfficeHour;

            var infoService = new OfficeHourInfoService(UserManager);

            var model = new OfficeHourDateSubscriptionViewModel();

            model.Date = date;
            model.Host = infoService.GetHost(officeHour);
            model.Semester = officeHour.Semester;
            model.Subscription = infoService.GetSubscription(date, user.Id);
            model.Slot = infoService.GetSubscribedSlot(date, user.Id);


            // Eintragungsfrist abgelaufen?
            model.IsExpired = infoService.IsExpired(date);



            return View(model);
        }

        public ActionResult EditDateSubscription(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);
            var user = GetCurrentUser();
            var officeHour = date.Activity as OfficeHour;

            var infoService = new OfficeHourInfoService(UserManager);

            var model = new OfficeHourDateSubscriptionViewModel();

            model.Date = date;
            model.Host = infoService.GetHost(officeHour);
            model.Subscription = infoService.GetSubscription(date, user.Id);
            model.Description = model.Subscription.SubscriberRemark;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditDateSubscription(OfficeHourDateSubscriptionViewModel model)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>()
                .SingleOrDefault(x => x.Id == model.Subscription.Id);

            subscription.SubscriberRemark = model.Description;
            
            Db.SaveChanges();

            return RedirectToAction("SubscriptionDetails", new { id = model.Date.Id });

        }


        public ActionResult UnsubscribeDateSubscription(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>()
                .SingleOrDefault(x => x.Id == id);
            var date = Db.ActivityDates.SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);

            if (date == null)
            {
                var slot = Db.ActivitySlots.SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);
                if (slot != null)
                {
                    date = slot.ActivityDate;
                }
            }


            var officeHour = date.Activity as OfficeHour;
            var infoService = new OfficeHourInfoService(UserManager);
            var host = infoService.GetHost(officeHour);

            Db.Subscriptions.Remove(subscription);
            Db.SaveChanges();

            return RedirectToAction("Lecturer", new { id = host.Id, semId = officeHour.Semester.Id });
        }


    }
}
