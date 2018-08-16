using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public enum ExamState
    {
        Registered,
        Accepted,
        Rejected,
        Delivered,
        Marked
    }

    public class StudentExam
    {
        public StudentExam()
        {
            ExamPapers = new HashSet<ExamPaper>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Der geprüfte Student
        /// </summary>
        public virtual Student Examinee { get; set; }

        /// <summary>
        /// Alle Prüfer
        /// </summary>
        public virtual ICollection<Examiner> Examiners { get; set; }

        /// <summary>
        /// Die zugehörige Modulprüfung
        /// </summary>
        public virtual Exam Exam { get; set; }

        /// <summary>
        /// Die eigentliche Arbeit
        /// </summary>
        public virtual ICollection<ExamPaper> ExamPapers { get; set; }

        /// <summary>
        /// Status der Prüfungsleistung
        /// </summary>
        public ExamState State { get; set; }

        /// <summary>
        /// Anmeldung
        /// </summary>
        public DateTime Registered { get; set; }

        /// <summary>
        /// Der Beginn
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Das Ende, z.B. die Abgabe der Abschlussarbeit
        /// </summary>
        public DateTime? End { get; set; }
    }
}
