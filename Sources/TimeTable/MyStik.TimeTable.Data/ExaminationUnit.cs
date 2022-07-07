using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class ExaminationUnit
    {
        public ExaminationUnit()
        {
            ExaminationAids = new HashSet<ExaminationAid>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Prüfungsform
        /// </summary>
        public ExaminationForm Form { get; set; }

        /// <summary>
        /// Prüfungsdauer in Minuten
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Gewichtung der Prüfungsleistung
        /// </summary>
        public double Weight { get; set; }


        // die Hilfsmittel

        /// <summary>
        /// Das zugehörige Module
        /// veraltet
        /// </summary>
        // public virtual TeachingBuildingBlock Module { get; set; }


        //public virtual TeachingAssessment Assessment { get; set; }


        public virtual ICollection<ExaminationAid> ExaminationAids { get; set; }
    }
}
