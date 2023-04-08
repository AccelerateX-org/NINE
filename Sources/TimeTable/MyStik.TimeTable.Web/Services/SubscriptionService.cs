using System;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    public class SubscriptionService : BaseService
    {
        public SubscriptionService(TimeTableDbContext db) : base(db)
        {
        }

        public OccurrenceSubscription GetSubscription(Guid occId, string userId)
        {
            return Db.Subscriptions.OfType<OccurrenceSubscription>()
                .FirstOrDefault(x => x.Occurrence.Id == occId && x.UserId.Equals(userId));
        }

        public void DeleteSubscription(OccurrenceSubscription subscription)
        {
            var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
            foreach (var drawing in allDrawings)
            {
                Db.SubscriptionDrawings.Remove(drawing);
            }

            var bets = subscription.Bets.ToList();
            foreach (var bet in bets)
            {
                Db.LotteriyBets.Remove(bet);
            }

            Db.Subscriptions.Remove(subscription);

            Db.SaveChanges();
        }
    }
}