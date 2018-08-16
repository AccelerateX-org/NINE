using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Der erzielte akademische Abschluss, nicht der Weg dahin das ist das Curriculum
    /// Also Eigenschaften wie Teilzeit oder Weiterbildung oder Berufsbezeichnung gehören zum Curriculum!!!
    /// Können auch Zertifikate sein
    /// </summary>
    public class Degree
    {
        public Degree()
        {
            Curricula = new HashSet<Curriculum>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name, z.B. "Bachelor"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kurznamne, z.B. "BA"
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Eine beliebige Beschreibung
        /// v.a. bei Zertifikaten, z.B. von wem, für was
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Studium bis zur untersten Qualificationsebene, z.B. Bachelor
        /// </summary>
        public bool IsUndergraduate { get; set; }

        /// <summary>
        /// Liste aller Studiengänge mit diesem Abschluss
        /// </summary>
        public virtual ICollection<Curriculum> Curricula { get; set; }

    }



}
