using System;
using System.Linq;
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
        /// <summary>
        /// 
        /// </summary>
        protected IdentifyConfig.ApplicationUserManager UserManager;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
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
                        var slot = new OfficeHourSlotViewModel
                        {
                            Date = date.Begin.Date,
                            From = date.Begin.TimeOfDay,
                            Until = date.End.TimeOfDay,
                            DateOccurrenceId = date.Occurrence.Id,
                            Member = null,
                            RowCount = 1,
                            RowNo = i,
                            SubscriptionCount = 1,
                            SubscriptionNo = i,
                            // ist der aktuelle User eingetragen?
                            Occurrence = date.Occurrence,
                            ActivityDate = date,
                        };
                        model.CurrentSlots.Add(slot);


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