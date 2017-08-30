using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Occurrence
    {
        public Occurrence()
        {
            Subscriptions = new HashSet<OccurrenceSubscription>();
            Groups = new HashSet<OccurrenceGroup>();
            Lotteries = new HashSet<Lottery>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Information zum Ereignis (optional), HTML encoded
        /// </summary>
        public string Information { get; set; }

        /// <summary>
        /// Anzahl der verfügbaren Plätze
        /// -1: keine Beschränkung
        /// > 0: Beschränkung, alles was darüber hinaus geht, kommt in die Warteliste
        /// = 0: dito, d.h. alles kommt automatisch in die Warteliste
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Ist abgesagt.
        /// Wenn abgesagt, dann kann auch keine Eintragung (mehr) erfolgen
        /// Wird vererbt
        /// </summary>
        public bool IsCanceled { get; set;  }

        /// <summary>
        /// Ist verschoben.
        /// Datum zeigt aktuellen Termin. Wird vererbt
        /// </summary>
        public bool IsMoved { get; set; }

        /// <summary>
        /// Generelle Verfügbarkeit, d.h. ob überhaupt eine Eintragung möglich ist
        /// Default: True
        /// Sprechstunde: Termin wird nicht angeboten
        /// Vorlesung: Es geht in die Warteliste
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Genereller Schalter: Eintragung ist (Zeit-)beschränkt
        /// Default: False
        /// </summary>
        public bool FromIsRestricted { get; set; }

        /// <summary>
        /// Restriktion ist entweder relativ zum Beginn des Ereignisses (true)
        /// oder besitzt absoluten Wert (false)
        /// Ist bei Verschiebung des zugehörigen Ereignisses zu beachten
        /// </summary>
        public bool UntilIsRestricted { get; set; }

        /// <summary>
        /// Eintragung ab Tagesdatum (absolute Zeitangabe)
        /// </summary>
        public DateTime? FromDateTime { get; set; }
        
        /// <summary>
        /// Eintragung bis Tagesdatum (absolute Zeitangabe)
        /// </summary>
        public DateTime? UntilDateTime { get; set; }

        /// <summary>
        /// Eintragung ab relative Zeitangabe
        /// </summary>
        public TimeSpan? FromTimeSpan { get; set; }
        
        /// <summary>
        /// Eintragung bis relative Zeitangabe
        /// </summary>
        public TimeSpan? UntilTimeSpan { get; set; }

        /// <summary>
        /// Ereignis nimmt an automatischer Platzverlosung teil
        /// </summary>
        public bool LotteryEnabled { get; set; }

        /// <summary>
        /// Platzverteilung erfolgt nach Gruppen
        /// </summary>
        public bool UseGroups { get; set; }

        /// <summary>
        /// Platzverteilung erfolgt exakt nach Studiengang und Studiengruppe
        /// </summary>
        public bool UseExactFit { get; set; }

        public virtual ICollection<OccurrenceSubscription> Subscriptions { get; set; }

        public virtual ICollection<OccurrenceGroup> Groups { get; set; }

        public virtual ICollection<Lottery> Lotteries { get; set; }
    }
}
