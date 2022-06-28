using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public enum DrawingFrequency
    {
        Daily = 1,
        Weekly,
        Monthly
    }

    public class Lottery
    {
        public Lottery()
        {
            Occurrences = new HashSet<Occurrence>();
            Drawings = new HashSet<LotteryDrawing>();
            Budgets = new List<LotteryBudget>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string JobId { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// An-/Abschalten
        /// Wenn abgeschaltet, dann kann sich niemand eintragen / austragen
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Nur der Owner darf die Einstellungen ändern
        /// Das sind "private" Lotterien, diese sind wiederverwendbar
        /// Der Owner ist auch für die Bearbeitung der Anträge verantwortlich
        /// </summary>
        public virtual OrganiserMember Owner { get; set; }

        /// <summary>
        /// Die immer wiederkehrenden Platzverlosungen in einem Semester
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// In Kombination zu einem Semester => dürfen nur von den CourseAdmins
        /// bearbeitet werden
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// Anzahl der Plätze, die ein Teilnehmer maximal annehmen darf
        /// Standardwert für CoursesWanted im LotteryGame des Teilnehmers
        /// </summary>
        public int MaxConfirm { get; set; }

        /// <summary>
        /// Im Ausnahmefall
        /// >0 gilt als gesetzt
        /// Auswahl wird als CoursesWanted im LotteryGame des Teilnehmers gesetzt
        /// </summary>
        public int MaxExceptionConfirm { get; set; }

        /// <summary>
        /// Textanzeige für Ausnahme
        /// </summary>
        public string ExceptionRemark { get; set; }

        /// <summary>
        /// Maximale Anzahl der Eintragungen: 0 ist unbeschränkt
        /// </summary>
        public int MaxSubscription { get; set; }

        /// <summary>
        /// Minimale Anzahl an Eintragungen
        /// </summary>
        public int MinSubscription { get; set; }

        /// <summary>
        /// Bei aktiver Lotterie können Studierende die Lotterie sehen
        /// </summary>
        public bool IsActive { get; set; }

        public DateTime? IsActiveFrom { get; set; }

        public DateTime? IsActiveUntil { get; set; }

        /// <summary>
        /// false: Windhundverfahren
        /// true: Verlosung
        /// </summary>
        public bool IsFixed { get; set; }

        /// <summary>
        /// Bewerbungsschreiben erforderlich
        /// </summary>
        public bool LoINeeded { get; set; }

        /// <summary>
        /// Voreinschreibung generell erlaubt oder nicht
        /// </summary>
        public bool AllowManualSubscription { get; set; }

        public DateTime FirstDrawing { get; set; }

        public DateTime LastDrawing { get; set; }

        public TimeSpan DrawingTime { get; set; }

        public DrawingFrequency DrawingFrequency { get; set; }

        /// <summary>
        /// Workaround: Nutze Pechvogelregel
        /// true: ohne Pechvogelregel
        /// false: mit Pechvogelregel (default)
        /// </summary>
        public bool IsScheduled { get; set; }

        /// <summary>
        /// Workaround: als "Studierende können ´Wahl nicht ändern"
        /// true:
        /// false: Studierende können die Wahl ändern d.h. sich austragen
        /// </summary>
        public bool UseLapCount { get; set; }

        /// <summary>
        /// TZ Studierende drüfen nicht teilnehmen
        /// </summary>
        public bool blockPartTime { get; set; }

        /// <summary>
        /// VZ Studierende dürfen nicht teilnehmen
        /// </summary>
        public bool blockFullTime { get; set; }


        public virtual LotteryBundle LotteryBundle { get; set; }

        /// <summary>
        /// Die Liste der Lehrveranstaltungen
        /// </summary>
        public virtual ICollection<Occurrence> Occurrences { get; set; }

        /// <summary>
        /// Die Liste der Ziehungen
        /// </summary>
        public virtual ICollection<LotteryDrawing> Drawings { get; set; }

        /// <summary>
        /// Die Liste der Budgets
        /// </summary>
        public virtual ICollection<LotteryBudget> Budgets { get; set; }

        /// <summary>
        /// Die Liste der Teilnehmer
        /// </summary>
        public virtual ICollection<LotteryGame> Games { get; set; }
    }
}
