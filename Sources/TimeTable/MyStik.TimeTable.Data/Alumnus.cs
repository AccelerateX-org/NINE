using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Alumnus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Email { get; set; }

        public int Code { get; set; }

        public DateTime CodeExpiryDateTime { get; set; }

        public bool IsValid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// FK::Kurz
        /// </summary>
        public string Degree { get; set; }

        /// <summary>
        /// Bezeichnung des Semesters
        /// </summary>
        public string FinishingSemester { get; set; }
        
        /// <summary>
        /// Studiengang des Absolventen
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Abschlusssemester
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// UserId - kann auch leer sein oder ins "nichts" zeigen
        /// </summary>
        public string UserId { get; set; }
    }
}
