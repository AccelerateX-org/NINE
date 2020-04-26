using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Assessment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsAvailable { get; set; }

        public virtual Curriculum Curriculum { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual Committee Committee { get; set; }

        public virtual ICollection<AssessmentStage> Stages { get; set; }

        public virtual ICollection<Candidature> Candidatures { get; set; }
    }

    public class AssessmentStage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// wird angezeigt
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Ab diesem Datum zugänglich
        /// </summary>
        public DateTime? OpeningDateTime { get; set; }

        /// <summary>
        /// Bis zu diesem Datum zugänglich
        /// </summary>
        public DateTime? ClosingDateTime { get; set; }

        /// <summary>
        /// An diesem Datum sind Ergebnisse zugänglich
        /// </summary>
        public DateTime? ReportingDateTime { get; set; }

        public virtual Assessment Assessment { get; set; }


        public virtual ICollection<AssessmentStageMaterial> Material { get; set; }

    }

    public class AssessmentStageMaterial
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual  AssessmentStage  Stage { get; set; }

        public virtual BinaryStorage Storage { get; set; }
    }
}
