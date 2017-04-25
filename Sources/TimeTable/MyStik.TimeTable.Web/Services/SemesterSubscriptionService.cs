using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    public class SemesterSubscriptionService
    {
        private readonly TimeTableDbContext _db = new TimeTableDbContext();


        public void Subscribe(string userId, Guid semGroupId)
        {
            var group = _db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);

            if (group == null)
                return;

            // es darf nur eine Semesterzugehörigkeit geben
            var currentSubscription = _db.Subscriptions.OfType<SemesterSubscription>().SingleOrDefault(x =>
                x.UserId.Equals(userId) &&
                x.SemesterGroup.Semester.Id == group.Semester.Id);

            if (currentSubscription != null)
            {
                currentSubscription.SemesterGroup.Subscriptions.Remove(currentSubscription);
                _db.Subscriptions.Remove(currentSubscription);
            }

            var subscription = new SemesterSubscription
            {
                TimeStamp = DateTime.Now,
                UserId = userId,
                LikesEMails = false,
                LikesNotifications = false,
                SemesterGroup = group
            };

            group.Subscriptions.Add(subscription);
            _db.Subscriptions.Add(subscription);

            _db.SaveChanges();
        }

        public bool IsSubscribed(string userId, Semester semester)
        {
            return _db.Subscriptions.OfType<SemesterSubscription>().Any(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id.Equals(semester.Id));
        }

        public bool IsSubscribed(string userId, ICollection<SemesterGroup> groupList, bool exactFit = true)
        {
            if (exactFit)
            {
                foreach (var group in groupList)
                {
                    var res = _db.Subscriptions.OfType<SemesterSubscription>().Any(s =>
                        s.UserId.Equals(userId) &&
                        s.SemesterGroup.Id == group.Id);
                    if (res)
                        return true;
                }
            }
            else
            {
                foreach (var group in groupList)
                {
                    var res = _db.Subscriptions.OfType<SemesterSubscription>().Any(s =>
                        s.UserId.Equals(userId) &&
                        s.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Id == group.CapacityGroup.CurriculumGroup.Curriculum.Id);
                    if (res)
                        return true;
                }
            }

            return false;
        }


        internal bool IsSubscribed(string userId, ICollection<OccurrenceGroup> groupList, bool exactFit = true)
        {
            foreach (var group in groupList)
            {
                if (IsSubscribed(userId, group.SemesterGroups, exactFit))
                    return true;
            }
            return false;
        }


        internal OccurrenceGroup GetBestFit(string userId, ICollection<OccurrenceGroup> groupList, bool exactFit = true)
        {
            foreach (var group in groupList)
            {
                if (IsSubscribed(userId, group.SemesterGroups, exactFit))
                    return group;
            }
            return null;
        }

        internal Curriculum GetBestCurriculum(string userId, Semester semester)
        {
            var subscription = _db.Subscriptions.OfType<SemesterSubscription>().FirstOrDefault(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id == semester.Id
                );

            if (subscription != null)
                return subscription.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum;
            return null;
        }

        internal List<SemesterGroup> GetSemesterGroups(string userId, Semester semester)
        {
            return _db.SemesterGroups.Where(g =>
                g.Semester.Id == semester.Id &&
                g.Subscriptions.Any(s => s.UserId.Equals(userId))).ToList();
        }

        internal SemesterSubscription GetSubscription(string userId, Guid semId)
        {
            return _db.Subscriptions.OfType<SemesterSubscription>().FirstOrDefault(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id == semId
                );
        }
    }
}