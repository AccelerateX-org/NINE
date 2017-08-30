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
using MyStik.TimeTable.DataServices;
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Liste aller Sprechstunden
            var semester = GetSemester();
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

        /// <summary>
        /// Terminliste einer bestimmten Spürechstunde
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
        /// <returns></returns>
        public ActionResult Lecturer(Guid id)
        {
            var semester = GetSemester();
            var hostRequested = Db.Members.FirstOrDefault(l => l.Id == id);

            if (hostRequested == null)
            {
                return RedirectToAction("Index");
            }

            var ohService = new OfficeHourService();
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
                // nach Slots suchen
                if (officeHour.Dates.Any(x => x.Slots.Any()))
                {
                    return SlotSystem(model2);
                }
                else
                {
                    return OpenSystem(model2);
                }
            }
        }

        private ActionResult ByAgreement(OfficeHourSubscriptionViewModel model)
        {
            return View("DateListByAgreement", model);
        }



        private ActionResult SlotSystem(OfficeHourSubscriptionViewModel model)
        {
            var userRequesting = GetCurrentUser();
            var officeHour = model.OfficeHour;

            var dates = officeHour.Dates.OrderBy(x => x.Begin).ToList();
            var now = DateTime.Now;

            var nFutureSub = officeHour.Dates.Count(x => x.End > DateTime.Now &&
                                                         x.Slots.Any(
                                                             y => y.Occurrence.Subscriptions.Any(
                                                                 s => s.UserId.Equals(userRequesting
                                                                     .Id))));

            foreach (var date in dates)
            {
                var dateModel = new OfficeHourDateViewModel();
                dateModel.Date = date;

                // zuerst alle, die vorbei sind
                // war ich drin
                // ja => Zeiraum Slot zeigen
                // nein => Zeitraum Date zeigen
                if (date.End < now)
                {
                    var slots =
                        date.Slots.Where(
                            x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id))).ToList();


                    if (slots.Any())
                    {
                        if (slots.Count == 1)
                        {
                            dateModel.Slot = slots.First();
                            dateModel.Remark = "gebucht";
                        }
                        else
                        {
                            dateModel.Remark = $"{slots.Count} Slots gebucht";
                        }
                    }
                    else
                    {
                        dateModel.Remark = "Keine Buchung";
                    }
                }
                else
                {
                    // für zukünftige
                    // bin ich drin?
                    var slots =
                        date.Slots.Where(
                            x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id))).ToList();

                    // ja
                    if (slots.Any())
                    {
                        var slot = slots.First();
                        // Anzahl meiner Eintragungen erhöhen


                        // kann ich mich noch austragen
                        var state = ActivityService.GetSubscriptionState(slot.Occurrence, date.Begin, date.End);

                        // nein
                        if (state == SubscriptionState.AfterSubscriptionPhase ||
                            state == SubscriptionState.DuringOccurrence ||
                            state == SubscriptionState.AfterOccurrence)
                        {
                            dateModel.Remark = "kein Austragen mehr möglich.";
                        }
                        else
                        {
                            // ja => Schalter zeigen
                            dateModel.Slot = slot;
                            dateModel.Subscription =
                                slot.Occurrence.Subscriptions.FirstOrDefault(
                                    x => x.UserId.Equals(userRequesting.Id));
                        }
                    }
                    else
                    {
                        // nein
                        // Ist was frei
                        var availableSlots = date.Slots.Where(x => !x.Occurrence.Subscriptions.Any() && x.Occurrence.IsAvailable).ToList();
                        if (availableSlots.Any())
                        {
                            // ja
                            // darf ich mich eintragen
                            // Anzahl der zukünftigen gebuchten Slots vs Vorgabe
                            if (officeHour.FutureSubscriptions.HasValue && officeHour.FutureSubscriptions.Value > 0)
                            {
                                var maxSub = officeHour.FutureSubscriptions.Value;
                                if (nFutureSub < maxSub)
                                {
                                    // eintragen erlaubt
                                    dateModel.AvailableSlots = availableSlots;
                                }
                                else
                                {
                                    dateModel.Remark = $"Habe bereits {maxSub} Slots gebucht";
                                }

                            }
                            else
                            {
                                // eintragen erlaubt
                                dateModel.AvailableSlots = availableSlots;
                            }

                        }
                        else
                        {
                            // nein
                            dateModel.Remark = "ausgebucht";
                        }
                    }
                }



                model.Dates.Add(dateModel);
            }

            return View("DateListSlotSystem", model);
        }

        private ActionResult OpenSystem(OfficeHourSubscriptionViewModel model)
        {
            var userRequesting = GetCurrentUser();
            var officeHour = model.OfficeHour;
            var semester = GetSemester();

            var dates = officeHour.Dates.OrderBy(x => x.Begin).ToList();
            var now = DateTime.Now;

            var capacity = officeHour.Occurrence.Capacity;
            var hasCapacity = capacity > 0;


            foreach (var date in dates)
            {
                var dateModel = new OfficeHourDateViewModel();
                dateModel.Date = date;

                // zuerst alle, die vorbei sind
                // war ich drin
                // nein => Zeitraum Date zeigen
                if (date.End < now)
                {
                    var booked = date.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id));

                    if (booked)
                    {
                        dateModel.Remark = "gebucht";
                    }
                    else
                    {
                        dateModel.Remark = "Keine Buchung";
                    }
                }
                else
                {
                    // für zukünftige
                    // bin ich drin?
                    var booked = date.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id));

                    // ja
                    if (booked)
                    {
                        // kann ich mich noch austragen
                        var state = ActivityService.GetSubscriptionState(date.Occurrence, date.Begin, date.End);

                        // nein
                        if (state == SubscriptionState.AfterSubscriptionPhase ||
                            state == SubscriptionState.DuringOccurrence ||
                            state == SubscriptionState.AfterOccurrence)
                        {
                            dateModel.Remark = "kein Austragen mehr möglich.";
                        }
                        else
                        {
                            // ja => Schalter zeigen
                            dateModel.Subscription =
                                date.Occurrence.Subscriptions.FirstOrDefault(
                                    x => x.UserId.Equals(userRequesting.Id));

                            dateModel.State =
                                ActivityService.GetActivityState(date.Occurrence, userRequesting, semester);
                        }
                    }
                    else
                    {
                        // nein
                        // Ist was frei
                        if (hasCapacity)
                        {
                            var availableSlots = capacity - date.Occurrence.Subscriptions.Count;
                            if (availableSlots > 0)
                            {
                                // ja
                                // darf ich mich eintragen
                                dateModel.AvailableDate = date;

                                dateModel.State =
                                    ActivityService.GetActivityState(date.Occurrence, userRequesting, semester);
                            }
                            else
                            {
                                // nein
                                dateModel.Remark = "ausgebucht";
                            }
                        }
                        else
                        {
                            dateModel.AvailableDate = date;

                            dateModel.State =
                                ActivityService.GetActivityState(date.Occurrence, userRequesting, semester);
                        }
                    }
                }



                model.Dates.Add(dateModel);
            }

            return View("DateListOpenSystem", model);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        public ActionResult SubscribeDate(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);


            return View(date);
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

                    slot.Occurrence.Subscriptions.Add(subscription);
                    Db.SaveChanges();
                }
            }

            var officeHour = slot.ActivityDate.Activity as OfficeHour;
            var owner = officeHour.Owners.First();


            return RedirectToAction("Lecturer", new {id = owner.Member.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public ActionResult Unsubscribe(Guid slotId)
        {
            var slot = Db.ActivitySlots.SingleOrDefault(x => x.Id == slotId);
            var user = GetCurrentUser();

            var mySub = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
            if (mySub != null)
            {
                slot.Occurrence.Subscriptions.Remove(mySub);
                Db.Subscriptions.Remove(mySub);
                Db.SaveChanges();
            }

            var officeHour = slot.ActivityDate.Activity as OfficeHour;
            var owner = officeHour.Owners.First();

            return RedirectToAction("Lecturer", new {id = owner.Member.Id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        public PartialViewResult DateDetails(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == dateId);
            var officeHour = date.Activity as OfficeHour;

            var semester = GetSemester();
            var myUser = AppUser;

            var model = new OfficeHourDateSlotViewModel
            {
                OfficeHour = officeHour,
                Semester = semester,
                //Host = hostRequested,
            };

            // aktuelles Datum
            var orderedDates = officeHour.Dates.OrderBy(x => x.End).ToList();
            var currentDate = date;
            if (currentDate == orderedDates.Last())
            {
                model.CurrentDate = orderedDates.LastOrDefault();
                model.NextDate = null;
                model.PreviousDate = orderedDates.LastOrDefault(x => x.End < model.CurrentDate.End);
            }
            else if (currentDate == orderedDates.First())
            {
                model.CurrentDate = currentDate;
                model.PreviousDate = null;
                model.NextDate = orderedDates.FirstOrDefault(x => x.End > currentDate.End);
            }
            else
            {
                model.CurrentDate = currentDate;
                model.PreviousDate = orderedDates.LastOrDefault(x => x.End < model.CurrentDate.End);
                model.NextDate = orderedDates.FirstOrDefault(x => x.End > currentDate.End);
            }

            model.State = ActivityService.GetActivityState(model.CurrentDate.Occurrence, myUser, semester);


            // die aktuellen Eintragungen
            // Ansatz: Es gibt Slots
            foreach (var slot in model.CurrentDate.Slots)
            {
                var ohSlot = new SingleSlotViewModel();
                ohSlot.Begin = slot.Begin;
                ohSlot.End = slot.End;
                ohSlot.Occurrence = slot.Occurrence;

                foreach (var subscription in slot.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                {
                    var user = UserManager.FindById(subscription.UserId);

                    var member = new CourseMemberModel()
                    {
                        Subscription = subscription,
                        User = user,
                    };

                    ohSlot.Member.Add(member);
                }
                model.Slots.Add(ohSlot);
            }

            if (!model.CurrentDate.Slots.Any())
            {
                var ohSlot = new SingleSlotViewModel();
                ohSlot.Begin = model.CurrentDate.Begin;
                ohSlot.End = model.CurrentDate.End;
                ohSlot.Occurrence = model.CurrentDate.Occurrence;

                foreach (var subscription in model.CurrentDate.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                {
                    var user = UserManager.FindById(subscription.UserId);

                    var member = new CourseMemberModel()
                    {
                        Subscription = subscription,
                        User = user,
                    };

                    ohSlot.Member.Add(member);

                }
                model.Slots.Add(ohSlot);
            }


            return PartialView("_SingleDateViewHost", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Inspect(Guid id)
        {
            var semester = GetSemester();

            // Die Aktivität Sprechstunde herausholen
            // im aktuellen Semester für den Dozenten
            var officeHour =
                Db.Activities.OfType<OfficeHour>().SingleOrDefault(a =>
                    a.Semester.Id == semester.Id &&
                    a.Dates.Any(oc => oc.Hosts.Any(l => l.Id == id)));

            return View(officeHour);
        }

        private void FillOfficeHourDateList(OfficeHourCharacteristicModel model, OrganiserMember hostRequested,
            ApplicationUser userRequesting)
        {
            var logger = LogManager.GetLogger("OfficeHour");

            var myUser = AppUser;
            var semester = GetSemester();


            if (model.OfficeHour != null)
            {

                var now = GlobalSettings.Now;

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
                                                var state = ActivityService.GetActivityState(ohSlot.Occurrence, myUser,
                                                    semester);

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
                                                        ActivityService.GetActivityState(ohSlot.Occurrence, myUser,
                                                            semester),
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
                                                    ActivityService.GetActivityState(ohSlot.Occurrence, myUser,
                                                        semester),
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
                                        State = ActivityService.GetActivityState(date.Occurrence, myUser, semester),
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
                                    State = ActivityService.GetActivityState(date.Occurrence, myUser, semester),
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
                StartTime = "16:00",
                EndTime = "17:00",
                Capacity = 5,
                SlotDuration = 0,
                SpareSlots = 0,
                SubscriptionLimit = 0,
                DateOption = 0,
                Text = "Terminvereinbarung per E-Mail",
                Type = -1
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

            if (oh.ByAgreement)
                return RedirectToAction("OfficeHour", "Lecturer");

            var firstDate = oh.Dates.FirstOrDefault();
            if (firstDate == null)
                return RedirectToAction("OfficeHour", "Lecturer");

            var owner = oh.Owners.FirstOrDefault();
            if (owner == null)
                return RedirectToAction("OfficeHour", "Lecturer");

            var lecturer = owner.Member;

            // Model in request umsetzen
            var day = DateTime.Parse(model.NewDate);
            var from = DateTime.Parse(model.StartTime);
            var to = DateTime.Parse(model.EndTime);

            var start = day.Add(from.TimeOfDay);
            var end = day.Add(to.TimeOfDay);


            //if (firstDate.Slots.Any())


            var date = new ActivityDate
            {
                Begin = start,
                End = end,
                Activity = oh,
                Hosts = new HashSet<OrganiserMember>
                {
                    lecturer,
                },
                Occurrence = new Occurrence
                {
                    IsAvailable = true,
                    Capacity = firstDate.Occurrence.Capacity,
                    FromIsRestricted = false,
                    UntilIsRestricted = firstDate.Occurrence.UntilIsRestricted,
                    UntilTimeSpan = firstDate.Occurrence.UntilTimeSpan,
                    IsCanceled = false,
                    IsMoved = false,
                }

            };

            if (firstDate.Slots.Any())
            {
                var ohDuration = end - start;
                var totalMinutes = ohDuration.TotalMinutes;
                var numSlots = firstDate.Slots.Count;
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
                Semester semester = GetSemester();

                if (semester != null)
                {
                    day = day.AddDays(7);
                    while (day < semester.EndCourses)
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
                                    lecturer,
                                },
                                Occurrence = new Occurrence
                                {
                                    IsAvailable = true,
                                    Capacity = firstDate.Occurrence.Capacity,
                                    FromIsRestricted = false,
                                    UntilIsRestricted = firstDate.Occurrence.UntilIsRestricted,
                                    UntilTimeSpan = firstDate.Occurrence.UntilTimeSpan,
                                    IsCanceled = false,
                                    IsMoved = false,
                                }

                            };

                            if (firstDate.Slots.Any())
                            {
                                var ohDuration = end - start;
                                var totalMinutes = ohDuration.TotalMinutes;
                                var numSlots = firstDate.Slots.Count;
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

            return RedirectToAction("OfficeHour", "Lecturer");
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

            return RedirectToAction("OfficeHour", "Lecturer");
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
                AdjustSlotCount = true
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

                if (start >= end)
                {
                    ModelState.AddModelError("", "Falsche Zeitangabe: Beginn liegt nach Ende");
                    return View(model);
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
                return RedirectToAction("OfficeHour", "Lecturer");
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateOpenSystem()
        {
            var semester = GetSemester();
            var m = GetMyMembership();

            if (m != null)
            {
                var courseService = new CourseService(UserManager);
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
                Text = "Terminvereinbarung per E-Mail",
            };

            SetTimeSelections();
            SetRestrictionSelections();

            var org = GetMyOrganisation();
            var sem = GetSemester();
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
        public ActionResult CreateOpenSystem(OfficeHourCreateModel model)
        {
            var semester = GetSemester();
            var org = GetMyOrganisation();
            var member = GetMyMembership();

            // Model in request umsetzen
            var start = TimeSpan.Parse(model.StartTime);
            var end = TimeSpan.Parse(model.EndTime);

            var ohService = new OfficeHourService();

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

            return RedirectToAction("OfficeHour", "Lecturer");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateSlotSystem()
        {
            var semester = GetSemester();

            var dozId = string.Empty;
            var m = GetMyMembership();

            if (m != null)
            {
                var courseService = new CourseService(UserManager);
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
            };

            SetTimeSelections();
            SetRestrictionSelections();

            var org = GetMyOrganisation();
            var sem = GetSemester();
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
        public ActionResult CreateSlotSystem(OfficeHourCreateModel model)
        {
            var semester = GetSemester();
            var org = GetMyOrganisation();
            var member = GetMyMembership();

            // Model in request umsetzen
            var start = TimeSpan.Parse(model.StartTime);
            var end = TimeSpan.Parse(model.EndTime);

            var ohService = new OfficeHourService();

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

            return RedirectToAction("OfficeHour", "Lecturer");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateByAgreement()
        {
            var semester = GetSemester();

            var dozId = string.Empty;
            var m = GetMyMembership();

            if (m != null)
            {
                var courseService = new CourseService(UserManager);
                var myOfficeHour = courseService.GetOfficeHour(m, semester);
                if (myOfficeHour != null)
                {
                    return RedirectToAction("Lecturer", new { id = m.Id });
                }
            }

            var model = new OfficeHourCreateModel
            {
                Description = "",
            };

            var org = GetMyOrganisation();
            var sem = GetSemester();
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
        public ActionResult CreateByAgreement(OfficeHourCreateModel model)
        {
            var semester = GetSemester();
            var org = GetMyOrganisation();
            var member = GetMyMembership();

            var ohService = new OfficeHourService();

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


            return RedirectToAction("OfficeHour", "Lecturer");
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
        public ActionResult RemoveSubscription(Guid id, string userId)
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

                    return RedirectToAction(summary.Action, summary.Controller, new {id = summary.Id});

                    // alt: return RedirectToAction("Index", new { id = summary.Activity.Id });
                }
            }

            return RedirectToAction("Missing", "Home");
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

            writer.Write("Datum;Von;Bis;Name;Vorname;E-Mail;Datum der Eintragung");
            writer.Write(Environment.NewLine);

            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(a => a.Id == id);

            if (officeHour != null)
            {
                var now = GlobalSettings.Now;

                foreach (var date in officeHour.Dates.Where(d => d.End >= now).OrderBy(d => d.Begin))
                {
                    foreach (var slot in date.Slots)
                    {
                        foreach (var sub in slot.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                        {
                            var user = UserManager.FindById(sub.UserId);
                            if (user != null)
                            {
                                writer.Write("{0};{1};{2};{3};{4};{5};{6}", date.Begin.Date.ToShortDateString(),
                                    slot.Begin.TimeOfDay.ToString(@"hh\:mm"), slot.End.TimeOfDay.ToString(@"hh\:mm"),
                                    user.LastName, user.FirstName, user.Email, sub.TimeStamp);
                                writer.Write(Environment.NewLine);
                            }
                        }
                    }
                    foreach (var sub in date.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                    {
                        var user = UserManager.FindById(sub.UserId);
                        if (user != null)
                        {
                            writer.Write(String.Format("{0};{1};{2};{3};{4};{5};{6}",
                                date.Begin.Date.ToShortDateString(),
                                date.Begin.TimeOfDay.ToString(@"hh\:mm"),
                                date.End.TimeOfDay.ToString(@"hh\:mm"),
                                user.LastName,
                                user.FirstName,
                                user.Email,
                                sub.TimeStamp
                            ));
                            writer.Write(Environment.NewLine);
                        }
                    }

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
            var sem = GetSemester();
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
                Capacity = officeHour.Occurrence.Capacity,
                SubscriptionLimit = 0,
            };

            SetRestrictionSelections();

            var org = GetMyOrganisation();
            var sem = GetSemester();
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
        public ActionResult EditOpenSystem(OfficeHourCreateModel model)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(o => o.Id == model.OfficeHourId);

            officeHour.Occurrence.Capacity = model.Capacity;
            officeHour.Description = model.Description;
            Db.SaveChanges();

            return RedirectToAction("OfficeHour", "Lecturer");

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
            var sem = GetSemester();
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

    }
}