using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class WpmService
    {

        internal ICollection<Course> GetAllLottryCourses(Semester semester)
        {
            var Db = new TimeTableDbContext();

            return Db.Activities.OfType<Course>().Where(c => 
                c.Occurrence.LotteryEnabled &&
                c.SemesterGroups.Any(g => g.Semester.Id == semester.Id)).ToList();
        }

        internal int GetNextPriority(Semester semester, string userId)
        {
            var Db = new TimeTableDbContext();

            // Alle WPM-Subscriptions für den User holen und nach Prio und Datum sortieren

            // alle Subscriptions durchgehen und Reihenfolge prüfen

            // ggf. Prio setzen

            // neue Prio ermitteln
            var wpmList = Db.Activities.OfType<Course>().Where(a =>
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(userId)) &&
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.SemesterGroups.Any(g => g.CapacityGroup.CurriculumGroup.Name.Equals("WPM"))).ToList();

            var subList = new List<SubscriptionCourseViewModel>();
            foreach (var wpm in wpmList)
            {
                // es könnten ja auch mehrere Eintragungen vorliegen
                var wpmsubs = wpm.Occurrence.Subscriptions.Where(s => s.UserId.Equals(userId));

                foreach (var subscription in wpmsubs)
                {
                    subList.Add(new SubscriptionCourseViewModel
                    {
                        SubscriptionId = subscription.Id,
                        Course = wpm,
                        Priority = subscription.Priority.HasValue ? subscription.Priority.Value : 999,
                        SubscriptionTotalCount = wpm.Occurrence.Subscriptions.Count,
                        SubscriptionPrio1Count = wpm.Occurrence.Subscriptions.Count(s => s.Priority == 1),
                        SubscriptionPrio2Count = wpm.Occurrence.Subscriptions.Count(s => s.Priority == 2),
                        Capacity = wpm.Occurrence.Capacity,
                        SubscriptionDateTime = subscription.TimeStamp,
                    });
                }
            }

            var orderdSubList = subList.OrderBy(item => item.Priority).ThenBy(item => item.SubscriptionDateTime);

            // Hypothese: alle sind nach Prio und ohne Löcher geordnet
            // zur Sicherheit hier alles durchgehen!
            var prio = 1;
            foreach (var item in orderdSubList)
            {
                if (item.Priority == 999 || item.Priority != prio)
                {
                    var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(s => s.Id == item.SubscriptionId);
                    subscription.Priority = prio;
                    Db.SaveChanges();
                }
                prio++;
            }

            return orderdSubList.Count() + 1;
        }

        internal int GetMaxConfiremd(Semester semester)
        {
            if (DateTime.Today < semester.StartCourses.AddDays(7))
                return 2;
            return 5;
        }
    }
}