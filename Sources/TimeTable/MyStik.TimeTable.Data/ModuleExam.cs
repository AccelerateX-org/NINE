using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public enum ExamType
    {
        SP,
        PA
    }

    /// <summary>
    /// Modul (Teil-)Prüfung
    /// </summary>
    public class ModuleExam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// Typ der Prüfung
        /// </summary>
        public ExamType ExamType { get; set; }

        /// <summary>
        /// Gewicht
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Bezeichnung aus externer Quelle, z.B. Prüfungsamt
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Das zugehörige Modul
        /// </summary>
        public virtual CurriculumModule Module { get; set; }
    }
}
