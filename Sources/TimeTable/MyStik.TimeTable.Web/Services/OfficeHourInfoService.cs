using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// Ermittelt freie Termine
    /// </summary>
    public class OfficeHourInfoService
    {
        protected IdentifyConfig.ApplicationUserManager UserManager;


        public OfficeHourInfoService(IdentifyConfig.ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }


        internal OfficeHourDatePreviewModel GetPreviewNextDate(OfficeHour officeHour)
        {
            var model = new OfficeHourDatePreviewModel()
            {
                OfficeHour = officeHour,
            };

            DateTime x = GlobalSettings.Now;

            // nächster Termin => endet in der Zukunft
            var nextDate = officeHour.Dates.Where(d => d.End >= x && d.Occurrence.IsAvailable)
                .OrderBy(d => d.Begin)
                .FirstOrDefault();

            model.Date = nextDate;

            if (nextDate == null)
                return model;


            var date = nextDate;

            if (date.Slots.Any())
            {
                #region Slots

                OfficeHourSlotViewModel firstSlotOnDate = null;
                var i = 1;

                // Slots
                foreach (var ohSlot in date.Slots)
                {
                    if (ohSlot.Occurrence.Subscriptions.Any())
                    {
                        #region Slot mit Eintragungen

                        OfficeHourSlotViewModel firstSlotOnSlot = null;
                        var j = 1;
                        var nSub = 0;

                        foreach (
                            var subscription in ohSlot.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
                        {
                            var user = UserManager.FindById(subscription.UserId);

                            // pro Subscription eine Zeile

                            var slot = new OfficeHourSlotViewModel
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
                                Occurrence = ohSlot.Occurrence,
                                ActivityDate = ohSlot.ActivityDate,
                            };

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
                        firstSlotOnSlot.SubscriptionCount = j - 1;

                        #endregion
                    }
                    else
                    {
                        #region Slot ohne Eintragungen - erst mal nicht
                        #endregion
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
                            // ist der aktuelle User eingetragen?
                            Occurrence = date.Occurrence,
                            ActivityDate = date,
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

                #endregion
            }


            return model;
        }


        /*
        internal OfficeHourDateViewModel GetNextAvailableDate(OfficeHour officeHour)
        {
            var activityService = new ActivityService();


            OfficeHourDateViewModel ohm = null;
            DateTime x = GlobalSettings.Now;

            while (ohm == null)
            {
                var nextDate = officeHour.Dates.Where(d => d.Begin >= x && d.Occurrence.IsAvailable)
                    .OrderBy(d => d.Begin)
                    .FirstOrDefault();

                // kein weiterer Termin mehr
                if (nextDate == null)
                {
                    ohm = new OfficeHourDateViewModel();

                    // Damit man weiss zum wem die Sprechstunde gehört der Versuch, einen termin zu finden
                    var firstDate = officeHour.Dates.FirstOrDefault();
                    if (firstDate != null)
                    {
                        ohm.Lecturer = firstDate.Hosts.FirstOrDefault();
                    }

                    break;
                }
                else
                {
                    // keine Slots
                    if (!nextDate.Slots.Any())
                    {
                        var state = activityService.GetActivityState(nextDate.Occurrence, null);

                        // kein Fehler => Termin verfügbar
                        if (!state.HasError)
                        {
                            ohm = new OfficeHourDateViewModel();
                            ohm.Date = nextDate;
                            ohm.Lecturer = nextDate.Hosts.FirstOrDefault();
                            ohm.State = state;
                            break;
                        }
                    }
                    else
                    {
                        // Jeden Slot durchsuchen
                        var slots = nextDate.Slots.OrderBy(s => s.Begin).ToList();
                        foreach (var slot in slots)
                        {
                            var state = activityService.GetActivityState(slot.Occurrence, null);

                            // kein Fehler => Termin verfügbar
                            if (!state.HasError && slot.Occurrence.IsAvailable)
                            {
                                ohm = new OfficeHourDateViewModel();
                                ohm.Date = nextDate;
                                ohm.Lecturer = nextDate.Hosts.FirstOrDefault();
                                ohm.Slot = slot;
                                ohm.State = state;
                                break;
                            }
                        }
                    }
                    // den nächsten Termin durchsuchen
                    x = nextDate.End;
                }
            }

            ohm.OfficeHour = officeHour;


            return ohm;
        }
         */


        internal OfficeHourDateViewModel GetNextSubscription(OfficeHour officeHour, string userId)
        {
            OfficeHourDateViewModel ohm = new OfficeHourDateViewModel();
            DateTime x = GlobalSettings.Now;

            // das nächste Datum
            var nextDate = officeHour.Dates.Where(d => 
                d.Begin >= x &&                                                         // in der Zukunft
                (d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId)) ||       // im Date eingeschrieben
                 d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId))))) // in irgendeinem Slot eingeschrieben
                .OrderBy(d => d.Begin)
                .FirstOrDefault();

            if (nextDate != null)
            {
                ohm.Date = nextDate;
                ohm.Lecturer = nextDate.Hosts.FirstOrDefault();

                // noch schauen, ob es eine Eintragung in einem Slot ist
                var nextSlot = 
                nextDate.Slots.Where(slot =>
                    slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId)))
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();

                ohm.Slot = nextSlot;
            }
            else
            {
                var firstDate = officeHour.Dates.FirstOrDefault();
                if (firstDate != null)
                {
                    ohm.Lecturer = firstDate.Hosts.FirstOrDefault();
                }
            }


            ohm.OfficeHour = officeHour;


            return ohm;
        }
        
    }
}