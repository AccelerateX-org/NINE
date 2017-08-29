using System;
using System.Linq;
using System.Text;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityService
    {
        private TimeTableDbContext _db = null;

        /// <summary>
        /// 
        /// </summary>
        public ActivityService()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public ActivityService(TimeTableDbContext db)
        {
            _db = db;
        }

        private TimeTableDbContext DB
        {
            get { return _db ?? (_db = new TimeTableDbContext()); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IActivitySummary GetSummary(Guid Id)
        {
            Occurrence occ = DB.Occurrences.SingleOrDefault(o => o.Id == Id);
            return GetSummary(occ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occ"></param>
        /// <returns></returns>
        public IActivitySummary GetSummary(Occurrence occ)
        {
            if (occ != null)
            {
                var activity = DB.Activities.SingleOrDefault(ac => ac.Occurrence.Id == occ.Id);
                if (activity != null)
                {
                    return new ActivitySummary
                    {
                        Activity = activity,
                    };
                }

                var date = DB.ActivityDates.SingleOrDefault(d => d.Occurrence.Id == occ.Id);
                if (date != null)
                {
                    return new ActivityDateSummary
                    {
                        Date = date,
                    };
                }

                var slot = DB.ActivitySlots.SingleOrDefault(s => s.Occurrence.Id == occ.Id);
                if (slot != null)
                {
                    return new ActivitySlotSummary
                    {
                        Slot = slot,
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Ermittelt den Eintragungsstatus eines Benutzers
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="user"></param>
        /// <param name="semester"></param>
        /// <returns>Der Benutzer ist bereits eingetragen, es ist eine Eintragung möglich. Wenn keine Eintragung möglich, dann wird es als Fehler angegeben (mit Grund).</returns>
        public OccurrenceStateModel GetActivityState(Occurrence occurrence, ApplicationUser user, Semester semester)
        {
            var state = new OccurrenceStateModel();
            state.Occurrence = occurrence;

            if (user == null)
            {
                // Benutzer unbekannt
                return state;
            }

            // Anmerkung: FirstOrDefault ist hier besser, da im Augenblick  noch nicht verhindert werden kann
            // dass es doppelte Eintragungen gibt
            // TODO: auf SingleOrDefault umstellen
            state.Subscription = occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(user.Id));

            var summary = GetSummary(occurrence.Id);

            // wenn es eine Einschreibung gibt, dann nicht weitermachen
            // Ausnahme: bei Sprechstunden darf man sich nach dem Ende der Eintragungsfrist bzw. 
            // nach Beginn des Termins nicht mehr austragen
            if (state.Subscription != null)
            {
                if (summary.Activity is OfficeHour)
                {
                    // den State einfügen
                    if (summary is ActivityDateSummary)
                    {
                        var dateSummary = summary as ActivityDateSummary;
                        state.State = GetSubscriptionState(occurrence, dateSummary.Date.Begin, dateSummary.Date.End);

                    }
                    else if (summary is ActivitySlotSummary)
                    {
                        // wenn es ein Slot ist, dann stecken die Rahmenbedingungen trotzdem im Termin
                        var slotSummary = summary as ActivitySlotSummary;
                        state.State = GetSubscriptionState(slotSummary.Slot.ActivityDate.Occurrence,
                            slotSummary.Slot.ActivityDate.Begin,
                            slotSummary.Slot.ActivityDate.End);
                    }

                    if (state.State == SubscriptionState.AfterSubscriptionPhase ||
                        state.State == SubscriptionState.DuringOccurrence ||
                        state.State == SubscriptionState.AfterOccurrence)
                    {
                        state.HasError = true;
                        state.ErrorMessage = "kein Austragen mehr möglich.";
                    }
                }

                return state;
            }



            // hier noch prüfen, ob Eintragung erlaubt
            // Gäste dürfen nur Sprechstunden
            // Studierende nur, wenn sie eine Eintragung im aktuellen Seemster haben
            if (user.MemberState == MemberState.Guest && !(summary.Activity is OfficeHour))
            {
                state.HasError = true;
                state.ErrorMessage = "Als Gast nur Eintragung in Sprechstunden möglich";
                return state;
            }

            if (user.MemberState == MemberState.Student && summary.Activity is Course)
            {
                /*
                 * Überprüfung, ob eine Einschreibung für das im Parameter angegebene Semester vorliegt
                var hasSubscription =
                    DB.SemesterGroups.Any(
                        g => g.Semester.Id == semester.Id && g.Subscriptions.Any(s => s.UserId.Equals(user.Id)));
                        */

                // Überprüfen, ob zu den Semestern des Kurses auch eine Einschreibung vorliegt
                var allSemIds = summary.Activity.SemesterGroups.Select(x => x.Semester.Id).ToList();
                var allSubIds = DB.Subscriptions.OfType<SemesterSubscription>().Where(x => x.UserId.Equals(user.Id))
                    .Select(x => x.SemesterGroup.Semester.Id).ToList();

                var hasSubscription = allSemIds.Any(x => allSubIds.Contains(x));

                if (!hasSubscription)
                {
                    state.HasError = true;
                    state.ErrorMessage = "Angabe der aktuellen Semestergruppe erforderlich";
                    return state;
                }
            }


            // der State alleine steuert die Anzeige der "Subscribe"-Buttons
            // dieser wird nur angezeigt, wenn State den Wert "DuringSubscriptionPhase" hat
            if (summary is ActivitySummary)
            {
                // Bei Aktivitäten gibt es Wartelisten, daher keine Beschränkungen mehr auf Zeit und Kapazität hier prüfen
                state.State = SubscriptionState.DuringSubscriptionPhase;
            }
            else if (summary is ActivityDateSummary)
            {
                var dateSummary = summary as ActivityDateSummary;
                state.State = GetSubscriptionState(occurrence, dateSummary.Date.Begin, dateSummary.Date.End);

                // Kapazität prüfen
                if (occurrence.Capacity >= 0)
                {
                    if (occurrence.Subscriptions.Count >= occurrence.Capacity)
                    {
                        state.HasError = true;
                        state.ErrorMessage = "belegt";
                    }
                }
            }
            else if (summary is ActivitySlotSummary)
            {
                // wenn es ein Slot ist, dann stecken die Rahmenbedingungen trotzdem im Termin
                var slotSummary = summary as ActivitySlotSummary;
                state.State = GetSubscriptionState(slotSummary.Slot.ActivityDate.Occurrence, slotSummary.Slot.ActivityDate.Begin,
                    slotSummary.Slot.ActivityDate.End);

                // Kapazität prüfen
                if (occurrence.Capacity >= 0)
                {
                    if (occurrence.Subscriptions.Count >= occurrence.Capacity)
                    {
                        state.HasError = true;
                        state.ErrorMessage = "belegt";
                    }
                }
            }
            else // gibt es nicht
            {
                state.State = SubscriptionState.AfterOccurrence;
            }

            // wenn belegt, dann nicht weiter machen
            if (state.HasError)
                return state;

            // frei? dann Zeitpunkt prüfen
            switch (state.State)
            {
                case SubscriptionState.BeforeSubscriptionPhase:
                    state.HasError = true;
                    state.ErrorMessage = "Noch keine Eintragung möglich";
                    break;
                case SubscriptionState.DuringSubscriptionPhase:
                    break;
                case SubscriptionState.AfterSubscriptionPhase:
                case SubscriptionState.DuringOccurrence:
                case SubscriptionState.AfterOccurrence:
                    state.HasError = true;
                    state.ErrorMessage = "Keine Eintragung mehr möglich";
                    break;
            }



            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrence"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public SubscriptionState GetSubscriptionState(Occurrence occurrence, DateTime start, DateTime end)
        {
            var now = GlobalSettings.Now;

            // alles nach Beginn des Ereignisses
            if (start <= now)
            {
                if (now <= end)
                {
                    return SubscriptionState.DuringOccurrence;
                }
                else
                {
                    return SubscriptionState.AfterOccurrence;
                }
            }
            else
            {
                if (!occurrence.FromIsRestricted && !occurrence.UntilIsRestricted)
                {
                    return SubscriptionState.DuringSubscriptionPhase;
                }

                if (occurrence.FromIsRestricted)
                {
                    DateTime beginSubscriptionPhase;
                    if (occurrence.FromDateTime.HasValue)
                    {
                        beginSubscriptionPhase = occurrence.FromDateTime.Value;
                    }
                    else if (occurrence.FromTimeSpan.HasValue)
                    {
                        beginSubscriptionPhase =
                            start.AddHours(-occurrence.FromTimeSpan.Value.Hours)
                                .AddMinutes(-occurrence.FromTimeSpan.Value.Minutes);
                    }
                    else
                    {
                        beginSubscriptionPhase = now;
                    }

                    if (now < beginSubscriptionPhase)
                        return SubscriptionState.BeforeSubscriptionPhase;
                }

                if (occurrence.UntilIsRestricted)
                {
                    DateTime untilSubscriptionPhase;
                    if (occurrence.UntilDateTime.HasValue)
                    {
                        untilSubscriptionPhase = occurrence.UntilDateTime.Value;
                    }
                    else if (occurrence.UntilTimeSpan.HasValue)
                    {
                        untilSubscriptionPhase =
                            start.AddHours(-occurrence.UntilTimeSpan.Value.Hours)
                                .AddMinutes(-occurrence.UntilTimeSpan.Value.Minutes);
                    }
                    else
                    {
                        untilSubscriptionPhase = now;
                    }

                    if (now > untilSubscriptionPhase)
                        return SubscriptionState.AfterSubscriptionPhase;
                }


                return SubscriptionState.DuringSubscriptionPhase;
            }
        }

        /// <summary>
        /// Ermittelt den Text für Eintragungen
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetStatusText(ActivityDate date)
        {
            var now = DateTime.Now;

            // nach Beginn keine Anzeige
            if (date.Begin <= now && now <= date.End)
                return "Termin findet gerade statt";
            if (date.End < now)
                return "Termin hat stattgefunden";


            if (!date.Occurrence.FromIsRestricted && !date.Occurrence.UntilIsRestricted)
            {
                // keine Beschränkung
                return "Eintragung jederzeit möglich";
            }

            var sb = new StringBuilder();

            if (date.Occurrence.FromIsRestricted)
            {
                if (date.Occurrence.FromDateTime.HasValue)
                {
                    sb.Append(string.Format("Eintragung ab {0:dd.MM.yyyy HH:mm} offen.", date.Occurrence.FromDateTime.Value));
                }
                
                if (date.Occurrence.FromTimeSpan.HasValue)
                {
                    var d = date.Begin.AddHours(-date.Occurrence.FromTimeSpan.Value.Hours)
                            .AddMinutes(-date.Occurrence.FromTimeSpan.Value.Minutes);
                    sb.Append(string.Format("Eintragung ab {0:dd.MM.yyyy HH:mm} offen", d));
                }
            }

            if (date.Occurrence.UntilIsRestricted)
            {
                if (date.Occurrence.UntilDateTime.HasValue)
                {
                    sb.Append(string.Format("Eintragung bis {0:dd.MM.yyyy HH:mm} offen.", date.Occurrence.UntilDateTime.Value));
                }
                
                if (date.Occurrence.UntilTimeSpan.HasValue)
                {
                    var d = date.Begin.AddHours(-date.Occurrence.UntilTimeSpan.Value.Hours)
                            .AddMinutes(-date.Occurrence.UntilTimeSpan.Value.Minutes);
                    sb.Append(string.Format("Eintragung bis {0:dd.MM.yyyy HH:mm} offen", d));
                }
            }

            return sb.ToString();

        }
    }
}