using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Das Fach eines Moduls
    /// </summary>
    public class CertificateSubject
    {
        public CertificateSubject()
        {
            ContentModules = new HashSet<ModuleAccreditation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Die deutsche Bezeichnung
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Das Fachsemester (immer eine Zahl, da gibt es kein 1,5tes Semester oder ähnliches
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// Die ECTS
        /// </summary>
        public double Ects { get; set; }

        /// <summary>
        /// Credit Points nach US-System
        /// </summary>
        public double USCredits { get; set; }

        /// <summary>
        /// das zugehörige Modul
        /// </summary>
        public virtual CertificateModule CertificateModule { get; set; }

        /// <summary>
        /// Anerkannte inhaltliche Module
        /// </summary>
        public virtual ICollection<ModuleAccreditation> ContentModules { get; set; }
    }
}
