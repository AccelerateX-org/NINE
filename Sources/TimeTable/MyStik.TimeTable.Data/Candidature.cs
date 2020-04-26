using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Candidature
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public virtual Assessment Assessment { get; set; }

        /// <summary>
        /// Start der Teilnahme des Verfahrens
        /// </summary>
        public DateTime Joined { get; set; }

        /// <summary>
        /// Steckbrief
        /// </summary>
        public string Characteristics { get; set; }

        /// <summary>
        /// Motivationsschreiben
        /// </summary>
        public string Motivation { get; set; }

        /// <summary>
        /// Akzeptiert, als Ganzes
        /// </summary>
        public bool? IsAccepted { get; set; }

        /// <summary>
        /// Rückmeldung
        /// </summary>
        public string Feedback { get; set; }

        public virtual ICollection<CandidatureStage> Stages { get; set; }
    }

    public class CandidatureStage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual  Candidature Candidature { get; set; }

        public virtual AssessmentStage AssessmentStage { get; set; }


        /// <summary>
        /// Akzeptiert, damit kann die nächste Stufe freigeschaltet werden
        /// </summary>
        public bool? IsAccepted { get; set; }


        public virtual ICollection<CandidatureStageMaterial> Material { get; set; }
    }

    public class CandidatureStageMaterial
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual  CandidatureStage Stage { get; set; }


        public virtual BinaryStorage Storage { get; set; }
    }
}
