using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Alumnus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

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
