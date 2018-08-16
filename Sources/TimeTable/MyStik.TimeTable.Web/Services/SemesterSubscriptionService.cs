using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterSubscriptionService
    {
        private readonly TimeTableDbContext _db;

        /// <summary>
        /// 
        /// </summary>
        public SemesterSubscriptionService()
        {
            _db = new TimeTableDbContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public SemesterSubscriptionService(TimeTableDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="semGroupId"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public bool IsSubscribed(string userId, Semester semester)
        {
            return _db.Subscriptions.OfType<SemesterSubscription>().Any(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id.Equals(semester.Id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="semester"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        public bool IsSubscribed(string userId, Semester semester, ActivityOrganiser org)
        {
            return _db.Subscriptions.OfType<SemesterSubscription>().Any(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id.Equals(semester.Id) &&
                s.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="semesterId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public bool IsSubscribed(string userId, Guid semesterId, Guid orgId)
        {
            return _db.Subscriptions.OfType<SemesterSubscription>().Any(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id.Equals(semesterId) &&
                s.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == orgId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupList"></param>
        /// <param name="exactFit"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupList"></param>
        /// <param name="exactFit"></param>
        /// <returns></returns>
        internal bool IsSubscribed(string userId, ICollection<OccurrenceGroup> groupList, bool exactFit = true)
        {
            foreach (var group in groupList)
            {
                if (IsSubscribed(userId, group.SemesterGroups, exactFit))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupList"></param>
        /// <param name="exactFit"></param>
        /// <returns></returns>
        internal OccurrenceGroup GetBestFit(string userId, ICollection<OccurrenceGroup> groupList, bool exactFit = true)
        {
            foreach (var group in groupList)
            {
                if (IsSubscribed(userId, group.SemesterGroups, exactFit))
                    return group;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public Curriculum GetBestCurriculum(string userId, Semester semester)
        {
            var subscription = _db.Subscriptions.OfType<SemesterSubscription>().FirstOrDefault(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id == semester.Id
                );

            return subscription?.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public SemesterGroup GetSemesterGroup(string userId, Semester semester)
        {
            return _db.SemesterGroups.FirstOrDefault(g =>
                g.Semester.Id == semester.Id &&
                g.Subscriptions.Any(s => s.UserId.Equals(userId)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="semId"></param>
        /// <returns></returns>
        public SemesterSubscription GetSubscription(string userId, Guid semId)
        {
            return _db.Subscriptions.OfType<SemesterSubscription>().FirstOrDefault(s =>
                s.UserId.Equals(userId) &&
                s.SemesterGroup.Semester.Id == semId
                );
        }
    }
}