using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using Postal;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class LotteryService
    {
        private ILog logger = LogManager.GetLogger("Lottery");

        private TimeTableDbContext db;
        private Lottery lottery;
        private LotteryDrawing _lotteryDrawing;

        private ICollection<Course> _courses;
        private ICollection<IGrouping<string, OccurrenceSubscription>> _subscribers;

        /// <summary>
        /// Reservierungen nach der Verlosung
        /// </summary>
        private ICollection<IGrouping<string, OccurrenceSubscription>> _reservations;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        public LotteryService(TimeTableDbContext db, Guid id)
        {
            this.db = db;
            lottery = db.Lotteries.SingleOrDefault(l => l.Id == id);
        }

        /// <summary>
        /// deprectated
        /// </summary>
        public void RunLottery(Guid id)
        {
            logger.Warn("Deprecated Lottery Service call");
        }

        /// <summary>
        /// Durchführung der Ziehung
        /// </summary>
        public void ExecuteLottery()
        {
            // report für Ziehung anlegen und sofort in db speichern
            _lotteryDrawing = new LotteryDrawing
            {
                Start = DateTime.Now,
                End = DateTime.MaxValue,
                Lottery = lottery
            };
            db.LotteryDrawings.Add(_lotteryDrawing);
            db.SaveChanges();

            try
            {
                InitLottery();

                logger.InfoFormat("Starte Verlosung {0} für {1} Kurse", lottery.Name, _courses.Count);

                foreach (var wpm in _courses)
                {
                    RunLotteryForCourse(wpm);
                }

                _lotteryDrawing.Message = "Lotterie durchgelaufen";
                logger.InfoFormat("Verlosung {0} beendet", lottery.Name);

                // Merke Dir die Reservierungen
                TouchReservations();


                // Plätze automatisch annehmen => nur bei Budgets 
                if (lottery.Budgets.Any())
                {
                    AutomaticAcceptance();
                }

            }
            catch (Exception ex)
            {
                _lotteryDrawing.Message = ex.Message;
                logger.FatalFormat("Abbruch der Verlosung {0} wegen Fehler: {1}", lottery.Name, ex.Message);
            }

            _lotteryDrawing.End = DateTime.Now;
            db.SaveChanges();
        }


        public Data.Lottery GetLottery()
        {
            return lottery;
        }


        public ICollection<Course> GetLotteryCourseList()
        {
            var courseList = new List<Course>();

            if (lottery != null)
            {
                courseList.AddRange(
                    lottery.Occurrences.Select(
                        occurrence => db.Activities.OfType<Course>().SingleOrDefault(
                            c => c.Occurrence.Id == occurrence.Id)).Where(course => course != null));
            }

            return courseList;
        }


        /// <summary>
        /// Verlosung für einen einzigen Kurs
        /// </summary>
        /// <param name="wpm">der Kurs</param>
        /// <returns></returns>
        public void RunLotteryForCourse(Course wpm)
        {
            // wir legen jetzt den Bericht an
            var courseDrawingReport = new OccurrenceDrawing
            {
                Occurrence = wpm.Occurrence,
                Start = DateTime.Now,
                End = DateTime.Now,
                LotteryDrawing = _lotteryDrawing
            };

            // wir sichern mal den Stand vor der verlosung
            foreach (var subscription in wpm.Occurrence.Subscriptions)
            {
                var subscriptionDrawingReport = new SubscriptionDrawing
                {
                    OccurrenceDrawing = courseDrawingReport,
                    Subscription = subscription,
                    DrawingTime = DateTime.Now
                };

                if (subscription.IsConfirmed)
                {
                    subscriptionDrawingReport.StateBeforeDrawing = DrawingState.Confirmed;
                    subscriptionDrawingReport.LapCountBeforeDrawing = -1;
                }
                else
                {
                    if (subscription.OnWaitingList)
                    {
                        subscriptionDrawingReport.StateBeforeDrawing = DrawingState.Waiting;
                        subscriptionDrawingReport.LapCountBeforeDrawing = subscription.LapCount;
                    }
                    else
                    {
                        subscriptionDrawingReport.StateBeforeDrawing = DrawingState.Reserved;
                        subscriptionDrawingReport.LapCountBeforeDrawing = -1;
                    }
                }

                courseDrawingReport.SubscriptionDrawings.Add(subscriptionDrawingReport);
                db.SubscriptionDrawings.Add(subscriptionDrawingReport);
            }
            db.OccurrenceDrawings.Add(courseDrawingReport);
            db.SaveChanges();

            logger.Debug("####################################");
            logger.DebugFormat("Starte Verlosung für [{0}]", wpm.ShortName);
            if (wpm.Occurrence.UseGroups)
            {
                logger.DebugFormat("Verlosung nach Semestergruppen");
            }
            else
            {
                logger.DebugFormat("Verlosung nach Gesamtanzahl");
            }

            // Warteliste
            var waitingList = wpm.Occurrence.Subscriptions.Where(s => s.OnWaitingList == true).ToList();
            // Teilnehmer und Reservierungen
            var participentList = wpm.Occurrence.Subscriptions.Where(s => s.OnWaitingList == false).ToList();


            // Alle die in der Warteliste drin sind um eine Runde nach oben bringen
            // Interne Prio der Warteliste, wer länger wartet, hat bessere Chancen, weil zuerst verlost
            foreach (var subscription in waitingList)
            {
                subscription.LapCount++;
            }

            // alle die nicht bestätigt haben zurück auf die Warteliste und dort ans Ende
            var notAccepted = participentList.Where(s => s.IsConfirmed == false).ToList();

            var n = wpm.Occurrence.Subscriptions.Count;
            var n1 = participentList.Count - notAccepted.Count;
            var n2 = notAccepted.Count;
            var n3 = waitingList.Count;

            logger.DebugFormat("Status vor Beginn der Verlosung:");
            logger.DebugFormat("Eintragungen gesamt [{0}]", n);
            logger.DebugFormat("davon Teilnehmer [{0}]", n1);
            logger.DebugFormat("davon Reservierungen [{0}]", n2);
            logger.DebugFormat("Auf Warteliste [{0}]", n3);
            
            foreach (var subscription in notAccepted)
            {
                subscription.OnWaitingList = true;
                subscription.LapCount = 0;

                // die entsprechenden Listen müssen angepasst werden.
                waitingList.Add(subscription);
                participentList.Remove(subscription);

                // Report
                var subReport = courseDrawingReport.SubscriptionDrawings.FirstOrDefault(s => s.Subscription.Id == subscription.Id);
                if (subReport != null)
                {
                    subReport.Remark = "<p>Reservierung nicht angenommen. Zurückgesetzt.</p>";
                }
            }

            // diesen Stand vor der Durchführung der Verlosung sichern
            // wer sich während der Verlosung einträgt rutscht somit ans Ende
            db.SaveChanges();

            n = wpm.Occurrence.Subscriptions.Count;
            n1 = participentList.Count;
            n2 = 0;
            n3 = waitingList.Count;
            
            // Das sind die Anzahl der Kandidaten
            var candidates = n3;
            var available = 0;

            logger.Debug("Nach Stornierung der Reservierungen");
            logger.DebugFormat("Eintragungen gesamt [{0}]", n);
            logger.DebugFormat("davon Teilnehmer [{0}]", n1);
            logger.DebugFormat("Auf Warteliste [{0}]", n3);


            // so jetzt sind die Listen aufgeteilt, ganz ohne Prüfung von Gruppen
            if (wpm.Occurrence.UseGroups)
            {
                // TODO: Unbedingt prüfen!!!
                // Für jede Gruppe separat
                // hier wird jetzt auch die Gruppenzugehörigkeit berücksichtig!
                foreach (var group in wpm.Occurrence.Groups)
                {
                    logger.Debug("------");
                    logger.DebugFormat("Zusammenstellung der Lostöpfe für Kapazitätsgruppe mit [{0}] Plätzen", group.Capacity);
                    foreach (var semesterGroup in group.SemesterGroups)
                    {
                        logger.DebugFormat("Zugehörige Semestergruppe [{0}]", semesterGroup.FullName);
                    }
                    var groupParticipantList = GetSubscriptionsForGroup(participentList, group, courseDrawingReport);
                    var groupWaitingList = GetSubscriptionsForGroup(waitingList, group, courseDrawingReport);
                    logger.Debug("------");

                    available += LotteryForGroup(groupWaitingList, groupParticipantList, group.Capacity);
                }
            }
            else
            {
                // es wird auf Gesamtebene verlost
                // d.h. hier müssen zuerst alle aus der Warteliste entfernt werden, die zu einem anderen
                // Studiengang gehören

                var validWaitingList = new List<OccurrenceSubscription>();

                foreach (var group in wpm.Occurrence.Groups)
                {
                    logger.Debug("------");
                    logger.Debug("Zusammenstellung des Lostopfs für alle Kapazitätsgruppen");
                    foreach (var semesterGroup in group.SemesterGroups)
                    {
                        logger.DebugFormat("Zugehörige Semestergruppe [{0}]", semesterGroup.FullName);
                    }

                    var groupWaitingList = GetSubscriptionsForGroup(waitingList, group, courseDrawingReport);
                    validWaitingList.AddRange(groupWaitingList);

                    logger.Debug("------");
                }

                // die Warteliste ist jetzt bereinigt
                // Hier wird jetzt verlost
                // Als Parameter kommt die Anzahl der Plätze mit
                available += LotteryForGroup(validWaitingList, participentList, wpm.Occurrence.Capacity);
            }

            db.SaveChanges();

            n = wpm.Occurrence.Subscriptions.Count;
            n1 = wpm.Occurrence.Subscriptions.Where(s => s.OnWaitingList == true && s.IsConfirmed == true).ToList().Count;
            n2 = wpm.Occurrence.Subscriptions.Where(s => s.OnWaitingList == true && s.IsConfirmed == false).ToList().Count;
            n3 = wpm.Occurrence.Subscriptions.Where(s => s.OnWaitingList == false).ToList().Count;

            logger.DebugFormat("Status vor Nach der Verlosung:");
            logger.DebugFormat("Eintragungen gesamt [{0}]", n);
            logger.DebugFormat("davon Teilnehmer [{0}]", n1);
            logger.DebugFormat("davon Reservierungen [{0}]", n2);
            logger.DebugFormat("Auf Warteliste [{0}]", n3);

            logger.Debug("####################################");

            logger.InfoFormat("Verlosung für [{0}]: {1} freie Plätze unter {2} Kandidaten verlost", wpm.ShortName, available, candidates);

            // Nachbereitung für Bericht
            foreach (var subscription in wpm.Occurrence.Subscriptions)
            {
                var subReport = courseDrawingReport.SubscriptionDrawings.FirstOrDefault(s => s.Subscription.Id == subscription.Id);

                if (subReport != null)
                {
                    if (subscription.IsConfirmed)
                    {
                        subReport.StateAfterDrawing = DrawingState.Confirmed;
                        subReport.LapCountAfterDrawing = -1;
                    }
                    else
                    {
                        if (subscription.OnWaitingList)
                        {
                            subReport.StateAfterDrawing = DrawingState.Waiting;
                            subReport.LapCountAfterDrawing = subscription.LapCount;
                        }
                        else
                        {
                            subReport.StateAfterDrawing = DrawingState.Reserved;
                            subReport.LapCountAfterDrawing = -1;
                        }
                    }

                    subReport.DrawingTime = DateTime.Now;
                }
            }

            courseDrawingReport.End = DateTime.Now;

            db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitingList">Die bereinigte Warteliste - unsortiert!</param>
        /// <param name="participantList"></param>
        /// <param name="capacity">Die Anzahl der Plätze insgesamt</param>
        /// <returns></returns>
        private int LotteryForGroup(List<OccurrenceSubscription> waitingList, List<OccurrenceSubscription> participantList, int capacity)
        {
            logger.Debug("Durchführung der Verlosung");
            logger.DebugFormat("Kapazität gesamt {0}", capacity);
            logger.DebugFormat("Anzahl Kandidaten {0}", waitingList.Count);
            logger.DebugFormat("Anzahl Teilnehmer bisher [{0}]", participantList.Count);

            var nAvailable = capacity - participantList.Count;

            // Bei einer leeren Warteliste muss nicht verlost werden
            if (waitingList.Count == 0)
            {
                logger.Debug("Warteliste ist leer. Keine Aktion.");
                return nAvailable;
            }

            // Wenn es keine gruppen gibt, dann wird dies auch nicht berücksichtigt
            logger.DebugFormat("Anzahl Restplätze [{0}]", nAvailable);

            // ohne Beschränkung => alle gehen rein
            if (capacity < 0 || nAvailable >= waitingList.Count)
            {
                logger.Debug("Anzahl verfügbare Plätze größer als Einträge Warteliste. Alle erhalten eine Reservierung.");
                // alle gehen rein
                foreach (var waiting in waitingList)
                {
                    waiting.OnWaitingList = false;
                    waiting.IsConfirmed = false;
                }
            }
            else
            {
                logger.Debug("Anzahl verfügbarer Plätze kleiner als Warteliste. Verlosung durchführen.");
                // Es muss verlost werden, aber ohne Gruppen
                // Hier die Änderung: Berechnung der Punkte aus Budgets und Rundenzähler


                var maxLapCount = waitingList.Max(s => s.LapCount);

                var maxBudgetSum = waitingList.Max(x => x.Bets.Sum(y => y.Amount));

                var lap = maxLapCount + maxBudgetSum;

                logger.DebugFormat("In der Warteliste befinden sich Einträge aus {0} Runden", lap);
                bool go = true;
                while (lap >= 0 && go)
                {
                    // weil es Punktemäßig ja rückwärts geht!
                    var candidates = waitingList.Where(s => (s.LapCount + s.Bets.Sum(b => b.Amount)) == lap).ToList();

                    var nLeft = capacity - participantList.Count;

                    logger.DebugFormat("Verlosungsrunde [{0}]", lap);
                    logger.DebugFormat("Anzahl freier Plätze [{0}]", nLeft);
                    logger.DebugFormat("Anzahl Kandidaten [{0}]", candidates.Count);
                    logger.DebugFormat("Anzahl Teilnehmer bis jetzt {0}", participantList.Count);

                    if (nLeft >= candidates.Count)
                    {
                        logger.Debug("Anzahl freier Plätze größer als Anzahl Kandidaten. Alle erhalten eine Reservierung.");
                        // alle gehen rein
                        foreach (var candidate in candidates)
                        {
                            candidate.OnWaitingList = false;
                            candidate.IsConfirmed = false;

                            // zur Teilnehmerliste hinzufügen ist hier erforderlich
                            // damit im nächsten Durchlauf die Anzahl passt
                            participantList.Add(candidate);
                        }

                        if (nLeft == candidates.Count)
                        {
                            go = false;
                        }
                    }
                    else
                    {
                        // Verlosen
                        logger.Debug("Verlose Plätze");
                        // Schüttelt die Liste durch
                        candidates.Shuffle();

                        // nimm die ersten n Kandidaten aus der Zufallsliste
                        for (var i = 0; i < nLeft; i++)
                        {
                            var candiate = candidates[i];
                            candiate.OnWaitingList = false;
                            candiate.IsConfirmed = false;

                            // Ist hier überflüssig, weil jetzt ja abgebrochen wird!
                            participantList.Add(candiate);

                        }

                        // Durch das Verlosen sind jetzt alle Plätze belegt
                        // Fortsetzung macht keinen Sinn
                        go = false;
                    }

                    lap--;
                }
            }

            return nAvailable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allSubscriptions">Liste aller Eintragungen</param>
        /// <param name="group">Liste von Semestergruppen, denen die LV zugeordnet ist</param>
        /// <param name="report"></param>
        /// <returns>gefiltterte Liste der Eintragungen</returns>
        public List<OccurrenceSubscription> GetSubscriptionsForGroup(ICollection<OccurrenceSubscription> allSubscriptions, 
            OccurrenceGroup group, OccurrenceDrawing report)
        {
            if (!group.Occurrence.UseExactFit)
            {
                logger.DebugFormat("Suche nach Studiengang");
            }
            else
            {
                logger.DebugFormat("Suche nach exakter Semestergruppe");
            }

            var subList = new List<OccurrenceSubscription>();


            foreach (var subscription in allSubscriptions)
            {
                var subReport = report.SubscriptionDrawings.FirstOrDefault(s => s.Subscription.Id == subscription.Id);

                var bFound = false;
                var sbError = new StringBuilder();

                foreach (var semesterGroup in group.SemesterGroups)
                {
                    if (!bFound)
                    {
                        if (subReport != null)
                        {
                            subReport.Remark += $"<p>Überprüfung von {semesterGroup.FullName}.</p>";
                        }

                        // ist der Student im Semester dieser Gruppe eingeschrieben?
                        // zur Sicherheit, falls er in mehreren Gruppen drin sein sollte
                        var semSub = db.Subscriptions.OfType<SemesterSubscription>()
                        .FirstOrDefault(s => s.SemesterGroup.Semester.Id == semesterGroup.Semester.Id &&
                                                s.UserId.Equals(subscription.UserId));

                        // er ist eingeschrieben
                        // jetzt nach den Anforderungen der Occurrence weiter
                        if (semSub != null)
                        {
                            bool isInGroup;
                            if (!group.Occurrence.UseExactFit)
                            {
                                // Hier genügt der Studiengang
                                isInGroup =
                                    group.SemesterGroups.Any(
                                        g =>
                                            g.CapacityGroup.CurriculumGroup.Curriculum.Id ==
                                            semSub.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Id);

                                if (!isInGroup)
                                {
                                    sbError.AppendFormat("<p>Studiengang passt nicht zu Semestergruppe: Eingetragen in {0}</p>",
                                        semSub.SemesterGroup.FullName);
                                }
                            }
                            else
                            {
                                // Die Semestergruppe muss exakt passen
                                isInGroup = group.SemesterGroups.Any(g => g.Id == semSub.SemesterGroup.Id);
                                if (!isInGroup)
                                {
                                    sbError.AppendFormat("<p>Semestergruppe passt nicht exakt: Eingetragen in {0}</p>", semSub.SemesterGroup.FullName);
                                }
                            }

                            // nur hinzufügen, wenn noch nicht drin
                            if (isInGroup && !subList.Contains(subscription))
                            {
                                subList.Add(subscription);
                                bFound = true;
                            }
                        }
                        else
                        {
                            sbError.AppendFormat("<p>Keine Angabe zu Semestergruppe - Eintragung wird ignoriert</p>");
                        }
                    }
                }

                // hier noch den Fehlertext eintragen
                if (subReport != null)
                {
                    if (bFound)
                    {
                        subReport.Remark += "<p>Eintragung erfüllt Bedingungen.</p>";
                    }
                    else
                    {
                        subReport.Remark += "<p>Ungültige Eintragung.</p>" + sbError;
                    }
                }
            }

            logger.DebugFormat("Anzahl Eintragungen gesamt [{0}]", allSubscriptions.Count);
            logger.DebugFormat("Anzahl passender Eintragungen [{0}]", subList.Count);

            return subList;
        }


        private void InitLottery()
        {
            _courses = GetLotteryCourseList();

            var allSubscriptionLists = _courses.Select(s => s.Occurrence.Subscriptions).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var subs in allSubscriptionLists)
            {
                allSubscriptions.AddRange(subs);
            }

            // hier hängen pro subscriber jeweils alle Eintragungen in dieser Lotterie dran!
            _subscribers = allSubscriptions.GroupBy(g => g.UserId).ToList();
        }


        private void TouchReservations()
        {
            var allSubscriptionLists = _courses.Select(s => s.Occurrence.Subscriptions).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var subs in allSubscriptionLists)
            {
                var r = subs.Where(x => !x.OnWaitingList && !x.IsConfirmed).ToList();

                allSubscriptions.AddRange(r);
            }

            // hier hängen pro subscriber jeweils alle Eintragungen in dieser Lotterie dran!
            _reservations = allSubscriptions.GroupBy(g => g.UserId).ToList();
        }


        private void AutomaticAcceptance()
        {
            // Gehe jeden teilnehmer einzeln durch
            foreach (var subscriber in _subscribers)
            {
                var userId = subscriber.Key;

                // die Anzahl der gesicherten Plätze
                var nSaveSeats = subscriber.Count(x => x.OnWaitingList == false && x.IsConfirmed);

                // Kontingent ausgeschöpft => keine weitere Aktion
                if (nSaveSeats >= lottery.MaxConfirm)
                    continue;

                var capacityLeft = lottery.MaxConfirm - nSaveSeats;

                // die Reservierungen, absteigend sortiert nach Punkten
                var reservations = subscriber
                    .Where(x => x.OnWaitingList == false && x.IsConfirmed == false)
                    .OrderByDescending(x => x.Bets.Sum(a => a.Amount) + x.LapCount)
                    .Take(capacityLeft)
                    .ToList();
                
                // alle noch offenen Reservierungen annehmen
                foreach (var reservation in reservations)
                {
                    reservation.IsConfirmed = true;
                    foreach (var bet in reservation.Bets)
                    {
                        // das bringt hier nix, weil die Subscription ja gelöscht wirde
                        // es müsste als ein Gedächtnis Budget <=> User geben => heikel
                        //bet.AmountConsumed = bet.Amount;
                        //bet.Amount = 0;

                        // zum Ausprobieren, sollte nicht erforderlich sein
                        bet.IsConsumed = true;
                    }
                }


            }

            db.SaveChanges();


        }

        /// <summary>
        /// Benachrichtung nach der ersten Verlosung
        /// </summary>
        public void SendInitialNotifications()
        {
            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            var engine = new FileSystemRazorViewEngine(viewsPath);
            engines.Add(engine);
            var emailService = new Postal.EmailService(engines);

            var userService = new UserInfoService();

            var email = new LotteryDrawingReportEmail("LotteryDrawingInitialNotification")
            {
                Subject = "[nine] Platzverlosung " + lottery.Name,
                Drawing = _lotteryDrawing
            };


            foreach (var subscriber in _subscribers)
            {
                var user = userService.GetUser(subscriber.Key);

                email.User = user;
                email.Courses.Clear();

                foreach (var course in _courses)
                {
                    var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                    email.Courses.Add(new LotteryDrawingCourseReport
                    {
                        Course = course,
                        Subscription = subscription
                    });
                }

                try
                {
                    emailService.Send(email);
                    logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.User.Email);
                }
                catch (Exception exMail)
                {
                    logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.User.Email, exMail.Message);
                }
            }
        }


        /// <summary>
        /// Benachrichtung nur für reservierungen
        /// </summary>
        public void SendDefaultNotifications()
        {
            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            var engine = new FileSystemRazorViewEngine(viewsPath);
            engines.Add(engine);
            var emailService = new Postal.EmailService(engines);

            var userService = new UserInfoService();

            var email = new LotteryDrawingReportEmail("LotteryDrawingDefaultNotification")
            {
                Subject = "[nine] Platzverlosung " + lottery.Name,
                Drawing = _lotteryDrawing
            };

            // nur die mit Reservierung
            foreach (var subscriber in _reservations)
            {
                var user = userService.GetUser(subscriber.Key);

                email.User = user;
                email.Courses.Clear();

                foreach (var course in _courses)
                {
                    // nur die Einschreibungen nicht auf Warteliste
                    var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id) && !x.OnWaitingList);

                    if (subscription != null)
                    {
                        email.Courses.Add(new LotteryDrawingCourseReport
                        {
                            Course = course,
                            Subscription = subscription
                        });
                    }
                }

                try
                {
                    emailService.Send(email);
                    logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.User.Email);
                }
                catch (Exception exMail)
                {
                    logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.User.Email, exMail.Message);
                }
            }
        }



        /// <summary>
        /// Benachrichtung nach der ersten Verlosung
        /// </summary>
        public void SendFinalNotifications()
        {
            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            var engine = new FileSystemRazorViewEngine(viewsPath);
            engines.Add(engine);
            var emailService = new Postal.EmailService(engines);

            var userService = new UserInfoService();

            var email = new LotteryDrawingReportEmail("LotteryDrawingFinalNotification")
            {
                Subject = "[nine] Platzverlosung " + lottery.Name,
                Drawing = _lotteryDrawing
            };


            foreach (var subscriber in _subscribers)
            {
                var user = userService.GetUser(subscriber.Key);

                email.User = user;
                email.Courses.Clear();

                foreach (var course in _courses)
                {
                    var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                    email.Courses.Add(new LotteryDrawingCourseReport
                    {
                        Course = course,
                        Subscription = subscription
                    });
                }

                try
                {
                    emailService.Send(email);
                    logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.User.Email);
                }
                catch (Exception exMail)
                {
                    logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.User.Email, exMail.Message);
                }
            }

        }


    }
}
