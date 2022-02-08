using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class TeachingAssessment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Bezeichnung, keine fachliche Auswirkung
        /// </summary>
        public string Name { get; set; }

        public virtual TeachingBuildingBlock Module { get; set; }

        /// <summary>
        /// Alle Prüfungen
        /// </summary>
        public virtual ICollection<ExaminationUnit> ExaminationUnits { get; set; }

    }
}
