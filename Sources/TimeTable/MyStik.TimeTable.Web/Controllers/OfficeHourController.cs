using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public class OfficeHourController : BaseController
    {
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


        //
        // GET: /OfficeHour/
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue)
            {
                var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(oh => oh.Id == id.Value);

                var fak = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

                if (officeHour != null && fak != null)
                {
                    var member = fak.Members.SingleOrDefault(m => m.ShortName.Equals(officeHour.ShortName));

                    if (member != null)
                    {
                        return RedirectToAction("Lecturer", new {id = member.Id});
                    }
                }
            }

            return RedirectToAction("Index", "Lecturer");
        }

        /// <summary>
        /// Terminliste einer bestimmten Spürechstunde
        /// </summary>
        /// <param name="id"></param>
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
                return RedirectToAction("Index", "Lecturer");
            }

            var officeHour =
                Db.Activities.OfType<OfficeHour>().SingleOrDefault(a =>
                    a.Semester.Id == semester.Id &&
                    a.Dates.Any(oc => oc.Hosts.Any(l => l.Id == id)));
            ViewBag.UserRight = GetUserRight(User.Identity.Name, officeHour);


            if (ViewBag.UserRight.IsHost)
            {
                officeHour =
                    Db.Activities.OfType<OfficeHour>().SingleOrDefault(a =>
                        a.Semester.Id == semester.Id &&
                        a.Dates.Any(oc => oc.Hosts.Any(l => l.Id == id)));

                var model = new OfficeHourDateSlotViewModel
                {
                    OfficeHour = officeHour,
                    Semester = semester,
                    //Host = hostRequested,
                };

                // aktuelles Datum
                var orderedDates = officeHour.Dates.OrderBy(x => x.End).ToList();
                var currentDate = orderedDates.FirstOrDefault(x => x.End >= DateTime.Now);
                if (currentDate == null)
                {
                    model.CurrentDate = orderedDates.LastOrDefault();
                }
                else
                {
                    model.CurrentDate = currentDate;
                }

                return View("DateListHost2", model);
            }

            var model2 = new OfficeHourSubscriptionViewModel
            {
                OfficeHour = officeHour,
                Semester = semester,
                Host = hostRequested,
            };
            

            var userRequesting = AppUser;

            // wenn eine Sprechstunde begonnen hat, dann ist sie für den Studenten bereits "Geschichte"
            var historicDates = officeHour.Dates.Where(x => x.Begin <= DateTime.Now).OrderBy(x => x.Begin).ToList();
            var futureDates = officeHour.Dates.Where(x => x.Begin > DateTime.Now).OrderBy(x => x.Begin).ToList();

            // alle historischen Eintragungen (zur Info)
            var historicSubscriptionDates = historicDates.Where(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id))).ToList();
            foreach (var date in historicSubscriptionDates)
            {
                var ohSlot = new OfficeHourAvailableSlotViewModel
                {
                    Date = date.Begin.Date,
                    Begin = date.Begin,
                    End = date.End,
                    Occurrence = date.Occurrence,
                    IsSubscribed = true
                };
                model2.Slots.Add(ohSlot);
            }

            var historicSubscriptionSlots = Db.ActivitySlots.Where(x => 
                x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id)) &&
                x.ActivityDate.Activity.Id == officeHour.Id &&
                x.ActivityDate.End <= DateTime.Now).ToList();
            foreach (var slot in historicSubscriptionSlots)
            {
                var ohSlot = new OfficeHourAvailableSlotViewModel
                {
                    Date = slot.Begin.Date,
                    Begin = slot.Begin,
                    End = slot.End,
                    Occurrence = slot.Occurrence,
                    IsSubscribed = true
                };
                model2.Slots.Add(ohSlot);
            }


            // alle zukünftigen Termine mit Status
            // - vor dem Ende der Eintragungszeit noch das Austragen zulassen
            // - nach dem Ende der Eintragungszeit kein Austragen mehr zulassen
            //var currentSubscriptonDates;
            //var currentSubscriptionSlots;

            
            // zukünftige freie Termine bei Dozent
            // - nur falls keine zukünftige Eintragung besteht

            // für diesen Dozenten die Liste der zukünftigen freien Termine zusammenstellen
            // aber nur falls der Benutzer nicht bereits bei einem zukünftigen Termin eingetragen ist
            foreach (var date in futureDates)
            {
                if (date.Slots.Any())
                {
                    // bin ich in in einem Slot eingetragen?
                    // ja => dazufügen (Status kommt später)
                    // nein => ist noch ein Platz frei
                    foreach (var slot in date.Slots)
                    {
                        var isSubscribed =
                            slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id));
                        if (isSubscribed)
                        {
                            var ohSlot = new OfficeHourAvailableSlotViewModel
                            {
                                Date = slot.Begin.Date,
                                Begin = slot.Begin,
                                End = slot.End,
                                Occurrence = slot.Occurrence,
                                IsSubscribed = true
                            };
                            model2.Slots.Add(ohSlot);
                        }
                        else
                        {
                            if (slot.Occurrence.Capacity < 0)
                            {
                                // keine Platzbeschränkung
                                var ohSlot = new OfficeHourAvailableSlotViewModel
                                {
                                    Date = slot.Begin.Date,
                                    Begin = slot.Begin,
                                    End = slot.End,
                                    Occurrence = slot.Occurrence
                                };
                                model2.Slots.Add(ohSlot);
                            }
                            else if (slot.Occurrence.Subscriptions.Count < slot.Occurrence.Capacity)
                            {
                                // Platzbeschränkung mit noch freien Plätzen
                                var n = slot.Occurrence.Capacity - slot.Occurrence.Subscriptions.Count;

                                var ohSlot = new OfficeHourAvailableSlotViewModel
                                {
                                    Date = slot.Begin.Date,
                                    Begin = slot.Begin,
                                    End = slot.End,
                                    Occurrence = slot.Occurrence,
                                    Remark = string.Format("Noch {0} Plätze verfügbar", n)
                                };
                                model2.Slots.Add(ohSlot);
                            }
                        }
                    }
                }
                else
                {
                    var isSubscribed =
                        date.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userRequesting.Id));
                    if (isSubscribed)
                    {
                        var ohSlot = new OfficeHourAvailableSlotViewModel
                        {
                            Date = date.Begin.Date,
                            Begin = date.Begin,
                            End = date.End,
                            Occurrence = date.Occurrence,
                            IsSubscribed = true
                        };
                        model2.Slots.Add(ohSlot);
                    }
                    else
                    {

                        if (date.Occurrence.Capacity < 0)
                        {
                            var ohSlot = new OfficeHourAvailableSlotViewModel
                            {
                                Date = date.Begin.Date,
                                Begin = date.Begin,
                                End = date.End,
                                Occurrence = date.Occurrence
                            };
                            model2.Slots.Add(ohSlot);
                        }
                        else if (date.Occurrence.Subscriptions.Count < date.Occurrence.Capacity)
                        {
                            var n = date.Occurrence.Capacity - date.Occurrence.Subscriptions.Count;

                            var ohSlot = new OfficeHourAvailableSlotViewModel
                            {
                                Date = date.Begin.Date,
                                Begin = date.Begin,
                                End = date.End,
                                Occurrence = date.Occurrence,
                                Remark = string.Format("Noch {0} Plätze verfügbar", n)
                            };
                            model2.Slots.Add(ohSlot);
                        }
                    }
                }
            }

            // Den Status einfügen
            foreach (var slot in model2.Slots)
            {
                slot.State =
                    ActivityService.GetActivityState(slot.Occurrence, userRequesting, semester);
            }


            
            return View("DateListPublic2", model2);
        }

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

        private void FillOfficeHourDateList(OfficeHourCharacteristicModel model, OrganiserMember hostRequested, ApplicationUser userRequesting)
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
                                                var state = ActivityService.GetActivityState(ohSlot.Occurrence, myUser, semester);

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
                                                        ActivityService.GetActivityState(ohSlot.Occurrence, myUser, semester),
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
                                                                              !string.IsNullOrEmpty(hostRequested.UserId) &&
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
                                                    ActivityService.GetActivityState(ohSlot.Occurrence, myUser, semester),
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

                        logger.ErrorFormat("Fehler bei Termin {0} für {1}: {2}", date.Begin, model.OfficeHour.ShortName, sb.ToString());
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
            var semester = GetSemester();

            var memberService = new MemberService(Db, UserManager);
            var member = memberService.GetMember(User.Identity.Name, "FK 09");
            var isProf = memberService.HasRole(User.Identity.Name, "FK 09", "Prof");

            var model = new OfficeHourCreateModel
            {
                OfficeHourId = id,
                Semester = semester.Name,
                DozId = isProf ? member.ShortName : string.Empty,
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


        [HttpPost]
        public ActionResult CreateDate(OfficeHourCreateModel model)
        {
            var oh = Db.Activities.OfType<OfficeHour>().SingleOrDefault(a => a.Id == model.OfficeHourId);
            var lecturer = Db.Members.FirstOrDefault(l => l.Dates.Any(d => d.Activity.Id == oh.Id));


            // Model in request umsetzen
            var day = DateTime.Parse(model.NewDate);
            var from = DateTime.Parse(model.StartTime);
            var to = DateTime.Parse(model.EndTime);

            var start = day.Add(from.TimeOfDay);
            var end = day.Add(to.TimeOfDay);


            var myCapacity = model.Type == 0 ? 1 : model.Capacity;
            var mySlotDuration = model.Type == 0 ? model.SlotDuration : 0;
            var byAgreement = (model.Type == 1);


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
                    Capacity = myCapacity,
                    FromIsRestricted = false,
                    UntilIsRestricted = (model.SubscriptionLimit > 0),
                    UntilTimeSpan =
                        (model.SubscriptionLimit > 0)
                            ? new TimeSpan(model.SubscriptionLimit-1, 59, 0)
                            : new TimeSpan?(),
                    IsCanceled = false,
                    IsMoved = false,
                }

            };

            if (mySlotDuration > 0)
            {
                var ohDuration = end - start;

                var numSlots = (int)(ohDuration.TotalMinutes / mySlotDuration + 0.01);


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
                            Capacity = myCapacity,
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
                                    Capacity = myCapacity,
                                    FromIsRestricted = false,
                                    UntilIsRestricted = (model.SubscriptionLimit > 0),
                                    UntilTimeSpan =
                                        (model.SubscriptionLimit > 0)
                                            ? new TimeSpan(model.SubscriptionLimit - 1, 59, 0)
                                            : new TimeSpan?(),
                                    IsCanceled = false,
                                    IsMoved = false,
                                }

                            };

                            if (mySlotDuration > 0)
                            {
                                var ohDuration = end - start;

                                var numSlots = (int)(ohDuration.TotalMinutes / mySlotDuration + 0.01);


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
                                            Capacity = myCapacity,
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

            return RedirectToAction("Index", new {id = model.OfficeHourId});
        }



        public ActionResult DeleteDate(Guid id)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == id);

            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == summary.Date.Id);

            var actSummary = new ActivitySummary { Activity = date.Activity };

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

            return RedirectToAction(actSummary.Action, actSummary.Controller, new { id = actSummary.Id });
        }


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

        [HttpPost]
        public ActionResult ReactivateDate(OccurrenceMailingModel model)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = false;
                Db.SaveChanges();
            }

            var m = new MailingController { ControllerContext = ControllerContext };
            return m.CustomOccurrenceMail(model);
        }

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

                    var slotDuration = (int)(ohDuration.TotalMinutes / nSlots + 0.01);

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
            }

            return RedirectToAction("Index", new { id = activityDate.Activity.Id });
        }

        public ActionResult CreateOfficeHour()
        {
            var semester = GetSemester();

            var memberService = new MemberService(Db, UserManager);
            var member = memberService.GetMember(User.Identity.Name, "FK 09");
            //var isProf = memberService.HasRole(User.Identity.Name, "FK 09", "Prof");

            var model = new OfficeHourCreateModel
            {
                Semester = semester.Name,
                //DozId = isProf ? member.ShortName : string.Empty,
                DozId = member.ShortName,
                DayOfWeek = 1,
                StartTime = "16:00",
                EndTime = "17:00",
                Capacity = 5,
                SlotDuration = 0,
                SpareSlots = 0,
                SubscriptionLimit = 0,
                DateOption = 0,
                Text = "Terminvereinbarung per E-Mail",
                Type = -1,
                DayOfWeek2 = 1,
                StartTime2 = "16:00",
                EndTime2 = "17:00",
                SubscriptionLimit2 = 0,
            };

            SetTimeSelections();
            SetRestrictionSelections();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateOfficeHour(OfficeHourCreateModel model)
        {
            var semester = GetSemester();
            
            // Model in request umsetzen
            var start = TimeSpan.Parse(model.Type == 0 ? model.StartTime2 : model.StartTime);
            var end = TimeSpan.Parse(model.Type == 0 ? model.EndTime2 : model.EndTime);

            var ohService = new OfficeHourService();

            var request = new OfficeHourCreateRequest
            {
                DozId = string.IsNullOrEmpty(model.DozId) ? new MemberService(Db, UserManager).GetMember(User.Identity.Name, "FK 09").ShortName : model.DozId,
                StartTime = start,
                EndTime = end,
                SubscriptionLimit = model.Type == 0 ? model.SubscriptionLimit2 : model.SubscriptionLimit,
                Capacity = model.Type == 0 ? 1 :  model.Capacity,
                SlotDuration = model.Type == 0 ? model.SlotDuration : 0,
                SpareSlots = model.SpareSlots,
                DayOfWeek = model.Type == 0 ? (DayOfWeek)model.DayOfWeek2 : (DayOfWeek)model.DayOfWeek,
                ByAgreement = (model.Type == 1),
                OrgId = "FK 09",
                SemesterId = semester.Name,
                CreateDates = (model.Type != 1),
            };

            var officeHour = ohService.CreateOfficeHour(request);

            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult DeleteOfficeHour()
        {
            var semester = GetSemester();

            var memberService = new MemberService(Db, UserManager);
            var member = memberService.GetMember(User.Identity.Name, "FK 09");
            var isProf = memberService.HasRole(User.Identity.Name, "FK 09", "Prof");

            var officeHour = new OfficeHourService().GetOfficeHour(member.ShortName, semester.Name);

            var model = new OfficeHourCreateModel
            {
                OfficeHourId = officeHour.Id,
                Semester = semester.Name,
                DozId = isProf ? member.ShortName : string.Empty,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteOfficeHour(OfficeHourCreateModel model)
        {
            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(act => act.Id == model.OfficeHourId);

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

                Db.SaveChanges();
            }

            // Zurück zur Übersicht
            return RedirectToAction("Index", "Dashboard");
        }

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

                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });

                    // alt: return RedirectToAction("Index", new { id = summary.Activity.Id });
                }
            }

            return RedirectToAction("Missing", "Home");
        }


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
                                writer.Write("{0};{1};{2};{3};{4};{5};{6}", date.Begin.Date.ToShortDateString(), slot.Begin.TimeOfDay.ToString(@"hh\:mm"), slot.End.TimeOfDay.ToString(@"hh\:mm"), user.LastName, user.FirstName, user.Email, sub.TimeStamp);
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

            ViewBag.SlotDurations = new SelectList(sd, "Value", "Text", "Slots a 10 min");
            ViewBag.SpareSlots = new SelectList(spare, "Value", "Text", "keine");
            ViewBag.Capacities = new SelectList(cap, "Value", "Text", "keine Beschränkung");
            ViewBag.Limits = new SelectList(limits, "Value", "Text", "keine Beschränkung");
        }
    }
}