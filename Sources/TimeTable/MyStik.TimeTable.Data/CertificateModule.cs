using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Das Modul, das im Zeugnis verzeichnet ist
    /// </summary>
    public class CertificateModule
    {
        public CertificateModule()
        {
            Subjects = new HashSet<CertificateSubject>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Die deutsche Bezeichnung
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Das Gweicht für die Berechnung der Durchschnittsnote
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Das zugehörige Studienprogramm
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Liste der Fächer
        /// </summary>
        public virtual ICollection<CertificateSubject> Subjects { get; set; }
    }
}
