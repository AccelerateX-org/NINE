using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Internship
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// Student
        /// </summary>
        public virtual Student Student { get; set; }


        /// <summary>
        /// Beginn der Anstellung
        /// </summary>
        public DateTime? StartDate { get; set; }


        /// <summary>
        /// Ende der Anstellung
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Datum des Kolloquiums (Einteilung)
        /// </summary>
        public DateTime? ColloqDate { get; set; }


        /// <summary>
        /// Hat teilgenommen
        /// </summary>
        public bool? HasColloqPassed { get; set; }


        /// <summary>
        /// Datum des Antrags
        /// </summary>
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// Datum des Ergebnisses des Antrags
        /// </summary>
        public DateTime? ResponseDate { get; set; }

        public virtual OrganiserMember RequestAuthority { get; set; }

        public string RequestMessage { get; set; }

        /// <summary>
        /// Das Ergebnis des Antrags auf Annmeldung
        /// </summary>
        public bool? IsPassed { get; set; }

        /// <summary>
        /// Vom Studierenden selbst festgelegter geplanter Beginn
        /// wird automatisch zum Anmlededatum, wenn nicht anderweitig festgelegt
        /// </summary>
        public DateTime? PlannedBegin { get; set; }

        /// <summary>
        /// Geplantes Ende - wird immer automatisch auf Basis des Bearbeitunsgzeitraums des Studiengangs gesetzt
        /// </summary>
        public DateTime? PlannedEnd { get; set; }

        /// <summary>
        /// Datum der letzten Planänderung
        /// Kann gemacht werden, solangde Arbeit noch nicht angemeldet ist
        /// </summary>
        public DateTime? LastPlanChange { get; set; }


        /// <summary>
        /// Akzeptiert durch Praktikumsbetreuer
        /// </summary>
        public DateTime? AcceptedDate { get; set; }


        /// <summary>
        /// realer Beginn
        /// </summary>
        public DateTime? RealBegin { get; set; }

        /// <summary>
        /// Reales Ende
        /// </summary>
        public DateTime? RealEnd { get; set; }



        /// <summary>
        /// Ansprechpartner im Unternehmen
        /// </summary>
        public virtual ICollection<Advisor> Advisors { get; set; }

    }
}
