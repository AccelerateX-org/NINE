using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.CRUD
{
    internal class ActivityDeleteService
    {
        private TimeTableDbContext _db;

        internal ActivityDeleteService(TimeTableDbContext db)
        {
            _db = db;
        }

        internal bool DeleteActivity(Guid activityId)
        {
            var activity = _db.Activities.SingleOrDefault(a => a.Id == activityId);
            if (activity == null)
            {
                return false;
            }

            // Entfernen von Abhängigkeiten, falls vorhanden
            DeleteActivity(activity);

            _db.SaveChanges();
            return true;
        }

        private void DeleteActivity(Activity activity)
        {
            // alle termine durchgehen
            foreach (var date in activity.Dates.ToList())
            {
                foreach (var slot in date.Slots.ToList())
                {
                    DeleteOccurrence(slot.Occurrence);
                    date.Slots.Remove(slot);
                    _db.ActivitySlots.Remove(slot);
                }

                foreach (var change in date.Changes.ToList())
                {
                    foreach (var changeNotificationState in change.NotificationStates.ToList())
                    {
                        change.NotificationStates.Remove(changeNotificationState);
                        _db.NotificationStates.Remove(changeNotificationState);
                    }

                    date.Changes.Remove(change);
                    _db.DateChanges.Remove(change);
                }

                DeleteOccurrence(date.Occurrence);

                foreach (var roomBooking in date.RoomBookings.ToList())
                {
                    foreach (var confirmation in roomBooking.Confirmations.ToList())
                    {
                        _db.BookingConfirmations.Remove(confirmation);
                    }
                    _db.RoomBookings.Remove(roomBooking);
                }

                date.Hosts.Clear();
                date.Rooms.Clear();

                foreach (var vRoom in date.VirtualRooms.ToList())
                {
                    _db.VirtualRoomAccesses.Remove(vRoom);
                }


                activity.Dates.Remove(date);
                _db.ActivityDates.Remove(date);
            }

            if (activity.Occurrence != null)
            {
                DeleteOccurrence(activity.Occurrence);
            }

            foreach (var activityOwner in activity.Owners.ToList())
            {
                _db.ActivityOwners.Remove(activityOwner);
            }


            var teachingDescriptions = _db.SubjectTeachings.Where(x => x.Course.Id == activity.Id).ToList();
            foreach (var teachingDescription in teachingDescriptions)
            {
                _db.SubjectTeachings.Remove(teachingDescription);
            }


            var teachings = _db.SubjectTeachings.Where(x => x.Course.Id == activity.Id).ToList();
            foreach (var teaching in teachings)
            {
                _db.SubjectTeachings.Remove(teaching);
            }

            _db.Activities.Remove(activity);
        }

        private void DeleteOccurrence(Occurrence occ)
        {
            if (occ == null)
                return;

            // Occurrence Groups löschen
            var occGroups = occ.Groups.ToList();
            occGroups.ForEach(g => _db.OccurrenceGroups.Remove(g));

            // Alle Eintragungen des Kurses
            var subs = occ.Subscriptions.ToList();

            foreach (var subscription in subs)
            {
                var allDrawings = _db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
                foreach (var drawing in allDrawings)
                {
                    _db.SubscriptionDrawings.Remove(drawing);
                }


                var allBets = _db.LotteriyBets.Where(x => x.Subscription.Id == subscription.Id).ToList();
                foreach (var lotteryBet in allBets)
                {
                    _db.LotteriyBets.Remove(lotteryBet);
                }

            }
            subs.ForEach(s => _db.Subscriptions.Remove(s));

            var drawings = _db.OccurrenceDrawings.Where(x => x.Occurrence.Id == occ.Id).ToList();
            foreach (var drawing in drawings)
            {
                _db.OccurrenceDrawings.Remove(drawing);
            }

            // Aus Lotterien austragen
            foreach (var lottery in occ.Lotteries.ToList())
            {
                lottery.Occurrences.Remove(occ);
            }

            // Platzkontingente löschen
            foreach (var quota in occ.SeatQuotas.ToList())
            {
                foreach (var fraction in quota.Fractions.ToList())
                {
                    if (fraction.ItemLabelSet != null)
                    {
                        _db.ItemLabelSets.Remove(fraction.ItemLabelSet);
                    }
                    _db.SeatQuotaFractions.Remove(fraction);
                }

                if (quota.ItemLabelSet != null)
                {
                    _db.ItemLabelSets.Remove(quota.ItemLabelSet);
                }

                _db.SeatQuotas.Remove(quota);
            }

            // jetzt die Ocurrence wegwerfen
            _db.Occurrences.Remove(occ);
        }

    }
}
