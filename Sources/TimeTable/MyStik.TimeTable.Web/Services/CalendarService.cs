using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Services
{
    public class CalendarService : BaseService
    {
        public CalendarService() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<ActivityDateSummary> GetActivityPlan(ApplicationUser user, DateTime startDate, DateTime endDate)
        {
            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            if (user != null)
            {
                // Was der User anbietet
                // Termine als Host
                var allMyDates = Db.ActivityDates.Where(c =>
                    c.Begin >= startDate && c.End <= endDate &&
                    c.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id))).ToList();

                foreach (var date in allMyDates)
                {
                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                }

                // 2. die gebuchten
                var myOcs = Db.Occurrences.Where(o => o.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();

                var ac = new ActivityService();

                foreach (var occ in myOcs)
                {
                    var summary = ac.GetSummary(occ.Id);

                    var dates = summary.GetDates(startDate, endDate);

                    foreach (var date in dates)
                    {
                        if (!dateMap.ContainsKey(date.Id))
                        {
                            var dateSummary = new ActivityDateSummary(date, ActivityDateType.Subscription);
                            if (summary is ActivitySlotSummary slotSummary)
                            {
                                dateSummary.Slot = slotSummary.Slot;
                            }
                            dateMap[date.Id] = dateSummary;

                        }
                    }
                }
            }

            return dateMap.Values.ToList();
        }

    }
}