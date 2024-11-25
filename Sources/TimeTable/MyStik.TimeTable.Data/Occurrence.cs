using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MyStik.TimeTable.Data
{
    public enum SeatAllocationMethod
    {
        Nothing,
        Manual,
        Auto
    }

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
        /// Pro Gruppe gibt es Plätze
        /// Ohne Semestergruppe gibt es das nicht mehr
        /// nur noch pro Studiengang
        /// </summary>
        public bool UseExactFit { get; set; }

        /// <summary>
        /// Studierende der zugeordneten Fakultäten werden bevorzugt
        /// </summary>
        public bool HasHomeBias { get; set; }

        /// <summary>
        /// Geschlossene Gesellschaft
        /// Zugang nur für Studierende der zugeordneten Fakultäten
        /// </summary>
        public bool IsCoterie { get; set; }

        /// <summary>
        /// greift nur bei Methdoe "Auto"
        /// </summary>
        public bool UseWaitingList { get; set; }

        public SeatAllocationMethod AllocationMethod { get; set; }

        public virtual ICollection<OccurrenceSubscription> Subscriptions { get; set; }

        public virtual ICollection<OccurrenceGroup> Groups { get; set; }

        public virtual ICollection<Lottery> Lotteries { get; set; }

        public virtual ICollection<SeatQuota> SeatQuotas { get; set; } = new HashSet<SeatQuota>();
    }

    public class SeatQuota
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Zuordnung zur Occurrence
        /// </summary>
        public virtual Occurrence Occurrence { get; set; }

        public int MinCapacity { get; set; } = 0;
        public int MaxCapacity { get; set; } = int.MaxValue;


        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Lotterie, die für die Vergabe des Kontingents zuständig ist
        /// </summary>
        public virtual Lottery Lottery { get; set; }

        /// <summary>
        /// Soll durch Fractions abgelöst werden
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Und Verknüpfung => soll durch Frations abgelöst werden
        /// </summary>
        public virtual ItemLabelSet ItemLabelSet { get; set; }

        public virtual ICollection<SeatQuotaFraction> Fractions { get; set; } 


        public string Summary
        {
            get
            {
                var sb = new StringBuilder();

                if (Fractions.Any())
                {
                    foreach (var fraction in Fractions)
                    {
                        sb.Append(fraction.FullName);
                        /*
                        if (fraction.Curriculum != null)
                        {
                            sb.Append(fraction.Curriculum.ShortName);
                        }
                        else
                        {
                            sb.Append("offen für alle Studiengänge");
                        }
                        */
                        if (fraction != Fractions.Last())
                        {
                            sb.Append(" oder ");
                        }
                    }
                }
                else
                {
                    if (Curriculum != null)
                    {
                        sb.AppendFormat("{0}", Curriculum.ShortName);
                    }
                    else
                    {
                        sb.Append("offen für alle Studiengänge");
                    }
                }

                return sb.ToString();
            }
        }
    }


    public class SeatQuotaFraction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Zuordnung zur Occurrence
        /// </summary>
        public virtual SeatQuota Quota { get; set; }

        public int Percentage { get; set; } = 0;
        public int Weight { get; set; } = 0;

        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Und Verknüpfung
        /// </summary>
        public virtual ItemLabelSet ItemLabelSet { get; set; }

        public string FullName
        {
            get
            {
                var sb = new StringBuilder();

                if (Curriculum != null)
                {
                    sb.Append(Curriculum.ShortName);
                }
                else
                {
                    sb.Append("Offen für alle Studiengänge");
                }

                if (ItemLabelSet?.ItemLabels != null && ItemLabelSet.ItemLabels.Any())
                {
                    sb.Append(" mit Kohorte(n): ");
                    foreach (var label in ItemLabelSet.ItemLabels)
                    {
                        if (label != ItemLabelSet.ItemLabels.Last())
                        {
                            sb.AppendFormat("{0}, ", label.Name);
                        }
                        else
                        {
                            sb.AppendFormat("{0}", label.Name);
                        }
                    }
                }

                return sb.ToString();
            }
        }
    }

}
