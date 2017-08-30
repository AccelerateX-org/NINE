using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Semester
    {
        public Semester()
        {
            this.Dates = new HashSet<SemesterDate>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime StartCourses { get; set; }

        public DateTime EndCourses { get; set; }

        /// <summary>
        /// Kurse buchbar, d.h. beim Import wird das flag auf false gesetzt
        /// erst nach dem Import auf true
        /// OH20170504: obsolet
        /// </summary>
        public bool BookingEnabled { get; set; }

        public virtual ICollection<SemesterGroup> Groups { get; set; }

        /// <summary>
        /// Termine im Semester, z.B. vorlesungsfreie Zeit
        /// </summary>
        public virtual ICollection<SemesterDate> Dates { get; set; }
    }
}
