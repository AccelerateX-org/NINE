using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class SemesterDate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Description { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        /// <summary>
        /// Markierung für vorlesungsfreie Zeit
        /// </summary>
        public bool HasCourses { get; set; }

        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Wenn gesetzt, dann ist es ein Termin einer Organisation
        /// sonst ein globaler Termin
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// Termine einer Organisation können intern sein
        /// dann sehen nur Mitglieder der Organisation diese
        /// </summary>
        public bool? IsInternal { get; set; }
    }
}
