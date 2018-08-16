using System;
using System.Collections.Generic;
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

        internal OrganiserMember GetHost(OfficeHour officeHour)
        {
            return officeHour.Owners.First().Member;
        }


        internal OfficeHourDatePreviewModel GetPreviewNextDate(OfficeHour officeHour)
        {
            var model = new OfficeHourDatePreviewModel()
            {
                OfficeHour = officeHour,
            };

            DateTime x = DateTime.Now;

            // nächster Termin => endet in der Zukunft
            var nextDate = officeHour.Dates.Where(d => d.End >= x && d.Occurrence.IsAvailable)
                .OrderBy(d => d.Begin)
                .FirstOrDefault();

            model.Date = nextDate;

            if (nextDate == null)
                return model;

            var date = nextDate;
            model.Subscriptions.AddRange(GetSubscriptions(date));

            return model;
        }




        internal OfficeHourDateViewModel GetNextSubscription(OfficeHour officeHour, string userId)
        {
            OfficeHourDateViewModel ohm = new OfficeHourDateViewModel();
            DateTime x = DateTime.Now;

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

        /// <summary>
        /// Liefert alle Eintragungen eines Users in einer Sprechstunde
        /// </summary>
        /// <param name="officeHour"></param>
        /// <param name="userId"></param>
        internal void GetSubscriptions(OfficeHour officeHour, string userId)
        {
            
        }

        /// <summary>
        /// Liste aller Termine aus Sicht eines Users
        /// </summary>
        /// <param name="officeHour"></param>
        /// <param name="userId"></param>
        internal ICollection<OfficeHourDateViewModel> GetDates(OfficeHour officeHour, string userId)
        {
            var allDates = officeHour.Dates.OrderBy(x => x.Begin).ToList();

            var model = new List<OfficeHourDateViewModel>();

            foreach (var date in allDates)
            {
                var dateModel = new OfficeHourDateViewModel();
                dateModel.OfficeHour = officeHour;
                dateModel.Date = date;

                // Weiche Slots / kein Slots
                if (date.Slots.Any())
                {
                    // bin ich im Slot eingetragen?
                    var slot = date.Slots.FirstOrDefault(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId)));
                    if (slot != null)
                    {
                        dateModel.Slot = slot;
                        dateModel.Subscription = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userId));
                    }
                }
                else
                {
                    dateModel.Subscription = date.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userId));
                }

                // Ergebnisse
                // bin eingetragen (ja/nein) => ist über die Subscription gekennzeichnet
                // darf mich eintragen (ja/nein)
                // Ende der Einschreibeperiode
                dateModel.EndOfSubscriptionPeriod = DateTime.MaxValue;
                if (date.Occurrence.UntilIsRestricted && date.Occurrence.UntilTimeSpan.HasValue)
                {
                    dateModel.EndOfSubscriptionPeriod =
                            date.Begin.AddHours(-date.Occurrence.UntilTimeSpan.Value.Hours)
                                .AddMinutes(-date.Occurrence.UntilTimeSpan.Value.Minutes);
                }



                // ja
                // Liste der verfügbaren Slots bzw. Plätze
                if (date.Slots.Any())
                {
                    dateModel.AvailableSlots.AddRange(date.Slots.Where(x => x.Occurrence.Subscriptions.Count == 0 && x.Occurrence.IsAvailable));
                }
                else
                {
                    dateModel.AvailableSeats = date.Occurrence.Capacity <= 0 ? 1 : date.Occurrence.Capacity - date.Occurrence.Subscriptions.Count;
                }

                model.Add(dateModel);
            }

            return model;
        }


        /// <summary>
        /// Liste aller Termine aus Sicht eines Users
        /// </summary>
        /// <param name="officeHour"></param>
        /// <param name="userId"></param>
        internal ICollection<OfficeHourDateViewModel> GetDates(OfficeHour officeHour)
        {
            var allDates = officeHour.Dates.OrderBy(x => x.Begin).ToList();

            var model = new List<OfficeHourDateViewModel>();
            var now = DateTime.Now;

            foreach (var date in allDates)
            {
                var dateModel = new OfficeHourDateViewModel();
                dateModel.OfficeHour = officeHour;
                dateModel.Date = date;

                // Ende der Einschreibeperiode
                dateModel.EndOfSubscriptionPeriod = DateTime.MaxValue;
                if (date.Occurrence.UntilIsRestricted && date.Occurrence.UntilTimeSpan.HasValue)
                {
                    dateModel.EndOfSubscriptionPeriod =
                        date.Begin.AddHours(-date.Occurrence.UntilTimeSpan.Value.Hours)
                            .AddMinutes(-date.Occurrence.UntilTimeSpan.Value.Minutes);
                }

                // ja
                // Liste der verfügbaren Slots bzw. Plätze
                if (date.Slots.Any())
                {
                    dateModel.AvailableSlots.AddRange(date.Slots.Where(x => x.Occurrence.Subscriptions.Count == 0 && x.Occurrence.IsAvailable));

                    foreach (var slot in date.Slots)
                    {
                        dateModel.Subscriptions.AddRange(slot.Occurrence.Subscriptions);
                    }
                }
                else
                {
                    dateModel.AvailableSeats = date.Occurrence.Capacity <= 0 ? 1 : date.Occurrence.Capacity - date.Occurrence.Subscriptions.Count;
                    dateModel.Subscriptions.AddRange(date.Occurrence.Subscriptions);
                }

                model.Add(dateModel);
            }

            return model;
        }


        internal bool HasSubscription(ActivityDate date, string userId)
        {
            if (date.Slots.Any())
            {
                return date.Slots.Any(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId)));
            }

            return date.Occurrence.Subscriptions.Any(x => x.UserId.Equals(userId));
        }

        internal OccurrenceSubscription GetSubscription(ActivityDate date, string userId)
        {
            if (date.Slots.Any())
            {
                foreach (var slot in date.Slots)
                {
                    var sub = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userId));
                    if (sub != null)
                        return sub;
                }
                return null;
            }

            return date.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userId));
        }

        internal ActivitySlot GetSubscribedSlot(ActivityDate date, string userId)
        {
            if (!date.Slots.Any())
                return null;

            return date.Slots.FirstOrDefault(x => x.Occurrence.Subscriptions.Any(y => y.UserId.Equals(userId)));
        }

        internal ICollection<OccurrenceSubscription> GetSubscriptions(ActivityDate date)
        {
            var list = new List<OccurrenceSubscription>();

            if (date.Slots.Any())
            {
                foreach (var slot in date.Slots)
                {
                    list.AddRange(slot.Occurrence.Subscriptions);
                }
            }

            list.AddRange(date.Occurrence.Subscriptions);

            return list;
        }

        internal bool IsExpired(ActivityDate date)
        {
            if (!date.Occurrence.UntilIsRestricted)
            {
                return date.Begin < DateTime.Now;
            }


            if (date.Occurrence.UntilTimeSpan.HasValue)
            {
                var endOfSubscriptionPeriod =
                    date.Begin.AddHours(-date.Occurrence.UntilTimeSpan.Value.Hours)
                        .AddMinutes(-date.Occurrence.UntilTimeSpan.Value.Minutes);

                return endOfSubscriptionPeriod < DateTime.Now;
            }

            return false;
        }
    }
}