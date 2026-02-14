using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class OccurrenceService
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();

        /// <summary>
        /// 
        /// </summary>
        protected IdentifyConfig.ApplicationUserManager UserManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        public OccurrenceService(IdentifyConfig.ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public SubscriptionResponse SubscribeOccurrence(Guid occId, ApplicationUser user)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(ac => ac.Id == occId);
            if (occurrence == null || user == null)
            {
                return GetErrorResponse("Missing Or Wrong Occurrence Or Missing User", false);
            }

            // Nach einer bestehenden Eintragung suchen
            // sollte aus irgendeinem Grund bereits mehrere Eintragungen bestehen, dann ist das hier nicht von Interesse
            // es genügt die Information, dass bereits eine Eintragung existiert
            OccurrenceSubscription subscription = occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(user.Id));
            if (subscription != null)
            {
                return new SubscriptionResponse
                {
                    HasError = false,
                    Subscription = subscription
                };
            }

            #region Zeitraum prüfen
            var timingState = GetSubscriptionState(occurrence);


            if (timingState == SubscriptionState.BeforeSubscriptionPhase)
            {
                return new SubscriptionResponse
                {
                    HasError = true,
                    ShowUser = true,
                    Message = string.Format("Noch zu früh! Eintragung erst ab {0} möglich", occurrence.FromDateTime.Value),
                };
            }
            
            if (timingState == SubscriptionState.AfterSubscriptionPhase ||
                        timingState == SubscriptionState.DuringOccurrence ||
                        timingState == SubscriptionState.AfterOccurrence)
            {
                return new SubscriptionResponse
                {
                    HasError = true,
                    ShowUser = true,
                    Message = "Zu spät! Keine Eintragung mehr möglich!",
                };
            }
            #endregion

            #region Alles auf Warteliste: Inaktiv gesetzt oder Platzverlosung

            var hasLottery = Db.Lotteries.Any(x => x.Occurrences.Any(y => y.Id == occurrence.Id));

            if (occurrence.IsAvailable == false || hasLottery)
            {
                subscription = AddOnWaitingList(occurrence, user);
                return new SubscriptionResponse
                {
                    HasError = false,
                    Subscription = subscription
                };
            }

            #endregion


            #region Gruppen berücksichtigen

            if (occurrence.UseGroups)
            {
                var semSubService = new SemesterSubscriptionService();

                // die beste Gruppe ermitteln => die erste, die passt
                var group = semSubService.GetBestFit(user.Id, occurrence.Groups);

                // Wenn der Benutzer nicht zur erforderlichen Gruppe gehört
                // dan automatisch auf der Warteliste positionieren
                if (group == null)
                {
                    subscription = AddOnWaitingList(occurrence, user);
                    return new SubscriptionResponse
                    {
                        HasError = false,
                        Subscription = subscription
                    };
                }

                // als nächstes die Anzahl der Teilnehmer in dieser Gruppe ermitteln
                var participants = GetSubscriptionsForGroup(occurrence.Subscriptions, group);
                
                // Es ist noch Platz in der Gruppe
                if (participants.Count < group.Capacity)
                {
                    subscription = AddOnParticipantList(occurrence, user);
                    return new SubscriptionResponse
                    {
                        HasError = false,
                        Subscription = subscription
                    };
                }

                // Alle Plätze in der Gruppe sind belegt
                // auf die Warteliste
                subscription = AddOnWaitingList(occurrence, user);
                return new SubscriptionResponse
                {
                    HasError = false,
                    Subscription = subscription
                };
            }
            else
            {
                // keine Gruppen berücksichtigen
                // und es interessiert hier nicht die Annahme des Platzes!
                var nParticipants = occurrence.Subscriptions.Count(s => s.OnWaitingList == false);
                if (occurrence.Capacity < 0 || nParticipants < occurrence.Capacity)
                {
                    subscription = AddOnParticipantList(occurrence, user);
                    return new SubscriptionResponse
                    {
                        HasError = false,
                        Subscription = subscription
                    };
                }

                
                // Auf Warteliste setzen
                // Spezialfälle
                // Bei einem Slot oder Datum (Sprechstunde) gibt es keine Wartelisten
                var summary = new ActivityService().GetSummary(occurrence.Id);
                if (summary is ActivitySlotSummary || summary is ActivityDateSummary)
                {
                    return new SubscriptionResponse
                    {
                        HasError = true,
                        ShowUser = true,
                        Message = "Leider bereits belegt",
                    };
                }
                else
                {
                    subscription = AddOnWaitingList(occurrence, user);
                    return new SubscriptionResponse
                    {
                        HasError = false,
                        Subscription = subscription
                    };
                }
            }

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allSubscriptions"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<OccurrenceSubscription> GetSubscriptionsForGroup(ICollection<OccurrenceSubscription> allSubscriptions, OccurrenceGroup group)
        {
            var semSubService = new SemesterSubscriptionService();

            var subList = new List<OccurrenceSubscription>();

            foreach (var subscription in allSubscriptions)
            {
                var user = UserManager.FindById(subscription.UserId);
                if (user != null)
                {
                    bool isInGroup = semSubService.IsSubscribed(user.Id, group.SemesterGroups, group.Occurrence.UseExactFit);
                    
                    if (isInGroup)
                    {
                        subList.Add(subscription);
                    }
                }
            }
            
            return subList;
        }

        /// <summary>
        /// Ermittelt die Teilnehmer aus einem Studiengang
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="curriculum"></param>
        /// <returns></returns>
        public List<OccurrenceSubscription> GetParticipiantList(Occurrence occurrence, string curriculum)
        {
            var participantList = occurrence.Subscriptions.Where(s => s.OnWaitingList == false && s.IsConfirmed).ToList();

            if (string.IsNullOrEmpty(curriculum))
                return participantList;

            return GetSubListByCurriculum(participantList, curriculum);
        }


        /// <summary>
        /// Ermittelt die Teilnehmer aus einem Studiengang
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="curriculum"></param>
        /// <returns></returns>
        public List<OccurrenceSubscription> GetPendingList(Occurrence occurrence, string curriculum)
        {
            var participantList = occurrence.Subscriptions.Where(s => s.OnWaitingList == false && !s.IsConfirmed).ToList();

            if (string.IsNullOrEmpty(curriculum))
                return participantList;

            return GetSubListByCurriculum(participantList, curriculum);
        }

        /// <summary>
        /// Warteliste nach Studiengang
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="curriculum"></param>
        /// <param name="minLapCount"></param>
        /// <returns></returns>
        public List<OccurrenceSubscription> GetWaitingList(Occurrence occurrence, string curriculum, int minLapCount)
        {
            var participantList = occurrence.Subscriptions.Where(s => s.OnWaitingList == true && s.LapCount >= minLapCount).ToList();

            if (string.IsNullOrEmpty(curriculum))
                return participantList;

            return GetSubListByCurriculum(participantList, curriculum);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrence"></param>
        /// <returns></returns>
        public int GetParticipiantCount(Occurrence occurrence)
        {
            return occurrence.Subscriptions.Count(s => s.OnWaitingList == false && s.IsConfirmed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrence"></param>
        /// <returns></returns>
        public int GetPendingCount(Occurrence occurrence)
        {
            return occurrence.Subscriptions.Count(s => s.OnWaitingList == false && !s.IsConfirmed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrence"></param>
        /// <returns></returns>
        public int GetWaitingCount(Occurrence occurrence)
        {
            return occurrence.Subscriptions.Count(s => s.OnWaitingList == true);
        }


        private List<OccurrenceSubscription> GetSubListByCurriculum(List<OccurrenceSubscription> participantList, string curriculum)
        {
            var subList = new List<OccurrenceSubscription>();

            // jetzt nur die Teilnehmer nach Studiengang raussuchen
            foreach (var subscription in participantList)
            {
                // entscheidend ist die letzte Eintragung!
                var semSub = Db.Subscriptions.OfType<SemesterSubscription>().OrderByDescending(s => s.SemesterGroup.Semester.StartCourses).FirstOrDefault(s => s.UserId.Equals(subscription.UserId));
                if (semSub != null && semSub.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(curriculum))
                {
                    subList.Add(subscription);
                }
            }

            return subList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="curriculum"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public int GetParticipiantCount(Occurrence occurrence, string curriculum, Semester semester)
        {
            var subList = occurrence.Subscriptions.Where(s => s.OnWaitingList == false && s.IsConfirmed).ToList();
            return GetSubListByCurriculum(subList, curriculum, semester).Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="curriculum"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public int GetPendingCount(Occurrence occurrence, string curriculum, Semester semester)
        {
            var subList = occurrence.Subscriptions.Where(s => s.OnWaitingList == false && !s.IsConfirmed).ToList();
            return GetSubListByCurriculum(subList, curriculum, semester).Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="curriculum"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public int GetWaitingCount(Occurrence occurrence, string curriculum, Semester semester)
        {
            var subList = occurrence.Subscriptions.Where(s => s.OnWaitingList == true).ToList();
            return GetSubListByCurriculum(subList, curriculum, semester).Count;
        }


        private List<OccurrenceSubscription> GetSubListByCurriculum(List<OccurrenceSubscription> participantList, string curriculum, Semester semester)
        {
            var subList = new List<OccurrenceSubscription>();

            // jetzt nur die Teilnehmer nach Studiengang raussuchen
            foreach (var subscription in participantList)
            {
                // liegt eine Eintragug im Semester vor, im angefragten Studiengang
                var semSub = Db.Subscriptions.OfType<SemesterSubscription>().SingleOrDefault(s => s.SemesterGroup.Semester.Id == semester.Id && s.UserId.Equals(subscription.UserId));
                if (semSub != null && semSub.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(curriculum))
                {
                    subList.Add(subscription);
                }
            }

            return subList;
        }


        /// <summary>
        /// Anzahl der verfügbaren Plätze im Studiengang
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="model"></param>
        /// <param name="curriculum"></param>
        /// <param name="lapCount"></param>
        /// <returns></returns>
        public void GetCapacity(Occurrence occurrence, WPMSubscriptionModel model, string curriculum, int lapCount)
        {
            model.IsRestricted = true;
            if (occurrence.UseGroups == false)
            {
                if (occurrence.Capacity == -1)
                {
                    model.IsRestricted = false;
                    return;
                }

                var parList = GetParticipiantList(occurrence, string.Empty);
                var wl = GetWaitingList(occurrence, string.Empty, lapCount);
                var pl = GetPendingList(occurrence, string.Empty);

                model.Capacity = occurrence.Capacity;
                model.Participients = parList.Count;
                model.Waiting = wl.Count;
                model.Pending = pl.Count;
                model.Free = model.Capacity - model.Participients - model.Pending;

                return;
            }

            // Gruppen
            
            var group = occurrence.Groups.SingleOrDefault(
                g => g.SemesterGroups.Any(s => s.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals(curriculum)));

            if (group == null)
            {
                // nimmt nicht an der Verlosung teil
                return;
            }

            var partList = GetParticipiantList(occurrence, curriculum);
            var waitingList = GetWaitingList(occurrence, curriculum, lapCount);
            var pendingList = GetPendingList(occurrence, curriculum);

            model.Capacity = group.Capacity;
            model.Participients = partList.Count;
            model.Waiting = waitingList.Count;
            model.Pending = pendingList.Count;
            model.Free = model.Capacity - model.Participients - model.Pending;

            return;
        }



        private OccurrenceSubscription AddOnParticipantList(Occurrence occurrence, ApplicationUser user)
        {
            var subscription = new OccurrenceSubscription
            {
                UserId = user.Id,
                TimeStamp = DateTime.Now,
                OnWaitingList = false,
                IsConfirmed = true
            };


            occurrence.Subscriptions.Add(subscription);

            /*
            var log = new ActivitySubscriptionLog
            {
                Timestamp = DateTime.Now,
                OccurrenceId = occurrence.Id,
                SubsscriberUserId = user.Id,
                SubscriptionTimeStamp = subscription.TimeStamp,
                ActorUserId = user.Id,
                Action = SubscriptionLogAction.Subscribe,
                State = SubscriptionLogState.Participant,
                Remark = "Über Standarddienst"
            };
            */
            //Db.ActivitySubscriptionLogs.Add(log);


            Db.SaveChanges();

            return subscription;
        }

        private OccurrenceSubscription AddOnWaitingList(Occurrence occurrence, ApplicationUser user)
        {
            var subscription = new OccurrenceSubscription
            {
                UserId = user.Id,
                TimeStamp = DateTime.Now,
                OnWaitingList = true,
                IsConfirmed = false
            };

            occurrence.Subscriptions.Add(subscription);

            /*
            var log = new ActivitySubscriptionLog
            {
                Timestamp = DateTime.Now,
                OccurrenceId = occurrence.Id,
                SubsscriberUserId = user.Id,
                SubscriptionTimeStamp = subscription.TimeStamp,
                ActorUserId = user.Id,
                Action = SubscriptionLogAction.Subscribe,
                State = SubscriptionLogState.WaitingList,
                Remark = "Über Standarddienst"
            };
            */
            //Db.ActivitySubscriptionLogs.Add(log);

            Db.SaveChanges();

            return subscription;
        }

        private SubscriptionState GetSubscriptionState(Occurrence occurrence)
        {
            var activityService = new ActivityService();
            var summary = activityService.GetSummary(occurrence);

            // der State alleine steuert die Anzeige der "Subscribe"-Buttons
            // dieser wird nur angezeigt, wenn State den Wert "DuringSubscriptionPhase" hat
            if (summary is ActivitySummary)
            {
                // es handelt sich um die Eintragung in eine Akivität an sich, d.h. etwas das keine Termine hat
                // daher geht dies immer (bisher)
                // Option: wenn die Occurrence eine absolute Beschränkung hat, dann kann man das hier überprüfen
                return SubscriptionState.DuringSubscriptionPhase;
            }
            
            if (summary is ActivityDateSummary)
            {
                var dateSummary = summary as ActivityDateSummary;
                return activityService.GetSubscriptionState(occurrence, dateSummary.Date.Begin, dateSummary.Date.End);
            }
            
            if (summary is ActivitySlotSummary)
            {
                // wenn es ein Slot ist, dann stecken die Rahmenbedingungen trotzdem im Termin
                var slotSummary = summary as ActivitySlotSummary;
                return activityService.GetSubscriptionState(slotSummary.Slot.ActivityDate.Occurrence, slotSummary.Slot.ActivityDate.Begin,
                    slotSummary.Slot.ActivityDate.End);
            }

            return SubscriptionState.AfterOccurrence;
        }

        private SubscriptionResponse GetErrorResponse(string msg, bool showUser)
        {
            return new SubscriptionResponse
            {
                HasError = true,
                ShowUser = showUser,
                Message = msg
            };

        }


    }

    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }
    }

}
